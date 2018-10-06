using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contactini.Models.Entities
{
    public class Client
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNum { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<Mission> Missions { get; set; }
        public virtual ICollection<Realisation> Realisations { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<ServiceProvider> favPresta { get; set; }
    }
}