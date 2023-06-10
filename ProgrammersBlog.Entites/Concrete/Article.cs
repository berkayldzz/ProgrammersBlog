using ProgrammersBlog.Shared.Entites.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.Concrete
{
    public class Article : EntityBase, IEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Thumbnail { get; set; } // fotoğraf
        public DateTime Date { get; set; }
        public int ViewsCount { get; set; } = 0;  // Makalenin okunma sayısı
        public int CommentCount { get; set; } = 0; // Makalenin yorum sayısı
        public string SeoAuthor { get; set; }  // Makaleyi kim yazdı kim paylaştı gibi bilgiler,etiketler
        public string SeoDescription { get; set; }
        public string SeoTags { get; set; }
        public int CategoryId { get; set; } // Bu makale hangi kategoriye ait
        public Category Category { get; set; } // Bir makalenin bir kategorisi vardır.
        public int UserId { get; set; }   // Makaleyi kim paylaştı
        
        public User User { get; set; }  // Bir makaleyi bir user paylaşır.
        public ICollection<Comment> Comments { get; set; } // Bir makale birden çok yoruma sahip olabilir.
    }
}
