using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Mvc.Helpers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // Diğer controllerlar içersinde ortak olan özellikleri bir controllerda topluyoruz.

        public BaseController(UserManager<User> userManager, IMapper mapper, IImageHelper imageHelper)
        {
            UserManager = userManager;
            Mapper = mapper;
            ImageHelper = imageHelper;
        }

        protected UserManager<User> UserManager { get; }
        protected IMapper Mapper { get; }
        protected IImageHelper ImageHelper { get; }

        // LoggedInUser ın controller içersine geldiğinde otomatik olarak set edilmesini istiyoruz.
        // BaseControllerdan türemiş tüm controllerda login olmuş kullanıcının bilgisini LoggedInUser sayesinde alıcaz.
        protected User LoggedInUser => UserManager.GetUserAsync(HttpContext.User).Result;
    }
}
