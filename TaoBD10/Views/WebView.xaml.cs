using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Drawing;
using System.IO;
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
            //Browser.Paint += Browser_Paint;
        }

        //private void Browser_Paint(object sender, CefSharp.Wpf.PaintEventArgs e)
        //{
        //    Bitmap newBitmap = new Bitmap(e.Width, e.Height, 4 * e.Width, System.Drawing.Imaging.PixelFormat.Format32bppRgb, e.Buffer);

        //    var aPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "TestImageCefSharpQuant.png");
        //    newBitmap.Save(aPath);
        //}

        private void Browser_IsBrowserInitializedChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "WebInitializer" });
        }
    }
}