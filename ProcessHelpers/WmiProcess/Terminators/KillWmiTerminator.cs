using System;
using System.Management;

namespace ProcessHelpers
{
    public class KillWmiTerminator : IWmiProcessTerminator
    {
        public bool Terminate(UInt32 processId, WmiCommandRunner wmiWrapper)
        {
            /* Command: Use task kill to end the process.
             * CMD
             * - /c : cmd carry out string command and terminate.
             * TASKKILL
             * - /f : Process(es) are forcefully terminated. Redundant for remote processes, all remote processes are forcefully terminated.
             * - /pid : The process ID of the process to be terminated. 
             * - /t : Tree kill. terminate all child processes along with the parent process.
             */

            var command = string.Format("cmd /c \"taskkill /f /pid {0}\" /t", processId);

            ManagementBaseObject outParams = wmiWrapper.StartProcess(command);
            var returnCode = outParams.GetReturnValue();
            if (returnCode != WmiReturnValue.SuccessfullCompletion)
            {
                throw new Exception(string.Format("Starting Taskkill returned: {0}.", returnCode));
            }

            return true;
        }
    }
}