using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Sage.Active.Models;

namespace Sage.Active.Cli.Commands
{
    public static class InitCommand
    {
        private const string PackageVersion = "1.0.0";

        public static async Task<int> ExecuteAsync(string[] args)
        {
            var directory = Directory.GetCurrentDirectory();
            var project = FrameworkDetector.Detect(directory);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=================================================");
            Console.WriteLine("       Sage Active CLI Setup & Scaffolder        ");
            Console.WriteLine("=================================================");
            Console.ResetColor();

            if (project != null)
            {
                Prompter.Success($"Auto-detected project: {project.Kind} ({Path.GetFileName(project.ProjectPath)})");
                Console.WriteLine($"Target Framework : {project.TargetFramework}");
                Console.WriteLine($"Dependency Injection: {(project.HasDependencyInjection ? "Yes (will install Sage.Active.AspNetCore)" : "No (will install Sage.Active core)")}\n");
            }
            else
            {
                Prompter.Warning("No .csproj or file-based app found in the current directory. Running in global config mode.\n");
            }

            var config = CliConfigLoader.LoadConfig();

            // Check if user passed arguments
            var targetEnvArg = args.FirstOrDefault(a => a.StartsWith("--env=", StringComparison.OrdinalIgnoreCase))?.Substring(6)
                ?? (args.Contains("sandbox", StringComparer.OrdinalIgnoreCase) ? "sandbox" : null)
                ?? (args.Contains("production", StringComparer.OrdinalIgnoreCase) ? "production" : null);

            if (!string.IsNullOrWhiteSpace(targetEnvArg))
            {
                if (Enum.TryParse<SageEnvironment>(targetEnvArg, true, out var parsedEnv))
                {
                    config.Environment = parsedEnv;
                }
            }

            // 1. Region
            var regionStr = Prompter.Prompt("Select Sage Region (FR, ES, DE, PT)", config.Region.ToString()).ToUpper();
            if (Enum.TryParse<SageRegion>(regionStr, true, out var reg))
            {
                config.Region = reg;
            }

            // 2. Environment
            var envStr = Prompter.Prompt("Select Environment (Sandbox, Production)", config.Environment.ToString());
            if (Enum.TryParse<SageEnvironment>(envStr, true, out var env))
            {
                config.Environment = env;
            }

            // 3. Credentials
            config.SubscriptionKey = Prompter.Prompt("Enter API Subscription Key (x-api-key)", config.SubscriptionKey ?? "");
            config.ClientId = Prompter.Prompt("Enter Client ID (from Developer Portal)", config.ClientId ?? "");
            config.ClientSecret = Prompter.Prompt("Enter Client Secret", config.ClientSecret ?? "");
            config.AccessToken = Prompter.Prompt("Enter Bearer Access Token (or press enter if using SBC Auth)", config.AccessToken ?? "");

            // 4. Test connection & query organizations
            Prompter.Info("Connecting to Sage Active to fetch your organizations...");
            try
            {
                var client = new SageActiveClient(config);
                var orgs = await client.Organizations.GetOrganizationsAsync(10);

                if (orgs.TotalCount > 0)
                {
                    Prompter.Success($"Found {orgs.TotalCount} organization(s):");
                    var index = 1;
                    foreach (var org in orgs.Nodes)
                    {
                        Console.WriteLine($"  [{index++}] {org.SocialName} ({org.LegislationCode}) - ID: {org.Id}");
                    }

                    var pick = Prompter.Prompt("Select default organization number", "1");
                    if (int.TryParse(pick, out var p) && p >= 1 && p <= orgs.TotalCount)
                    {
                        var i = 1;
                        foreach (var org in orgs.Nodes)
                        {
                            if (i++ == p)
                            {
                                config.OrganizationId = org.Id;
                                Prompter.Success($"Selected: {org.SocialName} ({org.Id})");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Prompter.Warning("No organizations were found for this account. You can enter an Organization ID manually.");
                    config.OrganizationId = Prompter.Prompt("Organization ID (UUID)", config.OrganizationId ?? "");
                }
            }
            catch (Exception ex)
            {
                Prompter.Warning($"Unable to automatically query organizations: {ex.Message}");
                config.OrganizationId = Prompter.Prompt("Enter Organization ID (UUID) manually", config.OrganizationId ?? "");
            }

            // 5. Save global CLI configuration
            CliConfigLoader.SaveConfig(config);
            Prompter.Success("Saved configuration to sageactive.config.json!");

            // 6. Automatic Package Installation and Scaffolding for Detected Project
            if (project != null)
            {
                var packages = new List<string> { "Sage.Active" };
                if (project.HasDependencyInjection)
                {
                    packages.Add("Sage.Active.AspNetCore");
                }

                if (project.IsFileBasedApp)
                {
                    InstallIntoFileBasedApp(project, packages);
                }
                else
                {
                    await InstallViaDotnetAddPackageAsync(project, packages);
                }

                // Write/Update appsettings.json if project uses DI
                if (project.HasDependencyInjection && !project.IsFileBasedApp)
                {
                    UpdateAppSettings(config);
                    Prompter.Success("Updated appsettings.json with SageActive configuration!");
                }

                // Scaffold starter code adapted to the project
                var scaffoldPath = CodeScaffolder.GetTargetPath(directory, project);
                if (scaffoldPath != null && !File.Exists(scaffoldPath))
                {
                    var written = CodeScaffolder.Scaffold(directory, project);
                    Prompter.Success($"Generated starter code tailored for {project.Kind}: {Path.GetRelativePath(directory, written!)}");
                }
            }

            Console.WriteLine();
            Prompter.Info("Run 'sage-active test' to verify your connection and permissions!");
            return 0;
        }

        private static async Task InstallViaDotnetAddPackageAsync(DetectedProject project, List<string> packages)
        {
            foreach (var package in packages)
            {
                Prompter.Info($"Adding package {package}...");
                var exitCode = await ProcessRunner.RunAsync("dotnet", $"add \"{project.ProjectPath}\" package {package}").ConfigureAwait(false);
                if (exitCode == 0)
                {
                    Prompter.Success($"Installed {package} into {Path.GetFileName(project.ProjectPath)}");
                }
                else
                {
                    Prompter.Warning($"'dotnet add package {package}' returned exit code {exitCode}.");
                }
            }
        }

        private static void InstallIntoFileBasedApp(DetectedProject project, List<string> packages)
        {
            var lines = File.ReadAllLines(project.ProjectPath).ToList();
            var directivesToAdd = new List<string>();

            if (project.Kind == ProjectFrameworkKind.AspNetCoreWeb &&
                !lines.Any(l => l.TrimStart().StartsWith("#:sdk", StringComparison.OrdinalIgnoreCase) && l.Contains("Microsoft.NET.Sdk.Web", StringComparison.OrdinalIgnoreCase)))
            {
                directivesToAdd.Add("#:sdk Microsoft.NET.Sdk.Web");
            }

            foreach (var package in packages)
            {
                if (lines.Any(l => l.Contains($"#:package {package}", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
                directivesToAdd.Add($"#:package {package}@{PackageVersion}");
            }

            if (directivesToAdd.Count > 0)
            {
                directivesToAdd.AddRange(lines);
                File.WriteAllLines(project.ProjectPath, directivesToAdd);
                Prompter.Success($"Added #:package directives to {Path.GetFileName(project.ProjectPath)}");
            }
        }

        private static void UpdateAppSettings(SageActiveConfig config)
        {
            var appSettings = new
            {
                SageActive = new
                {
                    Region = config.Region.ToString(),
                    Environment = config.Environment.ToString(),
                    SubscriptionKey = config.SubscriptionKey,
                    OrganizationId = config.OrganizationId,
                    ClientId = config.ClientId,
                    ClientSecret = config.ClientSecret,
                    AccessToken = config.AccessToken
                }
            };

            var json = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("appsettings.json", json);
        }
    }
}
