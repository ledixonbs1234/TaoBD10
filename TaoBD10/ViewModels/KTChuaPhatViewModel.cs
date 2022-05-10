using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class KTChuaPhatViewModel : ObservableObject
    {
        public KTChuaPhatViewModel()
        {
            ChiTietCommand = new RelayCommand(ChiTiet);

            Run593280Command = new RelayCommand(Run593280);
            Run593230Command = new RelayCommand(Run593230);
            CheckCommand = new RelayCommand(Check);
            AddAddressCommand = new RelayCommand(AddAddress);
            HangTons = new ObservableCollection<HangTonModel>();

            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "RChuaPhat")
                {
                    if (m.Content == "InfoOK")
                    {
                        //neu thuc hien thanh cong thi vao trong nay
                        IsOk = "OK";
                    }
                }
            });

            WeakReferenceMessenger.Default.Register<HangTonMessage>(this, (r, m) =>
            {
                if (m.Value != null)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        HangTons.Clear();
                        foreach (HangTonModel item in m.Value)
                        {
                            HangTons.Add(item);
                        }
                        Count = HangTons.Count;
                    });
                }
            });

            WeakReferenceMessenger.Default.Register<WebContentModel>(this, (r, m) =>
            {
                if (m.Key != "AddressChuaPhat")
                    return;
                var hangTon = HangTons.FirstOrDefault(c => m.Code.IndexOf(c.MaHieu.ToUpper()) != -1);
                if (hangTon != null)
                {
                    hangTon.Address = m.AddressReiceive.Trim();
                    hangTon.NguoiGui = m.NguoiGui;
                }
                AddAddress();
            });
        }

        public ICommand AddAddressCommand { get; }
        public ICommand CheckCommand { get; }
        public ICommand ChiTietCommand { get; }

        public int Count
        {
            get { return _Count; }
            set { SetProperty(ref _Count, value); }
        }

        public ObservableCollection<HangTonModel> HangTons
        {
            get { return _HangTons; }
            set { SetProperty(ref _HangTons, value); }
        }

        public string IsOk
        {
            get { return _IsOk; }
            set { SetProperty(ref _IsOk, value); }
        }

        //thuc hien lenh trong nay
        public ICommand Run593230Command { get; }

        public ICommand Run593280Command { get; }

        public HangTonModel Selected
        {
            get { return _Selected; }
            set { SetProperty(ref _Selected, value); }
        }

        private void AddAddress()
        {
            foreach (HangTonModel hangTon in HangTons)
            {
                if (string.IsNullOrEmpty(hangTon.Address))
                {
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "LoadAddressChuaPhat", Content = hangTon.MaHieu });
                    break;
                }
            }
        }

        private void Check()
        {
            IsOk = "Checking...";
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "KTChuaPhat", Content = "LoadUrl" });
        }

        private void ChiTiet()
        {
            Process.Start("chrome.exe", "https://bccp.vnpost.vn/BCCP.aspx?act=Trace&id=" + Selected.MaHieu);
        }

        private void Run593230()
        {
            currentChuaPhat = ChuaPhat.C593230;
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "KTChuaPhat", Content = "Run230" });

            /*
            Quy trinh thuc hien nhu sau
            vao trang web do
            //kiem tra thu ip address co thay doi gi khong
            //neu co thi tu dong dang nhap
            sau do kiem tra thu phai default khong
            neu dung thi thuc hien cac cong viec binh thuong
            neu chua thi tu dang nhap

             */
        }

        private void Run593280()
        {
            currentChuaPhat = ChuaPhat.C593280;
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "KTChuaPhat", Content = "Run280" });
        }

        private int _Count;
        private ObservableCollection<HangTonModel> _HangTons;
        private string _IsOk;
        private HangTonModel _Selected;
        private ChuaPhat currentChuaPhat = ChuaPhat.C593280;
    }
}