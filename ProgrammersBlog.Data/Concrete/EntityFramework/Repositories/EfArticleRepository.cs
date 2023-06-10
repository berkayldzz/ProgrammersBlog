using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Data.Abstract;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Shared.Data.Concrete.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Data.Concrete.EntityFramework.Repositories
{
    public class EfArticleRepository : EfEntityRepositoryBase<Article>, IArticleRepository
    {
        // EfEntityRepositoryBase ile metodumuzun içini doldurmuş olduk.IArticleRepositoryi implemente etmedik.

        // IArticleRepository i implemente etmemiz gerek.Çünkü şu an metotlarımızın içi boş.
        // Ancak implement edip metotların içine tek tek doldurmamıza gerek yok çünkü bunun için
        // EfEntityRepositoryBase sınıfımızı oluşturmuştuk.(Generic Repository)
        // Böylece kod yükündün kurtulmuş olduk.
        // Unutmamaız gereken bir konu da constructor oluşturmamız gerek.

        // Eğer bir sınıfımıza ait özel bir metot yazmak istersek onu burada ayrıca implemente edeceğiz.
        // Bu metodumuzu kodlarken de bu DbContext i kullanacağız.
        // O yüzden burada DbContext constructorı oluşturmamız gerek.
        public EfArticleRepository(DbContext context) : base(context)
        {

        }
    }
}
