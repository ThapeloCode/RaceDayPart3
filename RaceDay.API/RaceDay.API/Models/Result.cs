using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Models
{
    public class Result
    {
        [Key]
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int Position { get; set; }
    }
}