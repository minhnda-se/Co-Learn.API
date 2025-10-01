using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Domain.Models;
using System;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        //  Repositories 
        IUserProfileRepository UserProfileRepository { get; }
        IUserRepository UserRepository { get; }
        ITeacherRepository TeacherRepository { get; }
        IStudentRepository StudentRepository { get; }
        IParentRepository ParentRepository { get; }
        ICourseRepository CourseRepository { get; }
        ILessonRepository LessonRepository { get; }
        IAssignmentRepository AssignmentRepository { get; }
        ICourseMaterialRepository CourseMaterialRepository { get; }
        IEnrollmentRepository EnrollmentRepository { get; }
<<<<<<< HEAD
        ISubmissionRepository SubmissionRepository { get; }
        IScheduleRepository ScheduleRepository { get; }

=======
        IBookingRepository BookingRepository { get; }
>>>>>>> 4d1b3b33982aedec6c347bdf4ae48c36c8096abe
        // Generic Repositories
        IGenericRepository<User> UserGenericRepository { get; }

        //  Repositories 
      


        // Commit tất cả thay đổi
        Task<int> CommitAsync();
    }
}
