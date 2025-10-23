using FluentAssertions;
using MedicalImagingSystem.Services;
using MedicalImagingSystem.Models;
using MedicalImagingSystem.IServices;

namespace MedicalImagingSystem.Services.Tests
{
    public class PatientServiceTests
    {
        private readonly IPatientService _patientService;

        public PatientServiceTests()
        {
            _patientService = new PatientService();
        }

        [Fact]
        public async Task GetAllPatientsAsync_ShouldReturnNonEmptyList()
        {
            // Act
            var patients = await _patientService.GetAllPatientsAsync();

            // Assert
            patients.Should().NotBeNull();
            patients.Should().NotBeEmpty();
            patients.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task GetAllPatientsAsync_ShouldReturnPatientsWithValidData()
        {
            // Act
            var patients = await _patientService.GetAllPatientsAsync();

            // Assert
            patients.Should().NotBeNull();
            patients.Should().AllSatisfy(patient =>
            {
                patient.Id.Should().NotBeNullOrEmpty();
                patient.Name.Should().NotBeNullOrEmpty();
                patient.MedicalRecordNumber.Should().NotBeNullOrEmpty();
                patient.Gender.Should().NotBeNullOrEmpty();
                patient.Images.Should().NotBeNull();
            });
        }

        [Theory]
        [InlineData("P001")]
        [InlineData("P002")]
        [InlineData("P003")]
        public async Task GetPatientByIdAsync_WithValidId_ShouldReturnCorrectPatient(string patientId)
        {
            // Act
            var patient = await _patientService.GetPatientByIdAsync(patientId);

            // Assert
            patient.Should().NotBeNull();
            patient!.Id.Should().Be(patientId);
        }

        [Theory]
        [InlineData("")]
        [InlineData("INVALID_ID")]
        [InlineData("P999")]
        public async Task GetPatientByIdAsync_WithInvalidId_ShouldReturnNull(string invalidId)
        {
            // Act
            var patient = await _patientService.GetPatientByIdAsync(invalidId);

            // Assert
            patient.Should().BeNull();
        }

        [Theory]
        [InlineData("张")]
        [InlineData("李")]
        [InlineData("王")]
        public async Task SearchPatientsAsync_WithPartialName_ShouldReturnMatchingPatients(string searchTerm)
        {
            // Act
            var patients = await _patientService.SearchPatientsAsync(searchTerm);

            // Assert
            patients.Should().NotBeNull();
            patients.Should().AllSatisfy(patient =>
            {
                patient.Name.Should().Contain(searchTerm, StringComparison.OrdinalIgnoreCase);
            });
        }

        [Theory]
        [InlineData("MR001")]
        [InlineData("MR002")]
        [InlineData("MR003")]
        public async Task SearchPatientsAsync_WithMedicalRecordNumber_ShouldReturnMatchingPatient(string mrNumber)
        {
            // Act
            var patients = await _patientService.SearchPatientsAsync(mrNumber);

            // Assert
            patients.Should().NotBeNull();
            patients.Should().Contain(patient => patient.MedicalRecordNumber == mrNumber);
        }

        [Fact]
        public async Task SearchPatientsAsync_WithEmptySearchTerm_ShouldReturnAllPatients()
        {
            // Arrange
            var allPatients = await _patientService.GetAllPatientsAsync();

            // Act
            var searchResults = await _patientService.SearchPatientsAsync("");

            // Assert
            searchResults.Should().NotBeNull();
            searchResults.Should().HaveCount(allPatients.Count);
        }

        [Fact]
        public async Task SearchPatientsAsync_WithWhitespaceSearchTerm_ShouldReturnAllPatients()
        {
            // Arrange
            var allPatients = await _patientService.GetAllPatientsAsync();

            // Act
            var searchResults = await _patientService.SearchPatientsAsync("   ");

            // Assert
            searchResults.Should().NotBeNull();
            searchResults.Should().HaveCount(allPatients.Count);
        }

        [Theory]
        [InlineData("NONEXISTENT_NAME")]
        [InlineData("XYZ123")]
        public async Task SearchPatientsAsync_WithNonexistentSearchTerm_ShouldReturnEmptyList(string searchTerm)
        {
            // Act
            var patients = await _patientService.SearchPatientsAsync(searchTerm);

            // Assert
            patients.Should().NotBeNull();
            patients.Should().BeEmpty();
        }

        [Fact]
        public async Task AddPatientAsync_WithValidPatient_ShouldSucceed()
        {
            // Arrange
            var newPatient = new PatientModel
            {
                Id = "P999",
                Name = "测试患者",
                Gender = "男",
                BirthDate = new DateTime(1985, 3, 15),
                MedicalRecordNumber = "MR999"
            };

            // Act
            var result = await _patientService.AddPatientAsync(newPatient);

            // Assert
            result.Should().BeTrue();

            var addedPatient = await _patientService.GetPatientByIdAsync("P999");
            addedPatient.Should().NotBeNull();
            addedPatient!.Name.Should().Be("测试患者");
        }

        [Fact]
        public async Task AddPatientAsync_WithDuplicateId_ShouldFail()
        {
            // Arrange
            var existingPatients = await _patientService.GetAllPatientsAsync();
            var existingId = existingPatients.First().Id;
            
            var duplicatePatient = new PatientModel
            {
                Id = existingId,
                Name = "重复患者",
                Gender = "女",
                BirthDate = new DateTime(1990, 1, 1),
                MedicalRecordNumber = "MR_DUPLICATE"
            };

            // Act
            var result = await _patientService.AddPatientAsync(duplicatePatient);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task AddPatientAsync_WithNullPatient_ShouldFail()
        {
            // Act
            var result = await _patientService.AddPatientAsync(null!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdatePatientAsync_WithValidPatient_ShouldSucceed()
        {
            // Arrange
            var existingPatients = await _patientService.GetAllPatientsAsync();
            var patientToUpdate = existingPatients.First();
            var originalName = patientToUpdate.Name;
            patientToUpdate.Name = "更新后的姓名";

            // Act
            var result = await _patientService.UpdatePatientAsync(patientToUpdate);

            // Assert
            result.Should().BeTrue();

            var updatedPatient = await _patientService.GetPatientByIdAsync(patientToUpdate.Id);
            updatedPatient.Should().NotBeNull();
            updatedPatient!.Name.Should().Be("更新后的姓名");
            updatedPatient.Name.Should().NotBe(originalName);
        }

        [Fact]
        public async Task UpdatePatientAsync_WithNonexistentPatient_ShouldFail()
        {
            // Arrange
            var nonexistentPatient = new PatientModel
            {
                Id = "NONEXISTENT",
                Name = "不存在的患者"
            };

            // Act
            var result = await _patientService.UpdatePatientAsync(nonexistentPatient);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeletePatientAsync_WithValidId_ShouldSucceed()
        {
            // Arrange
            var existingPatients = await _patientService.GetAllPatientsAsync();
            var patientToDelete = existingPatients.First();

            // Act
            var result = await _patientService.DeletePatientAsync(patientToDelete.Id);

            // Assert
            result.Should().BeTrue();

            var deletedPatient = await _patientService.GetPatientByIdAsync(patientToDelete.Id);
            deletedPatient.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData("NONEXISTENT_ID")]
        public async Task DeletePatientAsync_WithInvalidId_ShouldFail(string invalidId)
        {
            // Act
            var result = await _patientService.DeletePatientAsync(invalidId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetAllPatientsAsync_PatientsWithImages_ShouldHaveImageData()
        {
            // Act
            var patients = await _patientService.GetAllPatientsAsync();

            // Assert
            patients.Should().NotBeNull();
            patients.Should().AllSatisfy(patient =>
            {
                patient.Images.Should().NotBeNull();
                if (patient.Images.Any())
                {
                    patient.Images.Should().AllSatisfy(image =>
                    {
                        image.PatientId.Should().Be(patient.Id);
                        image.FileName.Should().NotBeNullOrEmpty();
                        image.FilePath.Should().NotBeNullOrEmpty();
                    });
                }
            });
        }
    }
}