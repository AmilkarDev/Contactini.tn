using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Contactini.Models.Entities
{
    public enum State
    {
        En_Attente , Acceptée, Rejetée
    }
    public class Candidature
    {
       
        public int ID { get; set; }
        public State State  { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime   AppDate { get; set; }  
        [Required]
        public string Texte { get; set; }
        [Required]
        public string Title { get; set; }
        public virtual Mission Mission { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
    }
}