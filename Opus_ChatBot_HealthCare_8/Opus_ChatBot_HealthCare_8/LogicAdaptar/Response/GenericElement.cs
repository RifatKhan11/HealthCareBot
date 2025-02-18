using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.LogicAdaptar.Response
{
    public class GenericElement
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string image_url { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string subtitle { get; set; }

        public List<button> buttons { get; set; }

    }
}
