using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProgrammersBlog.Entites.Dtos;

namespace ProgrammersBlog.Mvc.Models
{
    public class ArticleSearchViewModel
    {
        public ArticleListDto ArticleListDto { get; set; }
        public string Keyword { get; set; }
    }
}
