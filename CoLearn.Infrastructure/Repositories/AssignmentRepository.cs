using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Models;
using CoLearn.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
    {
        public AssignmentRepository(AppDbContext context) : base(context) { }

        public async Task<List<Assignment>> GetByLessonIdAsync(int lessonId)
        {
            return await _context.Assignments
                .Where(a => a.LessonId == lessonId && !a.IsDeleted)
                .Include(a => a.Submissions)
                .Include(a => a.Lesson)
                .ToListAsync();
        }

        public async Task<Assignment?> GetByIdAsync(int id)
        {
            return await _context.Assignments
                .Include(a => a.Lesson)
                .Include(a => a.Submissions)
                .FirstOrDefaultAsync(a => a.AssignmentId == id && !a.IsDeleted);
        }
        public async Task<List<Submission>> GetSubmissionsByAssignmentIdAsync(int assignmentId)
        {
            return await _context.Submissions
                .Where(s => s.AssignmentId == assignmentId && !s.IsDeleted)
                // eager load Student + User
                .Include(s => s.Student)
                    .ThenInclude(st => st.User)
                // eager load Assignment
                .Include(s => s.Assignment)
                .ToListAsync();
        }



        public async Task<Submission?> UpdateFeedbackAsync(long submissionId, decimal? grade, string? feedback)
        {
            var submission = await _context.Submissions
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId && !s.IsDeleted);

            if (submission == null)
                return null;

            submission.Grade = grade;
            submission.Feedback = feedback;

            await _context.SaveChangesAsync();
            return submission;
        }
    }
}
