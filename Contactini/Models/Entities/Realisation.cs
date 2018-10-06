using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Contactini.Models.Entities
{
    public class Realisation
    {
        public int ID { get; set; }
        //[StringLength(maximumLength: 250, ErrorMessage = "Pas plus de 35 caractères")]
        public string Opinion { get; set; }
        //[StringLength(maximumLength: 350, ErrorMessage = "Pas plus de 35 caractères")]
        public string Description { get; set; }
        public bool Validation { get; set; }
        public string Stars { get; set; }
        [StringLength(maximumLength: 35, ErrorMessage = "Pas plus de 35 caractères")]
        public string  TakenTime { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
        public virtual Client Client { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
        public virtual Mission Mission { get; set; }

        public virtual Domain Domain { get; set; }

    }
}