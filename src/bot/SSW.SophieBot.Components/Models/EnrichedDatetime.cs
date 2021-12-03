﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SSW.SophieBot.Components.Models
{
    public class EnrichedDatetime
    {
        [JsonProperty("timex")]
        public List<string> Timex { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
