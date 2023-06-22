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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserManangementDBContext dbontext;
        private Dictionary<Type, object> repositories;

        public UnitOfWork(UserManangementDBContext context)
        {
           dbontext = context;
        }

        //public int commit()
        //{
        //    return dbontext.SaveChanges();
        //}


        //public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        //{
        //    repositories ??= new Dictionary<Type, object>();
        //    var type = typeof(TEntity);
        //    if (!repositories.ContainsKey(type))
        //    {
        //        repositories[type] = new Repository<TEntity>(dbontext);
        //    }
        //    return (IRepository<TEntity>)repositories[type];
        //}

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            repositories ??= new Dictionary<Type, object>();
            var type = typeof(TEntity);

            if (!repositories.ContainsKey(type))
            {
                repositories[type] = new Repository<TEntity>(dbontext);
            }
            return (IRepository<TEntity>)repositories[type];
        }
        public int commit()
        {
            return dbontext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(obj: this);
        }
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbontext != null)
                {
                    dbontext.Dispose();
                    //dbontext = null;
                }
            }
        }
    }
}
