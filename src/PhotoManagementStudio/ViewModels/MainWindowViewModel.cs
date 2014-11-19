using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Catel;
using Catel.Data;
using PhotoManagementStudio.Models;
using PhotoManagementStudio.Services.Interfaces;

namespace PhotoManagementStudio.ViewModels
{
    using Catel.MVVM;

    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private readonly IDataService _dataService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel(IDataService dataService)
            : base()
        {
            Argument.IsNotNull(() => dataService);

            _dataService = dataService;
        }

        protected override async Task Initialize()
        {
            await base.Initialize();

            //var media = await _dataService.GetAllMedia();
            //MediaList = new ObservableCollection<Media>(media);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return "View model title"; } }

        /// <summary>
        /// Gets or sets the Media list.
        /// </summary>
        public ObservableCollection<Media> MediaList
        {
            get { return GetValue<ObservableCollection<Media>>(MediaListProperty); }
            private set { SetValue(MediaListProperty, value); }
        }

        /// <summary>
        /// Register the MediaList property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MediaListProperty = RegisterProperty("MediaList", typeof(ObservableCollection<Media>), () => new ObservableCollection<Media>());

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        
        #endregion

        #region Commands
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets
        #endregion

        #region Methods
        // TODO: Create your methods here
        #endregion
    }
}
