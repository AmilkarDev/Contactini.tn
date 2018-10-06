using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Contactini.Models.Entities
{
    public enum stateName
    {
        Ariana , Beja ,Ben_Arous ,Bizerte , Gabes , Gafsa,Jendouba ,Kairouan, Kbelli,Kef , Mahdia,Mannouba,Medenine, Monastir, Nabeul,Sfax, Sidi_Bousid , siliana,Sousse , Tatatouine, Tozeur,Tunis,Zaghouan
    }
    public class Address
    {
        public int ID { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country  { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        public string DisplayAddress
        {
            get
            {
                string dspAddress =
                    string.IsNullOrWhiteSpace(this.StreetAddress) ? "" : this.StreetAddress;
                string dspCity =
                    string.IsNullOrWhiteSpace(this.City) ? "" : this.City;
                string dspState =
                    string.IsNullOrWhiteSpace(this.State) ? "" : this.State;
                string dspPostalCode =
                    string.IsNullOrWhiteSpace(this.PostalCode) ? "" : this.PostalCode;

                return string
                    .Format("{0} {1} {2} {3}", dspAddress, dspCity, dspState, dspPostalCode);
            }
        }
    }
}