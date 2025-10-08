using AutoMapper;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Interfaces.Services;
using CoLearn.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Services.Handler
{
    public class UserHandler
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteIfNotVerifiedAsync(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user != null)
            {
                await _unitOfWork.UserRepository.DeleteAsync(userId);
                Console.WriteLine($"[Cleanup] Deleted unverified user #{userId}");
            }
        }
    }
}
