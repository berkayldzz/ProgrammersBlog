using AutoMapper;
using ProgrammersBlog.Entites.Concrete;
using ProgrammersBlog.Entites.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgrammersBlog.Mvc.AutoMapper.Profiles
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<UserAddDto, User>();   // UserAddDto u User a map etmek istiyoruz.Yani aslında UserAddDto daki verileri alıp bir User sınıfı oluşturmak istiyoruz. 
            CreateMap<User, UserUpdateDto>();
            CreateMap<UserUpdateDto, User>();
        }
    }
}
