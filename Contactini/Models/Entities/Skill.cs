using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Contactini.Models.Entities
{
    public class Skill
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Please enter your skill name")]
        public string SkillName { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
    }
}