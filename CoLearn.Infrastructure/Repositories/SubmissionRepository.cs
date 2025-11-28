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
    public class SubmissionRepository : GenericRepository<Submission>, ISubmissionRepository
    {
        private readonly AppDbContext _context;
        public SubmissionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Submission> CreateAsync(Submission submission)
        {
            var assignment = await _context.Assignments.FindAsync(submission.AssignmentId);
            if (assignment == null || assignment.IsDeleted)
            { 
            throw new InvalidOperationException("Assignment not found");
            }
            if (assignment.DueDate.HasValue && submission.SubmittedAt > assignment.DueDate.Value)
            {
                throw new InvalidOperationException("Deadline has passed");
            }
            _context.Submissions.Add(submission);
            //await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<List<Submission>> GetByAssignmentIdAsync(int assignmentId)
        {
            return await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .Where(s => s.AssignmentId == assignmentId && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<Submission?> GetByIdAsync(long id)
        {
            return await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.SubmissionId == id && !s.IsDeleted);
        }

        public async Task<List<Submission>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .Where(s => s.StudentId == studentId && !s.IsDeleted)
                .ToListAsync();
        }

        public async Task<Submission?> UpdateAsync(Submission submission)
        {
            var existingSubmission = await _context.Submissions.FindAsync(submission.SubmissionId);
            if (existingSubmission == null || existingSubmission.IsDeleted)
                return null;
            _context.Entry(existingSubmission).CurrentValues.SetValues(submission);

            //await _context.SaveChangesAsync();
            return existingSubmission;
        }
    }
}
