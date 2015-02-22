using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.Entities
{
    public class DomainUser
    {
        public int DomainUserID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string UserName { get; set; }
        public bool Enabled { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }
}
