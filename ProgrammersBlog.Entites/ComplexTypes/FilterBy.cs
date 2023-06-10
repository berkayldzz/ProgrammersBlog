using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.ComplexTypes
{
    public enum FilterBy
    {
        // Makalelerin nasıl filtreleneceiğini belirticek.

        [Display(Name = "Kategori")]   
        Category = 0, //GetAllByUserIdOnDate(FilterBy=FilterBy.Category,int categoryId)
       
        [Display(Name = "Tarih")]                // dropdownlist de bu isimle gözükecek.
        Date = 1,
        
        [Display(Name = "Okunma Sayısı")]
        ViewCount = 2,
       
        [Display(Name = "Yorum Sayısı")]
        CommentCount = 3
    }
}
