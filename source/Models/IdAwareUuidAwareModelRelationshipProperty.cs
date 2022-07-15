namespace FFCEI.Microservices.Models
{
    /// <summary>
    /// Declare Model with Id / Uuid relationship
    /// </summary>
    /// <typeparam name="TModelRelationship"></typeparam>
    public class IdAwareUuidAwareModelRelationshipProperty<TModelRelationship>
        where TModelRelationship : Model, IIdAwareModel, IUuidAwareModel
    {
        private long? _id;
        private Guid? _uuid;
        private TModelRelationship? _modelRelationship;

        /// <summary>
        /// Relationship Id property
        /// </summary>
        public long? Id
        {
            get
            {
                return (_modelRelationship != null) ? _modelRelationship.Id : _id;
            }
            set
            {
                if ((_modelRelationship != null) && (value != null) && (_modelRelationship.Id == value))
                {
                    return;
                }

                _id = value;
                _uuid = null;
                _modelRelationship = null;
            }
        }

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

                _id = null;
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
                _id = null;
                _uuid = null;
                _modelRelationship = value;
            }
        }
    }
}
