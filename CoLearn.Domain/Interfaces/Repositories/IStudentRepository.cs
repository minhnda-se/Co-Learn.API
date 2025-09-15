using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student> GetStudentByIdAsync(int studentId);
        Task<Student> GetByUserIdAsync(int teacherId);
        Task<List<Student>> GetAllStudentsAsync();
        //Task<int> CreateStudentAsync(Student student);
        //Task<int> UpdateStudentAsync(Student student);
        //Task<int> DeleteStudentAsync(int studentId);
    }
}
