using Microsoft.EntityFrameworkCore;
using ReniBot.Entities;
using ReniBot.Repository.ModelConfiguration;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ReniBot.Repository
{
    public class BotContext: DbContext
    {
        public BotContext(DbContextOptions options) : base(options) { }

        //public DbSet<Setting> Settings { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Brain> Brains { get; set; }
        public DbSet<AimlDoc> AimlDocs { get; set; }
        public DbSet<BotUser> BotUsers { get; set; }
        public DbSet<BotUserResult> BotUserResults { get; set; }
        public DbSet<BotUserPredicate> BotUserPredicates { get; set; }
        public DbSet<BotUserRequest> BotUserRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            //modelBuilder.Configurations.Add(new SettingConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new TemplateConfiguration());
            modelBuilder.ApplyConfiguration(new BrainConfiguration());
            modelBuilder.ApplyConfiguration(new AimlDocConfiguration());
            modelBuilder.ApplyConfiguration(new BotUserConfiguration());
            modelBuilder.ApplyConfiguration(new BotUserResultConfiguration());
            modelBuilder.ApplyConfiguration(new BotUserPredicateConfiguration());
            modelBuilder.ApplyConfiguration(new BotUserRequestConfiguration());

        }

        public override int SaveChanges()
        {
            var entities = from e in ChangeTracker.Entries()
                           where e.State == EntityState.Added
                               || e.State == EntityState.Modified
                           select e.Entity;
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(entity, validationContext);
            }

            return base.SaveChanges();
        }

    }
}
