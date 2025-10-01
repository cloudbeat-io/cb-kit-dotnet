using System;

namespace CloudBeat.Kit.Common
{
    public class FullStackException : Exception
    {
        public string FullStackTrace { get; }

        public FullStackException(Exception inner, string fullStackTrace)
            : base(null, inner)
        {
            FullStackTrace = fullStackTrace;
        }
    }
}
