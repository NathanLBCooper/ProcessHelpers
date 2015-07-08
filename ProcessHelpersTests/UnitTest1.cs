using System;
using System.Diagnostics;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ProcessHelpers;

namespace ProcessHelpersTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //using (var notepad = new SystemProcess(Environment.SystemDirectory + "\\notepad.exe", new LeaveRunningSystemProcessTerminator()))
            //{
            //    notepad.Start();
            //}

            var process = new Process { StartInfo = new ProcessStartInfo(Environment.SystemDirectory + "\\notepad.exe") };
            process.Start();
            using (var notepad = new SystemProcess(process))
            {
                notepad.Stop();
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            using (var notepad = new WmiProcess(Environment.SystemDirectory + "\\notepad.exe", "localhost"))
            {
                notepad.Start();
            }
        }

        [TestMethod]
        public void TestMethod3()
        {
            //using (var notepad = new PsToolsProcess(@"c:\program files\internet explorer\iexplore.exe", "localhost"))
            //{
            //    notepad.Start();
            //}
        }
    }
}
