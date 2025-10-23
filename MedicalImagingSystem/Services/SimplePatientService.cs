using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MedicalImagingSystem.Models;

namespace MedicalImagingSystem.Services
{
    public class SimplePatientService : IPatientService
    {
        private readonly List<Patient> _patients;

        public SimplePatientService()
        {
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
                return true;
            }
            catch
            {
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
                    return true;
                }
                return false;
            }
            catch
            {
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
                    return true;
                }
                return false;
            }
            catch
            {
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
                            Id = "img1",
                            PatientId = "1",
                            FileName = "chest_xray.jpg",
                            FilePath = Path.Combine(AppContext.BaseDirectory, "Assets/SampleImages/chest_xray.jpg"),
                            CaptureDate = DateTime.Now.AddDays(-10),
                            Modality = "X-Ray",
                            BodyPart = "胸部",
                            Description = "胸部X光检查"
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
                            Id = "img2",
                            PatientId = "2",
                            FileName = "brain_mri.jpg",
                            FilePath = "Assets/SampleImages/brain_mri.jpg",
                            CaptureDate = DateTime.Now.AddDays(-5),
                            Modality = "MRI",
                            BodyPart = "头部",
                            Description = "脑部MRI扫描"
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
                            Id = "img3",
                            PatientId = "3",
                            FileName = "abdomen_ct.jpg",
                            FilePath = "Assets/SampleImages/abdomen_ct.jpg",
                            CaptureDate = DateTime.Now.AddDays(-3),
                            Modality = "CT",
                            BodyPart = "腹部",
                            Description = "腹部CT扫描"
                        }
                    }
                }
            });
        }
    }
}