using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MedicalImagingSystem.Models;

namespace MedicalImagingSystem.Services
{
    public class ImageService : IImageService
    {
        private readonly ILogger<ImageService> _logger;
        private readonly IConfiguration _configuration;
        private readonly List<MedicalImage> _images;
        private readonly string?[] _supportedFormats;

        public ImageService(ILogger<ImageService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _images = new List<MedicalImage>();

			// With this updated code:
			_supportedFormats = _configuration.GetSection("ImageSettings:SupportedFormats").GetChildren()
				.Select(section => section.Value).ToArray()
				?? new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff" };
        }

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

        public async Task<List<MedicalImage>> GetPatientImagesAsync(string patientId)
        {
            await Task.Delay(100);
            return _images.Where(img => img.PatientId == patientId).ToList();
        }

        public async Task<bool> AddImageAsync(MedicalImage image)
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

        public async Task<MedicalImage> ImportImageAsync(string filePath, string patientId)
        {
            var fileInfo = new FileInfo(filePath);
            
            var medicalImage = new MedicalImage
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