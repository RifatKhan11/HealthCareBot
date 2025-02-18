namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class BotMessageHistory:BotBase
    {
        public string userId {  get; set; }
        public string wraperdText {  get; set; }
        public int? wraperdId { get; set; }
        public string masseges {  get; set; }
        public int messagesType {  get; set; }// 1=send,2=rcv
        public string connectionId {  get; set; }
    }
}
