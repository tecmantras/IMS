using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagememet.Data.ViewModel
{
    public class EmailConfigurationViewModel
    {
        public string SmtpType { get; set; }
        public int SmtpPort { get; set; }
        public string EmailSender { get; set; }
        public string Password { get; set; }
        public bool IsSsl { get; set; }
    }
}
