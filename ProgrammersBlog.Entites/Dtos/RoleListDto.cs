using ProgrammersBlog.Entites.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.Dtos
{
    public class RoleListDto
    {
        public IList<Role> Roles { get; set; }
    }
}
