using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Data.Concrete.EntityFramework.Mappings;
using ProgrammersBlog.Entites.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Data.Concrete.EntityFramework.Contexts
{
    public class ProgrammersBlogContext :IdentityDbContext<User,Role,int,UserClaim,UserRole,UserLogin,RoleClaim,UserToken>
    {
        // Bu sınıfımın DbContext olabilmesi için EntityFrameworkCore dan gelen DbContext sınıfından da türemesi gerekiyor.
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Log> Logs { get; set; }


        // ConnetionStringimizi vermemiz gerek.

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(
        //         @"Server=DESKTOP-021EIIH;Database=ProgrammersBlog;Trusted_Connection=True;Connect Timeout=30;MultipleActiveResultSets=True;");

        //    // optionsBuilder.UseSqlServer("server=DESKTOP-021EIIH;database=ProgrammersBlog;integrated security=true;;Trust Server Certificate=true");



        //}

        public ProgrammersBlogContext(DbContextOptions<ProgrammersBlogContext> options):base(options)
        {

        }



        // Sınıflarımızın veritabanına giderken ayarlarını map sınıflarından almasını sağlayacağız.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ArticleMap()); // ApplyConfiguration: yapılandırmayı uygula
            modelBuilder.ApplyConfiguration(new CategoryMap());
            modelBuilder.ApplyConfiguration(new CommentMap());
            modelBuilder.ApplyConfiguration(new RoleMap());
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new RoleClaimMap());
            modelBuilder.ApplyConfiguration(new UserClaimMap());
            modelBuilder.ApplyConfiguration(new UserLoginMap());
            modelBuilder.ApplyConfiguration(new UserRoleMap());
            modelBuilder.ApplyConfiguration(new UserTokenMap());
            modelBuilder.ApplyConfiguration(new LogMap());
        }
    }
}// DESKTOP-021EIIH
 // @"Server=(localdb)\DESKTOP-021EIIH;Database=ProgrammersBlog;Trusted_Connection=True;Connect Timeout=30;MultipleActiveResultSets=True