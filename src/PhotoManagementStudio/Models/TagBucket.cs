using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Catel.Data;
using Newtonsoft.Json;

namespace PhotoManagementStudio.Models
{
    /// <summary>
    /// TagBucket model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    [JsonObject(MemberSerialization.OptIn)]
    public class TagBucket : ModelBase, ITag
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public TagBucket() { }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected TagBucket(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string TagBucketId
        {
            get { return GetValue<string>(TagBucketIdProperty); }
            set { SetValue(TagBucketIdProperty, value); }
        }

        /// <summary>
        /// Register the TagBucketId property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TagBucketIdProperty = RegisterProperty("TagBucketId", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string TagBucketRev
        {
            get { return GetValue<string>(TagBucketRevProperty); }
            set { SetValue(TagBucketRevProperty, value); }
        }

        /// <summary>
        /// Register the TagBucketRev property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TagBucketRevProperty = RegisterProperty("TagBucketRev", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Register the Name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string ParentId
        {
            get { return GetValue<string>(ParentIdProperty); }
            set { SetValue(ParentIdProperty, value); }
        }

        /// <summary>
        /// Register the ParentId property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ParentIdProperty = RegisterProperty("ParentId", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public ObservableCollection<string> ChildrenIds
        {
            get { return GetValue<ObservableCollection<string>>(ChildrenIdsProperty); }
            set { SetValue(ChildrenIdsProperty, value); }
        }

        /// <summary>
        /// Register the ChildrenIds property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ChildrenIdsProperty = RegisterProperty("ChildrenIds", typeof(ObservableCollection<string>), () => new ObservableCollection<string>());

        public TagTypes TagType
        {
            get { return TagTypes.Bucket; }
        }

        public string TagId
        {
            get { return TagBucketId; }
            set { TagBucketId = value; }
        }

        public int Colour
        {
            get { return Parent == null ? 0 : Parent.Colour; }
            set { }
        }

        public ITag Parent { get; set; }

        public ObservableCollection<ITag> Children { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Validates the field values of this object. Override this method to enable
        /// validation of field values.
        /// </summary>
        /// <param name="validationResults">The validation results, add additional results to this list.</param>
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
        }

        /// <summary>
        /// Validates the field values of this object. Override this method to enable
        /// validation of field values.
        /// </summary>
        /// <param name="validationResults">The validation results, add additional results to this list.</param>
        protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
        }
        #endregion
    }
}
