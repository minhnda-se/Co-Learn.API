using CoLearn.Domain.Interfaces.Repositories;
using CoLearn.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoLearn.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Query

        public IQueryable<T> GetAllQuery()
        {
            return _context.Set<T>().AsQueryable();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public T GetById(object id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        #endregion

        #region Add

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
            // ❌ Không commit, để UnitOfWork quyết định
        }

        public async Task<int> AddAndSaveAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            return await _context.SaveChangesAsync(); // commit ngay
        }

        #endregion

        #region Update

        public void Update(T entity)
        {
            //_context.ChangeTracker.Clear();
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            // ❌ Không commit, để UnitOfWork quyết định
        }

        public async Task<int> UpdateAndSaveAsync(T entity)
        {
            //_context.ChangeTracker.Clear();
            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return await _context.SaveChangesAsync(); // commit ngay
        }

        #endregion

        #region Remove

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
            // ❌ Không commit
        }

        public async Task<int> RemoveAndSaveAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return await _context.SaveChangesAsync(); // commit ngay
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
    => await _context.Set<T>().FirstOrDefaultAsync(predicate);
        #endregion
    }
}
