using System.Windows;
using MedicalImagingSystem.ViewModels;
using MedicalImagingSystem.Infrastructure;

namespace MedicalImagingSystem.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }

        // 无参数构造函数作为回退选项
        public MainWindow()
        {
            InitializeComponent();
            
            // 尝试从服务定位器获取ViewModel
            try
            {
                var mainViewModel = ServiceLocator.GetService<MainViewModel>();
                DataContext = mainViewModel;
            }
            catch (System.Exception ex)
            {
                // 如果依赖注入失败，显示错误消息
                MessageBox.Show($"无法初始化应用程序: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                DataContext = null;
            }
        }
    }
}