using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Domain.ActiveDirectory
{
    public class MyAdvancedFilters : AdvancedFilters
    {
        public MyAdvancedFilters(ADUser principal) : base(principal) { }

        /// <summary>
        /// This is used to get the users who were changed in the last
        /// number of 'days' specified.
        /// </summary>
        /// <param name="days"></param>
        public void WhenChangedInLastDays(int days, MatchType matchType)
        {
            AdvancedFilterSet("whenChanged", DateTime.Today.AddDays(days), typeof(DateTime), matchType);
        }
    }
}
