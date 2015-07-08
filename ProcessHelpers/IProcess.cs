using System;

namespace ProcessHelpers
{
    public interface IProcess : IDisposable
    {
        void Start();
        void Terminate();

        bool IsProcessRunning { get; }
    }
}
