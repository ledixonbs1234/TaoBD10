using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class KiemTraViewModel : ObservableObject
    {
        private string _MaHieu;

        public string MaHieu
        {
            get { return _MaHieu; }
            set
            {
                SetProperty(ref _MaHieu, value);
                CheckEnterKey();
            }
        }

        private ObservableCollection<KiemTraModel> _KiemTras;

        public ObservableCollection<KiemTraModel> KiemTras
        {
            get { return _KiemTras; }
            set { SetProperty(ref _KiemTras, value); }
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
                MaHieu = "";
                //thuc hien vao trang web
                //sau do lay du lieu ve
                //xu ly no
                //

            }
        }

        public KiemTraViewModel()
        {

            KiemTras = new ObservableCollection<KiemTraModel>();
            WeakReferenceMessenger.Default.Register<KiemTraMessage>(this, (r, m) => {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    m.Value.Index = KiemTras.Count + 1;
                    KiemTras.Add(m.Value);
                });
                //KiemTraModel a = new KiemTraModel();
                //a.Address = m.Value.Address;
                //a.BuuCucDong = m.Value.BuuCucDong;
                //a.BuuCucNhan = m.Value.BuuCucNhan;
                //a.Date = m.Value.Date;
                //a.Index = m.Value.Index;
                //a.MaHieu = m.Value.MaHieu;
                //a.Address = m.Value.Address;
                //KiemTras.Add(a);
            });
        }
    }
}
