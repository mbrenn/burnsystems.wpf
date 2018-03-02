using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BurnSystems.WPF
{
    /// <summary>
    /// Interaktionslogik für VerticalResizeableSplitter.xaml
    /// </summary>
    public partial class VerticalResizeableSplitter : UserControl
    {
        private bool _isMouseDown;
        private double _ratio = 0.5;
        private Point _lastPosition;
        private double _movingRatio;

        public VerticalResizeableSplitter()
        {
            InitializeComponent();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RecalculateColumnWidths();
        }

        private void RecalculateColumnWidths()
        {
            var totalWidth = ActualWidth;;
            if (totalWidth <= 2)
            {
                return;
            }

            var borderWidth = BorderColumn.Width.Value;
            totalWidth -= borderWidth;

            var leftWidth = totalWidth * _ratio;
            var rightWidth = totalWidth - leftWidth;

            LeftColumn.Width = new GridLength(leftWidth, GridUnitType.Pixel);
            RightColumn.Width = new GridLength(rightWidth, GridUnitType.Pixel);
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            LeftContent.Content = new TextBox();
            RightContent.Content = new TextBox();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BorderContent.CaptureMouse();

            _isMouseDown = true;
            _lastPosition = e.MouseDevice.GetPosition(this);
            _movingRatio = _ratio;
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                var currentPosition = e.MouseDevice.GetPosition(this);

                var diffX = currentPosition.X - _lastPosition.X;
                _movingRatio += diffX / ActualWidth;
                _ratio = Math.Max(0, Math.Min(1.0, _movingRatio));

                RecalculateColumnWidths();
                _lastPosition = currentPosition;
            }
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isMouseDown)
            {
                _isMouseDown = false;
                BorderContent.ReleaseMouseCapture();
            }
        }
    }
}
