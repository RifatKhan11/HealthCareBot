namespace Opus_ChatBot_HealthCare_8.Services.IServices
{
    public interface IAPIFunction
    {
        string Single_Sms(string phone, string msg);
        string Bulk_Sms();
        string Dynamic_Sms();
    }
}
