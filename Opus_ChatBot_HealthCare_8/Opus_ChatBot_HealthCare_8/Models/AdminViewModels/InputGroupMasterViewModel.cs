using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Models.KeyWord;
using Opus_ChatBot_HealthCare_8.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Models.AdminViewModels
{
    public class InputGroupMasterViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public int MenuId { get; set; }
        public string InputName { get; set; }
        public string ParaMeter { get; set; }
        public string PlaceHolder { get; set; }
        public string InputType { get; set; }
        public int masterId { get; set; }
        public IEnumerable<InputGroupMaster> InputGroupMasterS { get; set; }
        public IEnumerable<Menu> Menus { get; set; }

    }
}
