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
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TeacherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> CreateTeacherAsync(Teacher teacher)
        {
            return await _unitOfWork.TeacherRepository.AddAndSaveAsync(teacher);
        }

        public async Task<int> DeleteTeacherAsync(int teacherId)
        {
            var profile = _unitOfWork.TeacherRepository.GetById(teacherId);
            return await _unitOfWork.TeacherRepository.RemoveAndSaveAsync(profile);
        }

        public async Task<List<Teacher>> GetAllTeachersAsync()
        {
            return await _unitOfWork.TeacherRepository.GetAllTeachersAsync();
        }

        public async Task<Teacher> GetTeacherByIdAsync(int teacherId)
        {
            return await _unitOfWork.TeacherRepository.GetTeacherByIdAsync(teacherId);
        }

        public async Task<int> UpdateTeacherAsync(Teacher teacher)
        {
            return await _unitOfWork.TeacherRepository.UpdateAndSaveAsync(teacher);
        }
    }
}
