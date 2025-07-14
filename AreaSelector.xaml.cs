using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;

namespace ScreenRecorder
{
    public partial class AreaSelector : Window
    {
        private bool _isSelecting = false;
        private System.Windows.Point _startPoint;
        private System.Windows.Point _endPoint;

        public Rectangle SelectedArea { get; private set; }

        public AreaSelector()
        {
            InitializeComponent();
            KeyDown += AreaSelector_KeyDown;
            MouseLeftButtonDown += AreaSelector_MouseLeftButtonDown;
            MouseMove += AreaSelector_MouseMove;
            MouseLeftButtonUp += AreaSelector_MouseLeftButtonUp;
        }

        private void AreaSelector_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }

        private void AreaSelector_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isSelecting = true;
            _startPoint = e.GetPosition(this);
            _endPoint = _startPoint;
            
            SelectionRect.Visibility = Visibility.Visible;
            UpdateSelectionRect();
            
            CaptureMouse();
        }

        private void AreaSelector_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isSelecting)
            {
                _endPoint = e.GetPosition(this);
                UpdateSelectionRect();
            }
        }

        private void AreaSelector_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isSelecting)
            {
                _isSelecting = false;
                ReleaseMouseCapture();
                
                // Calculate the selected area
                double left = Math.Min(_startPoint.X, _endPoint.X);
                double top = Math.Min(_startPoint.Y, _endPoint.Y);
                double width = Math.Abs(_endPoint.X - _startPoint.X);
                double height = Math.Abs(_endPoint.Y - _startPoint.Y);
                
                // Convert to screen coordinates
                var screenPoint = PointToScreen(new System.Windows.Point(left, top));
                
                SelectedArea = new Rectangle(
                    (int)screenPoint.X,
                    (int)screenPoint.Y,
                    (int)width,
                    (int)height);
                
                // Only accept selection if it has reasonable size
                if (width > 50 && height > 50)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    // Reset selection if too small
                    SelectionRect.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void UpdateSelectionRect()
        {
            double left = Math.Min(_startPoint.X, _endPoint.X);
            double top = Math.Min(_startPoint.Y, _endPoint.Y);
            double width = Math.Abs(_endPoint.X - _startPoint.X);
            double height = Math.Abs(_endPoint.Y - _startPoint.Y);

            Canvas.SetLeft(SelectionRect, left);
            Canvas.SetTop(SelectionRect, top);
            SelectionRect.Width = width;
            SelectionRect.Height = height;
        }
    }
}