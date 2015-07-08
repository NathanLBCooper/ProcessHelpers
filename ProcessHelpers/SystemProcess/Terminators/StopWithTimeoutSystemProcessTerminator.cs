namespace ProcessHelpers
{
    public class StopWithTimeoutSystemProcessTerminator : ISystemProcessTerminator
    {
        private readonly int timeout;

        public StopWithTimeoutSystemProcessTerminator(int timeoutMs)
        {
            this.timeout = timeoutMs;
        }

        public void Terminate(System.Diagnostics.Process process)
        {
            process.CloseMainWindow();
            if (!process.WaitForExit(this.timeout))
            {
                process.Kill();
            }
        }
    }
}