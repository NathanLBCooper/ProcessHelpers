using System;
using System.Diagnostics;
using System.Threading;

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
            var process = new Process { StartInfo = new ProcessStartInfo(Environment.SystemDirectory + "\\notepad.exe") };
            process.Start();
            using (var notepad = new SystemProcess(process))
            {
                Thread.Sleep(500);
                notepad.Kill();
            }
        }

        [TestMethod]
        public void TestMethod1andabit()
        {
            using (var notepad = new SystemProcess(Environment.SystemDirectory + "\\notepad.exe", x => x.Kill()))
            {
                Thread.Sleep(500);
                notepad.Start();
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            using (var notepad = new WmiProcess(Environment.SystemDirectory + "\\notepad.exe", "localhost"))
            {
                Thread.Sleep(500);
                notepad.Start();
            }
        }

        [TestMethod]
        public void TestMethod2andabit()
        {
            using (var notepad = new WmiOwningProcess(Environment.SystemDirectory + "\\notepad.exe", "localhost", x => x.Kill()))
            {
                Thread.Sleep(500);
                notepad.Start();
            }
        }

        [TestMethod]
        public void TestMethod3()
        {
            var config = new PsToolsConfig()
            {
                ExecPath = @"C:\PSTools\PsExec.exe",
                KillPath = @"C:\PSTools\PsKill.exe",
                ToolTimeout = TimeSpan.FromMilliseconds(10000)
            };

            using (var notepad = new PsToolsProcess(Environment.SystemDirectory + "\\notepad.exe", "localhost", config))
            {
                notepad.Start();
                Thread.Sleep(500);
                notepad.Kill();
            }
        }

        [TestMethod]
        public void TestMethod3andabit()
        {
            var config = new PsToolsConfig()
            {
                ExecPath = @"C:\PSTools\PsExec.exe",
                KillPath = @"C:\PSTools\PsKill.exe",
                ToolTimeout = TimeSpan.FromMilliseconds(10000)
            };

            using (var notepad = new PsToolsOwningProcess(Environment.SystemDirectory + "\\notepad.exe", "localhost", config, x => x.Kill()))
            {
                notepad.Start();
                Thread.Sleep(500);
            }
        }
    }
}
