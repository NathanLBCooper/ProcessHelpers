using System;

namespace ProcessHelpers
{
    /// <summary>
    /// Startable and Stoppable Process using System.Diagnostics.Process
    /// </summary>
    public class SystemProcess : IProcess
    {
        private readonly System.Diagnostics.Process process;
        private bool disposed = false;
        private readonly ISystemProcessTerminator disposeStrategy;

        /// <summary>
        /// Gets a value indicating whether this instance is process running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is process running; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public bool IsProcessRunning {
            get
            {
                this.ThrowIfDisposed();
                try
                {
                    return !process.HasExited;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemProcess" /> class.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="disposeStrategy">The dispose strategy.</param>
        public SystemProcess(string executablePath, ISystemProcessTerminator disposeStrategy = null)
            : this(new System.Diagnostics.Process() { StartInfo = new System.Diagnostics.ProcessStartInfo(executablePath) }, disposeStrategy)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemProcess" /> class.
        /// Use in conjunction with System.Diagnostic.Process helpers to find processes by name, id etc
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="disposeStrategy">The dispose strategy.</param>
        public SystemProcess(
            System.Diagnostics.Process process,
            ISystemProcessTerminator disposeStrategy = null)
        {
            this.disposeStrategy = disposeStrategy ?? new StopWithTimeoutSystemProcessTerminator(500); ;
            this.process = process;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Start Running Process.</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Start()
        {
            this.CanStartCheck();
            this.process.Start();
        }

        /// <summary>
        /// Sends a close message to the process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Stop()
        {
            this.CanTerminateCheck();
            this.Terminate(new StopSystemProcessTerminator());
        }

        /// <summary>
        /// Sends a close message to the process. Immediately stops the process if it has not closed after maxExitWaitTime.
        /// </summary>
        /// <param name="maxExitWaitTime">The maximum exit wait time.</param>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Stop(int maxExitWaitTime)
        {
            this.CanTerminateCheck();
            this.Terminate(new StopWithTimeoutSystemProcessTerminator(maxExitWaitTime));
        }

        /// <summary>
        /// Immediately stops the process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Kill()
        {
            this.CanTerminateCheck();
            this.Terminate(new KillSystemProcessTerminator());
        }

        private void Terminate(ISystemProcessTerminator terminator)
        {
            terminator.Terminate(this.process);
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("SystemProcess Object has been disposed");
            }
        }

        private void CanTerminateCheck()
        {
            this.ThrowIfDisposed();
            if (!this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Terminate Non-Running Process.");
            }
        }

        private void CanStartCheck()
        {
            this.ThrowIfDisposed();
            if (this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Start Running Process.");
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
                this.disposeStrategy.Terminate(this.process);
            }
            this.process.Dispose();
            this.disposed = true;
        }

        ~SystemProcess()
        {
            this.Dispose(false);
        }
    }
}
