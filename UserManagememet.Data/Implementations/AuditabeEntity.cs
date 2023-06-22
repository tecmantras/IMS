using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagememet.Data.Interface;

namespace UserManagememet.Data.Implementations
{
    public class AuditabeEntity : IAuditabeEntity
    {
        [ScaffoldColumn(false)]
        public DateTime? CreationDate { get; set; }


        [ScaffoldColumn(false)]
        public string? CreatedBy { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? ModificationDate { get; set; }


        [ScaffoldColumn(false)]
        public string? ModifiedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
