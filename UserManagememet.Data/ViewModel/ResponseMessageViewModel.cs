using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagememet.Data.ViewModel
{
    public class ResponseMessageViewModel
    {
        public object? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}
