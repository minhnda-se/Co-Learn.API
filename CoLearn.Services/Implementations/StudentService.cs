using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> CreateStudentAsync(Student student)
        {
            return await _unitOfWork.StudentRepository.AddAndSaveAsync(student);
        }

        public async Task<int> DeleteStudentAsync(int studentId)
        {
            var student = _unitOfWork.StudentRepository.GetById(studentId);
            return await _unitOfWork.StudentRepository.RemoveAndSaveAsync(student);
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _unitOfWork.StudentRepository.GetAllStudentsAsync();
        }

        public async Task<Student> GetStudentByIdAsync(int studentId)
        {
            return await _unitOfWork.StudentRepository.GetStudentByIdAsync(studentId);
        }

        public async Task<int> UpdateStudentAsync(Student student)
        {
            return await _unitOfWork.StudentRepository.UpdateAndSaveAsync(student);
        }
    }
}
