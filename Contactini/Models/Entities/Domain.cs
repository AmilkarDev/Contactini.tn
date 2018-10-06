using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Contactini.Models.Entities
{
    public class Domain
    {
       
        public int domainId { get; set; }
        [Required]
        [StringLength(maximumLength: 35, ErrorMessage = "Pas plus de 35 caractères")]
        public string Name { get; set; }
        public int MissionCount { get; set; }
        public  string  PhotoLink { get; set; }

       // public virtual Sector sector { get; set; }
        public virtual ICollection<Mission> Missions { get; set; }
        public virtual ICollection<ServiceProvider> ServiceProviders { get; set; }
        public virtual ICollection<Realisation> Realisations { get; set; }

    }
}