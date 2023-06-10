using ProgrammersBlog.Shared.Entites.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.Concrete
{
    public class Comment:EntityBase,IEntity
    {
        public string Text { get; set; }
        public int ArticleId { get; set; } // Yorum hangi makaleye eklendi bunu bilmemiz lazım.
        public Article Article { get; set; } // Bir yorumun bir makalesi vardır.
    }
}
