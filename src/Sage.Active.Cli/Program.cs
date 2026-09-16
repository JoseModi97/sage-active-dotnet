using System;
using System.Linq;
using System.Threading.Tasks;
using Sage.Active.Cli.Commands;

namespace Sage.Active.Cli
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            if (args.Length == 0 || args[0] == "--help" || args[0] == "-h" || args[0] == "help")
            {
                PrintHelp();
                return 0;
            }

            var command = args[0].ToLower();
            var subArgs = args.Skip(1).ToArray();

            try
            {
                switch (command)
                {
                    case "init":
                        await InitCommand.ExecuteAsync(subArgs);
                        break;
                    case "test":
                        await TestCommand.ExecuteAsync(subArgs);
                        break;
                    case "org":
                    case "organizations":
                        await OrgCommand.ExecuteAsync(subArgs);
                        break;
                    case "query":
                    case "exec":
                        await QueryCommand.ExecuteAsync(subArgs);
                        break;
                    case "invoice":
                    case "invoices":
                        await InvoiceCommand.ExecuteAsync(subArgs);
                        break;
                    case "env":
                    case "environment":
                        EnvCommand.Execute(subArgs);
                        break;
                    case "scaffold":
                        ScaffoldCommand.Execute(subArgs);
                        break;
                    case "version":
                    case "-v":
                    case "--version":
                        Console.WriteLine("dotnet-sage-active version 1.0.0");
                        break;
                    default:
                        Prompter.Error($"Unknown command: '{command}'. Run 'sage-active --help' for usage.");
                        return 1;
                }
                return 0;
            }
            catch (Exception ex)
            {
                Prompter.Error($"Execution error: {ex.Message}");
                return 1;
            }
        }

        private static void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"
   _____                   ___         __  _           
  / ___/____ _____ ____   /   |  _____/ /_(_)   _____  
  \__ \/ __ `/ __ `/ _ \ / /| | / ___/ __/ / | / / _ \ 
 ___/ / /_/ / /_/ /  __// ___ |/ /__/ /_/ /| |/ /  __/ 
/____/\__,_/\__, /\___//_/  |_|\___/\__/_/ |___/\___/  
           /____/                                      ");
            Console.ResetColor();
            Console.WriteLine("Sage Active Public API V2 - Developer CLI & Setup Suite (1.0.0)\n");
            Console.WriteLine("Usage: sage-active [command] [options]\n");
            Console.WriteLine("Commands:");
            Console.WriteLine("  init              Interactive configuration setup wizard (saves credentials and org)");
            Console.WriteLine("  test              Run diagnostic self-test (checks connectivity, user, and access policies)");
            Console.WriteLine("  org [set <id>]    List available organizations or switch active organization ID");
            Console.WriteLine("  query <query>     Execute an arbitrary GraphQL query or query from a .graphql file");
            Console.WriteLine("  invoice [list]    List recent sales invoices");
            Console.WriteLine("  invoice create    Create and post a test sales invoice in Sandbox");
            Console.WriteLine("  env [production|sandbox] View or toggle active environment");
            Console.WriteLine("  scaffold [type]   Scaffold starter boilerplate: minimal (API) or console");
            Console.WriteLine("  version           Display CLI version\n");
            Console.WriteLine("Environment Variables:");
            Console.WriteLine("  SAGE_SUBSCRIPTION_KEY   API gateway key ('x-api-key')");
            Console.WriteLine("  SAGE_ACTIVE_ORG_ID      Active organization ID ('X-OrganizationId')");
            Console.WriteLine("  SAGE_ACTIVE_TOKEN       OAuth bearer token\n");
        }
    }
}
