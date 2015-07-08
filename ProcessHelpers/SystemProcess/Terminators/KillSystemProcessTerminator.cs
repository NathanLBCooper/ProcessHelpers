namespace ProcessHelpers
{
    public class KillSystemProcessTerminator : ISystemProcessTerminator
    {
        public void Terminate(System.Diagnostics.Process process)
        {
            process.Kill();
        }
    }
}