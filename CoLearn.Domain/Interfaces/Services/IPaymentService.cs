using CoLearn.Domain.Common;
using CoLearn.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<Result<CreateResponse>> CreateAsync(CreateRequest dto);
        Task<Result<Detail?>> GetByIdAsync(long id);
        Task<Result<bool>> HandleVnPayReturnAsync(IQueryCollection query);
        Task<Result<IEnumerable<Detail>>> GetAllAsync();
    }
}
