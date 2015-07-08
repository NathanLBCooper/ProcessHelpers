using System;

namespace ProcessHelpers
{
    /// <summary>
    /// Startable and Stoppable Process
    /// </summary>
    public interface IProcess : IDisposable
    {
        /// <summary>
        /// Starts the process.
        /// </summary>
        void Start();
        /// <summary>
        /// Sends a close message to the process
        /// Kill() is called if unsupported.
        /// </summary>
        void Stop();
        /// <summary>
        /// Sends a close message to the process. Immediately stops the process if it has not closed after maxExitWaitTime.
        /// Kill() is called if unsupported.
        /// </summary>
        /// <param name="maxExitWaitTime">The maximum exit wait time.</param>
        void Stop(int maxExitWaitTime);
        /// <summary>
        /// Immediately stops the process.
        /// </summary>
        void Kill();
        /// <summary>
        /// Gets a value indicating whether this instance is process running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is process running; otherwise, <c>false</c>.
        /// </value>
        bool IsProcessRunning { get; }
    }
}
