using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Catel.Data;
using Newtonsoft.Json;

namespace PhotoManagementStudio.Models
{
    /// <summary>
    /// Tag model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    [JsonObject(MemberSerialization.OptIn)]
    public class Tag : ModelBase, ITag
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public Tag() { }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected Tag(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string TagId
        {
            get { return GetValue<string>(TagIdProperty); }
            set { SetValue(TagIdProperty, value); }
        }

        /// <summary>
        /// Register the TagId property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TagIdProperty = RegisterProperty("TagId", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string TagRev
        {
            get { return GetValue<string>(TagRevProperty); }
            set { SetValue(TagRevProperty, value); }
        }

        /// <summary>
        /// Register the TagRev property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TagRevProperty = RegisterProperty("TagRev", typeof(string), null);

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

        public TagTypes TagType
        {
            get { return TagTypes.Tag; }
        }

        public int Colour
        {
            get { return Parent == null ? 0 : Parent.Colour; }
            set { }
        }

        public ITag Parent { get; set; }

        public ObservableCollection<ITag> Children
        {
            get { return null; }
            set { }
        }

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
