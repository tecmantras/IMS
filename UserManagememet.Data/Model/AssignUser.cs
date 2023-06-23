using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagememet.Data.Implementations;
using UserManagememet.Data.Interface;

namespace UserManagememet.Data.Model
{
    public class AssignUser : AuditabeEntity
    {
        public int AssignUserId { get; set; }
        public string UserId { get; set; }
        public string AssignedManagerId { get; set; }
        public string? AssignedHrId { get; set; }

        public virtual User User { get; set; }
        public virtual User AssignedManager { get; set; }
        public virtual User? AssignedHr { get; set; }

    }
}
