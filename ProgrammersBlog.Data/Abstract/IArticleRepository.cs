﻿using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Shared.Data.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Data.Abstract
{
    public interface IArticleRepository : IEntityRepository<Article>
    {
        // Genel metotlarımızı eklemiş olduk ancak bu sınıfa ait başka metot tanımlamak istersek bu kısımda tanımlayabiliriz.



    }
}
