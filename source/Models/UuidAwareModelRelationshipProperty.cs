namespace FFCEI.Microservices.Models
{
    /// <summary>
    /// Declare Model with Uuid relationship
    /// </summary>
    /// <typeparam name="TModelRelationship"></typeparam>
    public class UuidAwareModelRelationshipProperty<TModelRelationship>
        where TModelRelationship : Model, IUuidAwareModel
    {
        private Guid? _uuid;
        private TModelRelationship? _modelRelationship;

        /// <summary>
        /// Relationship Uuid property
        /// </summary>

        public Guid? Uuid
        {
            get
            {
                return (_modelRelationship != null) ? _modelRelationship.Uuid : _uuid;
            }
            set
            {
                if ((_modelRelationship != null) && (value != null) && (_modelRelationship.Uuid == value))
                {
                    return;
                }

                _uuid = value;
                _modelRelationship = null;
            }
        }

        /// <summary>
        /// Relationship Model property
        /// </summary>
        public virtual TModelRelationship? ModelRelationship
        {
            get
            {
                return _modelRelationship;
            }
            set
            {
                _uuid = null;
                _modelRelationship = value;
            }
        }
    }
}
