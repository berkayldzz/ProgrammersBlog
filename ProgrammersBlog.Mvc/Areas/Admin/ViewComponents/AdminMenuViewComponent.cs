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
    public class AdminMenuViewComponent:ViewComponent
    {
        // Her bir viewcomponentın bir tane Invoke metoduna ihtiyacı vardır.

        // Bu menünün kullanıcıya ve rollere göre tepki vermesini istiyoruz.O yüzden UserManagerı çağırıyoruz.

        private readonly UserManager<User> _userManager;

        public AdminMenuViewComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);  // HttpContext.User ile login olan kullanıcıyı alıyoruz.
            var roles = await _userManager.GetRolesAsync(user);            // Kullanıcıya ait rolleri alıyoruz.
            if (user == null)
                return Content("Kullanıcı Bulunamadı");
            if (roles == null)
                return Content("Roller Bulunamadı");

            // Burdaki viewı içersindeki model ile return ediyoruz.
            return View(new UserWithRolesViewModel   // UserWithRolesViewModel modelimiz içinde kullanıcı ve rollerimizi tutuyoruz.
            {
                User = user,
                Roles = roles
            });
        }
    }
}
