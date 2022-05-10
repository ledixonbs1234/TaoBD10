using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using TaoBD10.Manager;
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
            WeakReferenceMessenger.Default.Register<KiemTraMessage>(this, (r, m) =>
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    m.Value.Index = KiemTras.Count + 1;
                    KiemTras.Add(m.Value);

                    //thuc hien loc trong nay ten cua buu ta
                    ReadNameBuuTa(m.Value.BuuCucNhan);
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

        private void ReadNameBuuTa(string content)
        {
            content = content.ToUpper();
            if (content.IndexOf("59G001") != -1)
            {
                //tuan
                SoundManager.playSound(@"Name\tuan.wav");
            }
            else if (content.IndexOf("59G003") != -1)
            {
                //ngoc
                SoundManager.playSound(@"Name\ngoc.wav");
            }
            else if (content.IndexOf("59G004") != -1)
            {
                //dat
                SoundManager.playSound(@"Name\dat.wav");
            }
            else if (content.IndexOf("59G005") != -1)
            {
                //thi
                SoundManager.playSound(@"Name\thi.wav");
            }
            else if (content.IndexOf("59G007") != -1)
            {
                //loan
                SoundManager.playSound(@"Name\loan.wav");
            }
            else if (content.IndexOf("59G008") != -1)
            {
                //hao
                SoundManager.playSound(@"Name\hao.wav");
            }
            else if (content.IndexOf("59G010") != -1)
            {
                //thanh
                SoundManager.playSound(@"Name\thanh.wav");
            }
            else if (content.IndexOf("59G012") != -1)
            {
                //nhi
                SoundManager.playSound(@"Name\nhi.wav");
            }
            else if (content.IndexOf("59G024") != -1)
            {
                SoundManager.playSound(@"Name\phu.wav");
            }
            else if (content.IndexOf("59G026") != -1)
            {
                //toan
                SoundManager.playSound(@"Name\toan.wav");
            }
            else if (content.IndexOf("59G027") != -1)
            {
                //hau
                SoundManager.playSound(@"Name\hau.wav");
            }
            else if (content.IndexOf("59G032") != -1)
            {
                //van
                SoundManager.playSound(@"Name\van.wav");
            }
            else if (content.IndexOf("59G033") != -1)
            {
                //phong
                SoundManager.playSound(@"Name\phong.wav");
            }
        }
    }
}