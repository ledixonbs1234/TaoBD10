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
    public class KiemTraViewModel :ObservableObject
    {
        private string _MaHieu;

        public string MaHieu
        {
            get { return _MaHieu; }
            set { SetProperty(ref _MaHieu, value);

                CheckEnterKey();
            }
        }

        private void CheckEnterKey()
        {
            if (MaHieu.IndexOf('\n') != -1)
            {
                MaHieu = MaHieu.Trim();
                if (MaHieu.Length != 13)
                {
                    MaHieu = "";
                    return;
                }
                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "KiemTraWeb", Content = MaHieu });
                //thuc hien vao trang web
                //sau do lay du lieu ve
                //xu ly no
                //

            }
        }

        public KiemTraViewModel()
        {

        }
    }
}
