using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Catel.Data;
using Newtonsoft.Json;

namespace PhotoManagementStudio.Models
{
    /// <summary>
    /// Media model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    [JsonObject(MemberSerialization.OptIn)]
    public class Media : ModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public Media() { }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected Media(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string MediaId
        {
            get { return GetValue<string>(MediaIdProperty); }
            set { SetValue(MediaIdProperty, value); }
        }

        /// <summary>
        /// Register the MediaId property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MediaIdProperty = RegisterProperty("MediaId", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string MediaRev
        {
            get { return GetValue<string>(MediaRevProperty); }
            set { SetValue(MediaRevProperty, value); }
        }

        /// <summary>
        /// Register the MediaRev property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MediaRevProperty = RegisterProperty("MediaRev", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string FullFilePath
        {
            get { return GetValue<string>(FullFilePathProperty); }
            set { SetValue(FullFilePathProperty, value); }
        }

        /// <summary>
        /// Register the name property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FullFilePathProperty = RegisterProperty("FullFilePath", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string LoweredFilePath
        {
            get { return GetValue<string>(LoweredFilePathProperty); }
            set { SetValue(LoweredFilePathProperty, value); }
        }

        /// <summary>
        /// Register the LoweredFilePath property so it is known in the class.
        /// </summary>
        public static readonly PropertyData LoweredFilePathProperty = RegisterProperty("LoweredFilePath", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string FileName
        {
            get { return GetValue<string>(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        /// <summary>
        /// Register the FileName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FileNameProperty = RegisterProperty("FileName", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public DateTime ShotDate
        {
            get { return GetValue<DateTime>(ShotDateProperty); }
            set { SetValue(ShotDateProperty, value); }
        }

        /// <summary>
        /// Register the ShotDate property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ShotDateProperty = RegisterProperty("ShotDate", typeof(DateTime), new DateTime(1, 1, 1));

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public int Rating
        {
            get { return GetValue<int>(RatingProperty); }
            set { SetValue(RatingProperty, value); }
        }

        /// <summary>
        /// Register the Rating property so it is known in the class.
        /// </summary>
        public static readonly PropertyData RatingProperty = RegisterProperty("Rating", typeof(int), 0);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public int DateAccuracy
        {
            get { return GetValue<int>(DateAccuracyProperty); }
            set { SetValue(DateAccuracyProperty, value); }
        }

        /// <summary>
        /// Register the DateAccuracy property so it is known in the class.
        /// </summary>
        public static readonly PropertyData DateAccuracyProperty = RegisterProperty("DateAccuracy", typeof(int), 0);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public string Caption
        {
            get { return GetValue<string>(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        /// <summary>
        /// Register the Caption property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CaptionProperty = RegisterProperty("Caption", typeof(string), null);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public int Rotate
        {
            get { return GetValue<int>(RotateProperty); }
            set { SetValue(RotateProperty, value); }
        }

        /// <summary>
        /// Register the Rotate property so it is known in the class.
        /// </summary>
        public static readonly PropertyData RotateProperty = RegisterProperty("Rotate", typeof(int), 0);

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public ObservableCollection<Metadata> Metadata
        {
            get { return GetValue<ObservableCollection<Metadata>>(MetadataProperty); }
            set { SetValue(MetadataProperty, value); }
        }

        /// <summary>
        /// Register the Metadata property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MetadataProperty = RegisterProperty("Metadata", typeof(ObservableCollection<Metadata>), () => new ObservableCollection<Metadata>());

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        [JsonProperty]
        public ObservableCollection<string> TagIds
        {
            get { return GetValue<ObservableCollection<string>>(TagIdsProperty); }
            set { SetValue(TagIdsProperty, value); }
        }

        /// <summary>
        /// Register the TagIds property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TagIdsProperty = RegisterProperty("TagIds", typeof(ObservableCollection<string>), () => new ObservableCollection<string>());

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public ObservableCollection<Tag> Tags
        {
            get { return GetValue<ObservableCollection<Tag>>(TagsProperty); }
            set { SetValue(TagsProperty, value); }
        }

        /// <summary>
        /// Register the Tags property so it is known in the class.
        /// </summary>
        public static readonly PropertyData TagsProperty = RegisterProperty("Tags", typeof(ObservableCollection<Tag>), () => new ObservableCollection<Tag>());

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
