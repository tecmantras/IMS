using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagememet.Data.Interface;

namespace UserManagement.Services.IRepositories
{
    public interface IService<TEntity>
    {
        //T Create(T entity);
        //T Update(T entity);
        //Task<IEnumerable<T>> GetAllAsync();
        //Task<T> GetByIdAsync(int id);
        //bool SoftDelete<Tkey>(Tkey id);
        //bool HardDelete<Tkey>(Tkey id);

        TEntity Create(TEntity entity);
        TEntity Update(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync<Tkey>(Tkey id);
        bool SoftDelete<Tkey>(Tkey id);
        bool HardDelete<Tkey>(Tkey id);
    }
}
