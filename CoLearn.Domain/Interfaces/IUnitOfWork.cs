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
        ISubmissionRepository SubmissionRepository { get; }
        IScheduleRepository ScheduleRepository { get; }

        IBookingRepository BookingRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        // Generic Repositories

        IGenericRepository<User> UserGenericRepository { get; }

        //  Repositories 
      


        // Commit tất cả thay đổi
        Task<int> CommitAsync();
    }
}
