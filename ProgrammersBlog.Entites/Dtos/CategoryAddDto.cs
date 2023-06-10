using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.Dtos
{
    public class CategoryAddDto
    {
        // Kullanıcı bir kategori eklemek istediğinde 'Mvc' katmanımızdan 'Services' katmanımıza gerekli Category bilgilerini
        // bu dto objemiz ile taşıyor olacağız.
        // Category sınıfının tüm alanlarını dışarıya açmak yerine sadece kullanıcağımız alanları açıyoruz.Yani CreatedDate gibi değerleri 
        // arka planda kendimiz ekleyeceğiz.O yüzden Dto sınıflarını kullanıyoruz.

        // Kullanıcımızın ihtiyacı olduğu alanlar.
        // Biz FluentApi ile backend tarafındaki(veritabanına nasıl tutulucak) validasyonlaro yaptık.
        // Burda da değerlerin mvc katmanında nasıl gözükeceğini ayarlıyoruz.

        [DisplayName("Kategori Adı")]
        [Required(ErrorMessage = "{0} boş geçilmemelidir.")]
        [MaxLength(70, ErrorMessage = "{0} {1} karakterden büyük olmamalıdır.")]
        [MinLength(3, ErrorMessage = "{0} {1} karakterden az olmamalıdır.")]
        public string Name { get; set; }
        
        [DisplayName("Kategori Açıklaması")]
        [MaxLength(500, ErrorMessage = "{0} {1} karakterden büyük olmamalıdır.")]
        [MinLength(3, ErrorMessage = "{0} {1} karakterden az olmamalıdır.")]
        public string Description { get; set; }
       
        [DisplayName("Kategori Özel Not Alanı")]
        [MaxLength(500, ErrorMessage = "{0} {1} karakterden büyük olmamalıdır.")]
        [MinLength(3, ErrorMessage = "{0} {1} karakterden az olmamalıdır.")]
        public string Note { get; set; }
      
        [DisplayName("Aktif Mi?")]
        [Required(ErrorMessage = "{0} boş geçilmemelidir.")]
        public bool IsActive { get; set; }
    }
}
