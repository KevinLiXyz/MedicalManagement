using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MedicalImagingSystem.Models;
using MedicalImagingSystem.Services;

namespace MedicalImagingSystem.ViewModels
{
    public partial class PatientListViewModel : ObservableObject
    {
        private readonly ILogger<PatientListViewModel> _logger;
        private readonly IPatientService _patientService;

        [ObservableProperty]
        private ObservableCollection<Patient> _patients = new();

        [ObservableProperty]
        private Patient? _selectedPatient;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        public PatientListViewModel(ILogger<PatientListViewModel> logger, IPatientService patientService)
        {
            _logger = logger;
            _patientService = patientService;
            
            // Load initial data
            _ = LoadPatientsAsync();
        }

        [RelayCommand]
        private async Task LoadPatientsAsync()
        {
            IsLoading = true;
            try
            {
                var patients = await _patientService.GetAllPatientsAsync();
                Patients.Clear();
                foreach (var patient in patients)
                {
                    Patients.Add(patient);
                }
                _logger.LogInformation($"Loaded {patients.Count} patients");
                
                // 记录每个患者的图片数量
                foreach (var patient in patients)
                {
                    _logger.LogInformation($"Patient '{patient.Name}' has {patient.Images?.Count ?? 0} images");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading patients");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SearchPatientsAsync()
        {
            IsLoading = true;
            try
            {
                var patients = await _patientService.SearchPatientsAsync(SearchText);
                Patients.Clear();
                foreach (var patient in patients)
                {
                    Patients.Add(patient);
                }
                _logger.LogInformation($"Search returned {patients.Count} patients for term: {SearchText}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching patients with term: {SearchText}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task RefreshPatientsAsync()
        {
            await LoadPatientsAsync();
        }

        partial void OnSearchTextChanged(string value)
        {
            // Debounce search - in a real app, you might want to use a timer
            _ = SearchPatientsAsync();
        }

        partial void OnSelectedPatientChanged(Patient? value)
        {
            if (value != null)
            {
                _logger.LogInformation($"Selected patient: {value.Name}");
            }
        }
    }
}