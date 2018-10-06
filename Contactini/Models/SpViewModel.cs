using Contactini.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contactini.Models
{
    public class SpViewModel
    {
        public string Description { get; set; }
        public Domain domain { get; set; }
        public Sector sector { get; set; }
    }

    public class MissionsbyState
    {
        public string stateName { get; set; }
        public int MissionsCount { get; set; }
    }
    public class PrestabyMission
    {
        public string prestaName { get; set; }
        public int MissionsCount { get; set; }
    }
    public class missionByDomain
    {
        public string domainName { get; set; }
        public int MissionsCount { get; set; }
    }
}