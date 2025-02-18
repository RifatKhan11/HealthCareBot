using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Opus_ChatBot_HealthCare_8.Data;
using Opus_ChatBot_HealthCare_8.Models.BotModels;
using Opus_ChatBot_HealthCare_8.Services.IServices;


namespace Opus_ChatBot_HealthCare_8.Services
{
    public class FileService: IFileService
    {
        private readonly ApplicationDbContext _contex;

        public FileService(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        public async Task<IEnumerable<Models.BotModels.File>> GetAllFiles(int FbPageId)
        {
           return  await _contex.Files.Where(x => x.FacebookPageId == FbPageId).AsNoTracking().ToListAsync();
        }

        public async Task<bool> SaveNewFile(string FileName, string FileType, int FbPageId)
        {
            Models.BotModels.File file = new Models.BotModels.File
            {
                Name = FileName,
                Type = FileType,
                FacebookPageId = FbPageId
            };

            _contex.Files.Add(file);

            return 1 == await _contex.SaveChangesAsync();
        }
    }
}
