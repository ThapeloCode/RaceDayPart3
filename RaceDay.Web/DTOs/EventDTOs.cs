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

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal MinAge { get; set; }
        public decimal MaxAge { get; set; }
        public decimal Price { get; set; }
        public int EventId { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public decimal MinAge { get; set; } = 0;
        public decimal MaxAge { get; set; } = 120;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 10000, ErrorMessage = "Price must be a valid amount.")]
        public decimal Price { get; set; }

        public int EventId { get; set; }
    }

    public class EventEnrolmentDto
    {
        public int EnrolmentId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string ParticipantEmail { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}