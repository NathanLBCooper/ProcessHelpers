using System;
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
            //using (var notepad = new Process(Environment.SystemDirectory + "\\notepad.exe"))
            //{
            //    notepad.Start();
            //}


            //using (var notepad = new Process(System.Diagnostics.Process.GetProcessesByName("Firefox").First()))
            //{
            //    notepad.Terminate();
            //}
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
