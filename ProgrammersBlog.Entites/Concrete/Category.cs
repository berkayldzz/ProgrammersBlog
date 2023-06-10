using ProgrammersBlog.Shared.Entites.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.Concrete
{
    public class Category:EntityBase,IEntity
    {
        // EntityBase sınıfımızı ve IEntity interfacemizden miras alıyoruz.
        // EntityBasedeki belirli değerleri değiştirmek için override edebiliriz.

        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Article> Articles { get; set; } // Bir kategori birden fazla makalaye sahip olabilir.

    }
}
