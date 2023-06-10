using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Shared.Entites.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.Dtos
{
    public class ArticleListDto : DtoGetBase
    {
        public IList<Article> Articles { get; set; }

        public int? CategoryId { get; set; }   // Bir category seçildiğinde ve o categorynin makaleleri geldiğinde sayfalama işlemi yapabilmemiz için ekledik.

    }
}
