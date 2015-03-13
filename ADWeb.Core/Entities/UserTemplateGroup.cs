using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.Entities
{
    public class UserTemplateGroup
    {
        public int UserTemplateGroupID { get; set; }
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        
        [ForeignKey("UserTemplate")]
        public int UserTemplateID { get; set; }
        public virtual UserTemplate UserTemplate { get; set; }
    }
}
