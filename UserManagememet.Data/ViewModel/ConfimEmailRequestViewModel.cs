using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagememet.Data.ViewModel
{
    public class ConfimEmailRequestViewModel
    {
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }

        public string UserName { get; set; }
        public string ConfirmEmailLink { get; set; }
        public string ConfirmPasswordLink { get;  set; }

    }
}
