using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface IParentRepository : IGenericRepository<Parent>
    {
        Task<Parent> GetParentByIdAsync(int parentId);
        Task<List<Parent>> GetAllParentsAsync();
        //Task<int> CreateParentAsync(Parent parent);
        //Task<int> UpdateParentAsync(Parent parent);
        //Task<int> DeleteParentAsync(int parentId);
    }
}
