using System.Windows.Controls;
using TaoBD10.ViewModels;

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
            if (dataGrid.SelectedItems.Count > 0)
            {
                //var vm = this.DataContext as ChiTietViewModel;
                //vm.ListShowHangHoa.IndexOf(dataGrid.SelectedItems[0]);
                dataGrid.ScrollIntoView(dataGrid.SelectedItems[0]);
            }
        }
    }
}