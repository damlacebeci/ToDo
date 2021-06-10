using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDo.Models
{
    public class CetUser : IdentityUser 
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public virtual List<todoItem> TodoItems { get; set; }
    }
}
