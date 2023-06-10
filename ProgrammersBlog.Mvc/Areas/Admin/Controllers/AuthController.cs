using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Entites.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            if (ModelState.IsValid)   // Kullanıcı yanlış bir bilgi gönderdiyse bunu kontrol etmemiz gerekir.
            {
                var user = await _userManager.FindByEmailAsync(userLoginDto.Email);   // Öncelikle buraya bir user getirmemiz gerek.E posta adrsi ile kullanıcıyı aldık.
                if (user != null)  // eğer kullanıcı null değilse demek ki böyle bir kullanıcı var demektir.
                {

                    var result = await _signInManager.PasswordSignInAsync(user, userLoginDto.Password, userLoginDto.RememberMe, false);  // Şifreyle giriş yapmak istiyoruz.

                    // result değişkeni, oturum açma işleminin sonucunu temsil eder ve bir SignInResult nesnesi olarak döner. Bu nesne, oturum açma işleminin başarılı olup olmadığını ve ilgili hataları içerebilir.
                    if (result.Succeeded)  // Kullanıcı giriş yapmakta başarılı olduysa.
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "E-posta adresiniz veya şifreniz yanlıştır.");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "E-posta adresiniz veya şifreniz yanlıştır.");
                    return View();
                }
            }
            else
            {
                return View();
            }

        }
       
        [Authorize]
        [HttpGet]
        public ViewResult AccessDenied()
        {
            return View();  // Kendi adındaki viewı(AccessDenied ı ) döndürür.
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { Area = "" });  // Blog sayfasının bulunduğu home controller olucak.
        }
    }
}
