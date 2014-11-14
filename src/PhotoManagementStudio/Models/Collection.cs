using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Catel.Data;
using Newtonsoft.Json;

namespace PhotoManagementStudio.Models
{
    /// <summary>
    /// Collection model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    [JsonObject(MemberSerialization.OptIn)]
    public class Collection : ModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public Collection() { }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected Collection(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string CollectionId
        {
            get { return GetValue<string>(CollectionIdProperty); }
            set { SetValue(CollectionIdProperty, value); }
        }

        /// <summary>
        /// Register the CollectionId property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CollectionIdProperty = RegisterProperty("CollectionId", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string CollectionRev
        {
            get { return GetValue<string>(CollectionRevProperty); }
            set { SetValue(CollectionRevProperty, value); }
        }

        /// <summary>
        /// Register the CollectionRev property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CollectionRevProperty = RegisterProperty("CollectionRev", typeof(string), null);

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
        public ObservableCollection<string> MediaIds
        {
            get { return GetValue<ObservableCollection<string>>(MediaIdsProperty); }
            set { SetValue(MediaIdsProperty, value); }
        }

        /// <summary>
        /// Register the MediaIds property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MediaIdsProperty = RegisterProperty("MediaIds", typeof(ObservableCollection<ObservableCollection<string>>), () => new ObservableCollection<string>());

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public ObservableCollection<Media> Medias
        {
            get { return GetValue<ObservableCollection<Media>>(MediasProperty); }
            set { SetValue(MediasProperty, value); }
        }

        /// <summary>
        /// Register the Medias property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MediasProperty = RegisterProperty("Medias", typeof(ObservableCollection<Media>), () => new ObservableCollection<Media>());

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
