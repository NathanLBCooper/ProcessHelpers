namespace ProcessHelpers
{
    public class StopSystemProcessTerminator : ISystemProcessTerminator
    {
        public void Terminate(System.Diagnostics.Process process)
        {
            process.CloseMainWindow();
        }
    }
}