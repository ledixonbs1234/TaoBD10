using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using TaoBD10.Manager;
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
            HangTonsDelete = new ObservableCollection<HangTonModel>();
            LocCommand = new RelayCommand(Loc);
            CopyCommand = new RelayCommand(Copy);

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

        public ICommand CopyCommand { get; }


        void Copy()
        {
            List<TamQuanModel> listTQ = new List<TamQuanModel>();
            for (int i = 0; i < HangTons.Count; i++)
            {
                TamQuanModel tq = new TamQuanModel(i + 1, HangTons[i].MaHieu);
                listTQ.Add(tq);
            }
            if (listTQ.Count != 0)
                WeakReferenceMessenger.Default.Send(new TamQuansMessage(listTQ));

        }


        public ICommand LocCommand { get; }


        void Loc()
        {
            if (HangTonsDelete.Count == 0)
                return;
            foreach (HangTonModel deleteItem in HangTonsDelete)
            {
                foreach (HangTonModel hangTon in HangTons.ToList())
                {
                    bool isCanDelete = false;
                    if (!string.IsNullOrEmpty(deleteItem.MaHieu))
                    {
                        if (hangTon.MaHieu.ToLower().IndexOf(deleteItem.MaHieu.ToLower()) != -1)
                            isCanDelete = true;
                        else
                        {
                            continue;
                        }
                    }
                    if (!string.IsNullOrEmpty(deleteItem.KhoiLuong))
                    {
                        if (hangTon.KhoiLuong == deleteItem.KhoiLuong)
                            isCanDelete = true;
                        else
                        {
                            continue;
                        }
                    }

                    if (!string.IsNullOrEmpty(deleteItem.BuuCucPhatHanh))
                    {
                        if (hangTon.BuuCucPhatHanh.ToLower().IndexOf(deleteItem.BuuCucPhatHanh.ToLower()) != -1)
                            isCanDelete = true;
                        else
                        {
                            continue;
                        }
                    }

                    if (!string.IsNullOrEmpty(deleteItem.TimeGui))
                    {
                        if (hangTon.TimeGui.ToLower().IndexOf(deleteItem.TimeGui.ToLower()) != -1)
                            isCanDelete = true;
                        else
                        {
                            continue;
                        }
                    }
                    if (!string.IsNullOrEmpty(deleteItem.TimeCapNhat))
                    {
                        if (hangTon.TimeCapNhat.ToLower().IndexOf(deleteItem.TimeCapNhat.ToLower()) != -1)
                            isCanDelete = true;
                        else
                        {
                            continue;
                        }
                    }
                    if (!string.IsNullOrEmpty(deleteItem.NguoiGui))
                    {
                        if (APIManager.ConvertToUnSign3(hangTon.NguoiGui.ToLower()).IndexOf(APIManager.ConvertToUnSign3(deleteItem.NguoiGui.ToLower())) != -1)
                            isCanDelete = true;
                        else
                        {
                            continue;
                        }
                    }

                    if (isCanDelete)
                    {
                        HangTons.Remove(hangTon);
                    }

                }

            }
            Count = HangTons.Count;

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

        private ObservableCollection<HangTonModel> _HangTonsDelete;

        public ObservableCollection<HangTonModel> HangTonsDelete
        {
            get { return _HangTonsDelete; }
            set { SetProperty(ref _HangTonsDelete, value); }
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
            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "TopMost", Content = "False" });
            Process.Start("chrome.exe", "https://bccp.vnpost.vn/BCCP.aspx?act=Trace&id=" + Selected.MaHieu);
        }

        private void Run593230()
        {
            HangTons.Clear();
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
            HangTons.Clear();
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "KTChuaPhat", Content = "Run280" });
        }

        private int _Count;
        private ObservableCollection<HangTonModel> _HangTons;
        private string _IsOk;
        private HangTonModel _Selected;
    }
}