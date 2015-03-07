using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.Core.Entities
{
    public class OrganizationalUnit
    {
        public int OrganizationalUnitID { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
    }
}
