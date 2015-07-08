﻿using System;
using System.Management;

namespace ProcessHelpers
{
    /// <summary>
    ///  Startable and Stoppable Process using Windows Management Instrumentation (WMI)
    /// </summary>
    public class WmiProcess : IProcess
    {
        private bool disposed = false;
        private readonly string exePath;
        private readonly bool terminateOnDispose;
        private readonly WmiService wmiService;
        private UInt32 processId;

        /// <summary>
        /// Gets a value indicating whether this instance is process running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is process running; otherwise, <c>false</c>.
        /// </value>
        public bool IsProcessRunning { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiProcess"/> class.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="terminateOnDispose">if set to <c>true</c> [terminate on dispose].</param>
        /// <param name="wmiConnectionOptions">The WMI connection options.</param>
        public WmiProcess(string executablePath, string hostName, bool terminateOnDispose = true, ConnectionOptions wmiConnectionOptions = null)
        {
            // Note that executablePath need not be a network path because it's run on the remote machine.
            // (Using the network path may cause issues, with System.Reflection.Assembly.CodeBase for example)
            this.exePath = executablePath;
            this.IsProcessRunning = false;
            this.terminateOnDispose = terminateOnDispose;
            this.wmiService = new WmiService(wmiConnectionOptions ?? new ConnectionOptions(), hostName);
        }

        /// <summary>
        /// Starts the Process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Start Running Process.</exception>
        /// <exception cref="System.Exception">WMI command did not successfully complete</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Start()
        {
            this.ThrowIfDisposed();
            if (this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Start Running Process.");
            }

            ManagementBaseObject outParams = this.wmiService.StartProcess(this.exePath);
            var returnCode = outParams.GetReturnValue();
            if (returnCode != WmiReturnValue.SuccessfullCompletion)
            {
                throw new Exception(string.Format("StartProcess returned: {0}.", returnCode));
            }

            this.processId = outParams.GetPid();
            this.IsProcessRunning = true;
        }

        /// <summary>
        /// Terminates the Process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.Exception">WMI command did not successfully complete</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Terminate()
        {
            this.ThrowIfDisposed();
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
            var command = string.Format("cmd /c \"taskkill /f /pid {0}\" /t", this.processId);

            ManagementBaseObject outParams = this.wmiService.StartProcess(command);
            var returnCode = outParams.GetReturnValue();
            if (returnCode != WmiReturnValue.SuccessfullCompletion)
            {
                throw new Exception(string.Format("Starting Taskkill returned: {0}.", returnCode));
            }

            this.IsProcessRunning = false;
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("WmiProcess Object has been disposed");
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;

            if (disposing)
            {
                // Free managed
            }
            // Free unmanaged
            if (this.terminateOnDispose && this.IsProcessRunning)
            {
                this.Terminate();
            }
            this.disposed = true;
        }

        ~WmiProcess()
        {
            this.Dispose(false);
        }
    }
}
