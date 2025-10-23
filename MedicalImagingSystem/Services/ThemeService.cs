using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace MedicalImagingSystem.Services
{
    public class ThemeService : IThemeService
    {
        private readonly ILogger<ThemeService> _logger;
        private readonly string[] _availableThemes = { "Light", "Dark" };
        private string _currentTheme = "Light";

        public ThemeService(ILogger<ThemeService> logger)
        {
            _logger = logger;
        }

        public string CurrentTheme => _currentTheme;

        public async Task<bool> SetThemeAsync(string themeName)
        {
            try
            {
                if (!_availableThemes.Contains(themeName))
                {
                    _logger.LogWarning($"Theme not found: {themeName}");
                    return false;
                }

                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var app = Application.Current;
                        var resources = app.Resources;

                        // Remove current theme
                        var currentThemeDict = resources.MergedDictionaries
                            .FirstOrDefault(d => d.Source?.OriginalString.Contains("Theme.xaml") == true);
                        if (currentThemeDict != null)
                        {
                            resources.MergedDictionaries.Remove(currentThemeDict);
                        }

                        // Add new theme
                        var newThemeDict = new ResourceDictionary();
                        newThemeDict.Source = new Uri($"Themes/{themeName}Theme.xaml", UriKind.Relative);
                        resources.MergedDictionaries.Add(newThemeDict);

                        _currentTheme = themeName;
                        _logger.LogInformation($"Theme changed to: {themeName}");
                    });
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting theme: {themeName}");
                return false;
            }
        }

        public string[] GetAvailableThemes()
        {
            return _availableThemes.ToArray();
        }
    }
}