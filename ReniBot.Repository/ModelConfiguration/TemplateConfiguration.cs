using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReniBot.Entities;
using System;

namespace ReniBot.Repository.ModelConfiguration
{
    public class TemplateConfiguration: IEntityTypeConfiguration<Template>
    {
 
        public void Configure(EntityTypeBuilder<Template> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.HasKey(n => n.id);
            builder.Property(n => n.template).IsRequired().HasMaxLength(1024);
        }
    }
}
