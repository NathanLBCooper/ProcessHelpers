using System;
using System.Collections.Generic;
using System.Management;

namespace ProcessHelpers
{
    public class WmiCommandRunner
    {
        private readonly ConnectionOptions connectionOptions;
        private readonly string hostName;

        public WmiCommandRunner(ConnectionOptions connectionOptions, string hostName)
        {
            this.connectionOptions = connectionOptions;
            this.hostName = hostName;
        }

        public ManagementBaseObject RunCommand(string command)
        {
            return this.RunCommand(new Dictionary<string, string>() { { "CommandLine", command } });
        }

        public ManagementBaseObject RunCommand(Dictionary<string, string> inArgs)
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
}
