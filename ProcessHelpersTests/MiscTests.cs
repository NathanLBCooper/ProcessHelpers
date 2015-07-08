using System;
using System.IO;
using System.Management;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ProcessHelpers;

namespace ProcessHelpersTests
{
    [TestClass]
    public class MiscTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            //using (var notepad = new RemoteProcess(Environment.SystemDirectory + "\\notepad.exe", "localhost"))
            //{
            //    notepad.Start();
            //}

            using (var notepad = new LocalProcess(Environment.SystemDirectory + "\\notepad.exe"))
            {
                notepad.Start();
            }
        }
    }
}
