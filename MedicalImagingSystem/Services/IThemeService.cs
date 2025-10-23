using System.Threading.Tasks;

namespace MedicalImagingSystem.Services
{
    public interface IThemeService
    {
        string CurrentTheme { get; }
        Task<bool> SetThemeAsync(string themeName);
        string[] GetAvailableThemes();
    }
}