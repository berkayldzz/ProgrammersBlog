﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;


namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]  // Admin area içersinde olduğunu belirtmemiz gerek.
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly UserManager<User> _userManager;

        public HomeController(ICategoryService categoryService, IArticleService articleService, ICommentService commentService, UserManager<User> userManager)
        {
            _categoryService = categoryService;
            _articleService = articleService;
            _commentService = commentService;
            _userManager = userManager;
        }

        [Authorize(Roles = "SuperAdmin,AdminArea.Home.Read")]
        public async Task<IActionResult> Index()
        {
            var categoriesCountResult = await _categoryService.CountByNonDeletedAsync();
            var articlesCountResult = await _articleService.CountByNonDeletedAsync();
            var commentsCountResult = await _commentService.CountByNonDeletedAsync();
            var usersCount = await _userManager.Users.CountAsync();
            var articlesResult = await _articleService.GetAllAsync();
            if (categoriesCountResult.ResultStatus == ResultStatus.Success && articlesCountResult.ResultStatus == ResultStatus.Success && commentsCountResult.ResultStatus == ResultStatus.Success && usersCount > -1 && articlesResult.ResultStatus == ResultStatus.Success)
            {
                // Alınan veriler ve sonuç durumları, DashboardViewModel sınıfına atanan bir örnekle birlikte View'e iletilir.
                return View(new DashboardViewModel
                {
                    CategoriesCount = categoriesCountResult.Data,
                    ArticlesCount = articlesCountResult.Data,
                    CommentsCount = commentsCountResult.Data,
                    UsersCount = usersCount,
                    Articles = articlesResult.Data
                });
            }

            return NotFound();
        }
    }
}
