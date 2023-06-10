using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Entites.Dtos;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using ProgrammersBlog.Mvc.Helpers.Abstract;
using ProgrammersBlog.Shared.Utilities.Extensions;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : BaseController
    {
       
        private readonly RoleManager<Role> _roleManager;
        public RoleController(RoleManager<Role> roleManager, UserManager<User> userManager, IMapper mapper,IImageHelper imageHelper) : base(userManager, mapper, imageHelper)
        {
            _roleManager = roleManager;
        }
        // RoleManagerı kullanmaya hazırız.

        [Authorize(Roles ="SuperAdmin,Role.Read")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(new RoleListDto 
            {
                Roles=roles
            });
        }

        [Authorize(Roles = "SuperAdmin,Role.Read")]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles() // Yenile butonu için 
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleListDto = JsonSerializer.Serialize(new RoleListDto
            {
                Roles = roles
            });
            return Json(roleListDto);
        }
        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpGet]
        public async Task<IActionResult> Assign(int userId)
        {
            var user = await UserManager.Users.SingleOrDefaultAsync(u => u.Id == userId);  // Kullanıcı
            var roles = await _roleManager.Roles.ToListAsync();                            // Roller
            var userRoles = await UserManager.GetRolesAsync(user);                         // Kullanıcının sahip olduğu roller listelerini getirmiş olduk.

            // PartialView oluşturabilmek için UserRoleAssignDto'a ihtiyacımız var. UserRoleAssignDto içersinde 3 farklı değerimiz vardı.
            UserRoleAssignDto userRoleAssignDto = new UserRoleAssignDto
            {
                UserId = user.Id,
                UserName = user.UserName
            };
            foreach (var role in roles)   // Kullanıcın sahip olduğu roller seçili gelmeli.Rollerin içersinde dönüyoruz.
            {
                // Döngü içersinde yeni bir RoleAssignDto oluşturup rolün id,adı ve HasRole değerlerini vericez.
                RoleAssignDto rolesAssignDto = new RoleAssignDto   // Her bir rolü RoleAssignDto olarak oluşturduk.
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    HasRole = userRoles.Contains(role.Name)  // Rolün adı varsa bu role sahip demektir(true).
                };
                userRoleAssignDto.RoleAssignDtos.Add(rolesAssignDto);
            }

            return PartialView("_RoleAssignPartial", userRoleAssignDto);
        }

        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpPost]
        public async Task<IActionResult> Assign(UserRoleAssignDto userRoleAssignDto)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.Users.SingleOrDefaultAsync(u => u.Id == userRoleAssignDto.UserId);  // Hangi usera rol atayacağız.
                foreach (var roleAssignDto in userRoleAssignDto.RoleAssignDtos) // Rollerde dönüyoruz.
                {
                    if (roleAssignDto.HasRole)  // HasRole true ise o role sahiptir.
                        await UserManager.AddToRoleAsync(user, roleAssignDto.RoleName);
                    else                        // False ise o rol kullanıcıdan silinmesi gerek.
                    {
                        await UserManager.RemoveFromRoleAsync(user, roleAssignDto.RoleName);
                    }
                }

                await UserManager.UpdateSecurityStampAsync(user);

                var userRoleAssignAjaxViewModel = JsonSerializer.Serialize(new UserRoleAssignAjaxViewModel
                {
                    UserDto = new UserDto
                    {
                        User = user,
                        Message = $"{user.UserName} kullanıcısına ait rol atama işlemi başarıyla tamamlanmıştır.",
                        ResultStatus = ResultStatus.Success
                    },
                    RoleAssignPartial = await this.RenderViewToStringAsync("_RoleAssignPartial", userRoleAssignDto)
                });
                return Json(userRoleAssignAjaxViewModel);
            }
            else  // Modelstate yanlış gelirse
            {
                var userRoleAssignAjaxErrorModel = JsonSerializer.Serialize(new UserRoleAssignAjaxViewModel
                {
                    RoleAssignPartial = await this.RenderViewToStringAsync("_RoleAssignPartial", userRoleAssignDto),
                    UserRoleAssignDto = userRoleAssignDto
                });
                return Json(userRoleAssignAjaxErrorModel);
            }
        }
    }
}
