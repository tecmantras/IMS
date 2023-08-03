using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagememet.Data.ViewModel
{
    public class SearchViewModel
    {
        public string SearchValue { get;set; }
        public int Page { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 10;
    }
}
