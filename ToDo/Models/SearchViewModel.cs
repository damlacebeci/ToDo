using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDo.Models
{
    public class SearchViewModel
    {
        public string SearchText { get; set; }
        public bool showall { get; set; }
        public List<todoItem> Result { get; set; }
    }
}
