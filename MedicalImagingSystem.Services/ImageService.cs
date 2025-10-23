using System.Windows.Media.Imaging;
using MedicalImagingSystem.IServices;
using MedicalImagingSystem.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MedicalImagingSystem.Services
{
	/// <summary>
	/// Provides functionality for managing and processing images, including operations such as loading, saving, and
	/// transforming image data.
	/// </summary>
	/// <remarks>This class serves as a central service for image-related operations. It is designed to be extended
	/// with methods and properties  that handle specific image processing tasks, such as resizing, format conversion, and
	/// metadata manipulation.</remarks>
	public class ImageService : IImageService
	{
		private readonly ILogger<ImageService> _logger;
		private readonly IConfiguration _configuration;
		private readonly List<MedicalImageModel> _images;
		private readonly string?[] _supportedFormats;
		public ImageService(ILogger<ImageService> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
			_images = new List<MedicalImageModel>();

			// With this updated code:
			_supportedFormats = _configuration.GetSection("ImageSettings:SupportedFormats").GetChildren()
			.Select(section => section.Value).ToArray()
		   ?? new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff" };
		}

		/// <summary>
		/// Asynchronously loads an image from the specified file path and returns it as a <see cref="BitmapImage"/>.
		/// </summary>
		/// <remarks>The method attempts to load the image file from the specified path. If the file does not exist, a
		/// warning is logged, and the method returns <see langword="null"/>. If an error occurs during the loading process,
		/// the exception is logged, and the method also returns <see langword="null"/>.  The returned <see
		/// cref="BitmapImage"/> is frozen to make it thread-safe and immutable.</remarks>
		/// <param name="imagePath">The path to the image file. This can be either an absolute path or a relative path. If a relative path is
		/// provided, it is resolved relative to the application's current directory.</param>
		/// <returns>A <see cref="BitmapImage"/> representing the loaded image, or <see langword="null"/> if the file does not exist or
		/// an error occurs during loading.</returns>
		public async Task<BitmapImage?> LoadImageAsync(string imagePath)
		{
			try
			{
				// Convert relative path to absolute path
				string absolutePath;
				if (Path.IsPathRooted(imagePath))
				{
					absolutePath = imagePath;
				}
				else
				{
					// Get the application directory and combine with relative path
					var appDirectory = Directory.GetCurrentDirectory();
					absolutePath = Path.Combine(appDirectory, imagePath);
				}

				if (!File.Exists(absolutePath))
				{
					_logger.LogWarning($"Image file not found: {absolutePath}");
					return null;
				}

				var bitmap = new BitmapImage();
				
				bitmap.BeginInit();
				bitmap.CacheOption = BitmapCacheOption.OnLoad;
				bitmap.UriSource = new Uri(absolutePath, UriKind.Absolute);
				bitmap.EndInit();
				bitmap.Freeze();

				await Task.Delay(100); // Simulate loading time
				_logger.LogInformation($"Successfully loaded image: {Path.GetFileName(absolutePath)}");
				return bitmap;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error loading image: {imagePath}");
				return null;
			}
		}

		public async Task<List<MedicalImageModel>> GetPatientImagesAsync(string patientId)
		{
			await Task.Delay(100);
			return _images.Where(img => img.PatientId == patientId).ToList();
		}

		public async Task<bool> AddImageAsync(MedicalImageModel image)
		{
			try
			{
				await Task.Delay(50);
				image.Id = Guid.NewGuid().ToString();
				_images.Add(image);
				_logger.LogInformation($"Image added: {image.FileName}");
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error adding image: {image.FileName}");
				return false;
			}
		}

		public async Task<bool> DeleteImageAsync(string imageId)
		{
			try
			{
				await Task.Delay(50);
				var image = _images.FirstOrDefault(img => img.Id == imageId);
				if (image != null)
				{
					_images.Remove(image);
					_logger.LogInformation($"Image deleted: {image.FileName}");
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error deleting image with ID: {imageId}");
				return false;
			}
		}

		public bool IsSupportedFormat(string filePath)
		{
			var extension = Path.GetExtension(filePath).ToLowerInvariant();
			return _supportedFormats.Contains(extension);
		}

		/// <summary>
		/// Imports a medical image from the specified file path and associates it with the given patient ID.
		/// </summary>
		/// <remarks>This method reads the specified image file, extracts its metadata (e.g., dimensions, file size),
		/// and creates a new  <see cref="MedicalImageModel"/> instance. The image is then added to the system's storage or
		/// database. <para> If the image dimensions cannot be determined, the width and height properties of the returned
		/// model will remain unset. </para> <para> The method logs a warning if the image dimensions cannot be read, but it
		/// does not throw an exception in such cases. </para></remarks>
		/// <param name="filePath">The full path to the image file to be imported. The file must exist and be accessible.</param>
		/// <param name="patientId">The unique identifier of the patient to associate with the imported image. Cannot be null or empty.</param>
		/// <returns>A <see cref="MedicalImageModel"/> representing the imported medical image, including metadata such as file name,
		/// size, and dimensions.</returns>
		public async Task<MedicalImageModel> ImportImageAsync(string filePath, string patientId)
		{
			var fileInfo = new FileInfo(filePath);

			var medicalImage = new MedicalImageModel
			{
				Id = Guid.NewGuid().ToString(),
				PatientId = patientId,
				FileName = fileInfo.Name,
				FilePath = filePath,
				CaptureDate = DateTime.Now,
				FileSize = fileInfo.Length,
				Modality = "Unknown",
				BodyPart = "Unknown",
				Description = "Imported image"
			};

			try
			{
				// Get image dimensions
				using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
				if (decoder.Frames.Count > 0)
				{
					var frame = decoder.Frames[0];
					medicalImage.Width = frame.PixelWidth;
					medicalImage.Height = frame.PixelHeight;
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, $"Could not read image dimensions for: {filePath}");
			}

			await AddImageAsync(medicalImage);
			return medicalImage;
		}
	}
}
