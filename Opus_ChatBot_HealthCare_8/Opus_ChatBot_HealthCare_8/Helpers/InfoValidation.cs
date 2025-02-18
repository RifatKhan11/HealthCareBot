using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Helpers
{
    public static class InfoValidation
    {
        public static bool VlidateInformation(string message, string type)
        {
            if(type == "mobile")
            {
                if (message.Length != 11) return false;
                if (message[0] != '0' || message[1] != '1') return false;
                for (int i = 2; i < 11; i++) if (message[i] < '0' || '9' < message[i]) return false;
            }else if(type == "integer")
            {
                int tm;
                if (!int.TryParse(message, out tm)) return false;
            }
            return true;
        }

        public static bool CheckConfirmation(string message)
        {
            message = message.ToLower();
            if (message == "yes" || message == "yeh" || message == "ho" || message == "hmmm" || message == "হ্যাঁ") return true;
            return false;
        }
        public static bool CheckConfirmationNew(string message)
        {
            message = message.ToLower();
            if (message == "yes" || message == "হ্যাঁ"|| message.ToLower() == "no" || message == "না") return true;
            return false;
        }

        public static string CheckPassportOrRef(string message)
        {
            if (message.Count() >= 1) return "passport";
            if (message.Count() >= 1) return "ref";
            return "unknown";
        }
    }
}
