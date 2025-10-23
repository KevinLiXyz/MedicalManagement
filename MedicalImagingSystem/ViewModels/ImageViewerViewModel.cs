using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MedicalImagingSystem.Models;
using MedicalImagingSystem.Services;

namespace MedicalImagingSystem.ViewModels
{
    public partial class ImageViewerViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private readonly ILogger<ImageViewerViewModel> _logger;
        private readonly IImageService _imageService;
        private readonly IPatientService _patientService;
        private readonly IConfiguration _configuration;
        private readonly double[] _zoomLevels;

        [ObservableProperty]
        private ObservableCollection<MedicalImage> _images = new();

        [ObservableProperty]
        private MedicalImage? _selectedImage;

        [ObservableProperty]
        private BitmapImage? _currentImageSource;

        [ObservableProperty]
        private double _zoomLevel = 1.0;

        [ObservableProperty]
        private bool _isImageLoading;

        [ObservableProperty]
        private string _loadingMessage = "加载图像中...";

        private int _currentZoomIndex = 4; // Default to 1.0 (index 4 in zoom levels)

        public ImageViewerViewModel(
            ILogger<ImageViewerViewModel> logger,
            IImageService imageService,
            IPatientService patientService,
            IConfiguration configuration)
        {
            _logger = logger;
            _imageService = imageService;
            _patientService = patientService;
            _configuration = configuration;
            
            _zoomLevels = _configuration.GetSection("UISettings:ZoomLevels").GetChildren()
	                          .Select(section => double.Parse(section.Value)).ToArray() 
                ?? new[] { 0.1, 0.25, 0.5, 0.75, 1.0, 1.25, 1.5, 2.0, 3.0, 4.0, 5.0 };
        }

        [RelayCommand]
        public async Task LoadPatientImagesAsync(string patientId)
        {
            IsImageLoading = true;
            LoadingMessage = "加载患者图像...";
            
            try
            {
                // 从PatientService获取患者及其图片
                var patient = await _patientService.GetPatientByIdAsync(patientId);
                var images = patient?.Images ?? new List<MedicalImage>();
                
                Images.Clear();
                foreach (var image in images)
                {
                    Images.Add(image);
                }

                // Select first image if available
                if (Images.Count > 0)
                {
                    SelectedImage = Images.First();
                    await LoadSelectedImageAsync();
                }
                else
                {
                    SelectedImage = null;
                    CurrentImageSource = null;
                }

                _logger.LogInformation($"Loaded {images.Count} images for patient: {patientId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading images for patient: {patientId}");
            }
            finally
            {
                IsImageLoading = false;
            }
        }

        [RelayCommand]
        private async Task LoadSelectedImageAsync()
        {
            if (SelectedImage == null) return;

            IsImageLoading = true;
            LoadingMessage = $"加载图像: {SelectedImage.FileName}";
            SelectedImage.IsLoading = true;

            try
            {
                var imageSource = await _imageService.LoadImageAsync(SelectedImage.FilePath);
                CurrentImageSource = imageSource;
                
                if (imageSource != null)
                {
                    _logger.LogInformation($"Image loaded: {SelectedImage.FileName}");
                }
                else
                {
                    _logger.LogWarning($"Failed to load image: {SelectedImage.FileName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading image: {SelectedImage.FileName}");
            }
            finally
            {
                IsImageLoading = false;
                SelectedImage.IsLoading = false;
            }
        }

        [RelayCommand]
        private void ZoomIn()
        {
            if (_currentZoomIndex < _zoomLevels.Length - 1)
            {
                _currentZoomIndex++;
                ZoomLevel = _zoomLevels[_currentZoomIndex];
                _logger.LogDebug($"Zoomed in to: {ZoomLevel:P0}");
            }
        }

        [RelayCommand]
        private void ZoomOut()
        {
            if (_currentZoomIndex > 0)
            {
                _currentZoomIndex--;
                ZoomLevel = _zoomLevels[_currentZoomIndex];
                _logger.LogDebug($"Zoomed out to: {ZoomLevel:P0}");
            }
        }

        [RelayCommand]
        private void ResetZoom()
        {
            _currentZoomIndex = Array.IndexOf(_zoomLevels, 1.0);
            if (_currentZoomIndex == -1) _currentZoomIndex = 4; // Fallback
            ZoomLevel = 1.0;
            _logger.LogDebug("Zoom reset to 100%");
        }

        [RelayCommand]
        private void FitToWindow()
        {
            // This would be implemented based on the actual window size
            // For now, just reset to 100%
            ResetZoom();
        }

        partial void OnSelectedImageChanged(MedicalImage? value)
        {
            if (value != null)
            {
                _ = LoadSelectedImageAsync();
                _logger.LogInformation($"Selected image: {value.FileName}");
            }
        }

        public bool CanZoomIn => _currentZoomIndex < _zoomLevels.Length - 1;
        public bool CanZoomOut => _currentZoomIndex > 0;
        public string ZoomPercentage => $"{ZoomLevel:P0}";
    }
}