using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Shared.Entites.Concrete
{
    public class ValidationError
    {
        public string PropertyName { get; set; } //CategoryName
        public string Message { get; set; } // CategoryName alanı 100 karakterden büyük olmamalıdır.
    }
}
