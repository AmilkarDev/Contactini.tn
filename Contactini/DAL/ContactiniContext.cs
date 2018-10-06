using Contactini.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Contactini.DAL
{
    public class ContactiniContext: DbContext
    {
        public ContactiniContext() : base("DefaultConnection")
        {
        }
        static ContactiniContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ContactiniContext>(new ContactiniInitializer());
        }  
        public static ContactiniContext Create()
        {
            return new ContactiniContext();
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<ServiceProvider> ServiceProviders { get; set; }
        public DbSet<Candidature> Candidatures { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Realisation> Realisations { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Certification> Certifications { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<workExperience> WorkExperiences { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}