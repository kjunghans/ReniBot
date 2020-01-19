using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReniBot.Entities;
using System;

namespace ReniBot.Repository.ModelConfiguration
{
    public class BotUserResultConfiguration: IEntityTypeConfiguration<BotUserResult>
    {

        public void Configure(EntityTypeBuilder<BotUserResult> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.HasKey(n => n.id);
            builder.Property(n => n.userId).IsRequired();
        }
    }
}
