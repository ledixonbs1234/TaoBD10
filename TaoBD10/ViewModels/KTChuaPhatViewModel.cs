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
            LoginPNSCommand = new RelayCommand(LoginPNS);
            GetNameCommand = new RelayCommand(GetName);

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

            WeakReferenceMessenger.Default.Register<PNSNameMessage>(this, (r, m) =>
            {
                if (m.Value != null)
                {
                    List<PNSNameModel> pnsNames = m.Value;

                    foreach (PNSNameModel pnsName in pnsNames)
                    {

                        HangTonModel have = HangTons.FirstOrDefault(s => s.MaHieu.ToUpper() == pnsName.MaHieu.ToUpper());
                        if (have != null)
                        {
                            have.NameReceive = pnsName.NameReceive;
                        }
                    }
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
                    hangTon.BuuCucDong = m.KiemTraWeb.BuuCucDong;
                    hangTon.BuuCucNhan = m.KiemTraWeb.BuuCucNhan;

                }
                AddAddress();
            });
        }
        private string _TextsRange;
        public ICommand LoginPNSCommand { get; }

        public ICommand GetNameCommand { get; }



        void GetName()
        {
            string listMaHieu = "";
            //thuc hien send list ma hieu to name
            if (HangTons.Count == 0)
            {
                return;
            }
            foreach (HangTonModel hangTon in HangTons)
            {
                listMaHieu += hangTon.MaHieu + ",";
            }
            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "SendMaHieuPNS", Content = listMaHieu });
        }

        void LoginPNS()
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "KTChuaPhat", Content = "LoadUrlPNS" });
        }


        public string TextsRange
        {
            get { return _TextsRange; }
            set { SetProperty(ref _TextsRange, value); }
        }

        private List<MaHieuDiNgoaiInfo> LocTextTho(string textsRange)
        {
            //thuc hien loc ma hieu dia chi gui va dia chi nhan
            List<MaHieuDiNgoaiInfo> list = new List<MaHieuDiNgoaiInfo>();
            var datas = textsRange.Split('\n');
            foreach (string data in datas)
            {
                //thuc hien loc danh sach trong nay
                if (data.Count() < 13)
                    continue;
                string[] splitedData = data.Split(' ');

                int length = splitedData.Length;
                if (splitedData.Length >= 10 && data.ToUpper().IndexOf("VN") != -1)
                {
                    list.Add(new MaHieuDiNgoaiInfo(splitedData[1].ToUpper(), splitedData[3], splitedData[2], splitedData[4]));
                }
                else
                if (splitedData.Length == 7 && data.ToUpper().IndexOf("VN") != -1)
                {
                    list.Add(new MaHieuDiNgoaiInfo(splitedData[1].ToUpper(), splitedData[2], "", splitedData[3]));
                }
                else
                {
                    int indexVN = data.ToUpper().IndexOf("VN");
                    if (indexVN - 11 < 0)
                        continue;

                    list.Add(new MaHieuDiNgoaiInfo(data.Substring(indexVN - 11, 13).Trim().ToUpper()));
                }
                //if(splitedData.Length )
            }
            return list;
        }

        void LocRange()
        {
            foreach (MaHieuDiNgoaiInfo item in LocTextTho(TextsRange))
            {
                if (item.Code.Length != 13)
                {
                    continue;
                }                //    //kiem tra trung khong
                foreach (HangTonModel hangTon in HangTons.ToList())
                {
                    if (hangTon.MaHieu.ToUpper() == item.Code)
                    {
                        HangTons.Remove(hangTon);

                        break;
                    }
                }
            }
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
            if (HangTonsDelete.Count != 0)
            {
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
            }

            if (!string.IsNullOrEmpty(TextsRange))
            {
                LocRange();
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
            if (Selected == null)
            {
                return;
            }
            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "TopMost", Content = "False" });
            Process.Start("chrome.exe", "https://bccp.vnpost.vn/BCCP.aspx?act=Trace&id=" + Selected.MaHieu);
        }

        private void Run593230()
        {
            HangTons.Clear();
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "KTChuaPhat", Content = "Run230" });
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