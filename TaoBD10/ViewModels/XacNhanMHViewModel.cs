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
    public class XacNhanMHViewModel : ObservableObject
    {
        private string _MaHieu;
        private KiemTraModel _XacNhanMH;

        public KiemTraModel XacNhanMH
        {
            get { return _XacNhanMH; }
            set { SetProperty(ref _XacNhanMH, value); }
        }


        public string MaHieu
        {
            get { return _MaHieu; }
            set { SetProperty(ref _MaHieu, value); OnEnterKey(); }
        }

        private void OnEnterKey()
        {
            if (MaHieu.IndexOf('\n') != -1)
            {
                MaHieu = MaHieu.Trim();
                if (MaHieu.Length != 13)
                {
                    MaHieu = "";
                    return;
                }
                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "XacNhanMH", Content = _MaHieu.Trim().ToLower() });

                MaHieu = "";
            }
        }

        public XacNhanMHViewModel()
        {
            WeakReferenceMessenger.Default.Register<KiemTraMessage>(this, (r, m) =>
            {
                if (m.Value.Key == "XacNhanMH")
                {
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        XacNhanMH = m.Value;
                    });

                }
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
