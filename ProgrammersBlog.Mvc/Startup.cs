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
           

            // appsettings.json dan veri okurken bu verinin tipini ve nerden okuyacaðýmýzý belirtiyoruz.

            services.Configure<AboutUsPageInfo>(Configuration.GetSection("AboutUsPageInfo"));

            services.Configure<WebsiteInfo>(Configuration.GetSection("WebsiteInfo"));

            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
          
            services.Configure<ArticleRightSideBarWidgetOptions>(Configuration.GetSection("ArticleRightSideBarWidgetOptions"));

            // Yukarýdaki veri okuma iþlemini nasýl belirtiyorsak yazma iþlemini de belirtiyoruz.

            services.ConfigureWritable<AboutUsPageInfo>(Configuration.GetSection("AboutUsPageInfo"));

            services.ConfigureWritable<WebsiteInfo>(Configuration.GetSection("WebsiteInfo"));

            services.ConfigureWritable<SmtpSettings>(Configuration.GetSection("SmtpSettings"));

            services.ConfigureWritable<ArticleRightSideBarWidgetOptions>(Configuration.GetSection("ArticleRightSideBarWidgetOptions"));

            // AddControllersWithViews :  Mvc uygulamasý olarak çalýþacaðýný bildiriyoruz.
            services.AddControllersWithViews(options =>
            {
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(value => "Bu alan boþ geçilmemelidir.");
                options.Filters.Add<MvcExceptionFilter>();
            }).AddRazorRuntimeCompilation().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            }).AddNToastNotifyToastr();

            services.AddSession();

            services.AddAutoMapper(typeof(CategoryProfile), typeof(ArticleProfile), typeof(UserProfile), typeof(ViewModelsProfile), typeof(CommentProfile));

            services.LoadMyServices(connectionString:Configuration.GetConnectionString("LocalDB")); // Artýk uygulamamýz baþladýðý zaman burda bizim bu servislerimiz yükleniyor olacak.

            services.AddScoped<IImageHelper, ImageHelper>();


            // Authentication cookies: Bir internet sitesine kullanýcý adý ve þifresi ile giriþ yaptýktan sonra üyenin,
            // her sayfada tekrar kullanýcý bilgilerini girmesine gerek kalmamasý için kullanýlýr.

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Admin/Auth/Login");
                options.LogoutPath = new PathString("/Admin/Auth/Logout");
                options.Cookie = new CookieBuilder
                {
                    Name = "ProgrammersBlog",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest // Always olmalý
                };
                options.SlidingExpiration = true;                                         // kullanýcýnýn siteye belli bir süre tekrar giriþ yapmasýna gerek yoktur
                options.ExpireTimeSpan = System.TimeSpan.FromDays(7);                     // 7 gün boyunca kullanýcý tekrar giriþ yapmasýna gerek yok
                options.AccessDeniedPath = new PathString("/Admin/Auth/AccessDenied");    // kullanýcý yetkisi olmayan bir sayfaya giriþ yapmaya çalýþýrsa buraya yönlendirilicek
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages(); // Sitede bulunmayan bir viewa gitmek istersek 404 hatasýný göstericek.
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

            app.UseAuthentication();      // Kullanýcý sisteme kayýtlý mý kimlik doðrulamasý yapýlýyor.
            app.UseAuthorization();       // admin sayfasýna ulaþmak için yetkisi var mý onu kontrol eder.

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

                endpoints.MapDefaultControllerRoute(); // Varsayýlan olarak sitemiz açýldýðýnda home controller ve index kýsmýna gidecektir.
            });
        }
    }
}
