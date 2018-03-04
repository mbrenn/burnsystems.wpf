using System.Windows;
using BurnSystems.WPF;

namespace BurnSystemsWPFTest
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void VerticalButton_Click(object sender, RoutedEventArgs e)
        {
            Splitter.SplitterMode = SplitterMode.Vertical;
        }

        private void HorizontalButton_Click(object sender, RoutedEventArgs e)
        {
            Splitter.SplitterMode = SplitterMode.Horizontal;
        }
    }
}
