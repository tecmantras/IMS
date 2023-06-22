using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagememet.Data.Interface
{
    public interface IAuditabeEntity
    {
        DateTime? CreationDate { get; set; }
        string? CreatedBy { get; set; }
        DateTime? ModificationDate { get; set; }
        string? ModifiedBy { get; set; }
        bool IsDeleted { get; set; }
        bool IsActive { get; set; }
    }
}
