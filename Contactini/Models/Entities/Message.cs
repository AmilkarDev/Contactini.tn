using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Contactini.Models.Entities
{
    public class Message
    {
        public int ID { get; set; }
        [Required (ErrorMessage ="Titre Obligatoire !!")]
        [StringLength(maximumLength: 45, ErrorMessage = "Pas plus de 45 caractères")]
        public string Title { get; set; }
        [Required(ErrorMessage ="Message Obligatoire !!")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public string PhoneNumber { get; set; }
        public string  senderEmail { get; set; }
        public string  senderName { get; set; }
        public string senderPhone { get; set; }
        public string ReceiverEmail { get; set; }
    }
}