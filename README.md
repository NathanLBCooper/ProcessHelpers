# ProcessHelpers
Some wrapper classes to simplify dealing with processes and allow interchanagable use of different process running tactics.

Simplifies process interaction using:
  - System.Diagnostics.Process
  - Windows Management Instrumentation (WMI)
  - PsExec

Unifies them under the two interfaces IProcess and IStoppable (and IStoppableProcess). Allows for explicit selection of Diposal behaviour upfront on IStoppable objects.

          public interface IProcess : IDisposable
          {
              /// Starts the process.
              void Start();

              /// Gets a value indicating whether this instance is process running.
              bool IsProcessRunning { get; }
          }
          
          
          
          public interface IStoppable
          {
              /// Sends a close message to the process
              void Stop();

              /// Sends a close message to the process. Immediately stops the process if it has not closed after maxExitWaitTime.
              void Stop(int maxExitWaitTime);

              /// Immediately stops the process.
              void Kill();
          }
