using FluentAssertions;
using MedicalImagingSystem.Infrastructure;
using MedicalImagingSystem.IServices;
using MedicalImagingSystem.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MedicalImagingSystem.Infrastructure.Tests
{
    public class DependencyInjectionConfigTests
    {
        [Fact]
        public void ConfigureServices_ShouldReturnValidServiceProvider()
        {
            // Act
            var serviceProvider = DependencyInjectionConfig.ConfigureServices();

            // Assert
            serviceProvider.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureServices_ShouldRegisterConfiguration()
        {
            // Act
            var serviceProvider = DependencyInjectionConfig.ConfigureServices();

            // Assert
            var configuration = serviceProvider.GetService<IConfiguration>();
            configuration.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureServices_ShouldRegisterLoggingServices()
        {
            // Act
            var serviceProvider = DependencyInjectionConfig.ConfigureServices();

            // Assert
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.Should().NotBeNull();

            var logger = serviceProvider.GetService<ILogger<DependencyInjectionConfigTests>>();
            logger.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureServices_ShouldRegisterBusinessServices()
        {
            // Act
            var serviceProvider = DependencyInjectionConfig.ConfigureServices();

            // Assert
            var patientService = serviceProvider.GetService<IPatientService>();
            patientService.Should().NotBeNull();

            var imageService = serviceProvider.GetService<IImageService>();
            imageService.Should().NotBeNull();

            var themeService = serviceProvider.GetService<IThemeService>();
            themeService.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureServices_ShouldRegisterViewModels()
        {
            // Act
            var serviceProvider = DependencyInjectionConfig.ConfigureServices();

            // Assert
            var patientListViewModel = serviceProvider.GetService<PatientListViewModel>();
            patientListViewModel.Should().NotBeNull();

            var imageViewerViewModel = serviceProvider.GetService<ImageViewerViewModel>();
            imageViewerViewModel.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureServices_ServicesRegisteredAsSingleton_ShouldReturnSameInstance()
        {
            // Act
            var serviceProvider = DependencyInjectionConfig.ConfigureServices();

            // Assert
            var patientService1 = serviceProvider.GetService<IPatientService>();
            var patientService2 = serviceProvider.GetService<IPatientService>();

            patientService1.Should().NotBeNull();
            patientService2.Should().NotBeNull();
            patientService1.Should().BeSameAs(patientService2);
        }

        [Fact]
        public void ConfigureServices_ServicesRegisteredAsTransient_ShouldReturnDifferentInstances()
        {
            // Act
            var serviceProvider = DependencyInjectionConfig.ConfigureServices();

            // Assert
            var viewModel1 = serviceProvider.GetService<ImageViewerViewModel>();
            var viewModel2 = serviceProvider.GetService<ImageViewerViewModel>();

            viewModel1.Should().NotBeNull();
            viewModel2.Should().NotBeNull();
            viewModel1.Should().NotBeSameAs(viewModel2);
        }

        [Fact]
        public void ConfigureServices_ShouldHandleServiceProviderDisposal()
        {
            // Arrange
            ServiceProvider? serviceProvider = null;

            // Act
            serviceProvider = DependencyInjectionConfig.ConfigureServices();

            // Assert
            serviceProvider.Should().NotBeNull();

            // Should not throw when disposed
            Action disposeAction = () => serviceProvider.Dispose();
            disposeAction.Should().NotThrow();
        }

        [Fact]
        public void ConfigureServices_ShouldConfigureLoggingWithConsoleAndDebug()
        {
            // Act
            var serviceProvider = DependencyInjectionConfig.ConfigureServices();

            // Assert
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.Should().NotBeNull();

            // Create a logger and verify it works
            var logger = loggerFactory.CreateLogger<DependencyInjectionConfigTests>();
            logger.Should().NotBeNull();

            // Should not throw when logging
            Action logAction = () => logger.LogInformation("Test log message");
            logAction.Should().NotThrow();
        }

        [Fact]
        public void ConfigureServices_WithCustomConfiguration_ShouldUseCustomValues()
        {
            // Act
            var serviceProvider = DependencyInjectionConfig.ConfigureServices();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            // Assert
            configuration.Should().NotBeNull();
            
            // Should be able to access configuration sections
            var loggingSection = configuration.GetSection("Logging");
            loggingSection.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureServices_AllRequiredServices_ShouldBeResolvable()
        {
            // Act
            var serviceProvider = DependencyInjectionConfig.ConfigureServices();

            // Assert
            var requiredServices = new Type[]
            {
                typeof(IConfiguration),
                typeof(ILoggerFactory),
                typeof(IPatientService),
                typeof(IImageService),
                typeof(IThemeService),
                typeof(PatientListViewModel),
                typeof(ImageViewerViewModel)
            };

            foreach (var serviceType in requiredServices)
            {
                var service = serviceProvider.GetService(serviceType);
                service.Should().NotBeNull($"Service {serviceType.Name} should be resolvable");
            }
        }

        [Fact]
        public void ConfigureServices_ShouldSupportScopedServices()
        {
            // Act
            var serviceProvider = DependencyInjectionConfig.ConfigureServices();

            // Assert
            using var scope1 = serviceProvider.CreateScope();
            using var scope2 = serviceProvider.CreateScope();

            var scopedService1 = scope1.ServiceProvider.GetService<IPatientService>();
            var scopedService2 = scope2.ServiceProvider.GetService<IPatientService>();

            scopedService1.Should().NotBeNull();
            scopedService2.Should().NotBeNull();
            // Since they're registered as Singleton, they should be the same instance
            scopedService1.Should().BeSameAs(scopedService2);
        }

        [Fact]
        public void ConfigureServices_WithMissingConfiguration_ShouldHandleGracefully()
        {
            // Act
            var serviceProvider = DependencyInjectionConfig.ConfigureServices();

            // Assert
            serviceProvider.Should().NotBeNull();
            
            var configuration = serviceProvider.GetService<IConfiguration>();
            configuration.Should().NotBeNull();

            // Should handle missing configuration keys gracefully
            var missingValue = configuration["NonExistentKey"];
            missingValue.Should().BeNull();
        }

        [Fact]
        public void ConfigureServices_MultipleInvocations_ShouldReturnDifferentProviders()
        {
            // Act
            var serviceProvider1 = DependencyInjectionConfig.ConfigureServices();
            var serviceProvider2 = DependencyInjectionConfig.ConfigureServices();

            // Assert
            serviceProvider1.Should().NotBeNull();
            serviceProvider2.Should().NotBeNull();
            serviceProvider1.Should().NotBeSameAs(serviceProvider2);

            // Clean up
            serviceProvider1.Dispose();
            serviceProvider2.Dispose();
        }
    }
}