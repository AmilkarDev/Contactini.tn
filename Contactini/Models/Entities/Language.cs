using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contactini.Models.Entities
{
    public enum Proficiency
    {
        Débutant, Intermédiaire, Avancé
    }
    public class Language
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please enter Language Name")]
        public string LanguageName { get; set; }
        public string Proficiency { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
        [Required(ErrorMessage = "Please select Proficiency")]
        public Proficiency LangProficiency { get; set; }
    }
}