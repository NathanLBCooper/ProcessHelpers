using System;
using System.Management;

namespace ProcessHelpers
{
    /// <summary>
    /// Startable and Stoppable Process using Windows Management Instrumentation (WMI)
    /// </summary>
    public class WmiProcess : IProcess
    {
        private bool disposed = false;
        private readonly string exePath;
        private readonly WmiCommandRunner wmiWrapper;
        private UInt32 processId;
        private readonly IWmiProcessTerminator disposeStrategy;

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
        public WmiProcess(string executablePath, string hostName, IWmiProcessTerminator disposeStrategy = null, ConnectionOptions wmiConnectionOptions = null)
        {
            // Note that executablePath need not be a network path because it's run on the remote machine.
            // (Using the network path may cause issues, with System.Reflection.Assembly.CodeBase for example)
            this.exePath = executablePath;
            this.IsProcessRunning = false;
            this.disposeStrategy = disposeStrategy ?? new KillWmiTerminator();
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
            this.ThrowIfDisposed();
            if (this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Start Running Process.");
            }

            ManagementBaseObject outParams = this.wmiWrapper.StartProcess(this.exePath);
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
            this.ThrowIfDisposed();
            if (!this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Terminate Non-Running Process.");
            }

            this.IsProcessRunning = !new KillWmiTerminator().Terminate(this.processId, this.wmiWrapper);
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
            if (this.IsProcessRunning)
            {
                this.IsProcessRunning = !this.disposeStrategy.Terminate(this.processId, this.wmiWrapper);
            }
            this.disposed = true;
        }

        ~WmiProcess()
        {
            this.Dispose(false);
        }
    }
}
