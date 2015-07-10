using System;
using System.Management;

namespace ProcessHelpers
{
    /// <summary>
    /// Startable and Stoppable Process using Windows Management Instrumentation (WMI)
    /// </summary>
    public class WmiProcess : IStoppableProcess
    {
        private readonly string startCommand;
        private readonly WmiCommandRunner wmiWrapper;
        private UInt32 processId;

        /// <summary>
        /// Gets a value indicating whether this instance is process running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is process running; otherwise, <c>false</c>.
        /// </value>
        public bool IsProcessRunning { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiProcess" /> class.
        /// </summary>
        /// <param name="startCommand">The start command, typically the exe path (local, not network path) plus any arguments</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="wmiConnectionOptions">The WMI connection options.</param>
        public WmiProcess(string startCommand, string hostName, ConnectionOptions wmiConnectionOptions = null)
        {
            this.startCommand = startCommand;
            this.IsProcessRunning = false;
            this.wmiWrapper = new WmiCommandRunner(wmiConnectionOptions ?? new ConnectionOptions(), hostName);
        }

        /// <summary>
        /// Starts the Process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Start Running Process.</exception>
        /// <exception cref="System.Exception">WMI command did not successfully complete</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Start()
        {
            if (this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Start Running Process.");
            }

            ManagementBaseObject outParams = this.wmiWrapper.RunCommand(this.startCommand);
            var returnCode = outParams.GetReturnValue();
            if (returnCode != WmiReturnValue.SuccessfullCompletion)
            {
                throw new Exception(string.Format("StartProcess returned: {0}.", returnCode));
            }

            this.processId = outParams.GetPid();
            this.IsProcessRunning = true;
        }

        /// <summary>
        /// Sends a close message to the process.
        /// Soft close is unsupported for WMI, calls Kill()
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.Exception">WMI command did not successfully complete</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Stop()
        {
            this.Kill();
        }

        /// <summary>
        /// Sends a close message to the process. Immediately stops the process if it has not closed after maxExitWaitTime.
        /// Soft close is unsupported for WMI, calls Kill()
        /// </summary>
        /// <param name="maxExitWaitTime">The maximum exit wait time.</param>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.Exception">WMI command did not successfully complete</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Stop(int maxExitWaitTime)
        {
            this.Kill();
        }

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.Exception">WMI command did not successfully complete</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Kill()
        {
            if (!this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Terminate Non-Running Process.");
            }

            /* Command: Use task kill to end the process.
             * CMD
             * - /c : cmd carry out string command and terminate.
             * TASKKILL
             * - /f : Process(es) are forcefully terminated. Redundant for remote processes, all remote processes are forcefully terminated.
             * - /pid : The process ID of the process to be terminated. 
             * - /t : Tree kill. terminate all child processes along with the parent process.
             */

            var command = string.Format("cmd /c \"taskkill /f /pid {0}\" /t", processId);

            ManagementBaseObject outParams = wmiWrapper.RunCommand(command);
            var returnCode = outParams.GetReturnValue();
            if (returnCode != WmiReturnValue.SuccessfullCompletion)
            {
                throw new Exception(string.Format("Starting Taskkill returned: {0}.", returnCode));
            }

            this.IsProcessRunning = false;
        }

        public void Dispose()
        {
            // No-op
        }
    }
}
