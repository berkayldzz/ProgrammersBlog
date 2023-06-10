using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Mvc.AutoMapper.Profiles;
using ProgrammersBlog.Mvc.Filters;
using ProgrammersBlog.Mvc.Helpers.Abstract;
using ProgrammersBlog.Mvc.Helpers.Concrete;
using ProgrammersBlog.Services.AutoMapper.Profiles;
using ProgrammersBlog.Services.Extensions;
using ProgrammersBlog.Shared.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc
{
    public class Startup
    {
         
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           

            // appsettings.json dan veri okurken bu verinin tipini ve nerden okuyaca��m�z� belirtiyoruz.

            services.Configure<AboutUsPageInfo>(Configuration.GetSection("AboutUsPageInfo"));

            services.Configure<WebsiteInfo>(Configuration.GetSection("WebsiteInfo"));

            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
          
            services.Configure<ArticleRightSideBarWidgetOptions>(Configuration.GetSection("ArticleRightSideBarWidgetOptions"));

            // Yukar�daki veri okuma i�lemini nas�l belirtiyorsak yazma i�lemini de belirtiyoruz.

            services.ConfigureWritable<AboutUsPageInfo>(Configuration.GetSection("AboutUsPageInfo"));

            services.ConfigureWritable<WebsiteInfo>(Configuration.GetSection("WebsiteInfo"));

            services.ConfigureWritable<SmtpSettings>(Configuration.GetSection("SmtpSettings"));

            services.ConfigureWritable<ArticleRightSideBarWidgetOptions>(Configuration.GetSection("ArticleRightSideBarWidgetOptions"));

            // AddControllersWithViews :  Mvc uygulamas� olarak �al��aca��n� bildiriyoruz.
            services.AddControllersWithViews(options =>
            {
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(value => "Bu alan bo� ge�ilmemelidir.");
                options.Filters.Add<MvcExceptionFilter>();
            }).AddRazorRuntimeCompilation().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            }).AddNToastNotifyToastr();

            services.AddSession();

            services.AddAutoMapper(typeof(CategoryProfile), typeof(ArticleProfile), typeof(UserProfile), typeof(ViewModelsProfile), typeof(CommentProfile));

            services.LoadMyServices(connectionString:Configuration.GetConnectionString("LocalDB")); // Art�k uygulamam�z ba�lad��� zaman burda bizim bu servislerimiz y�kleniyor olacak.

            services.AddScoped<IImageHelper, ImageHelper>();


            // Authentication cookies: Bir internet sitesine kullan�c� ad� ve �ifresi ile giri� yapt�ktan sonra �yenin,
            // her sayfada tekrar kullan�c� bilgilerini girmesine gerek kalmamas� i�in kullan�l�r.

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Admin/Auth/Login");
                options.LogoutPath = new PathString("/Admin/Auth/Logout");
                options.Cookie = new CookieBuilder
                {
                    Name = "ProgrammersBlog",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest // Always olmal�
                };
                options.SlidingExpiration = true;                                         // kullan�c�n�n siteye belli bir s�re tekrar giri� yapmas�na gerek yoktur
                options.ExpireTimeSpan = System.TimeSpan.FromDays(7);                     // 7 g�n boyunca kullan�c� tekrar giri� yapmas�na gerek yok
                options.AccessDeniedPath = new PathString("/Admin/Auth/AccessDenied");    // kullan�c� yetkisi olmayan bir sayfaya giri� yapmaya �al���rsa buraya y�nlendirilicek
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages(); // Sitede bulunmayan bir viewa gitmek istersek 404 hatas�n� g�stericek.
            }

          

            else
            {
                app.UseStaticFiles();
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();      // Kullan�c� sisteme kay�tl� m� kimlik do�rulamas� yap�l�yor.
            app.UseAuthorization();       // admin sayfas�na ula�mak i�in yetkisi var m� onu kontrol eder.

            app.UseNToastNotify();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "Admin",
                    areaName: "Admin",
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "article",
                    pattern: "{title}/{articleId}",
                    defaults: new { controller = "Article", action = "Detail" }
                    );

                endpoints.MapDefaultControllerRoute(); // Varsay�lan olarak sitemiz a��ld���nda home controller ve index k�sm�na gidecektir.
            });
        }
    }
}
