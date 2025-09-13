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
        ITeacherRepository TeacherRepository { get; }
        IStudentRepository StudentRepository { get; }
        IParentRepository ParentRepository { get; }

        // Commit tất cả thay đổi
        Task<int> CommitAsync();
    }
}
