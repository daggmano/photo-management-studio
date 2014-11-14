using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Catel.Data;
using Newtonsoft.Json;

namespace PhotoManagementStudio.Models
{
    /// <summary>
    /// TagParent model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    [JsonObject(MemberSerialization.OptIn)]
    public class TagParent : ModelBase, ITag
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public TagParent() { }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected TagParent(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string TagParentId
        {
            get { return GetValue<string>(TagParentIdProperty); }
            set { SetValue(TagParentIdProperty, value); }
        }

        /// <summary>
        /// Register the TagParentId property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TagParentIdProperty = RegisterProperty("TagParentId", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string TagParentRev
        {
            get { return GetValue<string>(TagParentRevProperty); }
            set { SetValue(TagParentRevProperty, value); }
        }

        /// <summary>
        /// Register the TagParentRev property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TagParentRevProperty = RegisterProperty("TagParentRev", typeof(string), null);

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
        public int Colour
        {
            get { return GetValue<int>(ColourProperty); }
            set { SetValue(ColourProperty, value); }
        }

        /// <summary>
        /// Register the Colour property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ColourProperty = RegisterProperty("Colour", typeof(int), 0);

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
            get { return TagTypes.Parent; }
        }

        public string TagId
        {
            get { return TagParentId; }
            set { TagParentId = value; }
        }

        public ITag Parent
        {
            get { return null; }
            set { }
        }

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
