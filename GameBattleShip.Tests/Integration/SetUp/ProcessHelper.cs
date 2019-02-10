using System.Diagnostics;
using System.Text;
using Serilog;

namespace GameBattleShip.Tests.Integration.SetUp
{
    public static class ProcessHelper
    {
        public static int RunProcess(string fileName, string arugments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arugments,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                },
                EnableRaisingEvents = true
            };

            var output = new StringBuilder();
            var error = new StringBuilder();

            process.OutputDataReceived += (_, outLine) =>
            {
                if (string.IsNullOrEmpty(outLine.Data))
                {
                    return;
                }
                output.AppendLine(outLine.Data);
            };

            process.ErrorDataReceived += (_, outLine) =>
            {
                if (string.IsNullOrEmpty(outLine.Data))
                {
                    return;
                }
                error.AppendLine(outLine.Data);
            };

            var exitCode = 0;
            process.Exited += (_, __) => exitCode = process.ExitCode;
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit(15000);

            Log.Logger.Information("\r\n{@Filename} {@Arguments}\r\n{Output}\r\n{Error}", process.StartInfo.FileName, process.StartInfo.Arguments, output, error);
            return exitCode;
        }
    }
}
