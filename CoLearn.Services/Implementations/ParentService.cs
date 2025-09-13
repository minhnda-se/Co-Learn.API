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
    public class ParentService : IParentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ParentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> CreateParentAsync(Parent parent)
        {
            return await _unitOfWork.ParentRepository.AddAndSaveAsync(parent);
        }

        public async Task<int> DeleteParentAsync(int parentId)
        {
            var parent = _unitOfWork.ParentRepository.GetById(parentId);
            return await _unitOfWork.ParentRepository.RemoveAndSaveAsync(parent);
        }

        public async Task<List<Parent>> GetAllParentsAsync()
        {
            return await _unitOfWork.ParentRepository.GetAllParentsAsync();
        }

        public async Task<Parent> GetParentByIdAsync(int parentId)
        {
           return await _unitOfWork.ParentRepository.GetParentByIdAsync(parentId);
        }

        public async Task<int> UpdateParentAsync(Parent parent)
        {
            return await _unitOfWork.ParentRepository.UpdateAndSaveAsync(parent);
        }
    }
}
