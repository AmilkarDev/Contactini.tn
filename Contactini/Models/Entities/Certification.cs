using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contactini.Models.Entities
{
    public enum ListOfLevel
    {
        Debutant, Intermédiaire, Avancé
    }
    public class Certification
    {

        public int ID { get; set; }
        [Required(ErrorMessage = "Nom du certification obligatoire")]
        public string CertificationName { get; set; }

        [Required(ErrorMessage = "Authorité de certification obligatoire")]
        public string CertificationAuthority { get; set; }


        // public string LevelCertification { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Date d'acquisation obligatoire")]
        public DateTime? FromYear { get; set; }
        [Required(ErrorMessage = "Niveau obligatoire")]
        public ListOfLevel Level { get; set; }

        public virtual ServiceProvider ServiceProvider { get; set; }
    }
}