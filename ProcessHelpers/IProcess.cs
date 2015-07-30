using System;

namespace ProcessHelpers
{
    public interface IProcess : IDisposable
    {
        /// <summary>
        /// Starts the process.
        /// </summary>
        void Start();
        /// <summary>
        /// Gets a value indicating whether this instance is process running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is process running; otherwise, <c>false</c>.
        /// </value>
        bool IsProcessRunning { get; }
    }
}
