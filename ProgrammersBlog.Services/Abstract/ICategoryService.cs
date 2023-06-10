using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Entites.Dtos;
using ProgrammersBlog.Shared.Utilities.Results.Abstract;
using ProgrammersBlog.Shared.Utilities.Results.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Abstract
{
    public interface ICategoryService
    {
        // Bu interface service katmanımızda Category sınıfmız ile ilgili kullanmak istediğimiz işlemlerin imzasını tutucak. 
        // Asenkron yapı kurduğumuz için tüm işlemleri Task olarak ekliyoruz.

        // ! Dto sınıflarımız ile revize ediyoruz !

        Task<IDataResult<CategoryDto>> GetAsync(int categoryId);   // Verilerimizi taşımak için IDataResult ı  oluşturmuştuk.int categoryId predicate kısmımız.

        /// <summary>
        /// Verilen ID parametresine ait kategorinin CategoryUpdateDto temsilini geriye döner.
        /// </summary>
        /// <param name="categoryId">0'dan büyük integer bir ID değeri</param>
        /// <returns>Asenkron bir operasyon ile Task olarak işlem sonucu DataResult tipinde geriye döner.</returns>
        Task<IDataResult<CategoryUpdateDto>> GetCategoryUpdateDtoAsync(int categoryId);
        Task<IDataResult<CategoryListDto>> GetAllAsync();       // Tüm kategorileri getir demek.predicate vermiyoruz çünkü tüm categoriler gelicek zaten ama istersek verebiliriz. 
        Task<IDataResult<CategoryListDto>> GetAllByNonDeletedAsync();   // Silinmemiş olan tüm kategorileri getir.
        Task<IDataResult<CategoryListDto>> GetAllByNonDeletedAndActiveAsync();
        Task<IDataResult<CategoryListDto>> GetAllByDeletedAsync();

        /// <summary>
        /// Verilen CategoryAddDto ve CreatedByName parametrelerine ait bilgiler ile yeni bir Category ekler.
        /// </summary>
        /// <param name="categoryAddDto">categoryAddDto tipinde eklenecek kategori bilgileri</param>
        /// <param name="createdByName">string tipinde kullanıcının kullanıcı adı</param>
        /// <returns>Asenkron bir operasyon ile Task olarak bizlere ekleme işleminin sonucunu DataResult tipinde döner.</returns>
        Task<IDataResult<CategoryDto>> AddAsync(CategoryAddDto categoryAddDto, string createdByName);   // Kategori eklemek istersek.

        // Dto: CreatedDate gibi backend tarafında ekleyecğim değerleri kullancıya göstermiyor ve bunları da frontend tarafına açmıyor olacağım.
        Task<IDataResult<CategoryDto>> UpdateAsync(CategoryUpdateDto categoryUpdateDto, string modifiedByName); // Burda da mesela update işlemi için gerekli alanları kullancıya gönderiyor olacağoz.Kullanıcı da bunları bizim için update ettikten sonra geri gönderiyor olacak.
        Task<IDataResult<CategoryDto>> DeleteAsync(int categoryId, string modifiedByName);  // IsDeleted değerini true yapacak işlem sadece getall ile çağırdığımızda gelmeyecek
        Task<IDataResult<CategoryDto>> UndoDeleteAsync(int categoryId, string modifiedByName); 
        Task<IResult> HardDeleteAsync(int categoryId);                     // Gerçekten silme işlemini yapacak işlem veritabanından silecek

        Task<IDataResult<int>> CountAsync();
        Task<IDataResult<int>> CountByNonDeletedAsync();

        // Kullndığımız Dto sınıfları aynı zamanda Frontend validasyonları için de kullanıyoruz.

        // Refactoring
        // Artık veri eklediğimizde veya güncellediğimizde geriye CategoryDto dönücez bu sayede eklediğimiz veya güncellediğimiz
        // verinin son hali elimizde olacak ve tablomuza eklemek için kullanıyor olacağız.

    }
}
