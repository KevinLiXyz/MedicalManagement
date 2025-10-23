using MedicalImagingSystem.Models;

namespace MedicalImagingSystem.IServices
{
	/// <summary>
	/// Defines the contract for operations related to managing patient data.
	/// </summary>
	/// <remarks>This interface provides methods for creating, retrieving, updating, and deleting patient records.
	/// Implementations of this interface should ensure thread safety and proper validation of input data.</remarks>
	public interface IPatientService
	{
		/// <summary>
		/// Retrieves a list of all patients.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation. The task result contains a list of patients.</returns>
		Task<List<PatientModel>> GetAllPatientsAsync();
		/// <summary>
		/// Retrieves a patient by their unique identifier.
		/// </summary>
		/// <param name="patientId">The unique identifier of the patient.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains the patient if found; otherwise, null.</returns>
		Task<PatientModel?> GetPatientByIdAsync(string patientId);
		/// <summary>
		/// Searches for patients matching the specified search term.
		/// </summary>
		/// <param name="searchTerm">The term to search for in patient names or medical record numbers.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a list of matching patients.</returns>
		Task<List<PatientModel>> SearchPatientsAsync(string searchTerm);
		/// <summary>
		/// Adds a new patient to the system.
		/// </summary>
		/// <param name="patient">The patient to add.</param>
		/// <returns>A task that represents the asynchronous operation. The task result indicates whether the addition was successful.</returns>
		Task<bool> AddPatientAsync(PatientModel patient);
	}
}
