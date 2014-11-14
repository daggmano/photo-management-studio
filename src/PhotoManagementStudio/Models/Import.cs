using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Catel.Data;
using Newtonsoft.Json;

namespace PhotoManagementStudio.Models
{
    /// <summary>
    /// Import model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    [JsonObject(MemberSerialization.OptIn)]
    public class Import : ModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public Import() { }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected Import(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string ImportId
        {
            get { return GetValue<string>(ImportIdProperty); }
            set { SetValue(ImportIdProperty, value); }
        }

        /// <summary>
        /// Register the ImportId property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ImportIdProperty = RegisterProperty("ImportId", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string ImportRev
        {
            get { return GetValue<string>(ImportRevProperty); }
            set { SetValue(ImportRevProperty, value); }
        }

        /// <summary>
        /// Register the ImportRev property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ImportRevProperty = RegisterProperty("ImportRev", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public DateTime ImportDate
        {
            get { return GetValue<DateTime>(ImportDateProperty); }
            set { SetValue(ImportDateProperty, value); }
        }

        /// <summary>
        /// Register the ImportDate property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ImportDateProperty = RegisterProperty("ImportDate", typeof(DateTime), new DateTime(1, 1, 1));

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
        public static readonly PropertyData MediaIdsProperty = RegisterProperty("MediaIds", typeof(ObservableCollection<string>), () => new ObservableCollection<string>());

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
