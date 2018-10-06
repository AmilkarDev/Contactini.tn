using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Contactini.Models.Entities;

namespace Contactini.DAL
{
    public class ContactiniInitializer : CreateDatabaseIfNotExists<ContactiniContext>
    {
        //protected override void Seed(ContactiniContext context)
        //{
        //    var clients = new List<Client>
        //    {
        //    new Client{FullName="Carson",UserName="Alexander",ID=1,Email="email"},
        //   new Client{FullName="Carson",UserName="Alexander",ID=2,Email="email"},
        //   new Client{FullName="Carson",UserName="Alexander",ID=2,Email="email"},
        //    };

        //    clients.ForEach(s => context.Clients.Add(s));
        //    context.SaveChanges();
        //}
        }
    }