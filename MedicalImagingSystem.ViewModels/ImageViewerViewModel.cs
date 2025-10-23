using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MedicalImagingSystem.IServices;
using MedicalImagingSystem.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace MedicalImagingSystem.ViewModels
{
	/// <summary>
	/// Represents the view model for an image viewer, providing properties and commands to manage and interact with image
	/// data in a user interface.
	/// </summary>
	/// <remarks>This class is typically used in MVVM (Model-View-ViewModel) patterns to bind image-related data and
	/// operations to the user interface. It serves as the intermediary between the view and the underlying image data or
	/// logic.</remarks>
	public partial class ImageViewerViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
	{
		private readonly ILogger<ImageViewerViewModel> _logger;
		private readonly IImageService _imageService;
		private readonly IPatientService _patientService;
		private readonly IConfiguration _configuration;
		private readonly double[] _zoomLevels;

		[ObservableProperty]
		private ObservableCollection<MedicalImageModel> _images = new();

		[ObservableProperty]
		private MedicalImageModel? _selectedImage;

		[ObservableProperty]
		private BitmapImage? _currentImageSource;

		[ObservableProperty]
		private double _zoomLevel = 1.0;

		[ObservableProperty]
		private bool _isImageLoading;

		[ObservableProperty]
		private string _loadingMessage = "加载图像中...";

		[ObservableProperty]
		private double _panX;

		[ObservableProperty]
		private double _panY;

		[ObservableProperty]
		private Cursor _imageCursor = Cursors.Arrow;

		private int _currentZoomIndex = 4; // Default to 1.0 (index 4 in zoom levels)
		private System.Windows.Point _lastPanPoint;
		private bool _isPanning;

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
				var images = patient?.Images ?? new List<MedicalImageModel>();

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
				UpdateCursor();
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
				UpdateCursor();
				_logger.LogDebug($"Zoomed out to: {ZoomLevel:P0}");
			}
		}

		public void ZoomInAtPoint(System.Windows.Point point)
		{
			if (_currentZoomIndex < _zoomLevels.Length - 1)
			{
				var oldZoomLevel = ZoomLevel;
				_currentZoomIndex++;
				ZoomLevel = _zoomLevels[_currentZoomIndex];
				
				// Adjust pan position to zoom into the clicked point
				var zoomFactor = ZoomLevel / oldZoomLevel;
				PanX = point.X - (point.X - PanX) * zoomFactor;
				PanY = point.Y - (point.Y - PanY) * zoomFactor;
				
				UpdateCursor();
				_logger.LogDebug($"Zoomed in to: {ZoomLevel:P0} at point ({point.X:F0}, {point.Y:F0})");
			}
		}

		public void ZoomOutAtPoint(System.Windows.Point point)
		{
			if (_currentZoomIndex > 0)
			{
				var oldZoomLevel = ZoomLevel;
				_currentZoomIndex--;
				ZoomLevel = _zoomLevels[_currentZoomIndex];
				
				// Adjust pan position to zoom out from the clicked point
				var zoomFactor = ZoomLevel / oldZoomLevel;
				PanX = point.X - (point.X - PanX) * zoomFactor;
				PanY = point.Y - (point.Y - PanY) * zoomFactor;
				
				UpdateCursor();
				_logger.LogDebug($"Zoomed out to: {ZoomLevel:P0} at point ({point.X:F0}, {point.Y:F0})");
			}
		}

		[RelayCommand]
		private void ResetZoom()
		{
			_currentZoomIndex = Array.IndexOf(_zoomLevels, 1.0);
			if (_currentZoomIndex == -1) _currentZoomIndex = 4; // Fallback
			ZoomLevel = 1.0;
			PanX = 0;
			PanY = 0;
			UpdateCursor();
			_logger.LogDebug("Zoom reset to 100%");
		}

		[RelayCommand]
		private void FitToWindow()
		{
			// This would be implemented based on the actual window size
			// For now, just reset to 100%
			ResetZoom();
		}

		public void StartPanning(System.Windows.Point point)
		{
			_isPanning = true;
			_lastPanPoint = point;
			ImageCursor = Cursors.Hand;
		}

		public void UpdatePanning(System.Windows.Point currentPoint)
		{
			if (_isPanning && ZoomLevel > 1.0)
			{
				var deltaX = currentPoint.X - _lastPanPoint.X;
				var deltaY = currentPoint.Y - _lastPanPoint.Y;
				
				PanX += deltaX;
				PanY += deltaY;
				
				_lastPanPoint = currentPoint;
			}
		}

		public void StopPanning()
		{
			_isPanning = false;
			UpdateCursor();
		}

		private void UpdateCursor()
		{
			ImageCursor = ZoomLevel > 1.0 ? Cursors.SizeAll : Cursors.Arrow;
		}

		partial void OnSelectedImageChanged(MedicalImageModel? value)
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
