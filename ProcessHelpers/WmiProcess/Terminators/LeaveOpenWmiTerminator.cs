namespace ProcessHelpers
{
    public class LeaveOpenWmiTerminator : IWmiProcessTerminator
    {
        public bool Terminate(uint processId, WmiCommandRunner wmiWrapper)
        {
            return false;
        }
    }
}