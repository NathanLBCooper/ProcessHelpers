using System;

namespace ProcessHelpers
{
    // todo not sure how useful this object is, System.Diagnostics.Process is already pretty good. May be worth it just for the polymorphism.

    /// <summary>
    /// Startable and Stoppable Process using System.Diagnostics.Process
    /// </summary>
    public class SystemProcess : IProcess
    {
        private const int DefaultExitWaitTimeMs = 500;
        private const bool DefaultTerminateOnDispose = true;

        private readonly int exitWaitTimeMs;
        private readonly System.Diagnostics.Process process;
        private bool disposed = false;
        private readonly bool terminateOnDispose;

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
        /// Initializes a new instance of the <see cref="SystemProcess"/> class.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="terminateOnDispose">if set to <c>true</c> [terminate on dispose].</param>
        /// <param name="exitWaitTimeMs">The wait time between asking a process to close and killing it</param>
        public SystemProcess(string executablePath, bool terminateOnDispose = DefaultTerminateOnDispose, int exitWaitTimeMs = DefaultExitWaitTimeMs)
            : this(new System.Diagnostics.Process() { StartInfo = new System.Diagnostics.ProcessStartInfo(executablePath) }, terminateOnDispose, exitWaitTimeMs)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemProcess"/> class.
        /// Use in conjunction with System.Diagnostic.Process helpers to find processes by name, id etc
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="terminateOnDispose">if set to <c>true</c> [terminate on dispose].</param>
        /// <param name="exitWaitTimeMs">The wait time between asking a process to close and killing it</param>
        public SystemProcess(
            System.Diagnostics.Process process,
            bool terminateOnDispose = DefaultTerminateOnDispose,
            int exitWaitTimeMs = DefaultExitWaitTimeMs)
        {
            this.terminateOnDispose = terminateOnDispose;
            this.process = process;
            this.exitWaitTimeMs = exitWaitTimeMs;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Start Running Process.</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Start()
        {
            this.ThrowIfDisposed();
            if (this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Start Running Process.");
            }
            this.process.Start();
        }

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Terminate()
        {
            this.ThrowIfDisposed();
            if (!this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Terminate Non-Running Process.");
            }

            this.process.CloseMainWindow();
            if (!this.process.WaitForExit(exitWaitTimeMs))
            {
                this.process.Kill();
            }
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("SystemProcess Object has been disposed");
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
            this.process.Dispose();
            this.disposed = true;
        }

        ~SystemProcess()
        {
            this.Dispose(false);
        }
    }
}
