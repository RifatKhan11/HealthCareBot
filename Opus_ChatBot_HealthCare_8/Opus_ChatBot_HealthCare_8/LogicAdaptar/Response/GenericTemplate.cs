using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.LogicAdaptar.Response
{
    public class GenericTemplate
    {
        public string template_type { get; set; }

        public List<GenericElement> elements { get; set; }
    }
}
