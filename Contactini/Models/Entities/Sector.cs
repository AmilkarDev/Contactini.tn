using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Contactini.Models.Entities
{
    public class Sector
    {

        public int ID { get; set; }
        [Required]
        [StringLength(maximumLength: 45, ErrorMessage = "Pas plus de 45 caractères")]
        public string Name { get; set; }

        public virtual ICollection<Domain> Domain { get; set; }

    }
}