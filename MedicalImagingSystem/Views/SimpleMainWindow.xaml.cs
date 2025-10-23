using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MedicalImagingSystem.Models;

namespace MedicalImagingSystem.Views
{
    public partial class SimpleMainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<PatientModel> _patients = new();
        private PatientModel? _selectedPatient;
        private MedicalImageModel? _selectedImage;
        private BitmapImage? _currentImageSource;
        private double _zoomLevel = 1.0;
        private string _searchText = string.Empty;
        private bool _isLoading;
        private string _currentTheme = "Light";

        public ObservableCollection<PatientModel> Patients
        {
            get => _patients;
            set => SetProperty(ref _patients, value);
        }

        public PatientModel? SelectedPatient
        {
            get => _selectedPatient;
            set
            {
                SetProperty(ref _selectedPatient, value);
                if (value != null && value.Images.Any())
                {
                    SelectedImage = value.Images.First();
                    LoadImage();
                }
            }
        }

        public MedicalImageModel? SelectedImage
        {
            get => _selectedImage;
            set
            {
                SetProperty(ref _selectedImage, value);
                LoadImage();
            }
        }

        public BitmapImage? CurrentImageSource
        {
            get => _currentImageSource;
            set => SetProperty(ref _currentImageSource, value);
        }

        public double ZoomLevel
        {
            get => _zoomLevel;
            set => SetProperty(ref _zoomLevel, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterPatients();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string CurrentTheme
        {
            get => _currentTheme;
            set => SetProperty(ref _currentTheme, value);
        }

        public SimpleMainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            var patients = new[]
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
                            Id = "img1",
                            PatientId = "1",
                            FileName = "sample_image.jpg",
                            FilePath = "pack://application:,,,/Assets/sample_image.jpg",
                            CaptureDate = DateTime.Now.AddDays(-10),
                            Modality = "X-Ray",
                            BodyPart = "胸部",
                            Description = "胸部X光检查",
                            Width = 512,
                            Height = 512,
                            FileSize = 1024000
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
                        new MedicalImageModel
                        {
                            Id = "img2",
                            PatientId = "2",
                            FileName = "sample_image2.jpg",
                            FilePath = "pack://application:,,,/Assets/sample_image2.jpg",
                            CaptureDate = DateTime.Now.AddDays(-5),
                            Modality = "MRI",
                            BodyPart = "头部",
                            Description = "脑部MRI扫描",
                            Width = 256,
                            Height = 256,
                            FileSize = 512000
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
                    Images = new List<MedicalImageModel>()
                }
            };

            foreach (var patient in patients)
            {
                Patients.Add(patient);
            }
        }

        private void FilterPatients()
        {
            // Simple filter implementation
            // In a real app, you would filter from the original data source
        }

        private void LoadImage()
        {
            if (SelectedImage == null)
            {
                CurrentImageSource = null;
                return;
            }

            try
            {
                // Create a simple colored rectangle as placeholder
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                // For demo purposes, we'll create a simple image
                bitmap.UriSource = new Uri("pack://application:,,,/Assets/placeholder.png", UriKind.Absolute);
                bitmap.EndInit();
                CurrentImageSource = bitmap;
            }
            catch
            {
                CurrentImageSource = null;
            }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomLevel = Math.Min(ZoomLevel * 1.25, 5.0);
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomLevel = Math.Max(ZoomLevel / 1.25, 0.1);
        }

        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            ZoomLevel = 1.0;
        }

        private void SwitchTheme_Click(object sender, RoutedEventArgs e)
        {
            CurrentTheme = CurrentTheme == "Light" ? "Dark" : "Light";
            
            var app = Application.Current;
            var resources = app.Resources;

            // Remove current theme
            var currentThemeDict = resources.MergedDictionaries
                .FirstOrDefault(d => d.Source?.OriginalString.Contains("Theme.xaml") == true);
            if (currentThemeDict != null)
            {
                resources.MergedDictionaries.Remove(currentThemeDict);
            }

            // Add new theme
            var newThemeDict = new ResourceDictionary();
            newThemeDict.Source = new Uri($"Themes/{CurrentTheme}Theme.xaml", UriKind.Relative);
            resources.MergedDictionaries.Add(newThemeDict);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}