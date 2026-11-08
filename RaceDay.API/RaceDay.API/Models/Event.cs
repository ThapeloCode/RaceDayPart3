using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        [Required]
        public string EventName { get; set; } = string.Empty;
        [Required]
        public string EventType { get; set; } = string.Empty; // Running, Walking, Cycling
        [Required]
        public DateTime EventDate { get; set; }
        [Required]
        public string Location { get; set; } = string.Empty;
        public int OrganiserId { get; set; }
    }
}