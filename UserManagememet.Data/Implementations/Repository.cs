using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagememet.Data.Context;
using UserManagememet.Data.Interface;

namespace UserManagememet.Data.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private DbSet<T> _dbSet;
        private readonly UserManangementDBContext _context;
        public Repository(UserManangementDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        //public T Get<Tkey>(Tkey id)
        //{
        //    return _dbSet.Find(id);
        //}

        //public IQueryable<T> GetAll()
        //{
        //    return _dbSet.AsQueryable();
        //}

        //public async Task<T> GetByIdAsync<Tkey>(Tkey id)
        //{
        //    return await _dbSet.FindAsync(id);
        //}

        //public EntityState Add(T entity)
        //{
        //    return _dbSet.Add(entity).State;
        //}

        //public EntityState Update(T entity)
        //{
        //    return _dbSet.Update(entity).State;
        //}

        //public EntityState Delete(T entity)
        //{
        //    return _dbSet.Remove(entity).State;
        //}

        public T Get<Tkey>(Tkey id)
        {
            return _dbSet.Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T> GetByIdAsync<Tkey>(Tkey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public EntityState Add(T entity)
        {
            return _dbSet.Add(entity).State;
        }

        public EntityState Update(T entity)
        {
            return _dbSet.Update(entity).State;
        }

        public EntityState Delete(T entity)
        {
            return _dbSet.Remove(entity).State;
        }
    }
}
