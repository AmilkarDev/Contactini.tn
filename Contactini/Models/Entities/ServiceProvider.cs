using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contactini.Models.Entities
{
    public enum EducationLevels
    {
        Secondaire, Bac, Licence,Master,Ingénierat,Doctorat
    }
    public class ServiceProvider
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNum { get; set; }
        public string photoLink { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        [StringLength(maximumLength: 55, ErrorMessage = "Pas plus de 55 caractères")]
        public string Titre { get; set; }
        public bool Diponibility { get; set; }
        public int  Stars { get; set; }
        public bool HasDrivingLicence { get; set; }
        public bool HasPassport { get; set; }
        public bool HasACar { get; set; }
        public virtual Address Address { get; set; }
        //  [Required(ErrorMessage = "Please Your Date Of Birth ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }
        [DataType(DataType.Url)]
        public string LinkedInProfil { get; set; }
        [DataType(DataType.Url)]
        public string FacebookProfil { get; set; }
        // [Required(ErrorMessage = "Please Your Twitter Profil")]
        [DataType(DataType.Url)]
        public string TwitterProfil { get; set; }
        public byte[] Profil { get; set; }
      //  [Required(ErrorMessage = "Select Your Educational Level ")]
        public EducationLevels EducationalLevel { get; set; }
        public virtual ICollection<Mission> favMissions { get; set; }
        public virtual ICollection<Candidature> Candidatures { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }

        public virtual ICollection<Realisation> Realisations { get; set; }
        public virtual ICollection<Mission> Missions { get; set; }
        public virtual Domain Domain { get; set; }
        public virtual Sector sector { get; set; }
        public virtual ICollection<Certification> Certifications { get; set; }
        public virtual ICollection<Education> Educations { get; set; }
        public virtual ICollection<Language> Languages { get; set; }
        public virtual ICollection<Skill> Skills { get; set; }
        public virtual ICollection<workExperience> workExperiences { get; set; }
    }
}