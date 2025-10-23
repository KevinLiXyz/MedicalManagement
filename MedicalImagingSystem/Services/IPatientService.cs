using System.Collections.Generic;
using System.Threading.Tasks;
using MedicalImagingSystem.Models;

namespace MedicalImagingSystem.Services
{
    public interface IPatientService
    {
        Task<List<Patient>> GetAllPatientsAsync();
        Task<Patient?> GetPatientByIdAsync(string patientId);
        Task<List<Patient>> SearchPatientsAsync(string searchTerm);
        Task<bool> AddPatientAsync(Patient patient);
        Task<bool> UpdatePatientAsync(Patient patient);
        Task<bool> DeletePatientAsync(string patientId);
    }
}