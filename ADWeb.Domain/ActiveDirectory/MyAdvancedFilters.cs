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
        /// Gets the list of users who were changes within the specified
        /// date range.
        /// </summary>
        /// <param name="days"></param>
        public void WhenChangedInLastDays(DateTime date, MatchType matchType)
        {
            // Dates in Active Directory are stored in a Generalized Time
            // Format. For this filter to work, we must convert the date that
            // we are passing to this method to this format. Note: the Z indicates
            // no time differential.
            var formattedDateTime = date.ToString("yyyyMMddHHmmss.0Z");
            AdvancedFilterSet("whenChanged", formattedDateTime, typeof(string), matchType);
        }
        
        /// <summary>
        /// Gets the list of users that were created within the specified date
        /// range.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="matchType"></param>
        public void CreatedInTheLastDays(DateTime date, MatchType matchType)
        {
            var formattedDateTime = date.ToString("yyyyMMddHHmmss.0Z");
            AdvancedFilterSet("whenCreated", formattedDateTime, typeof(string), matchType);
        }
    }
}
