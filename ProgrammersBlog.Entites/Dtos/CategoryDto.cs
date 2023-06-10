using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Shared.Entites.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.Dtos
{
    public class CategoryDto:DtoGetBase  
    {
        // Get işleminde bulunucak(bir kategoriyi getirip bunu taşıyacak) o yüzden DtoGetBase miras aldık.

        public Category Category { get; set; }
    }
}
