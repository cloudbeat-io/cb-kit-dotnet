using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CloudBeat.Kit.MSTest.Models
{
    public class StepModel
    {
        public string Name { get; set; }

        public List<StepModel> Steps { get; set; }

        [JsonIgnore]
        public bool IsFinished { get; set; }

        public DateTime StartTime { get; set; }

        public double Duration { get; set; }

        public bool IsSuccess { get; set; }

        public long? DomContentLoadedEvent { get; set; }

        public long? LoadEvent { get; set; }
    }
}
