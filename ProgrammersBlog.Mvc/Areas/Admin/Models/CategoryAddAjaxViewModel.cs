using ProgrammersBlog.Entites.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.Areas.Admin.Models
{
    public class CategoryAddAjaxViewModel
    {
        // Sadece mvc katmanımızda kullanılacak bir model.Ajax işlemleriyle ilgili view kısmını ilgilendiren bir model.

        public CategoryAddDto CategoryAddDto { get; set; }
        public string CategoryAddPartial { get; set; }
        public CategoryDto CategoryDto { get; set; }  // işlemlerimiz doğru bir şekilde gerçekleştiyse efekt ile tablonun sonuna eklenmiş olucak.


    }
}
