namespace ProcessHelpers
{
    public interface IStoppable
    {
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
    }
}