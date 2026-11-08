namespace RaceDay.Web.DTOs
{
    public class EnrolmentRequestDto
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
    }

    public class ParticipantEnrolmentDto
    {
        public int Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
    }

    public class ParticipantResultDto
    {
        public int Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string FinishTime { get; set; } = string.Empty;
        public int Position { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}