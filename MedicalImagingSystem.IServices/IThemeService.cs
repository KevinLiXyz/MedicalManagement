namespace MedicalImagingSystem.IServices
{
	/// <summary>
	/// Defines a contract for managing application themes.
	/// </summary>
	/// <remarks>Implementations of this interface are responsible for providing functionality to manage and apply
	/// themes within an application. This may include operations such as retrieving available themes, setting the current
	/// theme, and persisting theme preferences.</remarks>
	public interface IThemeService
	{
		string CurrentTheme { get; }
		Task<bool> SetThemeAsync(string themeName);
		string[] GetAvailableThemes();
	}
}
