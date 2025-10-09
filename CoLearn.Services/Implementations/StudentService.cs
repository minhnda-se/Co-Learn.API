using AutoMapper;
using CoLearn.Domain.DTOs.Requests;
using CoLearn.Domain.DTOs.Responses;
using CoLearn.Domain.Interfaces;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Models;

namespace CoLearn.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateStudentAsync(StudentDtoRequest dto)
        {
            var existing = await _unitOfWork.StudentRepository.GetByUserIdAsync(dto.UserId);

            // Nếu đã tồn tại student active -> không cho tạo
            if (existing != null && existing.IsDeleted == false) return 0;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(dto.UserId);
            if (user != null)
            {
                user.FullName = dto.FullName;
                user.DateOfBirth = dto.Born;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.UserRepository.UpdateAsync(user);
            }

            // Update UserProfile
            var profile = await _unitOfWork.UserProfileRepository.GetUserProfileAsync(dto.UserId);
            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = dto.UserId,
                    AvatarUrl = dto.Photo

                };
                _unitOfWork.UserProfileRepository.Add(profile);
            }
            else
            {
                profile.AvatarUrl = dto.Photo;
                _unitOfWork.UserProfileRepository.Update(profile);
            }

            // Nếu có student đã bị xóa -> khôi phục lại
            if (existing != null && existing.IsDeleted == true)
            {
                _mapper.Map(dto, existing);
                existing.IsDeleted = false;
                existing.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.StudentRepository.UpdateAndSaveAsync(existing);
                return await _unitOfWork.CommitAsync();
            }

            // Nếu chưa có -> tạo mới
            var entity = _mapper.Map<Student>(dto);
            entity.CreatedAt = DateTime.UtcNow;

            return await _unitOfWork.StudentRepository.AddAndSaveAsync(entity);
        }

        public async Task<int> UpdateStudentAsync(StudentDtoRequest dto)
        {
            var existing = await _unitOfWork.StudentRepository.GetByUserIdAsync(dto.UserId);
            if (existing == null || existing.IsDeleted) return 0;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(dto.UserId);
            if (user != null)
            {
                user.FullName = dto.FullName;
                user.DateOfBirth = dto.Born;
                user.UpdatedAt = DateTime.UtcNow;
                
            }

            // Update UserProfile
            var profile = await _unitOfWork.UserProfileRepository.GetUserProfileAsync(dto.UserId);
            if (profile != null)
            {
                profile.AvatarUrl = dto.Photo;
            }

            _mapper.Map(dto, existing);
            existing.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.StudentRepository.UpdateAndSaveAsync(existing);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> DeleteStudentAsync(int studentId)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
            if (student == null) return 0;

            student.IsDeleted = true;
            student.DeletedAt = DateTime.UtcNow;

            return await _unitOfWork.StudentRepository.UpdateAndSaveAsync(student);
        }

        public async Task<List<StudentDtoResponse>> GetAllStudentsAsync()
        {
            var students = await _unitOfWork.StudentRepository.GetAllStudentsAsync();
            return _mapper.Map<List<StudentDtoResponse>>(students);
        }

        public async Task<StudentDtoResponse?> GetStudentByIdAsync(int studentId)
        {
            var student = await _unitOfWork.StudentRepository.GetStudentByIdAsync(studentId);
            return _mapper.Map<StudentDtoResponse?>(student);
        }

        public async Task<StudentDtoResponse?> GetStudentByUserIdAsync(int userId)
        {
            var student = await _unitOfWork.StudentRepository.GetByUserIdAsync(userId);
            return _mapper.Map<StudentDtoResponse?>(student);
        }
    }
}
