using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class LayBDHAViewModel : ObservableObject
    {
        private string _TestText;

        public string TestText
        {
            get { return _TestText; }
            set { SetProperty(ref _TestText, value); }
        }

        public LayBDHAViewModel()
        {
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
             {
                 if (m.Key == "Test")
                 {
                     TestText += m.Content + '\n';
                 }

             }
            );

        }


    }
}
