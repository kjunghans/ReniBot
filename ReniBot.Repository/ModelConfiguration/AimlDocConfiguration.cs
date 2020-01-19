using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReniBot.Entities;
using System;

namespace ReniBot.Repository.ModelConfiguration
{
    public class AimlDocConfiguration: IEntityTypeConfiguration<AimlDoc>
    {
 
        public void Configure(EntityTypeBuilder<AimlDoc> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.HasKey(n => new { n.appId, n.name });
            builder.Property(a => a.name).HasMaxLength(128);
            builder.Ignore(a => a.XmlDoc);
        }
    }
}
