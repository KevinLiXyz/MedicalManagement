using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MedicalImagingSystem.Models
{
	/// <summary>
	/// Represents a patient in the healthcare system, including their personal and medical information.
	/// </summary>
	/// <remarks>This model is typically used to store and transfer patient data between different components of the
	/// application. It may include properties such as the patient's name, date of birth, medical history, and other
	/// relevant details.</remarks>
	public class PatientModel : INotifyPropertyChanged
	{
		private string _id = string.Empty;
		private string _name = string.Empty;
		private DateTime _birthDate;
		private string _gender = string.Empty;
		private string _medicalRecordNumber = string.Empty;
		private List<MedicalImageModel> _images = new();

		public string Id
		{
			get => _id;
			set => SetProperty(ref _id, value);
		}

		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value);
		}

		public DateTime BirthDate
		{
			get => _birthDate;
			set => SetProperty(ref _birthDate, value);
		}

		public string Gender
		{
			get => _gender;
			set => SetProperty(ref _gender, value);
		}

		public string MedicalRecordNumber
		{
			get => _medicalRecordNumber;
			set => SetProperty(ref _medicalRecordNumber, value);
		}

		public List<MedicalImageModel> Images
		{
			get => _images;
			set => SetProperty(ref _images, value);
		}

		public int Age => DateTime.Now.Year - BirthDate.Year;


		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}
