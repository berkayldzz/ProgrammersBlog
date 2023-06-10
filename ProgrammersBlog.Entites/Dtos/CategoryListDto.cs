using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Shared.Entites.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.Dtos
{
    public class CategoryListDto:DtoGetBase
    {
        public IList<Category> Categories { get; set; }
    }
}
