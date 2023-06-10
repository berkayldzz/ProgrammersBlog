using ProgrammersBlog.Shared.Utilities.Results.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Shared.Entites.Abstract
{
    public abstract class DtoGetBase
    {
        // Bizler view içersinde de ResultStatus kullanarak success,error ya da warning gelmesine bağlı olarak viewı biraz daha değiştirebiliriz ve esneklik sağlayabiliriz.
        public virtual ResultStatus ResultStatus { get; set; } // override edebileyim

        public virtual string Message { get; set; }


        // Sayfalama işlemleri için değerlerimiz

        public virtual int CurrentPage { get; set; } = 1;   // Şu anki sayfamız
        public virtual int PageSize { get; set; } = 5;      // Bir sayfada kaç değer var
        public virtual int TotalCount { get; set; }         // Toplamda kaç makale
        public virtual int TotalPages => (int)Math.Ceiling(decimal.Divide(TotalCount, PageSize));  // Toplam makaleyi bir sayfada kaç makale varsa ona bölersek sayfa sayısı belirlenir       
        public virtual bool ShowPrevious => CurrentPage > 1;      // 1.sayfadaysak önceki butonu gözükmemeli
        public virtual bool ShowNext => CurrentPage < TotalPages;  // Şu an bulunduğumuz sayfa toplam sayfadan küçükse sonraki butonunu göster
        public virtual bool IsAscending { get; set; } = false;
    }
}
