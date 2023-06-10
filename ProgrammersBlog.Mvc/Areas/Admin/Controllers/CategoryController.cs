using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Entites.Dtos;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using ProgrammersBlog.Mvc.Helpers.Abstract;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Shared.Utilities.Extensions;
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
    
    public class CategoryController : BaseController
    {
        // Kategorileri göstermek istiyorsak ICategoryServicei çağırıyoruz.

        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService, UserManager<User> userManager, IMapper mapper, IImageHelper imageHelper) : base(userManager, mapper, imageHelper)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "SuperAdmin,Category.Read")]
        public async Task<IActionResult> Index()
        {
            // kategorileri getirme işlemi

            // Bizler CategoryService içersinde DataResult dönüyoruz. var result diyerek DataResultı aldık ve
            // ResultStatusa ulaştık ResultStatus success ise ona göre işlem yapıcaz.

            var result = await _categoryService.GetAllByNonDeletedAsync(); // Bu metodumuz bizlere DataResult dönecekti.Biz de bunu result değişkenine attık.

            //if (result.ResultStatus == ResultStatus.Success)
            //{                                                        
            //    return View(result.Data);                   // Bu kısma gerek kalmadı çünkü ihtiyacımız olacak her şey result.Data nın içersinde mevcut.
            //}                                               // Dtobase classımıza message kısmını da ekledik.
                                                              // View kısmında da if sorgusu yazdık hata olunca görebilicez ve category gelmeyecek.
             
            return View(result.Data);  // DataResult içindeki verimizi view içersinde dönmüş olduk.


        }

        [Authorize(Roles = "SuperAdmin,Category.Create")]
        [HttpGet]
        public IActionResult Add()
        {
            // partialview dönücez. async vermemize gerek asenkron bir şey yapmayacağız.

            return PartialView("_CategoryAddPartial");
        }

        [Authorize(Roles = "SuperAdmin,Category.Create")]
        [HttpPost]
        public async Task<IActionResult> Add(CategoryAddDto categoryAddDto)
        {
            // JsonSerializer : Frontend tarafına döndüğümüz modelin js tarafında da anlaşılabilmesi için bunu json formatına dönüştürmemiz gerkiyor.
            // Bu işlemi yaparken de JsonSerializer sınıfını kullanıcaz.

            if (ModelState.IsValid)
            {
                var result = await _categoryService.AddAsync(categoryAddDto,LoggedInUser.UserName);
                if (result.ResultStatus == ResultStatus.Success)
                {
                    var categoryAddAjaxModel = JsonSerializer.Serialize(new CategoryAddAjaxViewModel // JsonSerializer.Serialize: Burdaki değeri(CategoryAddAjaxViewModel ı içindeki değerler ile oluşturarak) Json a dönüştürdükten sonra almak istiyorum
                    {
                        CategoryDto = result.Data,
                        CategoryAddPartial = await this.RenderViewToStringAsync("_CategoryAddPartial", categoryAddDto)
                    });
                    return Json(categoryAddAjaxModel); // Js in bunu anlayabilmesi için Json dönmemiz gerek.
                }
            }
            // Modelstate valid değilse örneğin required bir alanı girmedi

            var categoryAddAjaxErrorModel = JsonSerializer.Serialize(new CategoryAddAjaxViewModel
            {
                CategoryAddPartial = await this.RenderViewToStringAsync("_CategoryAddPartial", categoryAddDto)  // Kullanıcıya hata validasyon mesajları dönecek.
            });
            return Json(categoryAddAjaxErrorModel);

        }

        [Authorize(Roles = "SuperAdmin,Category.Update")]
        [HttpGet]
        public async Task<IActionResult> Update(int categoryId)
        {
            var result = await _categoryService.GetCategoryUpdateDtoAsync(categoryId);
            if (result.ResultStatus == ResultStatus.Success)
            {
                return PartialView("_CategoryUpdatePartial", result.Data);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "SuperAdmin,Category.Update")]
        [HttpPost]
        public async Task<IActionResult> Update(CategoryUpdateDto categoryUpdateDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryService.UpdateAsync(categoryUpdateDto, LoggedInUser.UserName);
                if (result.ResultStatus == ResultStatus.Success)
                {
                    var categoryUpdateAjaxModel = JsonSerializer.Serialize(new CategoryUpdateAjaxViewModel
                    {
                        CategoryDto = result.Data,
                        CategoryUpdatePartial = await this.RenderViewToStringAsync("_CategoryUpdatePartial", categoryUpdateDto)
                    });
                    return Json(categoryUpdateAjaxModel);
                }
            }
            var categoryUpdateAjaxErrorModel = JsonSerializer.Serialize(new CategoryUpdateAjaxViewModel
            {
                CategoryUpdatePartial = await this.RenderViewToStringAsync("_CategoryUpdatePartial", categoryUpdateDto)
            });
            return Json(categoryUpdateAjaxErrorModel);

        }

        // Yenile butonunun yapacağı ajax get isteklerine cevap vericek action oluşturucaz.

        [Authorize(Roles = "SuperAdmin,Category.Read")]
        public async Task<JsonResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllByNonDeletedAsync();          // aldığımız categoryleri jsona dönüştürüp return edebiliriz.
            var categories = JsonSerializer.Serialize(result.Data, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return Json(categories);          // veriyi ajax işlemi için return ettik.
        }

        // DeleteAsync işlemi

        [Authorize(Roles = "SuperAdmin,Category.Delete")]
        [HttpPost]
        public async Task<JsonResult> Delete(int categoryId)
        {
            var result = await _categoryService.DeleteAsync(categoryId, LoggedInUser.UserName);
            var deletedCategory = JsonSerializer.Serialize(result.Data);  // Dönecek resultı jsona dönüştürüp return edicez.
            return Json(deletedCategory);                                 // Frontend tarafında ajaxResult geldiğinde ResultStatus durumunu kontrol edip işlemlerimizi gerçekleştiricez.


        }

        [Authorize(Roles = "SuperAdmin,Category.Read")]
        [HttpGet]
        public async Task<IActionResult> DeletedCategories()
        {
            var result = await _categoryService.GetAllByDeletedAsync();
            return View(result.Data);
        }
        [Authorize(Roles = "SuperAdmin,Category.Read")]
        [HttpGet]
        // Yenile butonu için
        public async Task<JsonResult> GetAllDeletedCategories()
        {
            var result = await _categoryService.GetAllByDeletedAsync();
            var categories = JsonSerializer.Serialize(result.Data, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return Json(categories);
        }
        [Authorize(Roles = "SuperAdmin,Category.Update")]
        [HttpPost]
        
        // Silinmiş olanları geri getirmek için
        public async Task<JsonResult> UndoDelete(int categoryId)
        {
            var result = await _categoryService.UndoDeleteAsync(categoryId, LoggedInUser.UserName);
            var undoDeletedCategory = JsonSerializer.Serialize(result.Data);
            return Json(undoDeletedCategory);
        }
        [Authorize(Roles = "SuperAdmin,Category.Delete")]
        [HttpPost]
        public async Task<JsonResult> HardDelete(int categoryId)
        {
            var result = await _categoryService.HardDeleteAsync(categoryId);
            var deletedCategory = JsonSerializer.Serialize(result);
            return Json(deletedCategory);
        }
    }
}
