using System;
using System.Windows;
using MedicalImagingSystem.Infrastructure;

namespace MedicalImagingSystem
{
    /// <summary>
    /// 应用程序入口点类
    /// </summary>
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                // 配置依赖注入容器
                var serviceProvider = DependencyInjectionConfig.ConfigureServices();
                
                // 设置服务定位器
                ServiceLocator.SetServiceProvider(serviceProvider);

                // 创建并启动WPF应用程序
                var app = new App();
                app.ServiceProvider = serviceProvider;
                
                // 初始化并运行应用程序
                app.InitializeComponent();
                app.Run();
            }
            catch (Exception ex)
            {
                // 处理启动异常
                MessageBox.Show($"应用程序启动失败: {ex.Message}", "启动错误", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // 清理资源
                ServiceLocator.Dispose();
            }
        }
    }
}