namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IFileService
    {
        Task<bool> SaveNewFile(string FileName, string FileType, int FbPageId);

        Task<IEnumerable<Models.BotModels.File>> GetAllFiles(int FbPageId);

    }
}
