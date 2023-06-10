using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProgrammersBlog.Data.Abstract;
using ProgrammersBlog.Data.Concrete;
using ProgrammersBlog.Data.Concrete.EntityFramework.Contexts;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Services.Abstract;
using ProgrammersBlog.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Extensions
{
    public static class ServiceCollectionExtensions // Extensions metotlarını kullanabilmek ve yapıyı kurabilmek için sınıflarımız ve metotlarımız static olmalı
    {
        // Services projemiz üzerinde; IServiceCollection'ı extend edecek, ServiceCollection Extensions sınıfımızı oluşturuyoruz.
        // Bu sınıfımız içerisindeki LoadMyServices() metodumuz aracılığıyla, Services ve Data katmanlarımızdaki
        // DI (Dependency Injection) servislerimizi startup.cs dosyamızda yüklüyoruz.

        // Data katmanımız ile Mvc katmanı arasında erişim yoktur.
        // Services katmanı bizim için gerekli olan bilgileri Data katmanından alacak kendi içersinde işleyecek ve bunu da Mvc katmanına taşıyacak.
        // O yüzden bu Extensions sınıfımızı services katmanında kodlayacağız IServiceCollection extend ederek 
        // içersine LoadMyServices metodunu ekleyeceğiz.
        // Bu metodumuz Data ve Services katmanlarımızdaki bu servicelerimizi kayıt edip kendisini startup içersine yükleyecek.

        // Yani burada farklı katmanlarda bulunan ve Mvc katmanını referans almayan katmanlarımızdaki yapıları kullanabilmek için extensions sınıf ve metot tanımladık.

        // Scoped: Bir istekte bulunduğumuz ve işlemlere başladığımız zaman bu işlemlerin bütünü bir scope içersine alınır ve orda yürütülür.İşlemler bittikten sonra kendisini kapatır.
        public static IServiceCollection LoadMyServices(this IServiceCollection serviceCollection,string connectionString)
        {
            serviceCollection.AddDbContext<ProgrammersBlogContext>(options=>options.UseSqlServer(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            serviceCollection.AddIdentity<User, Role>(options=> 
            {
                // User Password Options
                options.Password.RequireDigit = false;             // şifre içinde rakam bulundurma zorunluluğu olmasın
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                // User Username and Email Options
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+$";
                options.User.RequireUniqueEmail = true;    // bu email adresinden sistemde sadece bir tane olmasını sağlıyor.

            }).AddEntityFrameworkStores<ProgrammersBlogContext>();
            serviceCollection.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(15);
            });
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
            serviceCollection.AddScoped<ICategoryService, CategoryManager>();
            serviceCollection.AddScoped<IArticleService, ArticleManager>(); // Biri benden IArticleService isterse ona ArticleManagerı ver.
            serviceCollection.AddScoped<ICommentService, CommentManager>();
            serviceCollection.AddSingleton<IMailService, MailManager>();
            return serviceCollection;

            // Startup tarafında bu metodumuzu çağırdık.
        }
    }
}
