using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MedicalImagingSystem.Models;

namespace MedicalImagingSystem.Services
{
    public interface IImageService
    {
        Task<BitmapImage?> LoadImageAsync(string imagePath);
        Task<List<MedicalImage>> GetPatientImagesAsync(string patientId);
        Task<bool> AddImageAsync(MedicalImage image);
        Task<bool> DeleteImageAsync(string imageId);
        bool IsSupportedFormat(string filePath);
        Task<MedicalImage> ImportImageAsync(string filePath, string patientId);
    }
}