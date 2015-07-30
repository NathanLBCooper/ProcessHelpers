using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessHelpers
{
    public class PsToolsProcess : IStoppableProcess
    {
        private readonly string exePath;
        private readonly string hostname;
        private readonly IPsToolsConfig config;
        private readonly Credentials credentials;

        private UInt32 processId;

        /// <summary>
        /// Gets a value indicating whether this instance is process running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is process running; otherwise, <c>false</c>.
        /// </value>
        public bool IsProcessRunning { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PsToolsProcess"/> class.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="config">The configuration for using the PSTools</param>
        public PsToolsProcess(string executablePath, string hostname, IPsToolsConfig config)
        {
            this.exePath = executablePath;
            this.hostname = hostname;
            this.config = config;
            this.IsProcessRunning = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PsToolsProcess"/> class.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="config">The configuration for using the PSTools</param>
        public PsToolsProcess(string executablePath, string hostname, Credentials credentials, IPsToolsConfig config)
            : this(executablePath, hostname, config)
        {
            this.credentials = credentials;
        }


        /// <summary>
        /// Starts the process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Start Running Process.</exception>
        /// <exception cref="ProcessHelpers.PsExecCommandException">PSTools command did not successfully complete</exception>
        public void Start()
        {
            if (this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Start Running Process.");
            }

            /* Command: Use PsExec http://ss64.com/nt/psexec.html
             * - i : Interactive - Run the program so that it interacts with the desktop on the remote system.
             * - d : Don’t wait for the application to terminate.
             * - accepteula: Suppress the display of the license dialog.
             */
            const string Args = @" -accepteula -i -d \\{0}{1} ""{2}""";

            using (
                var psExecProcess = new Process()
                {
                    StartInfo =
                        new ProcessStartInfo()
                        {
                            UseShellExecute = false,
                            RedirectStandardOutput = false,
                            RedirectStandardError = true,
                            RedirectStandardInput = true,
                            FileName = this.config.ExecPath,
                            Arguments = string.Format(Args, this.hostname, this.GetCredentialPowershellArgs(), this.exePath)
                        }
                })
            using (var psExec = new SystemProcess(psExecProcess, x => x.Kill()))
            {
                psExec.Start();
                var standardError = ReadWithTimeout(psExecProcess.StandardError);

                if (!standardError.Contains(string.Format("started on {0} with process ID", this.hostname)))
                {
                    throw new PsExecCommandException(string.Format("Failure running PsExec: {0}", standardError));
                }

                this.processId = this.GetPid(standardError);
            }

            this.IsProcessRunning = true;
        }

        /// <summary>
        /// Sends a close message to the process.
        /// Soft close is unsupported for PSTools, calls Kill()
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.Exception">WMI command did not successfully complete</exception>
        public void Stop()
        {
            this.Kill();
        }

        /// <summary>
        /// Sends a close message to the process. Immediately stops the process if it has not closed after maxExitWaitTime.
        /// Soft close is unsupported for PSTools, calls Kill()
        /// </summary>
        /// <param name="maxExitWaitTime">The maximum exit wait time.</param>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="PsExecCommandException">PSTools command did not successfully complete</exception>
        public void Stop(int maxExitWaitTime)
        {
            this.Kill();
        }

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="PsExecCommandException">PSTools command did not successfully complete</exception>
        public void Kill()
        {
            if (!this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Terminate Non-Running Process.");
            }

            /* Command: Use PsKill http://ss64.com/nt/pskill.html
             * - accepteula: Suppress the display of the license dialog.
             */
            const string Args = @" -accepteula \\{0} ""{1}""";

            using (
                var psKillProcess = new Process()
                {
                    StartInfo =
                        new ProcessStartInfo()
                        {
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = false,
                            RedirectStandardInput = true,
                            FileName = this.config.KillPath,
                            Arguments = string.Format(Args, this.hostname, this.processId) // -t treekill seems to regularly cause PsKill to crash
                        }
                })
            using (var psKill = new SystemProcess(psKillProcess, x => x.Kill()))
            {
                psKill.Start();

                var standardOutput = ReadWithTimeout(psKillProcess.StandardOutput);

                if (
                    !(standardOutput.Contains(string.Format("Process {0} killed", this.processId))
                      || standardOutput.Contains(string.Format("Process {0} on {1} killed", this.processId, this.hostname))))
                {
                    throw new PsExecCommandException(string.Format("Failure running PsExec: {0}", standardOutput));
                }
            }

            this.IsProcessRunning = false;
        }

        private UInt32 GetPid(string standardError)
        {
            const string StartIndexText = "with process ID ";
            var startIndex = standardError.IndexOf("with process ID ", StringComparison.InvariantCulture);
            var endIndex = standardError.IndexOf(".", startIndex + StartIndexText.Length, StringComparison.InvariantCulture);

            if (startIndex < 0 || endIndex < 0)
            {
                throw new Exception(standardError);
            }

            return Convert.ToUInt32(standardError.Substring(startIndex + StartIndexText.Length, endIndex - startIndex - StartIndexText.Length));
        }

        private string ReadWithTimeout(StreamReader streamReader)
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(this.config.ToolTimeout);

            var readTask = Task.Run(async () => await streamReader.ReadToEndAsync(), tokenSource.Token);
            readTask.Wait(tokenSource.Token);
            return readTask.Result;
        }

        public void Dispose()
        {
            // No-op
        }

        private string GetCredentialPowershellArgs()
        {
            if (this.credentials == null)
            {
                return string.Empty;
            }

            return this.credentials.GetPsToolArgs();
        }
    }
}
