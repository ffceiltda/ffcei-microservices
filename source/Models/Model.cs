namespace FFCEI.Microservices.Models
{
    /// <summary>
    /// Abstract Model class
    /// </summary>
    public abstract class Model : IModel
    {
        /// <summary>
        /// Get a Model (relationship) Id proparty
        /// </summary>
        /// <typeparam name="TModelRelationship">Model descendant relationship</typeparam>
        /// <param name="idBackingField">Id backing field</param>
        /// <param name="modelRelationshipBackingField">Model relationship backing field</param>
        /// <returns>Model (relationship) Id proparty</returns>
        protected static long? GetRelationshipId<TModelRelationship>(long? idBackingField, TModelRelationship? modelRelationshipBackingField)
            where TModelRelationship : Model, IIdAwareModel
        {
            return (modelRelationshipBackingField != null) ? modelRelationshipBackingField.Id : idBackingField;
        }

        /// <summary>
        /// Set a Model (relationship) Id proparty
        /// </summary>
        /// <typeparam name="TModelRelationship">Model descendant relationship</typeparam>
        /// <param name="value"></param>
        /// <param name="idBackingField">Id backing field</param>
        /// <param name="modelRelationshipBackingField">Model relationship backing field</param>
        protected static void SetRelationshipId<TModelRelationship>(long? value, ref long? idBackingField, ref TModelRelationship? modelRelationshipBackingField)
            where TModelRelationship : Model, IIdAwareModel
        {
            if ((value != null) && (modelRelationshipBackingField != null) && (modelRelationshipBackingField.Id == value))
            {
                return;
            }

            idBackingField = value;
            modelRelationshipBackingField = null;
        }

        /// <summary>
        /// Set a Model (relationship) Id proparty
        /// </summary>
        /// <typeparam name="TModelRelationship">Model descendant relationship</typeparam>
        /// <param name="value"></param>
        /// <param name="idBackingField">Id backing field</param>
        /// <param name="uuidBackingField">Uuid backing field</param>
        /// <param name="modelRelationshipBackingField">Model relationship backing field</param>
        protected static void SetRelationshipId<TModelRelationship>(long? value, ref long? idBackingField, ref Guid? uuidBackingField, ref TModelRelationship? modelRelationshipBackingField)
            where TModelRelationship : Model, IIdAwareModel
        {
            if ((value != null) && (modelRelationshipBackingField != null) && (modelRelationshipBackingField.Id == value))
            {
                return;
            }

            idBackingField = value;
            uuidBackingField = null;
            modelRelationshipBackingField = null;
        }

        /// <summary>
        /// Get a Model (relationship) Uuid proparty
        /// </summary>
        /// <typeparam name="TModelRelationship">Model descendant relationship</typeparam>
        /// <param name="uuidBackingField">Uuid backing field</param>
        /// <param name="modelRelationshipBackingField">Model relationship backing field</param>
        /// <returns>Model (relationship) Uuid proparty</returns>
        protected static Guid? GetRelationshipUuid<TModelRelationship>(Guid? uuidBackingField, TModelRelationship? modelRelationshipBackingField)
            where TModelRelationship : Model, IUuidAwareModel
        {
            return (modelRelationshipBackingField != null) ? modelRelationshipBackingField.Uuid : uuidBackingField;
        }

        /// <summary>
        /// Set a Model (relationship) Uuid proparty
        /// </summary>
        /// <typeparam name="TModelRelationship">Model descendant relationship</typeparam>
        /// <param name="value"></param>
        /// <param name="uuidBackingField">Uuid backing field</param>
        /// <param name="modelRelationshipBackingField">Model relationship backing field</param>
        protected static void SetRelationshipUuid<TModelRelationship>(Guid? value, ref Guid? uuidBackingField, ref TModelRelationship? modelRelationshipBackingField)
            where TModelRelationship : Model, IUuidAwareModel
        {
            if ((value != null) && (modelRelationshipBackingField != null) && (modelRelationshipBackingField.Uuid == value))
            {
                return;
            }

            uuidBackingField = value;
            modelRelationshipBackingField = null;
        }

        /// <summary>
        /// Set a Model (relationship) Uuid proparty
        /// </summary>
        /// <typeparam name="TModelRelationship">Model descendant relationship</typeparam>
        /// <param name="value"></param>
        /// <param name="idBackingField">Id backing field</param>
        /// <param name="uuidBackingField">Uuid backing field</param>
        /// <param name="modelRelationshipBackingField">Model relationship backing field</param>
        protected static void SetRelationshipUuid<TModelRelationship>(Guid? value, ref long? idBackingField, ref Guid? uuidBackingField, ref TModelRelationship? modelRelationshipBackingField)
            where TModelRelationship : Model, IUuidAwareModel
        {
            if ((value != null) && (modelRelationshipBackingField != null) && (modelRelationshipBackingField.Uuid == value))
            {
                return;
            }

            idBackingField = null;
            uuidBackingField = value;
            modelRelationshipBackingField = null;
        }

        /// <summary>
        /// Set a Model (relationship) navigation property
        /// </summary>
        /// <typeparam name="TModelRelationship">Model descendant relationship</typeparam>
        /// <param name="value">Model relationship instance</param>
        /// <param name="idBackingField">Id backing field</param>
        /// <param name="uuidBackingField">Uuid backing field</param>
        /// <param name="modelRelationshipBackingField">Model relationship backing field</param>
        protected static void SetRelationshipModel<TModelRelationship>(TModelRelationship? value, ref long? idBackingField, ref Guid? uuidBackingField, ref TModelRelationship? modelRelationshipBackingField)
            where TModelRelationship : Model, IIdAwareModel, IUuidAwareModel
        {
            if (modelRelationshipBackingField == value)
            {
                return;
            }

            idBackingField = value?.Id;
            uuidBackingField = value?.Uuid;
            modelRelationshipBackingField = value;
        }

        /// <summary>
        /// Set a Model (relationship) navigation property
        /// </summary>
        /// <typeparam name="TModelRelationship">Model descendant relationship</typeparam>
        /// <param name="value">Model relationship instance</param>
        /// <param name="idBackingField">Id backing field</param>
        /// <param name="modelRelationshipBackingField">Model relationship backing field</param>
        protected static void SetRelationshipModel<TModelRelationship>(TModelRelationship? value, ref long? idBackingField, ref TModelRelationship? modelRelationshipBackingField)
            where TModelRelationship : Model, IIdAwareModel
        {
            if (modelRelationshipBackingField == value)
            {
                return;
            }

            idBackingField = value?.Id;
            modelRelationshipBackingField = value;
        }

        /// <summary>
        /// Set a Model (relationship) navigation property
        /// </summary>
        /// <typeparam name="TModelRelationship">Model descendant relationship</typeparam>
        /// <param name="value">Model relationship instance</param>
        /// <param name="uuidBackingField">Uuid backing field</param>
        /// <param name="modelRelationshipBackingField">Model relationship backing field</param>
        protected static void SetRelationshipModel<TModelRelationship>(TModelRelationship? value, ref Guid? uuidBackingField, ref TModelRelationship? modelRelationshipBackingField)
            where TModelRelationship : Model, IUuidAwareModel
        {
            if (modelRelationshipBackingField == value)
            {
                return;
            }

            uuidBackingField = value?.Uuid;
            modelRelationshipBackingField = value;
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
