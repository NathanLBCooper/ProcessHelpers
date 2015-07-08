using System;
using System.Collections.Generic;
using System.Management;

namespace ProcessHelpers
{
    public class WmiService
    {
        private readonly ConnectionOptions connectionOptions;
        private readonly string hostName;

        public WmiService(ConnectionOptions connectionOptions, string hostName)
        {
            this.connectionOptions = connectionOptions;
            this.hostName = hostName;
        }

        public ManagementBaseObject StartProcess(string command)
        {
            return this.StartProcess(new Dictionary<string, string>() { { "CommandLine", command } });
        }

        public ManagementBaseObject StartProcess(Dictionary<string, string> inArgs)
        {
            // WMI: Use Win32_Process in root\cimv2 namespace. 
            var processClass = new ManagementClass(
                new ManagementScope(String.Format(@"\\{0}\root\cimv2", this.hostName), this.connectionOptions),
                new ManagementPath("Win32_Process"),
                new ObjectGetOptions());
            ManagementBaseObject parameters = processClass.GetMethodParameters("Create");

            foreach (var item in inArgs)
            {
                parameters[item.Key] = item.Value;
            }

            return processClass.InvokeMethod("Create", parameters, null);
        }
    }

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
