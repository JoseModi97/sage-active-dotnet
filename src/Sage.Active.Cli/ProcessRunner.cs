using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sage.Active.Cli
{
    internal static class ProcessRunner
    {
        public static async Task<int> RunAsync(string fileName, string arguments)
        {
            var startInfo = new ProcessStartInfo(fileName, arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
            };

            using var process = Process.Start(startInfo);
            if (process == null) return -1;

            await process.WaitForExitAsync().ConfigureAwait(false);
            return process.ExitCode;
        }
    }
}
