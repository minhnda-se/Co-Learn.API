namespace CoLearn.Domain.DTOs
{
    public class SubmissionRequestDto
    {
        public int StudentId { get; set; }
        public decimal? Grade { get; set; }
        public string? Feedback { get; set; }
        public string? FilePath { get; set; }
    }

    public class SubmissionResponseDto
    {
        public long SubmissionId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public decimal? Grade { get; set; }
        public string? Feedback { get; set; }
        public string? FilePath { get; set; }

        // Profile Student
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }

        // Profile Assignment
        public int AssignmentId { get; set; }
        public string? Title { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class SubmissionFeedbackRequestDto
    {
        public decimal? Grade { get; set; }
        public string? Feedback { get; set; }
    }
}
