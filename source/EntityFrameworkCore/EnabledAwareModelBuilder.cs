using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FFCEI.Microservices.Models;
using FFCEI.Microservices.EntityFrameworkCore;

namespace FFCEI.Microservices.EntityFrameworkCore
{
    public static class EnabledAwareModelBuilder
    {
        public static void Register<TEntity>(EntityTypeBuilder<TEntity> entityBuilder) where TEntity : class, IEnabledAwareModel
        {
            if (entityBuilder == null)
            {
                throw new ArgumentNullException(nameof(entityBuilder));
            }

#pragma warning disable IDE0058 // Expression value is never used
            entityBuilder.Property(e => e.IsEnabled)
                .IsRequired()
                .IsBooleanColumn();
#pragma warning restore IDE0058 // Expression value is never used
        }
    }
}
