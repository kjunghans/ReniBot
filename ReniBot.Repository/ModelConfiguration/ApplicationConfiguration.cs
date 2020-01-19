using ReniBot.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ReniBot.Repository.ModelConfiguration
{
    public class ApplicationConfiguration: IEntityTypeConfiguration<Application>
    {
 
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            builder.HasKey(n => n.id);
        }
    }
}
