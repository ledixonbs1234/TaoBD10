using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows.Controls;
using TaoBD10.Model;

namespace TaoBD10.Views
{
    /// <summary>
    /// Interaction logic for XacNhanMHView.xaml
    /// </summary>
    public partial class XacNhanMHView : UserControl
    {
        public XacNhanMHView()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "Focus")
                {
                    if (m.Content == "Box")
                    {
                        Box.Focus();
                    }
                }
            });
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Box.Focus();
        }
    }
}