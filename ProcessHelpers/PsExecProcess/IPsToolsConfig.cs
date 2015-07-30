using System;

namespace ProcessHelpers
{
    public interface IPsToolsConfig
    {
        string ExecPath { get; }
        string KillPath { get; }
        TimeSpan ToolTimeout { get; }
    }
}