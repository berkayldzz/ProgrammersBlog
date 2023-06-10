using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProgrammersBlog.Entites.Dtos;

namespace ProgrammersBlog.Mvc.Areas.Admin.Models
{
    public class UserRoleAssignAjaxViewModel
    {
        public UserRoleAssignDto UserRoleAssignDto { get; set; }  // ModelState hatası alırsak geriye döneceğimiz model
        public string RoleAssignPartial{ get; set; }              // Hata alırsak geriye döneceğimiz model
        public UserDto UserDto { get; set; }                      // İşlemler başarılıysa geriye döneceğimiz model
    }
}
