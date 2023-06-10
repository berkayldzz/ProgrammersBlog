using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using ProgrammersBlog.Entites.ComplexTypes;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Entites.Dtos;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using ProgrammersBlog.Mvc.Helpers.Abstract;
using ProgrammersBlog.Shared.Utilities.Extensions;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : BaseController
    {
        // Kullanıcılarla ilgili işlemlerimizde userManager sınıfımız kullanıcaz.(BaseController)

        private readonly SignInManager<User> _signInManager;
        private readonly IToastNotification _toastNotification;

        public UserController(UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager, IImageHelper imageHelper, IToastNotification toastNotification) : base(userManager, mapper, imageHelper)
        {
            _signInManager = signInManager;
            _toastNotification = toastNotification;
        }

        [Authorize(Roles = "SuperAdmin,User.Read")]
        public async Task<IActionResult> Index()
        {
            var users = await UserManager.Users.ToListAsync();    // Bu işlem tüm kullanıcıları bir liste halinde döncektir.

            return View(new UserListDto       // Bu listeyi userlistdto içersine attıktan sonra view içersinde return ediyor olacağız.
            {
                Users = users,                            // UserListDto nun istediği Users değerini _userManager üzerinden aldık ve UserListDto içersine eklemiş olduk.
                ResultStatus = ResultStatus.Success       // UserListDto miras aldığı DtoGetBase in istediği ResultStatus değerini de verdik.
            });
        }

        [Authorize(Roles = "SuperAdmin,User.Read")]
        [HttpGet]
        public async Task<PartialViewResult> GetDetail(int userId)
        {
            var user = await UserManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            return PartialView("_GetDetailPartial", new UserDto { User = user });
        }


        // Yenileme işlemi için 

        [Authorize(Roles = "SuperAdmin,User.Read")]
        [HttpGet]

        public async Task<JsonResult> GetAllUsers()
        {
            var users = await UserManager.Users.ToListAsync();
            var userListDto = JsonSerializer.Serialize(new UserListDto        // Json formatında UserListDto dönücek.
            {
                Users = users,
                ResultStatus = ResultStatus.Success
            }, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return Json(userListDto);
        }
        [Authorize(Roles = "SuperAdmin,User.Create")]
        [HttpGet]
        public IActionResult Add()
        {
            return PartialView("_UserAddPartial");
        }
        [Authorize(Roles = "SuperAdmin,User.Create")]
        [HttpPost]
        public async Task<IActionResult> Add(UserAddDto userAddDto)
        {
            if (ModelState.IsValid)                                            // Modelstate validse yani gerçekten resim ve diğer bilgiler geldiyse
            {
                var uploadedImageDtoResult = await ImageHelper.Upload(userAddDto.UserName, userAddDto.PictureFile, PictureType.User);  // Resim upload işlemi sonucu resmin adını userAddDto içindeki picture alanına vermiş olduk.
                userAddDto.Picture = uploadedImageDtoResult.ResultStatus == ResultStatus.Success                              // Success değilse  "userImages/defaultUser.png" i dönücez.
                    ? uploadedImageDtoResult.Data.FullName
                    : "userImages/defaultUser.png";
                var user = Mapper.Map<User>(userAddDto);                       // Burdaki işlem valid ise userAddDto u bir usera map edicez.
                // Artık elimizde bir tane user mevcut bunu veritabanına ekleyelim.
                var result = await UserManager.CreateAsync(user, userAddDto.Password);     // Bu işlem ile kullanıcıyı ekliyoruz.
                // Yukarıdaki ifade sonucunda bize IdentityResult döndü ve aşağıda bunu kontrol ettik.
                if (result.Succeeded)      // Kullanıcı başarıyla eklendiyse
                {
                    var userAddAjaxModel = JsonSerializer.Serialize(new UserAddAjaxViewModel
                    {
                        UserDto = new UserDto
                        {
                            ResultStatus = ResultStatus.Success,
                            Message = $"{user.UserName} adlı kullanıcı adına sahip, kullanıcı başarıyla eklenmiştir.",
                            User = user
                        },
                        UserAddPartial = await this.RenderViewToStringAsync("_UserAddPartial", userAddDto)
                    });

                    // Json olarak formatladıktan sonra frontend tarafına return ettik.

                    return Json(userAddAjaxModel); // Serialize ettiğimiz modeli frontend tarfına döndük.
                }
                else        // Identity tarafında hata alırsak kullanıcı başarıyla eklenemezse
                {

                    foreach (var error in result.Errors)   // IdentiyResult içinde bulunan errors kısmında foreach ile dönmüş olduk.
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    //Bu hataları ve viewı frontend tarafına dönebilmek için tekrardan UserAddAjaxViewModel oluşturduk.

                    var userAddAjaxErrorModel = JsonSerializer.Serialize(new UserAddAjaxViewModel
                    {
                        UserAddDto = userAddDto,
                        UserAddPartial = await this.RenderViewToStringAsync("_UserAddPartial", userAddDto)
                    });
                    return Json(userAddAjaxErrorModel);
                }

            }
            // Modelstate valid gelmezse
            var userAddAjaxModelStateErrorModel = JsonSerializer.Serialize(new UserAddAjaxViewModel
            {
                UserAddDto = userAddDto,
                UserAddPartial = await this.RenderViewToStringAsync("_UserAddPartial", userAddDto)
            });
            return Json(userAddAjaxModelStateErrorModel);

        }


        [Authorize(Roles = "SuperAdmin,User.Delete")]
        [HttpPost] 
        public async Task<JsonResult> Delete(int userId)
        {
            var user = await UserManager.FindByIdAsync(userId.ToString());
            var result = await UserManager.DeleteAsync(user);             // Bu işlemin sonucunda bir IdentityResuolt dönücek ve bunu kontrol edicez.
            if (result.Succeeded)
            {
                // Kullanıcının resmini de siliyoruz.(default resmimiz değilse)
                if (user.Picture != "userImages/defaultUser.png")
                    ImageHelper.Delete(user.Picture);

                // İşlem başarılıysa frontend tarafına bir UserDto göndericez.
                var deletedUser = JsonSerializer.Serialize(new UserDto
                {
                    // UserDto değerlerini dolduruyoruz.
                    ResultStatus = ResultStatus.Success,
                    Message = $"{user.UserName} adlı kullanıcı adına sahip kullanıcı başarıyla silinmiştir.",
                    User = user
                });
                // Artık elimizde Json formatında silinmiş bir kullanıcı olucaktır.
                return Json(deletedUser); // deletedUser ı frontende gönderiyoruz.
            }
            else
            {
                string errorMessages = String.Empty;
                foreach (var error in result.Errors)   // IdentityResult içindeki hatalarımızı yakalıyoruz.
                {
                    errorMessages = $"*{error.Description}\n";
                }

                var deletedUserErrorModel = JsonSerializer.Serialize(new UserDto
                {
                    ResultStatus = ResultStatus.Error,
                    Message =
                        $"{user.UserName} adlı kullanıcı adına sahip kullanıcı silinirken bazı hatalar oluştu.\n{errorMessages}",
                    User = user
                });
                return Json(deletedUserErrorModel);
            }
        }
        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpGet]
        public async Task<PartialViewResult> Update(int userId)  // IActionResult da kullabilirdik.
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(u => u.Id == userId);  // userid ye sahip kullanıcıyı buluyoruz.
            var userUpdateDto = Mapper.Map<UserUpdateDto>(user);                          // Bizlere gelen user üzerinden UserUpdateDto almak istiyoruz.
            return PartialView("_UserUpdatePartial", userUpdateDto);
        }
        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateDto userUpdateDto)
        {
            if (ModelState.IsValid)
            {
                bool isNewPictureUploaded = false;
                var oldUser = await UserManager.FindByIdAsync(userUpdateDto.Id.ToString());  // Eski kullanıcıyı getirip UserUpdteDto ile kombin edip updateduser halini geri almamız gerekiyor.
                var oldUserPicture = oldUser.Picture;                                         // Resim kısmı değişicekse eski silinicek resmi silmek için saklamamız gerek.
                if (userUpdateDto.PictureFile != null)                                        // Bizlere bir resim geldiyse yeni resmi upload edicez.
                {
                    var uploadedImageDtoResult = await ImageHelper.Upload(userUpdateDto.UserName, userUpdateDto.PictureFile, PictureType.User);
                    userUpdateDto.Picture = uploadedImageDtoResult.ResultStatus == ResultStatus.Success
                        ? uploadedImageDtoResult.Data.FullName
                        : "userImages/defaultUser.png";
                    if (oldUserPicture != "userImages/defaultUser.png")   // Bu resmi kullanan başka kullanıcılar da olabilir bunu silersek onların resmi gider.
                    {
                        isNewPictureUploaded = true;
                    }
                }

                var updatedUser = Mapper.Map<UserUpdateDto, User>(userUpdateDto, oldUser);
                var result = await UserManager.UpdateAsync(updatedUser);
                if (result.Succeeded)           // Kullanıcıyı başarıyla veritabanına gönderebildiysek.
                {
                    if (isNewPictureUploaded)   // Yeni bir resim eklendiyse eskisini siliyoruz.
                    {
                        ImageHelper.Delete(oldUserPicture);
                    }
                    // View üzerine bir UserUpdateAjaxViewModel dönmemiz ve bunu da doldurduktan sonra json olarak view üzerine return etmemiz gerek.
                    var userUpdateViewModel = JsonSerializer.Serialize(new UserUpdateAjaxViewModel
                    {
                        // UserUpdateAjaxViewModelimizi dolduruyoruz.

                        UserDto = new UserDto
                        {
                            ResultStatus = ResultStatus.Success,
                            Message = $"{updatedUser.UserName} adlı kullanıcı başarıyla güncellenmiştir.",
                            User = updatedUser
                        },
                        UserUpdatePartial = await this.RenderViewToStringAsync("_UserUpdatePartial", userUpdateDto)
                    });
                    return Json(userUpdateViewModel);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    var userUpdateErorViewModel = JsonSerializer.Serialize(new UserUpdateAjaxViewModel
                    {
                        UserUpdateDto = userUpdateDto,
                        UserUpdatePartial = await this.RenderViewToStringAsync("_UserUpdatePartial", userUpdateDto)
                    });
                    return Json(userUpdateErorViewModel);
                }

            }
            else
            {
                var userUpdateModelStateErrorViewModel = JsonSerializer.Serialize(new UserUpdateAjaxViewModel
                {
                    UserUpdateDto = userUpdateDto,
                    UserUpdatePartial = await this.RenderViewToStringAsync("_UserUpdatePartial", userUpdateDto)
                });
                return Json(userUpdateModelStateErrorViewModel);
            }
        }

        // Profil düzenleme 

        [Authorize]
        [HttpGet]
        public async Task<ViewResult> ChangeDetails()
        {
            var user = await UserManager.GetUserAsync(HttpContext.User);  // Login olan kullanıcıyı aldık.

            // user nesnesini UserUpdateDto türüne dönüştürür.user nesnesinin değerlerini koruyarak updateDto değişkenine atar.
            // Bu şekilde, updateDto değişkeni, user nesnesinin özelliklerini UserUpdateDto türünde tutar ve bu nesneyi ilgili işlemlerde kullanabilirsiniz.

            var updateDto = Mapper.Map<UserUpdateDto>(user);
            return View(updateDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<ViewResult> ChangeDetails(UserUpdateDto userUpdateDto)
        {
            if (ModelState.IsValid)
            {
                bool isNewPictureUploaded = false;
                var oldUser = await UserManager.GetUserAsync(HttpContext.User);
                var oldUserPicture = oldUser.Picture;
                if (userUpdateDto.PictureFile != null)
                {
                    var uploadedImageDtoResult = await ImageHelper.Upload(userUpdateDto.UserName, userUpdateDto.PictureFile, PictureType.User);
                    userUpdateDto.Picture = uploadedImageDtoResult.ResultStatus == ResultStatus.Success
                        ? uploadedImageDtoResult.Data.FullName
                        : "userImages/defaultUser.png";
                    if (oldUserPicture != "userImages/defaultUser.png")   // Bu resmi kullanan başka kullanıcılar da olabilir bunu silersek onların resmi gider.
                    {
                        isNewPictureUploaded = true;
                    }

                }

                var updatedUser = Mapper.Map<UserUpdateDto, User>(userUpdateDto, oldUser);
                var result = await UserManager.UpdateAsync(updatedUser);
                if (result.Succeeded)
                {
                    if (isNewPictureUploaded)
                    {
                        ImageHelper.Delete(oldUserPicture);
                    }
                    _toastNotification.AddSuccessToastMessage($"Bilgileriniz başarıyla güncellenmiştir.");
                    return View(userUpdateDto);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(userUpdateDto);
                }

            }
            else
            {
                return View(userUpdateDto);
            }
        }

        [Authorize]
        [HttpGet]
        public ViewResult PasswordChange()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PasswordChange(UserPasswordChangeDto userPasswordChangeDto)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.GetUserAsync(HttpContext.User);                                        // Şifresini değiştireceğimiz kullanıcıyı (yani login olan) getiriyoruz.
                var isVerified = await UserManager.CheckPasswordAsync(user, userPasswordChangeDto.CurrentPassword); // Kullanıcının şu anki şifresini kontrol ediyoruz.
                if (isVerified)   // isVerified true ise kullanıcı doğru şifre girmiş.
                {
                    var result = await UserManager.ChangePasswordAsync(user, userPasswordChangeDto.CurrentPassword,  // Şifre değiştirme işlemi
                        userPasswordChangeDto.NewPassword);
                    if (result.Succeeded)  // Bu işlem IdentityResult dönücektir ve bunu kontrol ediyoruz.
                    {
                        await UserManager.UpdateSecurityStampAsync(user); // Kullanıcının şifresini değiştirmesi veya hesap bilgilerinde güncelleme yapması durumunda SecurityStamp değeri değişir.
                        await _signInManager.SignOutAsync();               // Kullanıcıya yeni şifresiyle tekrardan giriş yapması için çıkış yaptırıyoruz. 
                        await _signInManager.PasswordSignInAsync(user, userPasswordChangeDto.NewPassword, true, false);
                        _toastNotification.AddSuccessToastMessage($"Bilgileriniz başarıyla değiştirilmiştir.");
                        return View();
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(userPasswordChangeDto);
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Lütfen, girmiş olduğunuz şu anki şifrenizi kontrol ediniz.");
                    return View(userPasswordChangeDto);
                }
            }
            else
            {
                return View(userPasswordChangeDto);
            }


        }

    }
}
