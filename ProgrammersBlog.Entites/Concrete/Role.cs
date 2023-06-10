using Microsoft.AspNetCore.Identity;
using ProgrammersBlog.Shared.Entites.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.Concrete
{
    public class Role:IdentityRole<int>   // primary keyi int olsun.
    {
      
    }
}
