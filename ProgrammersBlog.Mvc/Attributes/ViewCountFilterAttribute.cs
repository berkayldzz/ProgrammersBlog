using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using ProgrammersBlog.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace ProgrammersBlog.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]  // Bu attributeyi nerde kullanmak istediğimiz belirttik.(sınıf ya da metot)

    // Böylece bu attributeyi Detail actionunda kullanabileceğiz.
    public class ViewCountFilterAttribute:Attribute,IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var articleId = context.ActionArguments["articleId"];
            if (articleId is not null)
            {
                // articleId null değilse cookimiz üzerinden articleIdyi almaya çalışıyoruz.
                string articleValue = context.HttpContext.Request.Cookies[$"article{articleId}"]; // article22 article1 article55

                // Eğer daha önce bu articleId cookimiz üzerine yazılmadıysa bu if bloğunun içine giriyoruz.
                if (string.IsNullOrEmpty(articleValue))   //  articleId cookimiz üzerine yazılmadıysa null gelecek.
                {
                    // Cookie üzerine articleId değerini Set metodu ile yazıyoruz .
                    Set($"article{articleId}", articleId.ToString(), 1, context.HttpContext.Response);
                    var articleService = context.HttpContext.RequestServices.GetService<IArticleService>();  // IArticleService articleService değişkenine alıyoruz ve  okunma sayısını artırıcaz.
                    await articleService.IncreaseViewCountAsync(Convert.ToInt32(articleId));        // Okunma sayısını arttırıyoruz.
                    await next();   // Viewın yüklenmesini ve işlemlerin devam etmesini sağlıyoruz.
                }
            }
            await next();   // Bu articleId cookimiz üzerine daha önce yazıldıysa makalemizin açılmasını sağlıyoruz.
        }
        /// <summary>  
        /// set the cookie  
        /// </summary>  
        /// <param name="key">key (unique indentifier)</param>  
        /// <param name="value">value to store in cookie object</param>  
        /// <param name="expireTime">expiration time</param>  
        /// 

        // Cookie yazma ile ilgili metodumuz.
        public void Set(string key, string value, int? expireTime, HttpResponse response)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddYears(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMonths(6);

            response.Cookies.Append(key, value, option);
        }

        /// <summary>  
        /// Delete the key  
        /// </summary>  
        /// <param name="key">Key</param>  
        /// 

        // Verilecek keyi cookiler arasından silmemizi sağlayan metot.
        public void Remove(string key, HttpResponse response)
        {
            response.Cookies.Delete(key);
        }

    }
}
