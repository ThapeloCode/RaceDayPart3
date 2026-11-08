using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; } = string.Empty; // Open, Junior, Veteran
        public int EventId { get; set; }
    }
}