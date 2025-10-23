using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MedicalImagingSystem.Models
{
    public partial class Patient : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        private string _name = string.Empty;
        private DateTime _birthDate;
        private string _gender = string.Empty;
        private string _medicalRecordNumber = string.Empty;
        private List<MedicalImage> _images = new();

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

        public List<MedicalImage> Images
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