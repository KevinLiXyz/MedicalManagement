using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedicalImagingSystem.IServices;
using MedicalImagingSystem.Models;
using Microsoft.Extensions.Logging;

namespace MedicalImagingSystem.ViewModels
{
	public partial class MainViewModel : ObservableObject
	{
		private readonly ILogger<MainViewModel> _logger;
		private readonly IPatientService _patientService;
		private readonly IThemeService _themeService;

		[ObservableProperty]
		private PatientListViewModel _patientListViewModel;

		[ObservableProperty]
		private ImageViewerViewModel _imageViewerViewModel;

		[ObservableProperty]
		private PatientModel? _selectedPatient;

		[ObservableProperty]
		private string _currentTheme;

		[ObservableProperty]
		private bool _isLoading;

		public MainViewModel(
			ILogger<MainViewModel> logger,
			IPatientService patientService,
			IThemeService themeService,
			PatientListViewModel patientListViewModel,
			ImageViewerViewModel imageViewerViewModel)
		{
			_logger = logger;
			_patientService = patientService;
			_themeService = themeService;
			_patientListViewModel = patientListViewModel;
			_imageViewerViewModel = imageViewerViewModel;
			_currentTheme = _themeService.CurrentTheme;

			// Subscribe to patient selection changes
			_patientListViewModel.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(PatientListViewModel.SelectedPatient))
				{
					SelectedPatient = _patientListViewModel.SelectedPatient;
					if (SelectedPatient != null)
					{
						_ = LoadPatientImagesAsync(SelectedPatient);
					}
				}
			};
		}

		[RelayCommand]
		private async Task SwitchThemeAsync()
		{
			var availableThemes = _themeService.GetAvailableThemes();
			var currentIndex = availableThemes.ToList().IndexOf(CurrentTheme);
			var nextIndex = (currentIndex + 1) % availableThemes.Length;
			var nextTheme = availableThemes[nextIndex];

			await _themeService.SetThemeAsync(nextTheme);
			CurrentTheme = nextTheme;
			_logger.LogInformation($"Theme switched to: {nextTheme}");
		}

		[RelayCommand]
		private async Task RefreshDataAsync()
		{
			IsLoading = true;
			try
			{
				await PatientListViewModel.RefreshPatientsAsync();
				_logger.LogInformation("Data refreshed successfully");
			}
			finally
			{
				IsLoading = false;
			}
		}

		private async Task LoadPatientImagesAsync(PatientModel patient)
		{
			await ImageViewerViewModel.LoadPatientImagesAsync(patient.Id);
		}
	}
}
