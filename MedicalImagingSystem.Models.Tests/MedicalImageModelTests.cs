using FluentAssertions;
using MedicalImagingSystem.Models;

namespace MedicalImagingSystem.Models.Tests
{
    public class MedicalImageModelTests
    {
        [Fact]
        public void MedicalImageModel_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var image = new MedicalImageModel();

            // Assert
            image.Id.Should().BeEmpty();
            image.PatientId.Should().BeEmpty();
            image.FileName.Should().BeEmpty();
            image.FilePath.Should().BeEmpty();
            image.CaptureDate.Should().Be(default(DateTime));
            image.Modality.Should().BeEmpty();
            image.BodyPart.Should().BeEmpty();
            image.Description.Should().BeEmpty();
            image.FileSize.Should().Be(0);
            image.Width.Should().Be(0);
            image.Height.Should().Be(0);
            image.IsLoading.Should().BeFalse();
        }

        [Theory]
        [InlineData("IMG001", "P001", "chest_xray.jpg", "C:\\Images\\chest_xray.jpg")]
        [InlineData("IMG002", "P002", "brain_mri.jpg", "C:\\Images\\brain_mri.jpg")]
        public void MedicalImageModel_BasicProperties_ShouldSetAndGetCorrectly(
            string id, string patientId, string fileName, string filePath)
        {
            // Arrange
            var image = new MedicalImageModel();

            // Act
            image.Id = id;
            image.PatientId = patientId;
            image.FileName = fileName;
            image.FilePath = filePath;

            // Assert
            image.Id.Should().Be(id);
            image.PatientId.Should().Be(patientId);
            image.FileName.Should().Be(fileName);
            image.FilePath.Should().Be(filePath);
        }

        [Theory]
        [InlineData("X-Ray", "胸部", "胸部正位片")]
        [InlineData("MRI", "头部", "脑部T1加权像")]
        [InlineData("CT", "腹部", "腹部平扫")]
        public void MedicalImageModel_MedicalProperties_ShouldSetAndGetCorrectly(
            string modality, string bodyPart, string description)
        {
            // Arrange
            var image = new MedicalImageModel();
            var captureDate = new DateTime(2024, 1, 15, 10, 30, 0);

            // Act
            image.Modality = modality;
            image.BodyPart = bodyPart;
            image.Description = description;
            image.CaptureDate = captureDate;

            // Assert
            image.Modality.Should().Be(modality);
            image.BodyPart.Should().Be(bodyPart);
            image.Description.Should().Be(description);
            image.CaptureDate.Should().Be(captureDate);
        }

        [Theory]
        [InlineData(1024, 768, 2048576)] // 1024x768, 2MB
        [InlineData(512, 512, 1048576)]  // 512x512, 1MB
        public void MedicalImageModel_ImageDimensions_ShouldSetAndGetCorrectly(
            int width, int height, long fileSize)
        {
            // Arrange
            var image = new MedicalImageModel();

            // Act
            image.Width = width;
            image.Height = height;
            image.FileSize = fileSize;

            // Assert
            image.Width.Should().Be(width);
            image.Height.Should().Be(height);
            image.FileSize.Should().Be(fileSize);
        }

        [Theory]
        [InlineData(1024L, "1.00 KB")]
        [InlineData(1048576L, "1.00 MB")]
        [InlineData(1073741824L, "1.00 GB")]
        [InlineData(0L, "0 B")]
        [InlineData(512L, "512 B")]
        public void MedicalImageModel_FileSizeFormatted_ShouldReturnCorrectFormat(long fileSize, string expected)
        {
            // Arrange
            var image = new MedicalImageModel { FileSize = fileSize };

            // Act
            var result = image.FileSizeFormatted;

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void MedicalImageModel_IsLoading_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var image = new MedicalImageModel();

            // Act & Assert
            image.IsLoading.Should().BeFalse();

            image.IsLoading = true;
            image.IsLoading.Should().BeTrue();

            image.IsLoading = false;
            image.IsLoading.Should().BeFalse();
        }

        [Fact]
        public void MedicalImageModel_PropertyChanged_ShouldRaiseWhenFileNameChanged()
        {
            // Arrange
            var image = new MedicalImageModel();
            var eventRaised = false;
            string? changedPropertyName = null;

            image.PropertyChanged += (sender, args) =>
            {
                eventRaised = true;
                changedPropertyName = args.PropertyName;
            };

            // Act
            image.FileName = "new_image.jpg";

            // Assert
            eventRaised.Should().BeTrue();
            changedPropertyName.Should().Be(nameof(MedicalImageModel.FileName));
        }

        [Fact]
        public void MedicalImageModel_PropertyChanged_ShouldRaiseWhenFileSizeChanged()
        {
            // Arrange
            var image = new MedicalImageModel();
            var fileSizeChangedEventRaised = false;
            var fileSizeFormattedChangedEventRaised = false;

            image.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(MedicalImageModel.FileSize))
                    fileSizeChangedEventRaised = true;
                if (args.PropertyName == nameof(MedicalImageModel.FileSizeFormatted))
                    fileSizeFormattedChangedEventRaised = true;
            };

            // Act
            image.FileSize = 1048576L;

            // Assert
            fileSizeChangedEventRaised.Should().BeTrue();
            fileSizeFormattedChangedEventRaised.Should().BeTrue();
        }

        [Fact]
        public void MedicalImageModel_PropertyChanged_ShouldRaiseWhenIsLoadingChanged()
        {
            // Arrange
            var image = new MedicalImageModel();
            var eventRaised = false;
            string? changedPropertyName = null;

            image.PropertyChanged += (sender, args) =>
            {
                eventRaised = true;
                changedPropertyName = args.PropertyName;
            };

            // Act
            image.IsLoading = true;

            // Assert
            eventRaised.Should().BeTrue();
            changedPropertyName.Should().Be(nameof(MedicalImageModel.IsLoading));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void MedicalImageModel_EmptyOrNullStringValues_ShouldBeHandledGracefully(string? value)
        {
            // Arrange
            var image = new MedicalImageModel();

            // Act & Assert - Should not throw exceptions
            image.FileName = value ?? string.Empty;
            image.FilePath = value ?? string.Empty;
            image.Modality = value ?? string.Empty;
            image.BodyPart = value ?? string.Empty;
            image.Description = value ?? string.Empty;

            image.FileName.Should().Be(value ?? string.Empty);
            image.FilePath.Should().Be(value ?? string.Empty);
            image.Modality.Should().Be(value ?? string.Empty);
            image.BodyPart.Should().Be(value ?? string.Empty);
            image.Description.Should().Be(value ?? string.Empty);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void MedicalImageModel_NegativeValues_ShouldBeSetWithoutException(int negativeValue)
        {
            // Arrange
            var image = new MedicalImageModel();

            // Act & Assert - Should not throw exceptions
            image.Width = negativeValue;
            image.Height = negativeValue;

            image.Width.Should().Be(negativeValue);
            image.Height.Should().Be(negativeValue);
        }
    }
}