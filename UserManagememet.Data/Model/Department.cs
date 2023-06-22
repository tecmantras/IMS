using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagememet.Data.Implementations;

namespace UserManagememet.Data.Model
{
    public class Department : AuditabeEntity
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
    }
}
