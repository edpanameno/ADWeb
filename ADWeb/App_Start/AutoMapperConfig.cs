using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADWeb
{
    using AutoMapper;
    using ADWeb.ViewModels;
    using ADWeb.Core.Models;

    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.CreateMap<CreateGroupVM, Group>();
            Mapper.CreateMap<CreateUserVM, User>();
            Mapper.CreateMap<UserViewModel, User>();
        }
    }
}