using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserManagememet.Data.Interface;
using UserManagememet.Data.Model;

namespace UserManagememet.Data.Context
{
    public class UserManangementDBContext : IdentityDbContext<User>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserManangementDBContext(DbContextOptions<UserManangementDBContext> options,IHttpContextAccessor httpContextAccessor):base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
            base.OnConfiguring(optionsBuilder);
        }
        public override int SaveChanges()
        {
            var modifiedEntites = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditabeEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entity in modifiedEntites)
            {
                if(entity.Entity is not IAuditabeEntity auditabeEntity)
                {
                    continue;
                }
                var userId = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var time = DateTime.Now;
                if(entity.State == EntityState.Added)
                {
                    auditabeEntity.CreatedBy = userId;
                    auditabeEntity.CreationDate = time;
                }
                else
                {
                    Entry(auditabeEntity).Property(x => x.CreationDate).IsModified = false;
                    Entry(auditabeEntity).Property(x => x.CreatedBy).IsModified = false;
                    auditabeEntity.ModifiedBy = userId;
                    auditabeEntity.ModificationDate = time;
                }
            }
            return base.SaveChanges();
        }

        public DbSet<Department> Departments { get; set; }
    }
}
