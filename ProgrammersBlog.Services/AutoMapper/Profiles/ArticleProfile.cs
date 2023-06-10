using AutoMapper;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Entites.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.AutoMapper.Profiles
{
    public class ArticleProfile:Profile
    {
        public ArticleProfile()
        {
            // Neyi neye dönüştürmek istiyorsak sırayla onu yazdık. 
            // Dtoları normal sınıfa dönüştürüyoruz.
            // ArticleAddDto içersinde CreatedDate alanı olmadığı için bunu ekleyebiliriz.
            CreateMap<ArticleAddDto, Article>().ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(x => DateTime.Now));
            CreateMap<ArticleUpdateDto, Article>().ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(x => DateTime.Now));
            CreateMap<Article,ArticleUpdateDto>();  
        }

    }
}
