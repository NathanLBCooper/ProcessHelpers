using System;

namespace ProcessHelpers
{
    public class PsExecCommandException : Exception
    {
        public PsExecCommandException(string message)
            : base(message)
        {           
        }
    }
}