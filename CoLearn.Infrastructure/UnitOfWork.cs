using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Infrastructure.Context;
using CoLearn.Domain.Models;
using CoLearn.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // backing fields cho lazy init
       
        private IUserProfileRepository _userProfileRepository;
        private ITeacherRepository _teacherRepository;
        private IStudentRepository _studentRepository;
        private IParentRepository _parentRepository;
        private ICourseRepository _courseRepository;
        private ILessonRepository _lessonRepository;
        private IAssignmentRepository _assignmentRepository;
        private ICourseMaterialRepository _courseMaterialRepository;
        private IEnrollmentRepository _enrollmentRepository;
        private IBookingRepository _bookingRepository;

        private ISubmissionRepository _submissionRepository;
        private IScheduleRepository _scheduleRepository;
        private IUserRepository _userRepository;
        private IGenericRepository<User> _userGenericRepository;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        // Custom repository
        public IUserProfileRepository UserProfileRepository
            => _userProfileRepository ??= new UserProfileRepository(_context);
        public ITeacherRepository TeacherRepository 
            => _teacherRepository ??= new TeacherRepository(_context);


        public IStudentRepository StudentRepository 
            => _studentRepository ??= new StudentRepository(_context);

        public IParentRepository ParentRepository 
            => _parentRepository ??= new ParentRepository(_context);
       public ICourseRepository CourseRepository
            => _courseRepository ??= new CourseRepository(_context);
        public ILessonRepository LessonRepository
            => _lessonRepository ??= new LessonRepository(_context);
        public IAssignmentRepository AssignmentRepository
            => _assignmentRepository ??= new AssignmentRepository(_context);
        public ICourseMaterialRepository CourseMaterialRepository
            => _courseMaterialRepository ??= new CourseMaterialRepository(_context);
        public IEnrollmentRepository EnrollmentRepository
            => _enrollmentRepository ??= new EnrollmentRepository(_context);
        public IBookingRepository BookingRepository
            => _bookingRepository ??= new BookingRepository(_context);
        public IUserRepository UserRepository
            => _userRepository ??= new UserRepository(_context);

        public ISubmissionRepository SubmissionRepository
            => _submissionRepository ??= new SubmissionRepository(_context);

        public IScheduleRepository ScheduleRepository 
            => _scheduleRepository ??= new ScheduleRepository(_context);
        // Generic repository

        public IGenericRepository<User> UserGenericRepository
            => _userGenericRepository ??= new GenericRepository<User>(_context);


        // Commit tất cả thay đổi
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose DbContext
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
