 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Contactini.Models.Entities
{
    public enum Status
    {
        Ouverte , Fermée,Treminée
    }
    public class Mission
    {
        public int ID { get; set; }
        public string Description  { get; set; }
        public string Title { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PublishDate { get; set; }
        [StringLength(maximumLength:15, ErrorMessage = "Pas plus de 15 caractères")]
        public string StartDate { get; set; }
        [StringLength(maximumLength: 15, ErrorMessage = "Pas plus de 15 caractères")]
        public string Duration { get; set; }
        public Status State { get; set; }
        public virtual Address Address { get; set; }
        public virtual Client Client { get; set; }
        public virtual Sector Sector { get; set; }
        public virtual Domain Domain { get; set; }
        public virtual ServiceProvider ServiceProvider { get; set; }
        public virtual  ICollection<Candidature>  Candidatures { get; set; }
        public string  PhotoLink { get; set; }

    }
}