using System;
using System.Management;

namespace ProcessHelpers
{
    public static class ManagementBaseObjectExtensions
    {
        public static WmiReturnValue GetReturnValue(this ManagementBaseObject managementBase)
        {
            return (WmiReturnValue)(UInt32)managementBase["ReturnValue"];
        }

        public static UInt32 GetPid(this ManagementBaseObject managementBase)
        {
            return (UInt32)managementBase["processId"];
        }
    }
}