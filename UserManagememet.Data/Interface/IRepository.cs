using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagememet.Data.Interface
{
    public interface IRepository<T> 
    {
        //T Get<Tkey>(Tkey id);
        //IQueryable<T> GetAll();
        //Task<T> GetByIdAsync<Tkey>(Tkey id);
        //EntityState Add(T entity);
        //EntityState Update(T entity);
        //EntityState Delete(T entity);

        T Get<Tkey>(Tkey id);
        IQueryable<T> GetAll();
        Task<T> GetByIdAsync<Tkey>(Tkey id);
        EntityState Add(T entity);
        EntityState Update(T entity);
        EntityState Delete(T entity);
    }
}
