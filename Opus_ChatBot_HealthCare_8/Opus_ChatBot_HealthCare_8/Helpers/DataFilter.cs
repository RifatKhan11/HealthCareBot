using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Helpers
{
    public static class DataFilter
    {
        //Filtering User Text Data
        public static string FilterUserString(string Data)
        {
           Data = Data.Replace(System.Environment.NewLine, "\\n"); //New Line Escape
           Data = Data.Replace('\t', ' '); // Tab Escape
           return  Data.Replace("\"", "`"); // string Char Escape
        }
    }
}
