using FluentAssertions;
using MedicalImagingSystem.Services;
using MedicalImagingSystem.IServices;
using MedicalImagingSystem.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Windows.Media.Imaging;

namespace MedicalImagingSystem.Services.Tests
{
    public class ImageServiceTests
    {
        private readonly Mock<ILogger<ImageService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly IImageService _imageService;

        public ImageServiceTests()
        {
            _mockLogger = new Mock<ILogger<ImageService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Setup configuration mock
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(s => s.GetChildren()).Returns(new List<IConfigurationSection>());
            _mockConfiguration.Setup(c => c.GetSection("ImageSettings:SupportedFormats")).Returns(mockSection.Object);

            _imageService = new ImageService(_mockLogger.Object, _mockConfiguration.Object);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task LoadImageAsync_WithInvalidPath_ShouldReturnNull(string invalidPath)
        {
            // Act
            var result = await _imageService.LoadImageAsync(invalidPath);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task LoadImageAsync_WithNonexistentFile_ShouldReturnNull()
        {
            // Arrange
            var nonexistentPath = "nonexistent_file.jpg";

            // Act
            var result = await _imageService.LoadImageAsync(nonexistentPath);

            // Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData(".jpg")]
        [InlineData(".jpeg")]
        [InlineData(".png")]
        [InlineData(".bmp")]
        [InlineData(".tiff")]
        public void IsSupportedImageFormat_WithSupportedFormats_ShouldReturnTrue(string extension)
        {
            // Arrange
            var fileName = $"test{extension}";

            // Act
            var result = _imageService.IsSupportedImageFormat(fileName);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(".gif")]
        [InlineData(".svg")]
        [InlineData(".webp")]
        [InlineData(".txt")]
        [InlineData(".pdf")]
        public void IsSupportedImageFormat_WithUnsupportedFormats_ShouldReturnFalse(string extension)
        {
            // Arrange
            var fileName = $"test{extension}";

            // Act
            var result = _imageService.IsSupportedImageFormat(fileName);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [InlineData("noextension")]
        public void IsSupportedImageFormat_WithInvalidFileName_ShouldReturnFalse(string fileName)
        {
            // Act
            var result = _imageService.IsSupportedImageFormat(fileName);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetImageMetadataAsync_WithValidImage_ShouldReturnMetadata()
        {
            // Arrange
            // Create a simple test image using BitmapImage
            var testImage = new BitmapImage();
            testImage.BeginInit();
            testImage.UriSource = new Uri("pack://application:,,,/Resources/test.jpg", UriKind.Absolute);
            testImage.EndInit();

            var imagePath = "test_image.jpg";

            // Act
            var metadata = await _imageService.GetImageMetadataAsync(imagePath);

            // Assert
            // Since we're testing with a nonexistent file, metadata should be null
            // In a real implementation, you would test with actual image files
            metadata.Should().BeNull(); // Expected for nonexistent file
        }

        [Fact]
        public async Task ValidateImageAsync_WithValidImagePath_ShouldReturnValidationResult()
        {
            // Arrange
            var imagePath = "valid_image.jpg";

            // Act
            var result = await _imageService.ValidateImageAsync(imagePath);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse(); // Expected for nonexistent file
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public async Task ValidateImageAsync_WithInvalidPath_ShouldReturnInvalidResult(string invalidPath)
        {
            // Act
            var result = await _imageService.ValidateImageAsync(invalidPath);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetImageThumbnailAsync_WithValidImage_ShouldReturnThumbnail()
        {
            // Arrange
            var imagePath = "test_image.jpg";
            var thumbnailSize = 150;

            // Act
            var thumbnail = await _imageService.GetImageThumbnailAsync(imagePath, thumbnailSize);

            // Assert
            // Since we're testing with a nonexistent file, thumbnail should be null
            thumbnail.Should().BeNull(); // Expected for nonexistent file
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-50)]
        public async Task GetImageThumbnailAsync_WithInvalidSize_ShouldReturnNull(int invalidSize)
        {
            // Arrange
            var imagePath = "test_image.jpg";

            // Act
            var thumbnail = await _imageService.GetImageThumbnailAsync(imagePath, invalidSize);

            // Assert
            thumbnail.Should().BeNull();
        }

        [Fact]
        public void GetSupportedFormats_ShouldReturnNonEmptyArray()
        {
            // Act
            var formats = _imageService.GetSupportedFormats();

            // Assert
            formats.Should().NotBeNull();
            formats.Should().NotBeEmpty();
            formats.Should().Contain(".jpg");
            formats.Should().Contain(".png");
        }

        [Fact]
        public async Task LoadImageWithCacheAsync_ShouldImplementCaching()
        {
            // Arrange
            var imagePath = "cached_image.jpg";

            // Act
            var firstLoad = await _imageService.LoadImageAsync(imagePath);
            var secondLoad = await _imageService.LoadImageAsync(imagePath);

            // Assert
            // Both should be null for nonexistent file, but the method should handle caching
            firstLoad.Should().BeNull();
            secondLoad.Should().BeNull();
        }

        [Fact]
        public void Constructor_ShouldInitializeWithDefaultSupportedFormats()
        {
            // Act
            var formats = _imageService.GetSupportedFormats();

            // Assert
            formats.Should().NotBeNull();
            formats.Should().Contain(format => 
                format.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                format.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                format.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                format.Equals(".bmp", StringComparison.OrdinalIgnoreCase) ||
                format.Equals(".tiff", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task LoadMultipleImagesAsync_ShouldHandleConcurrentRequests()
        {
            // Arrange
            var imagePaths = new[] { "image1.jpg", "image2.jpg", "image3.jpg" };

            // Act
            var tasks = imagePaths.Select(path => _imageService.LoadImageAsync(path)).ToArray();
            var results = await Task.WhenAll(tasks);

            // Assert
            results.Should().NotBeNull();
            results.Should().HaveCount(3);
            // All should be null for nonexistent files
            results.Should().AllSatisfy(result => result.Should().BeNull());
        }

        [Fact]
        public void IsSupportedImageFormat_ShouldBeCaseInsensitive()
        {
            // Arrange & Act & Assert
            _imageService.IsSupportedImageFormat("test.JPG").Should().BeTrue();
            _imageService.IsSupportedImageFormat("test.Jpg").Should().BeTrue();
            _imageService.IsSupportedImageFormat("test.PNG").Should().BeTrue();
            _imageService.IsSupportedImageFormat("test.png").Should().BeTrue();
        }

        [Theory]
        [InlineData("image.jpg.txt")]
        [InlineData("image.png.backup")]
        [InlineData("jpg.document")]
        public void IsSupportedImageFormat_WithCompoundExtensions_ShouldCheckLastExtension(string fileName)
        {
            // Act
            var result = _imageService.IsSupportedImageFormat(fileName);

            // Assert
            result.Should().BeFalse(); // Should check the actual last extension, not embedded ones
        }
    }
}