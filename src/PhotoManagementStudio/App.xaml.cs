using Catel.IoC;
using NetworkSupervisor;
using PhotoManagementStudio.Services;
using PhotoManagementStudio.Services.Interfaces;

namespace PhotoManagementStudio
{
    using System.Windows;

    using Catel.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NetworkManager _networkManager;
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            Catel.Logging.LogManager.AddDebugListener();
#endif

            StyleHelper.CreateStyleForwardersForDefaultStyles();

            // TODO: Using a custom IoC container like Unity? Register it here:
            // Catel.IoC.ServiceLocator.Instance.RegisterExternalContainer(MyUnityContainer);
            var serviceLocator = ServiceLocator.Default;
            serviceLocator.RegisterType<IDataService, DataService>();

            _networkManager = new NetworkManager();
            _networkManager.Initialize();

            base.OnStartup(e);
        }
    }
}