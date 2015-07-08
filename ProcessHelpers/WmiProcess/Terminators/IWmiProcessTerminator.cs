using System;

namespace ProcessHelpers
{
    public interface IWmiProcessTerminator
    {
        bool Terminate(UInt32 processId, WmiCommandRunner wmiWrapper);
    }
}
