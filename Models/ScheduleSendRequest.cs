namespace EmailApplication.Models
{
    public class ScheduleSendRequest
    {
        public string Id { get; set; }
        public List<string> EmailIds { get; set; }
        public DateTime ScheduleDate { get; set; }
    }
}
