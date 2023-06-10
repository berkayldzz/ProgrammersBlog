using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Shared.Entites.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.Dtos
{
    public class ArticleDto:DtoGetBase
    {
        // Services ve Mvc katmanları arasında verilerimizi doğru bir şekilde taşıyabilmek için dto sınıflarımız önemlidir.

        public Article Article { get; set; }   // Bir tane article taşıyoruz.

        // ResultStatus de taşıyor olacağız.
    }
}
