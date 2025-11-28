using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface ISubmissionRepository
    {
        Task<Submission> CreateAsync(Submission submission);
        Task<List<Submission>> GetByAssignmentIdAsync(int assignmentId);
        Task<List<Submission>> GetByStudentIdAsync(int studentId);
        Task<Submission?> GetByIdAsync(long id);
        Task<Submission?> UpdateAsync(Submission submission);
    }
}
