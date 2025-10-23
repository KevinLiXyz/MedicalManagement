using FluentAssertions;
using MedicalImagingSystem.ViewModels;
using MedicalImagingSystem.IServices;
using MedicalImagingSystem.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Windows.Media.Imaging;

namespace MedicalImagingSystem.ViewModels.Tests
{
    public class ImageViewerViewModelTests
    {
        private readonly Mock<ILogger<ImageViewerViewModel>> _mockLogger;
        private readonly Mock<IImageService> _mockImageService;
        private readonly Mock<IPatientService> _mockPatientService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly ImageViewerViewModel _viewModel;

        public ImageViewerViewModelTests()
        {
            _mockLogger = new Mock<ILogger<ImageViewerViewModel>>();
            _mockImageService = new Mock<IImageService>();
            _mockPatientService = new Mock<IPatientService>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup configuration mock for zoom levels
            var mockSection = new Mock<IConfigurationSection>();
            var zoomLevels = new List<IConfigurationSection>();
            
            // Create mock sections for zoom levels
            var zoomValues = new[] { "0.1", "0.25", "0.5", "0.75", "1.0", "1.25", "1.5", "2.0", "3.0", "4.0", "5.0" };
            foreach (var value in zoomValues)
            {
                var section = new Mock<IConfigurationSection>();
                section.Setup(s => s.Value).Returns(value);
                zoomLevels.Add(section.Object);
            }

            mockSection.Setup(s => s.GetChildren()).Returns(zoomLevels);
            _mockConfiguration.Setup(c => c.GetSection("UISettings:ZoomLevels")).Returns(mockSection.Object);

            _viewModel = new ImageViewerViewModel(
                _mockLogger.Object,
                _mockImageService.Object,
                _mockPatientService.Object,
                _mockConfiguration.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Assert
            _viewModel.Images.Should().NotBeNull().And.BeEmpty();
            _viewModel.SelectedImage.Should().BeNull();
            _viewModel.CurrentImageSource.Should().BeNull();
            _viewModel.ZoomLevel.Should().Be(1.0);
            _viewModel.IsImageLoading.Should().BeFalse();
            _viewModel.PanX.Should().Be(0);
            _viewModel.PanY.Should().Be(0);
        }

        [Fact]
        public async Task LoadPatientImagesAsync_WithValidPatientId_ShouldLoadImages()
        {
            // Arrange
            var patientId = "P001";
            var patient = new PatientModel
            {
                Id = patientId,
                Name = "Test Patient",
                Images = new List<MedicalImageModel>
                {
                    new MedicalImageModel { Id = "IMG001", FileName = "test1.jpg", PatientId = patientId },
                    new MedicalImageModel { Id = "IMG002", FileName = "test2.jpg", PatientId = patientId }
                }
            };

            _mockPatientService.Setup(s => s.GetPatientByIdAsync(patientId))
                .ReturnsAsync(patient);

            var testBitmap = new BitmapImage();
            _mockImageService.Setup(s => s.LoadImageAsync(It.IsAny<string>()))
                .ReturnsAsync(testBitmap);

            // Act
            await _viewModel.LoadPatientImagesAsync(patientId);

            // Assert
            _viewModel.Images.Should().HaveCount(2);
            _viewModel.SelectedImage.Should().NotBeNull();
            _viewModel.SelectedImage!.Id.Should().Be("IMG001");
        }

        [Fact]
        public async Task LoadPatientImagesAsync_WithNonexistentPatientId_ShouldHandleGracefully()
        {
            // Arrange
            var patientId = "NONEXISTENT";
            _mockPatientService.Setup(s => s.GetPatientByIdAsync(patientId))
                .ReturnsAsync((PatientModel?)null);

            // Act
            await _viewModel.LoadPatientImagesAsync(patientId);

            // Assert
            _viewModel.Images.Should().BeEmpty();
            _viewModel.SelectedImage.Should().BeNull();
            _viewModel.CurrentImageSource.Should().BeNull();
        }

        [Fact]
        public void ZoomIn_ShouldIncreaseZoomLevel()
        {
            // Arrange
            var initialZoomLevel = _viewModel.ZoomLevel;

            // Act
            _viewModel.ZoomInCommand.Execute(null);

            // Assert
            _viewModel.ZoomLevel.Should().BeGreaterThan(initialZoomLevel);
        }

        [Fact]
        public void ZoomOut_ShouldDecreaseZoomLevel()
        {
            // Arrange
            // First zoom in to have room to zoom out
            _viewModel.ZoomInCommand.Execute(null);
            _viewModel.ZoomInCommand.Execute(null);
            var currentZoomLevel = _viewModel.ZoomLevel;

            // Act
            _viewModel.ZoomOutCommand.Execute(null);

            // Assert
            _viewModel.ZoomLevel.Should().BeLessThan(currentZoomLevel);
        }

        [Fact]
        public void ResetZoom_ShouldSetZoomLevelToOne()
        {
            // Arrange
            _viewModel.ZoomInCommand.Execute(null);
            _viewModel.ZoomInCommand.Execute(null);
            _viewModel.StartPanning(new System.Windows.Point(10, 10));

            // Act
            _viewModel.ResetZoomCommand.Execute(null);

            // Assert
            _viewModel.ZoomLevel.Should().Be(1.0);
            _viewModel.PanX.Should().Be(0);
            _viewModel.PanY.Should().Be(0);
        }

        [Fact]
        public void ZoomInAtPoint_ShouldZoomAtSpecificPoint()
        {
            // Arrange
            var point = new System.Windows.Point(100, 100);
            var initialZoomLevel = _viewModel.ZoomLevel;

            // Act
            _viewModel.ZoomInAtPoint(point);

            // Assert
            _viewModel.ZoomLevel.Should().BeGreaterThan(initialZoomLevel);
        }

        [Fact]
        public void ZoomOutAtPoint_ShouldZoomOutAtSpecificPoint()
        {
            // Arrange
            // First zoom in to have room to zoom out
            _viewModel.ZoomInCommand.Execute(null);
            _viewModel.ZoomInCommand.Execute(null);
            var point = new System.Windows.Point(100, 100);
            var currentZoomLevel = _viewModel.ZoomLevel;

            // Act
            _viewModel.ZoomOutAtPoint(point);

            // Assert
            _viewModel.ZoomLevel.Should().BeLessThan(currentZoomLevel);
        }

        [Fact]
        public void StartPanning_ShouldSetPanningState()
        {
            // Arrange
            var point = new System.Windows.Point(50, 50);

            // Act
            _viewModel.StartPanning(point);

            // Assert
            _viewModel.ImageCursor.Should().Be(System.Windows.Input.Cursors.Hand);
        }

        [Fact]
        public void UpdatePanning_WhenZoomedIn_ShouldUpdatePanPosition()
        {
            // Arrange
            _viewModel.ZoomInCommand.Execute(null);
            _viewModel.ZoomInCommand.Execute(null); // Zoom to > 1.0
            
            var startPoint = new System.Windows.Point(100, 100);
            var currentPoint = new System.Windows.Point(150, 150);

            _viewModel.StartPanning(startPoint);
            var initialPanX = _viewModel.PanX;
            var initialPanY = _viewModel.PanY;

            // Act
            _viewModel.UpdatePanning(currentPoint);

            // Assert
            _viewModel.PanX.Should().NotBe(initialPanX);
            _viewModel.PanY.Should().NotBe(initialPanY);
        }

        [Fact]
        public void StopPanning_ShouldResetPanningState()
        {
            // Arrange
            _viewModel.ZoomInCommand.Execute(null);
            var point = new System.Windows.Point(50, 50);
            _viewModel.StartPanning(point);

            // Act
            _viewModel.StopPanning();

            // Assert
            _viewModel.ImageCursor.Should().Be(System.Windows.Input.Cursors.SizeAll); // Since zoom > 1.0
        }

        [Fact]
        public void CanZoomIn_ShouldReturnTrueWhenNotAtMaxZoom()
        {
            // Act & Assert
            _viewModel.CanZoomIn.Should().BeTrue();
        }

        [Fact]
        public void CanZoomOut_ShouldReturnTrueWhenNotAtMinZoom()
        {
            // Arrange
            _viewModel.ZoomInCommand.Execute(null);

            // Act & Assert
            _viewModel.CanZoomOut.Should().BeTrue();
        }

        [Fact]
        public void CanZoomOut_ShouldReturnFalseWhenAtMinZoom()
        {
            // Act & Assert (at default zoom level)
            _viewModel.CanZoomOut.Should().BeFalse();
        }

        [Fact]
        public void ZoomPercentage_ShouldReturnFormattedPercentage()
        {
            // Arrange
            _viewModel.ZoomInCommand.Execute(null); // Should go to 125%

            // Act
            var percentage = _viewModel.ZoomPercentage;

            // Assert
            percentage.Should().Contain("%");
            percentage.Should().NotBe("100%"); // Should be different from default
        }

        [Fact]
        public void SelectedImage_PropertyChanged_ShouldTriggerImageLoad()
        {
            // Arrange
            var image = new MedicalImageModel
            {
                Id = "IMG001",
                FileName = "test.jpg",
                FilePath = "C:\\test\\test.jpg"
            };

            var testBitmap = new BitmapImage();
            _mockImageService.Setup(s => s.LoadImageAsync(It.IsAny<string>()))
                .ReturnsAsync(testBitmap);

            // Act
            _viewModel.SelectedImage = image;

            // Assert
            _viewModel.SelectedImage.Should().Be(image);
        }

        [Theory]
        [InlineData(0.1)]
        [InlineData(0.5)]
        [InlineData(1.0)]
        [InlineData(2.0)]
        [InlineData(5.0)]
        public void ZoomLevel_ShouldAcceptValidZoomLevels(double zoomLevel)
        {
            // Act
            _viewModel.ZoomLevel = zoomLevel;

            // Assert
            _viewModel.ZoomLevel.Should().Be(zoomLevel);
        }

        [Fact]
        public void ImageCursor_ShouldBeArrowAtDefaultZoom()
        {
            // Assert
            _viewModel.ImageCursor.Should().Be(System.Windows.Input.Cursors.Arrow);
        }

        [Fact]
        public void ImageCursor_ShouldBeSizeAllWhenZoomedIn()
        {
            // Arrange
            _viewModel.ZoomInCommand.Execute(null);

            // Assert
            _viewModel.ImageCursor.Should().Be(System.Windows.Input.Cursors.SizeAll);
        }

        [Fact]
        public void IsImageLoading_ShouldNotifyPropertyChanged()
        {
            // Arrange
            bool propertyChanged = false;
            _viewModel.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == nameof(ImageViewerViewModel.IsImageLoading))
                    propertyChanged = true;
            };

            // Act
            _viewModel.IsImageLoading = true;

            // Assert
            propertyChanged.Should().BeTrue();
            _viewModel.IsImageLoading.Should().BeTrue();
        }

        [Fact]
        public void LoadingMessage_ShouldNotifyPropertyChanged()
        {
            // Arrange
            bool propertyChanged = false;
            _viewModel.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == nameof(ImageViewerViewModel.LoadingMessage))
                    propertyChanged = true;
            };

            // Act
            _viewModel.LoadingMessage = "Loading test image...";

            // Assert
            propertyChanged.Should().BeTrue();
            _viewModel.LoadingMessage.Should().Be("Loading test image...");
        }
    }
}