using System.Diagnostics;

namespace ProcessUtils32
{
    public static class ProcessManager
    {
        /// <summary>
        /// Executes the specified executable with the parameter
        /// </summary>
        /// <param name="executable">The path to the executable</param>
        /// <param name="standardOutput">The variable that should hold the standardOutput</param>
        /// <param name="standardError">The variable that should hold the standardError</param>
        /// <param name="args">The additionally arguments the executeable should be started with</param>
        /// <returns>Returns the ExitCode aswell as the output</returns>
        public static int GetCommandOutput(string executable, out string standardOutput, out string standardError, params string[] args)
        {
            using (Process process = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    Arguments = string.Join(" ", args),
                    FileName = executable,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                process.StartInfo = startInfo;
                process.Start();

                standardOutput = process.StandardOutput.ReadToEnd();
                standardError = process.StandardError.ReadToEnd();

                return process.ExitCode;
            }
        }
    }
}
