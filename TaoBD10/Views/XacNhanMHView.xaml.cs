using Microsoft.Toolkit.Mvvm.Messaging;
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
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r,m) => {

                if (m.Key == "Focus")
                {
                    if (m.Content == "Box")
                    {
                        Box.Focus();
                    }
                }
            });
        }

        
    }
}
