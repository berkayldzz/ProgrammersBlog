using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Shared.Entites.Concrete
{
    public class MvcErrorModel            // Hata sayfalarımızda kullanacağımız model.
    {
        public string Message { get; set; }
        public string Detail { get; set; }
    }
}
