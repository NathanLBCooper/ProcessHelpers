using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessHelpers
{
    public class PsToolsProcess : IStoppableProcess
    {
        private bool disposed = false;
        private readonly bool terminateOnDispose;
        private readonly string exePath;
        private readonly string hostname; //todo surely something more than hostname needed for connection
        private readonly string psExecToolPath = @"C:\PSTools\PsExec.exe"; //todo

        private UInt32 processId;

        public PsToolsProcess(string executablePath, string hostName)
        {
            this.exePath = executablePath;
        }

        public void Start()
        {
            using (
                var psExec = new Process()
                {
                    StartInfo =
                        new ProcessStartInfo()
                        {
                            UseShellExecute = false,
                            RedirectStandardOutput = false,
                            RedirectStandardError = true,
                            RedirectStandardInput = true,
                            FileName = psExecToolPath,
                            Arguments = string.Format(@" -i -d \\{0} ""{1}""", this.hostname, this.exePath)
                        }
                })
            {
                psExec.Start();
                this.processId = this.GetPid(psExec.StandardError.ReadToEnd());
            }



        }

        private UInt32 GetPid(string standardError)
        {
            const string StartIndexText = "with process ID ";
            var startIndex = standardError.IndexOf("with process ID ", StringComparison.InvariantCulture);
            var endIndex = standardError.IndexOf(".", startIndex + StartIndexText.Length, StringComparison.InvariantCulture);
            return Convert.ToUInt32(standardError.Substring(startIndex + StartIndexText.Length, endIndex - startIndex - StartIndexText.Length));
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Stop(int maxExitWaitTime)
        {
            throw new NotImplementedException();
        }

        public void Kill()
        {
            throw new NotImplementedException();
        }

        public bool IsProcessRunning
        {
            get { throw new NotImplementedException(); }
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("PsExecProcess Object has been disposed");
            }
        }

        public void Dispose()
        {
            // todo
            //this.Dispose(true);
            //GC.SuppressFinalize(this);
        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (this.disposed) return;

        //    if (disposing)
        //    {
        //        // Free managed
        //    }
        //    // Free unmanaged
        //    // todo see other implementations
        //    this.disposed = true;
        //}

        //~PsToolsProcess()
        //{
        //    this.Dispose(false);
        //}
    }
}
