using System;
using System.Management;

namespace ProcessHelpers
{
    /// <summary>
    /// Specialisation of WmiProcess to provide process stopping on dispose
    /// </summary>
    public class WmiOwningProcess : IStoppableProcess //todo better name
    {
        private readonly WmiProcess process;
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

        public WmiOwningProcess(string startCommand, string hostname, Action<IStoppable> disposalAction, ConnectionOptions wmiConnectionOptions = null)
        {
            this.process = new WmiProcess(startCommand, hostname, wmiConnectionOptions);
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
        /// Soft close is unsupported for WMI, calls Kill()
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
        /// Soft close is unsupported for WMI, calls Kill()
        /// </summary>
        /// <param name="maxExitWaitTime">The maximum exit wait time.</param>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.Exception">WMI command did not successfully complete</exception>
        /// <exception cref="System.ObjectDisposedException">Object Has Been Disposed</exception>
        public void Stop(int maxExitWaitTime)
        {
            this.process.Stop(maxExitWaitTime);
        }

        /// <summary>
        /// Immediately stops the associated process.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot Terminate Non-Running Process.</exception>
        /// <exception cref="System.Exception">WMI command did not successfully complete</exception>
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
                // Free managed
            }
            // Free unmanaged
            if (this.IsProcessRunning)
            {
                this.disposalAction.Invoke(this);
            }
            this.disposed = true;
        }

        ~WmiProcess2()
        {
            this.Dispose(false);
        }
    }
}
