using System;

namespace ProcessHelpers
{
    /// <summary>
    /// Specialisation of PsToolsProcess to provide process stopping on dispose
    /// </summary>
    public class PsToolsOwningProcess : IStoppableProcess
    {
        private readonly PsToolsProcess process;
        private readonly Action<IStoppable> disposalAction;
        private bool disposed = false;

        /// <summary>
        /// Gets a value indicating whether this instance is process running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is process running; otherwise, <c>false</c>.
        /// </value>
        public bool IsProcessRunning
        {
            get { return this.process.IsProcessRunning; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PsToolsProcess" /> class.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="config">The configuration for using the PSTools</param>
        /// <param name="disposalAction">The disposal action.</param>
        public PsToolsOwningProcess(string executablePath, string hostname, IPsToolsConfig config, Action<IStoppable> disposalAction)
        {
            this.process = new PsToolsProcess(executablePath, hostname, config);
            this.disposalAction = disposalAction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PsToolsProcess" /> class.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="config">The configuration for using the PSTools</param>
        /// <param name="disposalAction">The disposal action.</param>
        public PsToolsOwningProcess(string executablePath, string hostname, Credentials credentials, IPsToolsConfig config, Action<IStoppable> disposalAction)
        {
            this.process = new PsToolsProcess(executablePath, hostname, credentials, config);
            this.disposalAction = disposalAction;
        }

        /// <summary>
        /// Starts the Process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Start Running Process.</exception>
        /// <exception cref="System.Exception">WMI command did not successfully complete</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Start()
        {
            this.process.Start();
        }

        /// <summary>
        /// Sends a close message to the process.
        /// Soft close is unsupported for PSTools, calls Kill()
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.Exception">WMI command did not successfully complete</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Stop()
        {
            this.process.Stop();
        }

        /// <summary>
        /// Sends a close message to the process. Immediately stops the process if it has not closed after maxExitWaitTime.
        /// Soft close is unsupported for PSTools, calls Kill()
        /// </summary>
        /// <param name="maxExitWaitTime">The maximum exit wait time.</param>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="PsExecCommandException">PSTools command did not successfully complete</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Stop(int maxExitWaitTime)
        {
            this.process.Stop(maxExitWaitTime);
        }

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="PsExecCommandException">PSTools command did not successfully complete</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Kill()
        {
            this.process.Kill();
        }

        /// <summary>
        /// This is a no-op that will leave the object in a still useable state
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            //GC.SuppressFinalize(this); Object can be "undisposed" so still need finalize.
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;

            if (disposing)
            {
                this.process.Dispose();
            }
            // Free unmanaged
            if (this.IsProcessRunning)
            {
                this.disposalAction.Invoke(this);
            }
            this.disposed = true;
        }

        ~PsToolsOwningProcess()
        {
            this.Dispose(false);
        }
    }
}
