using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Data.Abstract
{
    public interface IUnitOfWork:IAsyncDisposable    // Contextimizi dispose ediyor olacağız.
    {
        // UnitOfWork sayesinde tüm repositorylerimizi tek bir yerden yöneteceğiz.
                            
                                              // get işlemi yapacağız ondan dolayı sadece get propertysi
        IArticleRepository Articles { get; }  // unitofwork.Articles diyerek makalelerime erişebilicem.
        ICategoryRepository Categories { get; }  // bunların hepsini bir protery olarak ekliyoruz.
        ICommentRepository Comments { get; }  

        // Kullanım örneği : _unitOfWork.Categories.AddAsync();

        // Save metodumuzu burada oluşturuyoruz.

        // 2 tane ekleme işlemi yaptık ama tek bir save işlemi yazıyoruz.
        // _unitOfWork.Categories.AddAsync(category);
        // _unitOfWork.Users.AddAsync(user);
        // _unitOfWork.SaveAsync();

        Task<int> SaveAsync();  // veritabanına kaydetme işlemi // int dememizin sebebi etkilenen kayıt sayısını almak isteyebiliriz.

    }
}
