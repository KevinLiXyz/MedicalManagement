using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MedicalImagingSystem.Views;

namespace MedicalImagingSystem
{
    public partial class App : Application
    {
        public ServiceProvider? ServiceProvider { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

			// Create the main window directly and set its dependencies
			var mainWindow = new MainWindow();

			// If the window has any dependencies, inject them here
			// For example, if it has a DataContext that needs DI:
			// var viewModel = ServiceProvider.GetRequiredService<MainViewModel>();
			// mainWindow.DataContext = viewModel;

			mainWindow.Show();
		}

        protected override void OnExit(ExitEventArgs e)
        {
            // 清理资源
            ServiceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}