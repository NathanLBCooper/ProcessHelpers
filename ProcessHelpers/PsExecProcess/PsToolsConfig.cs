using System;

namespace ProcessHelpers
{
    public class PsToolsConfig : IPsToolsConfig
    {
        public string ExecPath { get; set; }
        public string KillPath { get; set; }
        public TimeSpan ToolTimeout { get; set; }
    }
}