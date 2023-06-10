using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using ProgrammersBlog.Entites.ComplexTypes;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Entites.Dtos;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using ProgrammersBlog.Mvc.Helpers.Abstract;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticleController : BaseController
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly IToastNotification _toastNotification;

        public ArticleController(IArticleService articleService, ICategoryService categoryService, UserManager<User> userManager, IMapper mapper, IImageHelper imageHelper, IToastNotification toastNotification) : base(userManager, mapper, imageHelper)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _toastNotification = toastNotification;
        }
        [Authorize(Roles = "SuperAdmin,Article.Read")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _articleService.GetAllByNonDeletedAsync();  // Silinmemiş olan tüm makaleleri getirmek istiyoruz.
            if (result.ResultStatus == ResultStatus.Success) return View(result.Data);
            return NotFound();
        }

        [Authorize(Roles = "SuperAdmin,Article.Create")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var result = await _categoryService.GetAllByNonDeletedAndActiveAsync();
            if (result.ResultStatus == ResultStatus.Success)
            {
                return View(new ArticleAddViewModel
                {
                    Categories = result.Data.Categories
                });
            }

            return NotFound();
        }
        
        [Authorize(Roles = "SuperAdmin,Article.Create")]
        [HttpPost]
        public async Task<IActionResult> Add(ArticleAddViewModel articleAddViewModel)
        {
            if (ModelState.IsValid)
            {
                var articleAddDto = Mapper.Map<ArticleAddDto>(articleAddViewModel);

                // Upload metodu verilen parametrelerle resim dosyasını yükler ve yükleme işlemi sonucunda imageResult adında bir nesne döndürür.
                var imageResult = await ImageHelper.Upload(articleAddViewModel.Title,articleAddViewModel.ThumbnailFile, PictureType.Post);
                // imageResult nesnesinin Data özelliği, yüklenen resim dosyasının bilgilerini içerir.FullName özelliği, yüklenen resim dosyasının tam dosya adını temsil eder.
                articleAddDto.Thumbnail = imageResult.Data.FullName;    // articleAddDto nesnesinin Thumbnail özelliği, bu tam dosya adına atanarak makale DTO'sunda thumbnail resminin dosya adının saklanmasını sağlar.
                var result = await _articleService.AddAsync(articleAddDto,LoggedInUser.UserName,LoggedInUser.Id);
                if (result.ResultStatus == ResultStatus.Success)
                {
                    _toastNotification.AddSuccessToastMessage(result.Message, new ToastrOptions
                    {
                        Title = "Başarılı İşlem!"
                    });
                    return RedirectToAction("Index", "Article");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                   
                }
            }
            var categories = await _categoryService.GetAllByNonDeletedAndActiveAsync();
            articleAddViewModel.Categories = categories.Data.Categories;
            return View(articleAddViewModel);
            
        }

        [Authorize(Roles = "SuperAdmin,Article.Update")]
        [HttpGet]
        public async Task<IActionResult> Update(int articleId)
        {
            var articleResult = await _articleService.GetArticleUpdateDtoAsync(articleId);    // Güncellenecek makaleyi aldık.
            var categoriesResult = await _categoryService.GetAllByNonDeletedAndActiveAsync(); // Silinmemiş aktif categoryleri aldık (dropdownlist için)
            if (articleResult.ResultStatus == ResultStatus.Success && categoriesResult.ResultStatus == ResultStatus.Success)
            {
                // Bizler bir articleupdatedto aldık fakat view üzerinde ArticleUpdateViewModela ihtiyacımız var.Bunu map işlemiyle hallediyoruz.
                // articleResult.Data nesnesinin verileri ArticleUpdateViewModel türündeki articleUpdateViewModel nesnesine aktarılır.
                // Bu şekilde, makalenin güncelleme işlemi için kullanılacak olan modeli hazırlanmış olur.
                var articleUpdateViewModel = Mapper.Map<ArticleUpdateViewModel>(articleResult.Data);
                
                articleUpdateViewModel.Categories = categoriesResult.Data.Categories;
                return View(articleUpdateViewModel);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "SuperAdmin,Article.Update")]
        [HttpPost]
        public async Task<IActionResult> Update(ArticleUpdateViewModel articleUpdateViewModel)
        {
            if (ModelState.IsValid)
            {
                bool isNewThumbnailUploaded = false;                     // Yeni bir Thumbnail yüklendi mi onu kontrol edeceğiz.Başlangıçta hayır dedik.
                var oldThumbnail = articleUpdateViewModel.Thumbnail;     // Eski resmimizin yolunu tutucak.
                if (articleUpdateViewModel.ThumbnailFile != null)        // Boş değilse yani bir Thumbnail dosyası seçildiyse
                {
                    var uploadedImageResult = await ImageHelper.Upload(articleUpdateViewModel.Title,   // Seçilen Thumbnail dosyasını upload ediyoruz.
                        articleUpdateViewModel.ThumbnailFile, PictureType.Post);
                    articleUpdateViewModel.Thumbnail = uploadedImageResult.ResultStatus == ResultStatus.Success // uploadedImageResult değerimiz success gelmezse Thumbnail değerimize default bir değer atadık.
                        ? uploadedImageResult.Data.FullName 
                        : "postImages/defaultThumbnail.jpg";
                    if (oldThumbnail != "postImages/defaultThumbnail.jpg")   // Eğer eski resmimiz postImagesdaki resmimizse bu silinmesin değilse silinsin
                    {
                        isNewThumbnailUploaded = true;
                    }
                }
                var articleUpdateDto = Mapper.Map<ArticleUpdateDto>(articleUpdateViewModel);
                var result = await _articleService.UpdateAsync(articleUpdateDto, LoggedInUser.UserName);     // Mapping işleminden sonra update işlemini gerçekleştiriyoruz.
                if (result.ResultStatus == ResultStatus.Success)
                {
                    if (isNewThumbnailUploaded)             
                    {
                        ImageHelper.Delete(oldThumbnail);  // Eğer eski resim default resmimiz değilse silinsin
                    }
                    _toastNotification.AddSuccessToastMessage(result.Message, new ToastrOptions
                    {
                        Title = "Başarılı İşlem!"
                    });
                    return RedirectToAction("Index", "Article");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            var categories = await _categoryService.GetAllByNonDeletedAndActiveAsync();
            articleUpdateViewModel.Categories = categories.Data.Categories;
            return View(articleUpdateViewModel);
        }

        [Authorize(Roles = "SuperAdmin,Article.Delete")]
        [HttpPost]
        public async Task<JsonResult> Delete(int articleId)
        {
            var result = await _articleService.DeleteAsync(articleId, LoggedInUser.UserName);
            var articleResult = JsonSerializer.Serialize(result);
            return Json(articleResult);
        }

        // Yenile butonu işlemlerimiz

        [Authorize(Roles = "SuperAdmin,Article.Read")]
        [HttpGet]
        public async Task<JsonResult> GetAllArticles()
        {
            var articles = await _articleService.GetAllByNonDeletedAndActiveAsync();
            var articleResult = JsonSerializer.Serialize(articles, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return Json(articleResult);
        }

        [Authorize(Roles = "SuperAdmin,Article.Read")]
        [HttpGet]
        public async Task<IActionResult> DeletedArticles()
        {
            var result = await _articleService.GetAllByDeletedAsync();
            return View(result.Data);

        }
        [Authorize(Roles = "SuperAdmin,Article.Read")]
        [HttpGet]
        public async Task<JsonResult> GetAllDeletedArticles()
        {
            var result = await _articleService.GetAllByDeletedAsync();
            var articles = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return Json(articles);
        }
        [Authorize(Roles = "SuperAdmin,Article.Update")]
        [HttpPost]
        public async Task<JsonResult> UndoDelete(int articleId)
        {
            var result = await _articleService.UndoDeleteAsync(articleId, LoggedInUser.UserName);
            var undoDeleteArticleResult = JsonSerializer.Serialize(result);
            return Json(undoDeleteArticleResult);
        }
        [Authorize(Roles = "SuperAdmin,Article.Delete")]
        [HttpPost]
        public async Task<JsonResult> HardDelete(int articleId)
        {
            var result = await _articleService.HardDeleteAsync(articleId);
            var hardDeletedArticleResult = JsonSerializer.Serialize(result);
            return Json(hardDeletedArticleResult);
        }
        [Authorize(Roles = "SuperAdmin,Article.Read")]
        [HttpGet]
        public async Task<JsonResult> GetAllByViewCount(bool isAscending, int takeSize)
        {
            // Makaleleri okunma sayılarına göre getir ve json formatında return et.

            var result = await _articleService.GetAllByViewCountAsync(isAscending, takeSize);
            var articles = JsonSerializer.Serialize(result.Data.Articles, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return Json(articles);
        } 
    }
}
