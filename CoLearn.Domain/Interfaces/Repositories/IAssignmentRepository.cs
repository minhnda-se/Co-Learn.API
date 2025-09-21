using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface IAssignmentRepository : IGenericRepository<Assignment>
    {
        Task<List<Assignment>> GetByLessonIdAsync(int lessonId);
        Task<Assignment?> GetByIdAsync(int id);
        Task<List<Submission>> GetSubmissionsByAssignmentIdAsync(int assignmentId);
        Task<Submission?> UpdateFeedbackAsync(long submissionId, decimal? grade, string? feedback);
    }
}
