using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ADWeb.Core.ActiveDirectory;

namespace ADWeb.ViewModels
{
    public class SearchUsersModel
    {
        [Display(Name="Search Value")]
        [Required(ErrorMessage="Search Value is Required.")]
        public string SearchValue { get; set; }

        /// <summary>
        /// Specifies what field will be searched for
        /// </summary>
        public SearchField SearchField { get; set; }
    }
}