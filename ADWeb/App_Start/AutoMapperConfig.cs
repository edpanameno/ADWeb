using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ADWeb
{
    using AutoMapper;
    using ADWeb.ViewModels;
    using ADWeb.Core.Models;
    using ADWeb.Core.Entities;
    using ADWeb.Core.ActiveDirectory;

    /// <summary>
    /// Used to setup the mappings of View Models to Model
    /// objects that are used in the application.
    /// </summary>
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.CreateMap<CreateGroupVM, Group>();
            Mapper.CreateMap<CreateUserVM, User>();
            Mapper.CreateMap<UserViewModel, User>();
            Mapper.CreateMap<ADGroup, ViewGroupVM>();
            Mapper.CreateMap<ViewGroupVM, ADGroup>();
            
            Mapper.CreateMap<CreateUserTemplateVM, UserTemplate>()
                  .ForMember(x => x.Groups, opt => opt.Ignore());
        }
    }
}