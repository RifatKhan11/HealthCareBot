namespace Opus_ChatBot_HealthCare_8.Models.BotModels
{
    public class DoctorVisitTimePeriod:BotBase
    {
        public int? doctorInfoId { get; set; }
        public virtual DoctorInfo doctorInfo { get; set; }
        public int? timePeriodId { get; set; }
        public virtual TimePeriod timePeriod { get; set; }
        public int? weeksId { get; set; }
        public virtual Weeks weeks { get; set; }
        public int? status { get; set; }
    }
}
