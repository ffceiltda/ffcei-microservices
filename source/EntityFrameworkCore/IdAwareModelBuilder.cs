using FFCEI.Microservices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public static class IdAwareModelBuilder
    {
        public static void Register<TEntity>(EntityTypeBuilder<TEntity> entityBuilder) where TEntity : class, IIdAwareModel
        {
            if (entityBuilder == null)
            {
                throw new ArgumentNullException(nameof(entityBuilder));
            }

#pragma warning disable IDE0058 // Expression value is never used
            entityBuilder.Property(e => e.Id)
                .IsLongColumn()
                .ValueGeneratedOnAdd();

            entityBuilder.HasKey(e => e.Id)
                .HasName($"{entityBuilder.Metadata.GetTableName()}_PK");
#pragma warning restore IDE0058 // Expression value is never used
        }
    }
}
