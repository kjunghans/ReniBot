using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReniBot.Entities;
using System;

namespace ReniBot.Repository.ModelConfiguration
{
    public class BotUserConfiguration: IEntityTypeConfiguration<BotUser>
    {
  
        public void Configure(EntityTypeBuilder<BotUser> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.HasKey(n => n.id);
            builder.Property(n => n.appId).IsRequired();
            builder.Property(n => n.userKey).HasMaxLength(50).IsRequired();
            builder.Property(n => n.topic).HasMaxLength(256);
        }
    }
}
