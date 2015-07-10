using System;

namespace ProcessHelpers
{
    /// <summary>
    /// Startable and Stoppable Process using System.Diagnostics.Process
    /// </summary>
    public class SystemProcess : IStoppableProcess
    {
        private readonly System.Diagnostics.Process process;
        private readonly Action<IStoppable> disposeAction;
        private bool disposed = false;

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
        /// <param name="disposeAction">The dispose action.</param>
        public SystemProcess(string executablePath, Action<IStoppable> disposeAction = null)
            : this(new System.Diagnostics.Process() { StartInfo = new System.Diagnostics.ProcessStartInfo(executablePath) }, disposeAction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemProcess" /> class.
        /// Use in conjunction with System.Diagnostic.Process helpers to find processes by name, id etc
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="disposeAction">The dispose action.</param>
        public SystemProcess(
            System.Diagnostics.Process process, Action<IStoppable> disposeAction = null)
        {
            this.process = process;
            this.disposeAction = disposeAction ?? (x => { });
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Start Running Process.</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Start()
        {
            this.ThrowIfDisposed();
            this.ThrowIfCannotStart();

            this.process.Start();
        }

        /// <summary>
        /// Sends a close message to the process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Stop()
        {
            this.ThrowIfDisposed();
            this.ThrowIfCannotTerminate();

            process.CloseMainWindow();
        }

        /// <summary>
        /// Sends a close message to the process. Immediately stops the process if it has not closed after maxExitWaitTime.
        /// </summary>
        /// <param name="maxExitWaitTime">The maximum exit wait time.</param>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Stop(int maxExitWaitTime)
        {
            this.ThrowIfDisposed();
            this.ThrowIfCannotTerminate();

            process.CloseMainWindow();
            if (!process.WaitForExit(maxExitWaitTime))
            {
                process.Kill();
            }
        }

        /// <summary>
        /// Immediately stops the process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Kill()
        {
            this.ThrowIfDisposed();
            this.ThrowIfCannotTerminate();

            process.Kill();
        }

        private void ThrowIfCannotTerminate()
        {
            if (!this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Terminate Non-Running Process.");
            }
        }

        private void ThrowIfCannotStart()
        {
            if (this.IsProcessRunning)
            {
                throw new InvalidOperationException("Cannot Start Running Process.");
            }
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("SystemProcess Object has been disposed");
            }
        }

        /// <summary>
        /// Disposes of underlying process object.
        /// Makes decision whether and how to stop process according to disposeAction
        /// </summary>
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
                this.disposeAction(this);
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
