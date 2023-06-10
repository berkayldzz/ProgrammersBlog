using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.ComplexTypes
{
    public enum OrderBy
    {
        // Filtrelenen makalelerin nasıl sıralanacağını ya da neye göre sıralanacağını belirticek.

        [Display(Name = "Tarih")]
        Date = 0,
        
        [Display(Name = "Okunma Sayısı")]
        ViewCount = 1,
        
        [Display(Name = "Yorum Sayısı")]
        CommentCount = 2
    }
}
