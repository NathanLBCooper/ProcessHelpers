using System;
using System.Management;

namespace ProcessHelpers
{
    /// <summary>
    /// Specialisation of WmiProcess to provide process stopping on dispose
    /// </summary>
    public class WmiProcess2 : IStoppableProcess //todo better name
    {
        private readonly WmiProcess process;
        private readonly Action<IStoppable> disposalAction;
        private bool disposed = false;

        public WmiProcess2(string startCommand, string hostname, Action<IStoppable> disposalAction, ConnectionOptions wmiConnectionOptions = null)
        {
            this.process = new WmiProcess(startCommand, hostname, wmiConnectionOptions);
            this.disposalAction = disposalAction;
        }

        public void Start()
        {
            this.process.Start();
        }

        public void Stop()
        {
            this.process.Stop();
        }

        public void Stop(int maxExitWaitTime)
        {
            this.process.Stop(maxExitWaitTime);
        }

        public void Kill()
        {
            this.process.Kill();
        }

        public bool IsProcessRunning
        {
            get { return this.process.IsProcessRunning; }
        }

        public void Dispose()
        {
            this.Dispose(true);
            //GC.SuppressFinalize(this); Object can be "undisposed".
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
