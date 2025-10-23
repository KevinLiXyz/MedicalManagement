using System.IO;
using MedicalImagingSystem.ILogger;
using MedicalImagingSystem.IServices;
using MedicalImagingSystem.Logger;
using MedicalImagingSystem.Services;
using MedicalImagingSystem.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MedicalImagingSystem.Infrastructure
{
    /// <summary>
    /// 依赖注入配置类
    /// </summary>
    public static class DependencyInjectionConfig
    {
        /// <summary>
        /// 配置所有服务的依赖注入
        /// </summary>
        public static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // 配置系统
            var configuration = BuildConfiguration();
            services.AddSingleton<IConfiguration>(configuration);

            // 日志系统
            ConfigureLogging(services, configuration);

            // 业务服务
            RegisterBusinessServices(services);

            // ViewModels
            RegisterViewModels(services);

            return services.BuildServiceProvider();
        }

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }

        private static void ConfigureLogging(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddConsole();
                builder.AddDebug();
            });
        }

        private static void RegisterBusinessServices(IServiceCollection services)
        {
			// 注册服务接口和实现
			services.AddSingleton<ILogManager, LogManager>();
			services.AddSingleton<IPatientService, PatientService>();
            services.AddSingleton<IImageService, ImageService>();
            services.AddSingleton<IThemeService, ThemeService>();
        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            // 注册ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<PatientListViewModel>();
            services.AddTransient<ImageViewerViewModel>();
        }
    }
}