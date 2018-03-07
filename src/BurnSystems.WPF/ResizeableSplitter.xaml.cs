using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BurnSystems.WPF
{
    /// <summary>
    /// Defines the orientation of the splitter. 
    /// </summary>
    public enum SplitterMode
    {
        /// <summary>
        /// The splitter is horizontal, so we have a top and a bottom element
        /// </summary>
        Horizontal, 

        /// <summary>
        /// The splitter is vertical, so we have a left and a right element
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Interaktionslogik für ResizeableSplitter.xaml
    /// </summary>
    public partial class ResizeableSplitter : UserControl
    {
        /// <summary>
        /// Defines the width/height of the border element, depending on the used orientation
        /// </summary>
        private const double BorderSize = 4.0;

        private bool _isMouseDown;
        private double _ratio = 0.5;
        private Point _lastPosition;
        private double _movingRatio;

        /// <summary>
        /// Stores the definitions of the columns
        /// </summary>
        private readonly ColumnDefinition[] _columnDefinitions = new ColumnDefinition[3];

        /// <summary>
        /// Stores the definition of the rows
        /// </summary>
        private readonly RowDefinition[] _rowDefinitions = new RowDefinition[3];

        public static readonly DependencyProperty SplitterModeProperty = DependencyProperty.Register(
            "SplitterMode", 
            typeof(SplitterMode),
            typeof(ResizeableSplitter),
            new FrameworkPropertyMetadata(
                SplitterMode.Vertical, 
                FrameworkPropertyMetadataOptions.AffectsRender,
                (o, args) => ((ResizeableSplitter)o).ResetLayout()));

        public static readonly DependencyProperty FirstElementProperty = DependencyProperty.Register(
            "FirstElement", typeof(UIElement), typeof(ResizeableSplitter),
            new PropertyMetadata(
                default(UIElement),
                (o, args) => ((ResizeableSplitter) o).LeftContent.Content = args.NewValue));

        public static readonly DependencyProperty SecondElementProperty = DependencyProperty.Register(
            "SecondElement", typeof(UIElement), typeof(ResizeableSplitter), new PropertyMetadata(
                default(UIElement),
                (o, args) => ((ResizeableSplitter) o).RightContent.Content = args.NewValue));

        public static readonly DependencyProperty SplitterColorProperty = DependencyProperty.Register(
            "SplitterColor", typeof(Brush), typeof(ResizeableSplitter), new PropertyMetadata(
                Brushes.Black,
                (o, args) => ((ResizeableSplitter) o).BorderContent.Background = (Brush) args.NewValue));

        /// <summary>
        /// Gets or sets the splitter mode
        /// </summary>
        public SplitterMode SplitterMode
        {
            get => (SplitterMode) GetValue(SplitterModeProperty);
            set => SetValue(SplitterModeProperty, value);
        }

        /// <summary>
        /// Gets or sets the content of the first element, which is the left or the top element
        /// </summary>
        public UIElement FirstElement
        {
            get => (UIElement) GetValue(FirstElementProperty);
            set => SetValue(FirstElementProperty, value);
        }

        /// <summary>
        /// Gets or sets the content of the second element, which is the left or the top element
        /// </summary>
        public UIElement SecondElement
        {
            get => (UIElement) GetValue(SecondElementProperty);
            set => SetValue(SecondElementProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of the splitter element itself
        /// </summary>
        public Brush SplitterColor
        {
            get => (Brush)GetValue(SplitterColorProperty);
            set => SetValue(SplitterColorProperty, value);
        }

        /// <summary>
        /// Initializes a new element of the ResizeableSplitter
        /// </summary>
        public ResizeableSplitter()
        {
            InitializeComponent();
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            LeftContent.Content = new TextBox
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                TextWrapping = TextWrapping.Wrap
            };

            RightContent.Content = new TextBox
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                TextWrapping = TextWrapping.Wrap
            };

            ResetLayout();

            LostMouseCapture += OnLostMouseCapture;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            LostMouseCapture -= OnLostMouseCapture;
        }

        /// <summary>
        /// Called, when the size of the element was changed
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Arguments of the event</param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RecalculateContentSizes();
        }

        /// <summary>
        /// Sets the layout dependent on the current splitter mode
        /// </summary>
        internal void ResetLayout()
        {
            if (!IsInitialized)
            {
                return;
            }

            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();

            if (SplitterMode == SplitterMode.Vertical)
            {
                _columnDefinitions[0] = new ColumnDefinition();
                _columnDefinitions[1] = new ColumnDefinition { Width = new GridLength(BorderSize) };
                _columnDefinitions[2] = new ColumnDefinition();

                MainGrid.ColumnDefinitions.Add(_columnDefinitions[0]);
                MainGrid.ColumnDefinitions.Add(_columnDefinitions[1]);
                MainGrid.ColumnDefinitions.Add(_columnDefinitions[2]);

                Grid.SetColumn(LeftContent, 0);
                Grid.SetColumn(BorderContent, 1);
                Grid.SetColumn(RightContent, 2);
                Grid.SetRow(LeftContent, 0);
                Grid.SetRow(BorderContent, 0);
                Grid.SetRow(RightContent, 0);

                BorderContent.Cursor = Cursors.SizeWE;
            }
            else if (SplitterMode == SplitterMode.Horizontal)
            {
                _rowDefinitions[0] = new RowDefinition();
                _rowDefinitions[1] = new RowDefinition { Height = new GridLength(BorderSize) };
                _rowDefinitions[2] = new RowDefinition();

                MainGrid.RowDefinitions.Add(_rowDefinitions[0]);
                MainGrid.RowDefinitions.Add(_rowDefinitions[1]);
                MainGrid.RowDefinitions.Add(_rowDefinitions[2]);

                Grid.SetColumn(LeftContent, 0);
                Grid.SetColumn(BorderContent, 0);
                Grid.SetColumn(RightContent, 0);
                Grid.SetRow(LeftContent, 0);
                Grid.SetRow(BorderContent, 1);
                Grid.SetRow(RightContent, 2);

                BorderContent.Cursor = Cursors.SizeNS;
            }

            _ratio = _movingRatio = 0.5;
            RecalculateContentSizes();
        }

        /// <summary>
        /// Recalculates the width of the content fields
        /// </summary>
        private void RecalculateContentSizes()
        {
            if (SplitterMode == SplitterMode.Vertical)
            {
                var totalWidth = ActualWidth;
                if (totalWidth <= BorderSize)
                {
                    return;
                }

                var borderWidth = _columnDefinitions[1].Width.Value;
                totalWidth -= borderWidth;

                var leftWidth = totalWidth * _ratio;
                var rightWidth = totalWidth - leftWidth;

                _columnDefinitions[0].Width = new GridLength(leftWidth, GridUnitType.Pixel);
                _columnDefinitions[2].Width = new GridLength(rightWidth, GridUnitType.Pixel);
            }
            else if (SplitterMode == SplitterMode.Horizontal)
            {
                var totalHeight = ActualHeight;
                if (totalHeight <= BorderSize)
                {
                    return;
                }

                var borderHeight = _rowDefinitions[1].Height.Value;
                totalHeight -= borderHeight;

                var leftWidth = totalHeight * _ratio;
                var rightWidth = totalHeight - leftWidth;

                _rowDefinitions[0].Height = new GridLength(leftWidth, GridUnitType.Pixel);
                _rowDefinitions[2].Height = new GridLength(rightWidth, GridUnitType.Pixel);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            BorderContent.CaptureMouse();

            _isMouseDown = true;
            _lastPosition = e.MouseDevice.GetPosition(this);
            _movingRatio = _ratio;
        }

        private void OnLostMouseCapture(object x, MouseEventArgs y)
        {
            if (_isMouseDown)
            {
                _isMouseDown = false;
                BorderContent.ReleaseMouseCapture();
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                var currentPosition = e.MouseDevice.GetPosition(this);

                if (SplitterMode == SplitterMode.Vertical)
                {
                    var diffX = currentPosition.X - _lastPosition.X;
                    _movingRatio += diffX / ActualWidth;
                }
                else if (SplitterMode == SplitterMode.Horizontal)
                {
                    var diffY = currentPosition.Y - _lastPosition.Y;
                    _movingRatio += diffY / ActualHeight;
                }

                _ratio = Math.Max(0, Math.Min(1.0, _movingRatio));
                RecalculateContentSizes();
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
