using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Opus_ChatBot_HealthCare_8.Services
{
    public class BotFlowService : IBotFlowService
    {
        private readonly ApplicationDbContext _contex;

        public BotFlowService(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        public string GetCurrentFlowStatus(string CombinedId)
        {
            try
            {
                BotFlow data = _contex.BotFlows.Find(CombinedId);
                if (data != null) return data.currentFlow;
                return "default";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "default";
            }
        }

        public DateTime GetCurrentFlowStatusDateTime(string CombinedId)
        {
            try
            {
                BotFlow data = _contex.BotFlows.Find(CombinedId);
                if (data != null) return data.DateTime;
                return DateTime.Now;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return DateTime.Now;
            }
        }

        public string UpdateFlow(string CombinedId, string flowMessage)
        {
            try
            {
                BotFlow data = _contex.BotFlows.Find(CombinedId);
                if (data != null)
                {
                    data.currentFlow = flowMessage;
                    data.DateTime = DateTime.Now;
                }
                else
                {
                    data = new BotFlow
                    {
                        ID = CombinedId,
                        currentFlow = flowMessage,
                        DateTime = DateTime.Now
                    };
                    _contex.BotFlows.Add(data);
                }
                if (_contex.SaveChanges() == 1) return data.currentFlow;
                return "default";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "default";
            }  
        }

        public string UpdateFlowAppointment(string CombinedId, string flowMessage)
        {
            try
            {
                BotFlow data = _contex.BotFlows.Find(CombinedId);
                if (data != null)
                {
                    data.currentFlow = flowMessage;
                    data.DateTime = DateTime.Now;
                }
                else
                {
                    data = new BotFlow
                    {
                        ID = CombinedId,
                        currentFlow = flowMessage,
                        DateTime = DateTime.Now
                    };
                    _contex.BotFlows.Add(data);
                }
                if (_contex.SaveChanges() == 1) return data.currentFlow;
                return "appointment";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "appointment";
            }
        }

        public async Task<ChatbotInfo> GetBotInfoByBotKey(string botKey)
        {
            var data = await _contex.ChatbotInfos.Where(x => x.botKey == botKey).AsNoTracking().FirstOrDefaultAsync();

            return data;
        }
        public async Task<IEnumerable<BotRackInfoDetail>> GetBotRackInfoByBotKey(string botKey)
        {
            var data = await _contex.botRackInfoDetails.Include(x => x.master).Where(x => x.botKey == botKey).AsNoTracking().ToListAsync();

            return data;
        }
        public async Task<List<WrapperHeaderImg>> GetWrapperHeaderImageList(string botKey)
        {
            var data = await _contex.WrapperHeaderImgs.Where(x => x.botKey == botKey && x.isDelete != 1).Include(x => x.WrapperHeader).OrderBy(x => x.sortOrder).ToListAsync();

            return data;
        }

    }
}
