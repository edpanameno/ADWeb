using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADWeb.ViewModels
{
    using ADWeb.Core.Entities;

    public class OUViewModel
    {
        public OUViewModel()
        {
            NewOU = new DomainOU();
            //ActiveOUs = new List<DomainOU>();
            //DisabledOUs = new List<DomainOU>();
        }

        public DomainOU NewOU { get; set; }
        public List<DomainOU> ActiveOUs { get; set; }
        public List<DomainOU> DisabledOUs { get; set; }
    }
}
