using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contactini.Models
{
    public class PrestaViewModel
    {
        public string titre { get; set; }
        public string photoLink { get; set; }
        public string dispo { get; set; }
        public string fullName { get; set; }
        public string domain { get; set; }
        public string sector { get; set; }
        public int stars { get; set; }
        public int Nstars { get; set; }
    }
}