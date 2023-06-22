using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagememet.Data.Interface;
using UserManagement.Services.IRepositories;

namespace UserManagement.Services.Repositories
{
    public class GenericServices<TEntity> : IGenericServices<TEntity> where TEntity : class, IAuditabeEntity
    {
    }
}
