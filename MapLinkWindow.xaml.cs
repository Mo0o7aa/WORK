using System.Windows;
using System.Windows.Input;

namespace ntra_missions
{
    /// <summary>
    /// Interaction logic for MapLinkWindow.xaml
    /// </summary>
    public partial class MapLinkWindow : Window
    {
        public MapLinkWindow()
        {
            InitializeComponent();
            linkTextBox.Focus();
        }

        private void OnClickConvert(object sender, RoutedEventArgs e)
        {
            convertButton.IsEnabled = false;
            Cursor = Cursors.Wait;

            try
            {
                GoogleMapsToExcel googleMapsToExcel = new GoogleMapsToExcel();
                googleMapsToExcel.ExportFromLink(linkTextBox.Text);
            }
            finally
            {
                Cursor = Cursors.Arrow;
                convertButton.IsEnabled = true;
            }
        }

        private void OnClickClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
