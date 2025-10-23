using Microsoft.Extensions.DependencyInjection;
using System;

namespace MedicalImagingSystem.Infrastructure
{
    /// <summary>
    /// 服务定位器 - 提供对依赖注入容器的静态访问
    /// </summary>
    public static class ServiceLocator
    {
        private static ServiceProvider? _serviceProvider;

        public static void SetServiceProvider(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T GetService<T>() where T : class
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceProvider has not been set. Call SetServiceProvider first.");

            return _serviceProvider.GetRequiredService<T>();
        }

        public static T? GetOptionalService<T>() where T : class
        {
            return _serviceProvider?.GetService<T>();
        }

        public static object GetService(Type serviceType)
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceProvider has not been set. Call SetServiceProvider first.");

            return _serviceProvider.GetRequiredService(serviceType);
        }

        public static void Dispose()
        {
            _serviceProvider?.Dispose();
            _serviceProvider = null;
        }
    }
}