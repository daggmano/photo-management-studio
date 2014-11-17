using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using Catel.Data;

namespace PhotoManagementStudio.Models
{
    /// <summary>
    /// NetworkConfiguration model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class NetworkConfiguration : ModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public NetworkConfiguration() { }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected NetworkConfiguration(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the server path.
        /// </summary>
        public string ServerPath
        {
            get { return GetValue<string>(ServerPathProperty); }
            set { SetValue(ServerPathProperty, value); }
        }

        /// <summary>
        /// Register the ServerPath property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ServerPathProperty = RegisterProperty("ServerPath", typeof(string), String.Empty);

        /// <summary>
        /// Gets or sets the image cache folder.
        /// </summary>
        public string CacheFolder
        {
            get { return GetValue<string>(CacheFolderProperty); }
            set { SetValue(CacheFolderProperty, value); }
        }

        /// <summary>
        /// Register the CacheFolder property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CacheFolderProperty = RegisterProperty("CacheFolder", typeof(string), String.Empty);

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
