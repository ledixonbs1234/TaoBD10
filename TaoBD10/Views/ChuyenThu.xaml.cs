using System.Windows.Controls;

namespace TaoBD10.Views
{
    /// <summary>
    /// Interaction logic for ChuyenThu.xaml
    /// </summary>
    public partial class ChuyenThu : UserControl
    {
        public ChuyenThu()
        {
            InitializeComponent();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataGrid.ScrollIntoView(dataGrid.SelectedItems[0]);
        }
    }
}