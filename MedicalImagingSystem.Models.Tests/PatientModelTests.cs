using FluentAssertions;
using MedicalImagingSystem.Models;
using System.ComponentModel;

namespace MedicalImagingSystem.Models.Tests
{
    public class PatientModelTests
    {
        [Fact]
        public void PatientModel_Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var patient = new PatientModel();

            // Assert
            patient.Id.Should().BeEmpty();
            patient.Name.Should().BeEmpty();
            patient.BirthDate.Should().Be(default(DateTime));
            patient.Gender.Should().BeEmpty();
            patient.MedicalRecordNumber.Should().BeEmpty();
            patient.Images.Should().NotBeNull().And.BeEmpty();
        }

        [Theory]
        [InlineData("P001", "张三", "男", "MR001")]
        [InlineData("P002", "李四", "女", "MR002")]
        public void PatientModel_Properties_ShouldSetAndGetCorrectly(string id, string name, string gender, string medicalRecordNumber)
        {
            // Arrange
            var patient = new PatientModel();
            var birthDate = new DateTime(1990, 5, 15);

            // Act
            patient.Id = id;
            patient.Name = name;
            patient.Gender = gender;
            patient.MedicalRecordNumber = medicalRecordNumber;
            patient.BirthDate = birthDate;

            // Assert
            patient.Id.Should().Be(id);
            patient.Name.Should().Be(name);
            patient.Gender.Should().Be(gender);
            patient.MedicalRecordNumber.Should().Be(medicalRecordNumber);
            patient.BirthDate.Should().Be(birthDate);
        }

        [Fact]
        public void PatientModel_Age_ShouldCalculateCorrectly()
        {
            // Arrange
            var patient = new PatientModel();
            var birthDate = DateTime.Now.AddYears(-30).AddMonths(-6);
            patient.BirthDate = birthDate;

            // Act
            var age = patient.Age;

            // Assert
            age.Should().Be(30);
        }

        [Fact]
        public void PatientModel_AgeString_ShouldReturnFormattedAge()
        {
            // Arrange
            var patient = new PatientModel();
            patient.BirthDate = DateTime.Now.AddYears(-25);

            // Act
            var ageString = patient.AgeString;

            // Assert
            ageString.Should().Be("25岁");
        }

        [Fact]
        public void PatientModel_PropertyChanged_ShouldRaiseWhenIdChanged()
        {
            // Arrange
            var patient = new PatientModel();
            var eventRaised = false;
            string? changedPropertyName = null;

            patient.PropertyChanged += (sender, args) =>
            {
                eventRaised = true;
                changedPropertyName = args.PropertyName;
            };

            // Act
            patient.Id = "P001";

            // Assert
            eventRaised.Should().BeTrue();
            changedPropertyName.Should().Be(nameof(PatientModel.Id));
        }

        [Fact]
        public void PatientModel_PropertyChanged_ShouldRaiseWhenNameChanged()
        {
            // Arrange
            var patient = new PatientModel();
            var eventRaised = false;
            string? changedPropertyName = null;

            patient.PropertyChanged += (sender, args) =>
            {
                eventRaised = true;
                changedPropertyName = args.PropertyName;
            };

            // Act
            patient.Name = "测试患者";

            // Assert
            eventRaised.Should().BeTrue();
            changedPropertyName.Should().Be(nameof(PatientModel.Name));
        }

        [Fact]
        public void PatientModel_BirthDate_ShouldTriggerAgePropertyChanged()
        {
            // Arrange
            var patient = new PatientModel();
            var ageChangedEventRaised = false;

            patient.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(PatientModel.Age) || 
                    args.PropertyName == nameof(PatientModel.AgeString))
                {
                    ageChangedEventRaised = true;
                }
            };

            // Act
            patient.BirthDate = DateTime.Now.AddYears(-30);

            // Assert
            ageChangedEventRaised.Should().BeTrue();
        }

        [Fact]
        public void PatientModel_Images_ShouldBeModifiable()
        {
            // Arrange
            var patient = new PatientModel();
            var image = new MedicalImageModel
            {
                Id = "IMG001",
                FileName = "test.jpg",
                PatientId = patient.Id
            };

            // Act
            patient.Images.Add(image);

            // Assert
            patient.Images.Should().HaveCount(1);
            patient.Images.First().Should().Be(image);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void PatientModel_EmptyOrNullValues_ShouldBeHandledGracefully(string? value)
        {
            // Arrange
            var patient = new PatientModel();

            // Act & Assert - Should not throw exceptions
            patient.Name = value ?? string.Empty;
            patient.Gender = value ?? string.Empty;
            patient.MedicalRecordNumber = value ?? string.Empty;

            patient.Name.Should().Be(value ?? string.Empty);
            patient.Gender.Should().Be(value ?? string.Empty);
            patient.MedicalRecordNumber.Should().Be(value ?? string.Empty);
        }
    }
}