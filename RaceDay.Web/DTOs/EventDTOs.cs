using System.ComponentModel.DataAnnotations;

namespace RaceDay.Web.DTOs
{
    public class EventDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? BannerImageUrl { get; set; }
        public int OrganiserId { get; set; }
        public int EnrolmentCount { get; set; }
    }

    public class CreateEventDto
    {
        [Required(ErrorMessage = "Event name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; } = DateTime.Now.AddDays(7);

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Distance is required.")]
        [Range(0.1, 1000.0, ErrorMessage = "Distance must be greater than 0.")]
        public double Distance { get; set; }

        [Required(ErrorMessage = "Event type is required.")]
        public string EventType { get; set; } = "Running"; // Running, Cycling, Walking
    }

    public class OrganiserDashboardDto
    {
        public int TotalEvents { get; set; }
        public int TotalEnrolments { get; set; }
        public List<EventDto> UpcomingEvents { get; set; } = new();
        public List<EventDto> AllEvents { get; set; } = new();
    }
}