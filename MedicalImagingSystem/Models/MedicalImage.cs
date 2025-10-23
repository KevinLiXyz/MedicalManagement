using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace MedicalImagingSystem.Models
{
    public class MedicalImage : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        private string _patientId = string.Empty;
        private string _fileName = string.Empty;
        private string _filePath = string.Empty;
        private DateTime _captureDate;
        private string _modality = string.Empty;
        private string _bodyPart = string.Empty;
        private string _description = string.Empty;
        private long _fileSize;
        private int _width;
        private int _height;
        private bool _isLoading;

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string PatientId
        {
            get => _patientId;
            set => SetProperty(ref _patientId, value);
        }

        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        public DateTime CaptureDate
        {
            get => _captureDate;
            set => SetProperty(ref _captureDate, value);
        }

        public string Modality
        {
            get => _modality;
            set => SetProperty(ref _modality, value);
        }

        public string BodyPart
        {
            get => _bodyPart;
            set => SetProperty(ref _bodyPart, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public long FileSize
        {
            get => _fileSize;
            set => SetProperty(ref _fileSize, value);
        }

        public int Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        public int Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string FileSizeFormatted => FormatBytes(FileSize);

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

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