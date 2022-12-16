﻿namespace CloudBeat.Kit.Common.Models
{
    public enum FailureReasonEnum
    {
        FlakyTest = 1,
        Environment = 2,
        RealDefect = 3,
        TestData = 4,
        Device = 5,
        Ambiguous = 6,
        ToInvestigate = 7
    }
}
