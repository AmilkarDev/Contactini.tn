using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contactini.Models.Entities
{
    public class Education
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Université obligatoire")]
        public string InstituteUniversity { get; set; }

        [Required(ErrorMessage = "Diplome obligatoire")]
        public string TitleOfDiploma { get; set; }

        [Required(ErrorMessage = "Niveau obligatoire")]
        public string Degree { get; set; }

        [Required(ErrorMessage = "Année début obligatoire")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FromYear { get; set; }

        [Required(ErrorMessage = "Année du fin obligatoire")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ToYear { get; set; }
        [Required(ErrorMessage = "Cité obligatoire")]
        public string City { get; set; }
        [Required(ErrorMessage = "Pays obligatoire")]
        public string Country { get; set; }

        public virtual ServiceProvider ServiceProvider { get; set; }
        //public List<SelectListItem> ListOfCountry { get; set; }
        //public List<SelectListItem> ListOfCity { get; set; }
    }
}