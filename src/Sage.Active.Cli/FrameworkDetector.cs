using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Sage.Active.Cli
{
    /// <summary>
    /// Figures out what kind of .NET project lives in a directory - ASP.NET Core web app, worker
    /// service, console app, class library, or a .NET 10+ file-based app (a single .cs file, no
    /// .csproj at all) - so 'init' can automatically wire up the right package(s) and registration style
    /// without requiring the user to specify their project type.
    /// </summary>
    public static class FrameworkDetector
    {
        /// <summary>
        /// Scans directory for a .csproj first; if none exists, falls back to
        /// treating a single .cs file as a file-based app (dotnet run app.cs).
        /// </summary>
        public static DetectedProject? Detect(string directory)
        {
            var csprojPath = Directory.GetFiles(directory, "*.csproj").FirstOrDefault();
            if (csprojPath != null)
            {
                return DetectFromCsproj(directory, csprojPath);
            }

            return DetectFileBasedApp(directory);
        }

        private static DetectedProject DetectFromCsproj(string directory, string csprojPath)
        {
            XDocument doc;
            try
            {
                doc = XDocument.Load(csprojPath);
            }
            catch (Exception)
            {
                return new DetectedProject { ProjectPath = csprojPath, Kind = ProjectFrameworkKind.Unknown };
            }

            var sdk = doc.Root?.Attribute("Sdk")?.Value ?? string.Empty;
            var outputType = doc.Descendants("OutputType").FirstOrDefault()?.Value;
            var targetFramework = doc.Descendants("TargetFramework").FirstOrDefault()?.Value
                ?? doc.Descendants("TargetFrameworks").FirstOrDefault()?.Value?.Split(';').FirstOrDefault()
                ?? "net8.0";

            var referencesAspNetCore = doc.Descendants("FrameworkReference")
                .Any(e => string.Equals(e.Attribute("Include")?.Value, "Microsoft.AspNetCore.App", StringComparison.OrdinalIgnoreCase));

            var kind = ClassifyFromProjectFile(sdk, outputType, referencesAspNetCore);

            // Program.cs content is a strong secondary signal when .csproj alone is ambiguous
            if (kind is ProjectFrameworkKind.Console or ProjectFrameworkKind.Unknown)
            {
                var programCsPath = Path.Combine(directory, "Program.cs");
                if (File.Exists(programCsPath))
                {
                    kind = ClassifyFromProgramSource(File.ReadAllText(programCsPath)) ?? kind;
                }
            }

            return new DetectedProject
            {
                ProjectPath = csprojPath,
                Kind = kind,
                TargetFramework = targetFramework,
            };
        }

        private static DetectedProject? DetectFileBasedApp(string directory)
        {
            var csFiles = Directory.GetFiles(directory, "*.cs", SearchOption.TopDirectoryOnly);

            string? entryFile = csFiles.Length switch
            {
                0 => null,
                1 => csFiles[0],
                _ => csFiles.FirstOrDefault(f => string.Equals(Path.GetFileName(f), "app.cs", StringComparison.OrdinalIgnoreCase))
                     ?? csFiles.FirstOrDefault(f => string.Equals(Path.GetFileName(f), "Program.cs", StringComparison.OrdinalIgnoreCase)),
            };

            if (entryFile == null) return null;

            var source = File.ReadAllText(entryFile);

            var declaresWebSdk = source
                .Split('\n')
                .Any(line => line.TrimStart().StartsWith("#:sdk", StringComparison.OrdinalIgnoreCase)
                             && line.Contains("Microsoft.NET.Sdk.Web", StringComparison.OrdinalIgnoreCase));

            var kind = declaresWebSdk
                ? ProjectFrameworkKind.AspNetCoreWeb
                : ClassifyFromProgramSource(source) ?? ProjectFrameworkKind.Console;

            return new DetectedProject
            {
                ProjectPath = entryFile,
                Kind = kind,
                IsFileBasedApp = true,
                TargetFramework = "net10.0",
            };
        }

        private static ProjectFrameworkKind ClassifyFromProjectFile(string sdk, string? outputType, bool referencesAspNetCore)
        {
            if (referencesAspNetCore || sdk.IndexOf("Microsoft.NET.Sdk.Web", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ProjectFrameworkKind.AspNetCoreWeb;
            }

            if (sdk.IndexOf("Microsoft.NET.Sdk.Worker", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ProjectFrameworkKind.WorkerService;
            }

            if (string.Equals(outputType, "Exe", StringComparison.OrdinalIgnoreCase))
            {
                return ProjectFrameworkKind.Console;
            }

            if (sdk.IndexOf("Microsoft.NET.Sdk", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ProjectFrameworkKind.ClassLibrary;
            }

            return ProjectFrameworkKind.Unknown;
        }

        private static ProjectFrameworkKind? ClassifyFromProgramSource(string source)
        {
            if (source.Contains("WebApplication.CreateBuilder")) return ProjectFrameworkKind.AspNetCoreWeb;
            if (source.Contains("Host.CreateApplicationBuilder") || source.Contains("Host.CreateDefaultBuilder")) return ProjectFrameworkKind.WorkerService;
            return null;
        }
    }
}
