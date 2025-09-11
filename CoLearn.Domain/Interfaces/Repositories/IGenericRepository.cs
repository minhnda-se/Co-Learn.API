using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        #region Query
        IQueryable<T> GetAllQuery();           // Cho phép filter bằng LINQ trước khi query DB
        Task<List<T>> GetAllAsync();           // Lấy tất cả record
        T GetById(object id);                  // Lấy theo ID
        Task<T> GetByIdAsync(object id);       // Async
        #endregion

        #region Add
        void Add(T entity);                    // Chỉ thêm vào memory (dùng với UnitOfWork)
        Task<int> AddAndSaveAsync(T entity);        // Thêm và commit ngay
        #endregion

        #region Update
        void Update(T entity);                 // Chỉ update state (dùng với UnitOfWork)
        Task<int> UpdateAndSaveAsync(T entity);     // Update và commit ngay
        #endregion

        #region Remove
        void Remove(T entity);                 // Chỉ xóa state
        Task<int> RemoveAndSaveAsync(T entity);     // Xóa và commit ngay
        #endregion
    }
}
