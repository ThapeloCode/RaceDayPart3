namespace RaceDay.Web.DTOs
{
    public class EventSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public int EnrolmentCount { get; set; }
    }
}