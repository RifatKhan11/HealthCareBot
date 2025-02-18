using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.LogicAdaptar.Response
{
    public class quick_replies
    {
        public string content_type;
        public string title;
        public string payload;

        public quick_replies(string content_type, string title, string payload)
        {
            this.content_type = content_type;
            this.title = title;
            this.payload = payload;
        }
    }
}
