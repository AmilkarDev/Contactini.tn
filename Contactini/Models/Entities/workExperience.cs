using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contactini.Models.Entities
{
    public class workExperience
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please Your Company")]
        public string Company { get; set; }

        [Required(ErrorMessage = "Please Your Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please Country is required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Please enter Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FromYear { get; set; }

        [Required(ErrorMessage = "Please enter End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ToYear { get; set; }

        [Required(ErrorMessage = "Please enter Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
        //public List<SelectListItem> ListeOfCountries { get; set; }
    }
}