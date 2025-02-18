using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.LogicAdaptar.Response
{
    public class button
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string url { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string webview_height_ratio { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool messenger_extensions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string webview_share_button { get; set; }


    }
}
