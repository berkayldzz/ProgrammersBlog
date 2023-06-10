using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Shared.Entites.Abstract
{
    public abstract class EntityBase
    {
        // Neden Abstract : Bu verdiğimiz temel değerlerin başka sınıflarda override edilip değişmesini
        // isteyebiliriz bundan dolayı abstract tanımladık.Ayrıca override olabilmesi için propertyleri virtual tanımladık. 

        // ! Bu özellikler tüm sınıflar için ortak özellikler !
        public virtual int Id { get; set; }

        public virtual DateTime CreatedDate { get; set; } = DateTime.Now; // override CreatedDate=new DateTime(2023/01/01); başka bir sınıfta bu özelliğin değerini değiştirmek isreyebilirim

        public virtual DateTime ModifiedDate { get; set; } = DateTime.Now;

        public virtual bool IsDeleted { get; set; } = false;
        public virtual bool IsActive { get; set; } = true;
        public virtual string CreatedByName { get; set; } = "Admin";
        public virtual string ModifiedByName { get; set; } = "Admin";
        public virtual string Note { get; set; } 


    }
}
