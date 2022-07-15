namespace FFCEI.Microservices.Models
{
    /// <summary>
    /// Declare Model with Id relationship
    /// </summary>
    /// <typeparam name="TModelRelationship"></typeparam>
    public class ModelRelationshipProperty<TModelRelationship>
        where TModelRelationship : Model, IIdAwareModel
    {
        private long? _id;
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
                _modelRelationship = value;
            }
        }
    }
}
