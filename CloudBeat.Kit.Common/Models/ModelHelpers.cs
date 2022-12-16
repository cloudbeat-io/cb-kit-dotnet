using System;
using System.Collections.Generic;
using System.Text;

namespace CloudBeat.Kit.Common.Models
{
    internal static class ModelHelpers
    {
        public static long? CalculateDuration(DateTime startTime, DateTime? endTime)
        {
            if (!endTime.HasValue)
                return null;
            return (long)(endTime.Value - startTime).TotalMilliseconds;
        }
    }
}
