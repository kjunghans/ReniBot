using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReniBot.Entities;
using System;

namespace ReniBot.Repository.ModelConfiguration
{
    public class BotUserPredicateConfiguration : IEntityTypeConfiguration<BotUserPredicate>
    {

        public void Configure(EntityTypeBuilder<BotUserPredicate> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.HasKey(n => new { n.userId, n.key });
            builder.Property(n => n.key).HasMaxLength(50);
            builder.Property(n => n.key).HasMaxLength(128);
        }
    }
}
