using MedicalImagingSystem.IServices;
using MedicalImagingSystem.Models;

namespace MedicalImagingSystem.Services
{
	/// <summary>
	/// Provides operations and services related to managing patient records and data.
	/// </summary>
	/// <remarks>This class serves as the primary entry point for interacting with patient-related functionality. It
	/// may include methods for creating, retrieving, updating, and deleting patient information.</remarks>
	public class PatientService : IPatientService
	{
		//private readonly ILogger<PatientService> _logger;
		private readonly List<PatientModel> _patients;

		public PatientService()
		{
			_patients = new List<PatientModel>();
			InitializeSampleData();
		}

		/// <summary>
		/// Asynchronously retrieves a list of all patients.
		/// </summary>
		/// <remarks>This method returns a collection of patients represented as <see cref="PatientModel"/> objects. 
		/// The returned list will be empty if no patients are found.</remarks>
		/// <returns>A task that represents the asynchronous operation. The task result contains a list of  <see cref="PatientModel"/>
		/// objects representing all patients.</returns>
		/// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
		public async Task<List<PatientModel>> GetAllPatientsAsync()
		{
			await Task.Delay(100); // Simulate async operation
			return _patients.ToList();
		}

		public async Task<PatientModel?> GetPatientByIdAsync(string patientId)
		{
			await Task.Delay(50);
			return _patients.FirstOrDefault(p => p.Id == patientId);
		}

		public async Task<List<PatientModel>> SearchPatientsAsync(string searchTerm)
		{
			await Task.Delay(100);

			if (string.IsNullOrWhiteSpace(searchTerm))
				return _patients.ToList();

			return _patients.Where(p =>
					p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
					p.MedicalRecordNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
				.ToList();
		}

		public async Task<bool> AddPatientAsync(PatientModel patient)
		{
			try
			{
				await Task.Delay(50);
				patient.Id = Guid.NewGuid().ToString();
				_patients.Add(patient);
				//_logger.LogInformation($"Patient added: {patient.Name}");
				return true;
			}
			catch (Exception ex)
			{
				//_logger.LogError(ex, $"Error adding patient: {patient.Name}");
				return false;
			}
		}

		private void InitializeSampleData()
		{
			_patients.AddRange(new[]
			{
				new PatientModel
				{
					Id = "1",
					Name = "张三",
					BirthDate = new DateTime(1980, 5, 15),
					Gender = "男",
					MedicalRecordNumber = "MR001",
					Images = new List<MedicalImageModel>
					{
						new MedicalImageModel
						{
							Id = "img1_1",
							PatientId = "1",
							FileName = "chest_xray_front.jpg",
							FilePath = "Assets/SampleImages/chest_xray_front.jpg",
							CaptureDate = DateTime.Now.AddDays(-10),
							Modality = "X-Ray",
							BodyPart = "胸部",
							Description = "胸部X光检查 - 正位"
						},
						new MedicalImageModel
						{
							Id = "img1_2",
							PatientId = "1",
							FileName = "chest_xray_side.jpg",
							FilePath = "Assets/SampleImages/chest_xray_side.jpg",
							CaptureDate = DateTime.Now.AddDays(-10),
							Modality = "X-Ray",
							BodyPart = "胸部",
							Description = "胸部X光检查 - 侧位"
						}
					}
				},
				new PatientModel
				{
					Id = "2",
					Name = "李四",
					BirthDate = new DateTime(1975, 8, 22),
					Gender = "女",
					MedicalRecordNumber = "MR002",
					Images = new List<MedicalImageModel>
					{
						new()
						{
							Id = "img2_1",
							PatientId = "2",
							FileName = "brain_mri_t1.jpg",
							FilePath = "Assets/SampleImages/brain_mri_t1.jpg",
							CaptureDate = DateTime.Now.AddDays(-5),
							Modality = "MRI",
							BodyPart = "头部",
							Description = "脑部MRI扫描 - T1加权"
						},
						new()
						{
							Id = "img2_2",
							PatientId = "2",
							FileName = "brain_mri_t2.jpg",
							FilePath = "Assets/SampleImages/brain_mri_t2.jpg",
							CaptureDate = DateTime.Now.AddDays(-5),
							Modality = "MRI",
							BodyPart = "头部",
							Description = "脑部MRI扫描 - T2加权"
						}
					}
				},
				new PatientModel
				{
					Id = "3",
					Name = "王五",
					BirthDate = new DateTime(1990, 12, 3),
					Gender = "男",
					MedicalRecordNumber = "MR003",
					Images = new List<MedicalImageModel>
					{
						new MedicalImageModel
						{
							Id = "img3_1",
							PatientId = "3",
							FileName = "abdomen_ct_plain.jpg",
							FilePath = "Assets/SampleImages/abdomen_ct_plain.jpg",
							CaptureDate = DateTime.Now.AddDays(-3),
							Modality = "CT",
							BodyPart = "腹部",
							Description = "腹部CT扫描 - 平扫"
						},
						new MedicalImageModel
						{
							Id = "img3_2",
							PatientId = "3",
							FileName = "abdomen_ct_contrast.jpg",
							FilePath = "Assets/SampleImages/abdomen_ct_contrast.jpg",
							CaptureDate = DateTime.Now.AddDays(-3),
							Modality = "CT",
							BodyPart = "腹部",
							Description = "腹部CT扫描 - 增强"
						}
					}
				}
			});
		}
	}
}