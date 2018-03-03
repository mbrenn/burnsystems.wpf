using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BurnSystems.WPF
{
    public enum SplitterMode
    {
        Horizontal, 
        Vertical
    }

    /// <summary>
    /// Interaktionslogik für VerticalResizeableSplitter.xaml
    /// </summary>
    public partial class VerticalResizeableSplitter : UserControl
    {
        private bool _isMouseDown;
        private double _ratio = 0.5;
        private Point _lastPosition;
        private double _movingRatio;

        public SplitterMode SplitterMode { get; set; } = SplitterMode.Vertical;

        public VerticalResizeableSplitter()
        {
            InitializeComponent();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RecalculateColumnWidths();
        }

        /// <summary>
        /// Sets the layout dependent on the current splitter mode
        /// </summary>
        private void SetLayout()
        {
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();

            if (SplitterMode == SplitterMode.Vertical)
            {
                MainGrid.ColumnDefinitions.Add(
                    new ColumnDefinition());
                MainGrid.ColumnDefinitions.Add(
                    new ColumnDefinition() {Width = new GridLength(2)});
                MainGrid.ColumnDefinitions.Add(
                    new ColumnDefinition());
            }
            else if (SplitterMode == SplitterMode.Horizontal)
            {
                MainGrid.RowDefinitions.Add(
                    new RowDefinition());
                MainGrid.RowDefinitions.Add(
                    new RowDefinition() { Height = new GridLength(2) });
                MainGrid.RowDefinitions.Add(
                    new RowDefinition());

            }
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
