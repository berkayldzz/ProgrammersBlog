using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Areas.Admin.ViewComponents
{
    public class UserMenuViewComponent:ViewComponent
    {
        private readonly UserManager<User> _userManager;

        public UserMenuViewComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);   // HttpContext.User ile login olan kullanıcıyı alıyoruz.

            if (user == null)
                return Content("Kullanıcı Bulunamadı");

            // userı direkt olarak viewa göndermek istemiyoruz.Dolayısıyla bir viewmodel oluşturucaz.
            // İlerde daha farklı detaylar eklenmek istendiğinde UserViewModel üzerinde eklemeler yapabilicez.

            return View(new UserViewModel
            {
                User = user
            });
        }
    }
}
