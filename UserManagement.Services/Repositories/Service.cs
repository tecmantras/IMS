using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagememet.Data.Implementations;
using UserManagememet.Data.Interface;
using UserManagement.Services.IRepositories;

namespace UserManagement.Services.Repositories
{
    //public class Service<T> : IService<T> where T : class, IAuditabeEntity
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private readonly IRepository<T> _repository;

    //    public Service(IUnitOfWork unitOfWork)
    //    {
    //        _unitOfWork = unitOfWork;
    //        _repository = _unitOfWork.GetRepository<T>();
    //    }

    //    public T Create(T entity)
    //    {
    //        _ = _repository.Add(entity);
    //        _ = _unitOfWork.commit();
    //        return entity;
    //    }

    //    public async Task<IEnumerable<T>> GetAllAsync()
    //    {
    //        return await _repository.GetAll().OrderByDescending(x => x.CreationDate).ToListAsync();
    //    }

    //    public async Task<T> GetByIdAsync(int id)
    //    {
    //        return await _repository.GetByIdAsync(id);
    //    }

    //    public bool HardDelete<Tkey>(Tkey id)
    //    {
    //        var data = _repository.Get(id);
    //        _ = _repository.Delete(data);
    //        _ = _unitOfWork.commit();
    //        return true;
    //    }

    //    public bool SoftDelete<Tkey>(Tkey id)
    //    {
    //        var data = _repository.Get(id);
    //        if (data is IAuditabeEntity)
    //        {
    //            data.GetType().GetProperty("IsDeleted").SetValue(data, true);
    //        }
    //        _ = _repository.Update(data);
    //        return true;
    //    }

    //    public T Update(T entity)
    //    {
    //        _ = _repository.Update(entity);
    //        _ = _unitOfWork.commit();
    //        return entity;
    //    }
    //}

    public class Service<TEntity> : IService<TEntity> where TEntity : class, IAuditabeEntity
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly IRepository<TEntity> repository;
        public Service(IUnitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
            repository = _unitofWork.GetRepository<TEntity>();
        }
        public virtual TEntity Create(TEntity entity)
        {
            _ = repository.Add(entity);
            _ = _unitofWork.commit();
            return entity;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await repository.GetAll().Where(t => !t.IsDeleted).OrderByDescending(t => t.CreationDate).ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync<Tkey>(Tkey id)
        {
            return await repository.GetByIdAsync(id);
        }

        public virtual bool HardDelete<Tkey>(Tkey id)
        {
            var data = repository.Get(id);
            _ = repository.Delete(data);
            _ = _unitofWork.commit();
            return true;
        }

        public virtual bool SoftDelete<Tkey>(Tkey id)
        {
            var data = repository.Get(id);
            if (data is IAuditabeEntity)
            {
                data.GetType().GetProperty("IsDeleted").SetValue(data, true);
            }
            _ = repository.Update(data);
            return true;
        }

        public virtual TEntity Update(TEntity entity)
        {
            _ = repository.Update(entity);
            _ = _unitofWork.commit();
            return entity;
        }
    }
}
