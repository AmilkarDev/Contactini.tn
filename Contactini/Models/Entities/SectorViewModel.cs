using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contactini.Models.Entities
{
    public class SectorViewModel
    {
        public IEnumerable<Sector> sectors { get; set; }
        public IEnumerable<Domain> domains { get; set; }
        public IEnumerable<Mission> missions { get; set; }
    }
}