using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MedicalImagingSystem.ViewModels;

namespace MedicalImagingSystem.Views
{
    public partial class ImageViewerView : UserControl
    {
        private ImageViewerViewModel? ViewModel => DataContext as ImageViewerViewModel;

        public ImageViewerView()
        {
            InitializeComponent();
        }

        private void MainImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null) return;

            var position = e.GetPosition(MainImage);
            ViewModel.StartPanning(position);
            MainImage.CaptureMouse();
        }

        private void MainImage_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null) return;

            var position = e.GetPosition(MainImage);
            
            // Right click to zoom in at mouse position
            ViewModel.ZoomInAtPoint(position);
            e.Handled = true;
        }

        private void MainImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel == null) return;

            var position = e.GetPosition(MainImage);
            ViewModel.UpdatePanning(position);
        }

        private void MainImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null) return;

            ViewModel.StopPanning();
            MainImage.ReleaseMouseCapture();

            // If this was just a click (not a drag), zoom in at clicked position
            var position = e.GetPosition(MainImage);
            if (e.ClickCount == 1)
            {
                ViewModel.ZoomInAtPoint(position);
            }
        }

        private void MainImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (ViewModel == null) return;

            var position = e.GetPosition(MainImage);
            
            if (e.Delta > 0)
            {
                ViewModel.ZoomInAtPoint(position);
            }
            else
            {
                ViewModel.ZoomOutAtPoint(position);
            }
            
            e.Handled = true;
        }
    }
}