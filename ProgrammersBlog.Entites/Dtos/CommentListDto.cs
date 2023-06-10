using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgrammersBlog.Entites.Concrete;

namespace ProgrammersBlog.Entites.Dtos
{
    public class CommentListDto
    {
        public IList<Comment> Comments { get; set; }
    }
}
