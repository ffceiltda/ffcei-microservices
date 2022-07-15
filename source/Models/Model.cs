using System.Reflection;

namespace FFCEI.Microservices.Models
{
    /// <summary>
    /// Abstract Model class
    /// </summary>
    public abstract class Model : IModel
    {
        public static Guid UuidRelationPropertyGetOrDefault<TEntity>(Guid? storageProperty, TEntity entityFrameworkCoreNavigationEntity) where TEntity : class, IUuidAwareModel
        {
            if (entityFrameworkCoreNavigationEntity is not null)
            {
                return entityFrameworkCoreNavigationEntity.Uuid;
            }

            if (storageProperty.HasValue)
            {
                return storageProperty.Value;
            }

            return Guid.Empty;
        }

        public static Guid? UuidRelationPropertyGetOrNull<TEntity>(Guid? storageProperty, TEntity entityFrameworkCoreNavigationEntity)
            where TEntity : class, IUuidAwareModel
        {
            if (entityFrameworkCoreNavigationEntity is not null)
            {
                return entityFrameworkCoreNavigationEntity.Uuid;
            }

            if (storageProperty.HasValue)
            {
                return storageProperty.Value;
            }

            return null;
        }

        public void UuidRelationPropertySet<TEntity>(ref Guid? storageProperty, PropertyInfo? entityFrameworkCoreNavigationEntityProperty, Guid newValue)
            where TEntity : class, IUuidAwareModel
        {
            if (entityFrameworkCoreNavigationEntityProperty is null)
            {
                throw new ArgumentNullException(nameof(entityFrameworkCoreNavigationEntityProperty));
            }

            if ((entityFrameworkCoreNavigationEntityProperty.GetValue(this) is not TEntity propertyValue) || (propertyValue.Uuid != newValue))
            {
                storageProperty = newValue;
                entityFrameworkCoreNavigationEntityProperty.SetValue(this, null);
            }
        }

        public void UuidRelationPropertySet<TEntity>(ref Guid? storageProperty, PropertyInfo? entityFrameworkCoreNavigationEntityProperty, Guid? newValue)
            where TEntity : class, IUuidAwareModel
        {
            if (entityFrameworkCoreNavigationEntityProperty is null)
            {
                throw new ArgumentNullException(nameof(entityFrameworkCoreNavigationEntityProperty));
            }

            if (!newValue.HasValue)
            {
                storageProperty = newValue;
                entityFrameworkCoreNavigationEntityProperty.SetValue(this, null);
            }
            else
            {
                if ((entityFrameworkCoreNavigationEntityProperty.GetValue(this) is not TEntity propertyValue) || (propertyValue.Uuid != newValue))
                {
                    storageProperty = newValue;
                    entityFrameworkCoreNavigationEntityProperty.SetValue(this, null);
                }
            }
        }

        public void UuidRelationPropertyCopyFrom<TEntity, TSource>(TSource source, Guid? sourceUuid, PropertyInfo? property, PropertyInfo? propertyId, PropertyInfo? propertyUuid)
            where TEntity : class, IIdAwareModel, IUuidAwareModel
            where TSource : class, IModel
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            if (propertyId is null)
            {
                throw new ArgumentNullException(nameof(propertyId));
            }

            if (propertyUuid is null)
            {
                throw new ArgumentNullException(nameof(propertyUuid));
            }

            if (source is TEntity entity)
            {
                property.SetValue(this, entity);
                propertyId.SetValue(this, entity.Id);
            }
            else
            {
                if (sourceUuid.HasValue && sourceUuid != Guid.Empty)
                {
                    propertyUuid.SetValue(this, sourceUuid.Value);
                }
                else
                {
                    propertyUuid.SetValue(this, null);
                }
            }
        }

        public virtual void CopyModelPropertiesFrom(IModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var propertyList = GetType().GetProperties().Where(prop => prop.IsDefined(typeof(ModelPropertyAttribute), true));

            foreach (var property in propertyList)
            {
                var destinationProperty = model.GetType().GetProperty(property.Name);

                if (destinationProperty is null)
                {
                    throw new ArgumentNullException($"Cannot find source property {property.Name} in {model.GetType().FullName}");
                }

                var propertyValue = property.GetValue(model);

                destinationProperty.SetValue(this, propertyValue);
            }
        }
    }
}
