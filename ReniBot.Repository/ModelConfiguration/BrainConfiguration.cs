using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReniBot.Entities;
using System;

namespace ReniBot.Repository.ModelConfiguration
{
    public class BrainConfiguration : IEntityTypeConfiguration<Brain>
    {

        public void Configure(EntityTypeBuilder<Brain> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasKey(n => n.appId);
            //builder.Property(a => a.appId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            builder.Property(a => a.appId).ValueGeneratedNever();

        }
    }
}
