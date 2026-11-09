namespace RaceDay.Web.Models
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int OrganiserId { get; set; }
    }
}