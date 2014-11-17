using System;
using Catel;
using Catel.Data;
using Catel.MVVM;
using PhotoManagementStudio.Models;

namespace PhotoManagementStudio.ViewModels
{
    /// <summary>
    /// Media view model.
    /// </summary>
    public class MediaViewModel : ViewModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaViewModel"/> class.
        /// </summary>
        public MediaViewModel(Media media, NetworkConfiguration networkConfiguration)
        {
            Argument.IsNotNull(() => media);
            Argument.IsNotNull(() => networkConfiguration);

            Media = media;
            NetworkConfiguration = networkConfiguration;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return "View model title"; } }

        // TODO: Register models with the vmpropmodel codesnippet

        /// <summary>
        /// Gets or sets the media item.
        /// </summary>
        [Model]
        public Media Media
        {
            get { return GetValue<Media>(MediaProperty); }
            private set { SetValue(MediaProperty, value); }
        }

        /// <summary>
        /// Register the MediaItem property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MediaProperty = RegisterProperty("Media", typeof(Media), null);

        /// <summary>
        /// Gets or sets the network configuration item.
        /// </summary>
        [Model]
        public NetworkConfiguration NetworkConfiguration
        {
            get { return GetValue<NetworkConfiguration>(NetworkConfigurationProperty); }
            private set { SetValue(NetworkConfigurationProperty, value); }
        }

        /// <summary>
        /// Register the NetworkConfiguration property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NetworkConfigurationProperty = RegisterProperty("NetworkConfiguration", typeof(NetworkConfiguration), null);

        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets

        /// <summary>
        /// Gets or sets the full file path.
        /// </summary>
        [ViewModelToModel("Media")]
        public string FullFilePath
        {
            get { return GetValue<string>(FullFilePathProperty); }
            set { SetValue(FullFilePathProperty, value); }
        }

        /// <summary>
        /// Register the FullFilePath property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FullFilePathProperty = RegisterProperty("FullFilePath", typeof(string), null);

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        [ViewModelToModel("Media")]
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
        /// Gets the image server path.
        /// </summary>
        [ViewModelToModel("NetworkConfiguration")]
        public string ServerPath
        {
            get { return GetValue<string>(ServerPathProperty); }
            set { SetValue(ServerPathProperty, value); }
        }

        /// <summary>
        /// Register the ServerPath property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ServerPathProperty = RegisterProperty("ServerPath", typeof(string), null);

        /// <summary>
        /// Gets the image cache path.
        /// </summary>
        [ViewModelToModel("NetworkConfiguration")]
        public string CacheFolder
        {
            get { return GetValue<string>(CacheFolderProperty); }
            set { SetValue(CacheFolderProperty, value); }
        }

        /// <summary>
        /// Register the CacheFolder property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CacheFolderProperty = RegisterProperty("CacheFolder", typeof(string), null);

        #endregion

        #region Commands
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
        #endregion

        #region Methods
        #endregion
    }

}
