using CheckersApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace CheckersApp.View
{
    public partial class GameControl : UserControl
    {
        private const double ZoomIncrement = 0.1;
        private const double MinZoom = 0.5;
        private const double MaxZoom = 3.0;

        public GameControl()
        {
            InitializeComponent();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Calculate the new scale factor
            double scaleFactor = e.Delta > 0 ? (1.0 + ZoomIncrement) : (1.0 - ZoomIncrement);
            double newScaleX = zoomTransform.ScaleX * scaleFactor;
            double newScaleY = zoomTransform.ScaleY * scaleFactor;

            // Check the boundaries for zoom levels
            if (newScaleX >= MinZoom && newScaleX <= MaxZoom)
            {
                zoomTransform.ScaleX = newScaleX;
                zoomTransform.ScaleY = newScaleY;

                // Optionally adjust the ScrollViewer's offsets to keep the content centered
                AdjustScrollViewer(e.GetPosition(contentGrid), scaleFactor);
            }
        }

        private void AdjustScrollViewer(Point zoomCenter, double scaleFactor)
        {
            ScrollViewer viewer = FindParent<ScrollViewer>(contentGrid);
            if (viewer != null)
            {
                double centerOfViewportX = viewer.HorizontalOffset + viewer.ViewportWidth / 2;
                double centerOfViewportY = viewer.VerticalOffset + viewer.ViewportHeight / 2;
                double newOffsetX = centerOfViewportX * scaleFactor - viewer.ViewportWidth / 2;
                double newOffsetY = centerOfViewportY * scaleFactor - viewer.ViewportHeight / 2;

                viewer.ScrollToHorizontalOffset(newOffsetX);
                viewer.ScrollToVerticalOffset(newOffsetY);
            }
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return null;
            T parentT = parent as T;
            return parentT ?? FindParent<T>(parent);
        }

    }
}
