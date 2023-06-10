using ProgrammersBlog.Data.Abstract;
using ProgrammersBlog.Data.Concrete.EntityFramework.Contexts;
using ProgrammersBlog.Data.Concrete.EntityFramework.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Data.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        // Burada kullanıcağımız repositoryleri ve contextimizi ekleyelim.

        private readonly ProgrammersBlogContext _context;

        // Bu repositorylerin somut hallerine ihtiyacımız var çünkü bir interfacei newleyemeyiz.Yani return edeceğimiz için normal bir interfaci return etmemiz mümkün değil
        // Repositorylerimizi oluşturuyoruz.
        private EfArticleRepository _articleRepository;
        private EfCategoryRepository _categoryRepository;
        private EfCommentRepository _commentRepository;


        public UnitOfWork(ProgrammersBlogContext context)
        {
            _context = context;
        }

        // Eğer birisi bizden IArticleRepository isterse biz o kullanıcıya EfArticleRepository döndürücez.
        // Yukarıdaki somut halimiz herhangi bir şeye atanmış değil yani bir newleme işlemi yapmadık.O yüzden
        // _articleRepository null ise  yani newlenmemiş ise ?? operatörü ile new EfArticleReposirtory i return ediyoruz.
        // ?? operatörü sayesinde bir değişkenin değerinin null olduğu durumlarda alternatif değer döndürebiliriz.Yani bunu koyduktan sonra eğer elimizdeki değer null gelirse ne yapmak istediğimizi yazıyoruz.

        // Kısaca birisi örneğin Articles.AddAsync dediğinde eğer elimizde bir _articleRepository var ise bunu dönüyoruz yoksa yeni bir aticlerpoesitory newleyerek kullanıcıya gönderiyoruz.    
        public IArticleRepository Articles => _articleRepository ??= new EfArticleRepository(_context); // EfArticleRepository constructorında bir DbContext istiyor.

        public ICategoryRepository Categories => _categoryRepository ??= new EfCategoryRepository(_context);


        public ICommentRepository Comments => _commentRepository ??= new EfCommentRepository(_context);


        public async ValueTask DisposeAsync() // Bu metot içersinde contextimizi dispose edicez.
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
