using System.Windows.Media.Imaging;
using MedicalImagingSystem.Models;

namespace MedicalImagingSystem.IServices
{
	public interface IImageService
	{
		Task<BitmapImage?> LoadImageAsync(string imagePath);
		Task<List<MedicalImageModel>> GetPatientImagesAsync(string patientId);
		Task<bool> AddImageAsync(MedicalImageModel image);
		Task<bool> DeleteImageAsync(string imageId);
		bool IsSupportedFormat(string filePath);
		Task<MedicalImageModel> ImportImageAsync(string filePath, string patientId);
	}
}
