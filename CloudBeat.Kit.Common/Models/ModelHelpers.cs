using System;

namespace CloudBeat.Kit.Common.Models
{
    internal static class ModelHelpers
    {
        public static long? CalculateDuration(DateTime? startTime, DateTime? endTime)
        {
            if (startTime == null || endTime == null)
                return null;
            return (long)(endTime.Value - startTime.Value).TotalMilliseconds;
        }
    }
}
