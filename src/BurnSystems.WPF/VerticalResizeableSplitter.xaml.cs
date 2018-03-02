using System;
using System.Collections.Generic;
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

namespace BurnSystems.WPF
{
    /// <summary>
    /// Interaktionslogik für VerticalResizeableSplitter.xaml
    /// </summary>
    public partial class VerticalResizeableSplitter : UserControl
    {
        private double ratio = 0.5;

        public VerticalResizeableSplitter()
        {
            InitializeComponent();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var totalWidth = e.NewSize.Width;
            if (totalWidth <= 2)
            {
                return;
            }

            var borderWidth = BorderColumn.Width.Value;
            totalWidth -= borderWidth;

            var leftWidth = totalWidth * ratio;
            var rightWidth = totalWidth - leftWidth;

            LeftColumn.Width = new GridLength(leftWidth, GridUnitType.Pixel);
            RightColumn.Width = new GridLength(rightWidth, GridUnitType.Pixel);
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            LeftContent.Content = new TextBox();
            RightContent.Content = new TextBox();
        }
    }
}
