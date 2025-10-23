using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicalImagingSystem.Models;

namespace MedicalImagingSystem.Services
{
    public class PatientService : IPatientService
    {
        //private readonly ILogger<PatientService> _logger;
        private readonly List<Patient> _patients;

        public PatientService()
        {
            //_logger = logger;
            _patients = new List<Patient>();
            InitializeSampleData();
        }

        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            await Task.Delay(100); // Simulate async operation
            return _patients.ToList();
        }

        public async Task<Patient?> GetPatientByIdAsync(string patientId)
        {
            await Task.Delay(50);
            return _patients.FirstOrDefault(p => p.Id == patientId);
        }

        public async Task<List<Patient>> SearchPatientsAsync(string searchTerm)
        {
            await Task.Delay(100);
            
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _patients.ToList();

            return _patients.Where(p => 
                p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.MedicalRecordNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<bool> AddPatientAsync(Patient patient)
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

        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            try
            {
                await Task.Delay(50);
                var existingPatient = _patients.FirstOrDefault(p => p.Id == patient.Id);
                if (existingPatient != null)
                {
                    var index = _patients.IndexOf(existingPatient);
                    _patients[index] = patient;
                    //_logger.LogInformation($"Patient updated: {patient.Name}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"Error updating patient: {patient.Name}");
                return false;
            }
        }

        public async Task<bool> DeletePatientAsync(string patientId)
        {
            try
            {
                await Task.Delay(50);
                var patient = _patients.FirstOrDefault(p => p.Id == patientId);
                if (patient != null)
                {
                    _patients.Remove(patient);
                    //_logger.LogInformation($"Patient deleted: {patient.Name}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"Error deleting patient with ID: {patientId}");
                return false;
            }
        }

        private void InitializeSampleData()
        {
            _patients.AddRange(new[]
            {
                new Patient
                {
                    Id = "1",
                    Name = "张三",
                    BirthDate = new DateTime(1980, 5, 15),
                    Gender = "男",
                    MedicalRecordNumber = "MR001",
                    Images = new List<MedicalImage>
                    {
                        new MedicalImage
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
                        new MedicalImage
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
                new Patient
                {
                    Id = "2",
                    Name = "李四",
                    BirthDate = new DateTime(1975, 8, 22),
                    Gender = "女",
                    MedicalRecordNumber = "MR002",
                    Images = new List<MedicalImage>
                    {
                        new MedicalImage
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
                        new MedicalImage
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
                new Patient
                {
                    Id = "3",
                    Name = "王五",
                    BirthDate = new DateTime(1990, 12, 3),
                    Gender = "男",
                    MedicalRecordNumber = "MR003",
                    Images = new List<MedicalImage>
                    {
                        new MedicalImage
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
                        new MedicalImage
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