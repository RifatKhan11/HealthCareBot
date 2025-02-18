namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class GetTimeSlotsByDoctorIdVm
    {
        public int Id { get; set; }
        public string shiftPeriod { get; set; }
        public string timeSlot { get; set; }
        public int Sunday { get; set; }
        public int Monday { get; set; }
        public int TuesDay { get; set; }
        public int Wednesday { get; set; }
        public int Thursday { get; set; }
        public int Friday { get; set; }
        public int Saturday { get; set; }
    }
}
