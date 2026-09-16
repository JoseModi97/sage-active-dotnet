using System;
using Sage.Active.Models;

namespace Sage.Active.Cli.Commands
{
    public static class EnvCommand
    {
        public static void Execute(string[] args)
        {
            var config = CliConfigLoader.LoadConfig();

            if (args.Length == 0)
            {
                Prompter.Info($"Current Environment: {config.Environment} | Region: {config.Region}");
                Prompter.Info($"Base Address: {config.GetEffectiveBaseAddress()}");
                Prompter.Info("To switch, run: sage-active env [production|sandbox]");
                return;
            }

            var target = args[0].ToLower();
            if (target == "production" || target == "prod")
            {
                config.Environment = SageEnvironment.Production;
                CliConfigLoader.SaveConfig(config);
                Prompter.Success("Switched environment to: PRODUCTION");
                Prompter.Info($"Base Address: {config.GetEffectiveBaseAddress()}");
            }
            else if (target == "sandbox" || target == "dev")
            {
                config.Environment = SageEnvironment.Sandbox;
                CliConfigLoader.SaveConfig(config);
                Prompter.Success("Switched environment to: SANDBOX");
                Prompter.Info($"Base Address: {config.GetEffectiveBaseAddress()}");
            }
            else
            {
                Prompter.Error($"Unknown environment '{target}'. Use 'production' or 'sandbox'.");
            }
        }
    }
}
