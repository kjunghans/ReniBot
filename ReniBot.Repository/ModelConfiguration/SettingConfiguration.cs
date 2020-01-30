using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReniBot.Entities;
using System;

namespace ReniBot.Repository.ModelConfiguration
{
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {

        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.HasKey(n => new { n.appId, n.type, n.key });
            builder.Property(n => n.key).IsRequired().HasMaxLength(45);
            builder.Property(n => n.val).HasMaxLength(512);
        }
    }
}
