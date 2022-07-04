using Microsoft.Toolkit.Mvvm.Messaging;
using System.Windows.Controls;
using TaoBD10.Model;

namespace TaoBD10.Views
{
    /// <summary>
    /// Interaction logic for WebView.xaml
    /// </summary>
    public partial class WebView : UserControl
    {
        public WebView()
        {
            InitializeComponent();
        }

        private void Browser_IsBrowserInitializedChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "WebInitializer" });
        }
    }
}