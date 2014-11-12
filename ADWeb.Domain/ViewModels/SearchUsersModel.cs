using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ADWeb.Domain.ActiveDirectory;

namespace ADWeb.Domain.ViewModels
{
    public class SearchUsersModel
    {
        [Display(Name="Search Value")]
        [Required(ErrorMessage="Search Value is Required.")]
        public string SearchValue { get; set; }
    }
}