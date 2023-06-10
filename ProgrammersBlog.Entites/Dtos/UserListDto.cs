using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Shared.Entites.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.Dtos
{
    public class UserListDto:DtoGetBase
    {
        public IList<User> Users { get; set; }

    }
}
