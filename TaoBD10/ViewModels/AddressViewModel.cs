using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class AddressViewModel : ObservableObject
    {
        private string[] fillTamQuan = { "tam quan", "hoai son", "hoai chau", "hoai hao", "hoai phu", "hoai thanh" };

        public AddressViewModel()
        {
            HangHoas = new ObservableCollection<HangHoaDetailModel>();
            LoaiAddress = new ObservableCollection<string>
            {
                "None",
                "TamQuan",
                "ChuaXacDinh"
            };

            LayDanhSachCommand = new RelayCommand(LayDanhSach);
            LocCommand = new RelayCommand(Loc);
            LayDiaChiCommand = new RelayCommand(LayDiaChi);
            SendDataCommand = new RelayCommand(SendData);
            LayDiaChiGuiCommand = new RelayCommand(LayDiaChiGui);

            ChuyenTamQuanVeLayCTCommand = new RelayCommand(ChuyenTamQuanVeLayCT);

            WeakReferenceMessenger.Default.Register<TuiHangHoaMessage>(this, (r, m) =>
            {
                if (m.Value == null)
                    return;
                HangHoas.Clear();
                foreach (HangHoaDetailModel item in m.Value)
                {
                    HangHoas.Add(item);
                }
                Loc();
            });

            WeakReferenceMessenger.Default.Register<SHTuiMessage>(this, (r, m) =>
            {
                if (m.Value == null)
                    return;
                if (m.Value.Key == "ReturnSHTui")
                {
                    foreach (HangHoaDetailModel item in HangHoas)
                    {
                        if (m.Value.SHTui.ToLower() == item.TuiHangHoa.SHTui.ToLower())
                        {
                            item.Code = m.Value.Code.ToUpper();
                            break;
                        }
                    }
                    Loc();
                }
            });
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "IsTamQuan")
                {
                    if (m.Content == "Update")
                    {
                        SetCountTamQuan();
                    }
                }
                else if (m.Key == "ToAddress_LayDanhSach")
                {
                    LayDanhSach();
                }
            });

            WeakReferenceMessenger.Default.Register<WebContentModel>(this, (r, m) =>
            {
                if (m.Key != "AddressTamQuan")
                    return;
                HangHoaDetailModel hangHoa = HangHoas.FirstOrDefault(c => m.Code.ToUpper().IndexOf(c.Code.ToUpper()) != -1);
                if (hangHoa != null)
                {
                    hangHoa.Address = m.AddressReiceive.Trim();
                    hangHoa.AddressSend = m.AddressSend.Trim();
                    //thuc hien kiem tra tam quan
                    if (!string.IsNullOrEmpty(hangHoa.Address))
                    {
                        foreach (var fill in fillTamQuan)
                        {
                            if (APIManager.ConvertToUnSign3(hangHoa.Address).ToLower().IndexOf(fill) != -1)
                            {
                                hangHoa.IsTamQuan = "TamQuan";
                                SetCountTamQuan();
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(hangHoa.AddressSend))
                    {
                        foreach (var fill in fillTamQuan)
                        {
                            if (APIManager.ConvertToUnSign3(hangHoa.AddressSend).ToLower().IndexOf(fill) != -1)
                            {
                                hangHoa.IsTamQuan = "TamQuan";
                                SetCountTamQuan();
                                break;
                            }
                        }
                    }
                    LayDiaChi();
                }
            });

            WeakReferenceMessenger.Default.Register<ChiTietTuiMessage>(this, (r, m) =>
            {
                if (m.Value != null)
                {
                    if (m.Value.Key == "TamQuanAddress")
                    {
                        List<ChiTietTuiModel> chiTietTuis = m.Value.ChiTietTuis;

                        foreach (ChiTietTuiModel chiTietTui in chiTietTuis)
                        {
                            var HaveHangHoa = HangHoas.FirstOrDefault(s => s.Code.ToUpper() == chiTietTui.MaHieu.ToUpper());
                            if (HaveHangHoa != null)
                            {
                                HaveHangHoa.Address = chiTietTui.Address.Trim();
                                HaveHangHoa.BuuCucChapNhan = chiTietTui.BCChapNhan;

                                //if (!string.IsNullOrEmpty(hangHoa.AddressSend))
                                //{
                                //    foreach (var fill in fillTamQuan)
                                //    {
                                //        if (APIManager.ConvertToUnSign3(hangHoa.AddressSend).ToLower().IndexOf(fill) != -1)
                                //        {
                                //            hangHoa.IsTamQuan = "TamQuan";
                                //            SetCountTamQuan();
                                //            break;
                                //        }
                                //    }
                                //}
                            }
                        }
                        TimTamQuanFromDiaChi();
                    }
                }
            }
           );
        }

        private bool _IsFast = true;

        public bool IsFast
        {
            get { return _IsFast; }
            set { SetProperty(ref _IsFast, value); }
        }

        private void TimTamQuanFromDiaChi()
        {
            foreach (var hangHoa in HangHoas)
            {
                if (!string.IsNullOrEmpty(hangHoa.Address))
                {
                    foreach (var fill in fillTamQuan)
                    {
                        if (APIManager.ConvertToUnSign3(hangHoa.Address).ToLower().IndexOf(fill) != -1)
                        {
                            hangHoa.IsTamQuan = "TamQuan";
                            SetCountTamQuan();
                            break;
                        }
                    }
                }
            }
        }

        private void ChuyenTamQuanVeLayCT()
        {
            var hangHoaKT = HangHoas.Where(m => m.PhanLoai == FileManager.optionModel.MaKhaiThac);
            WeakReferenceMessenger.Default.Send(new ChuyenTamQuanMessage(hangHoaKT.ToList().FindAll(m => m.IsTamQuan == "TamQuan")));
        }

        private void LayDanhSach()
        {
            //thuc hien lay danh sach trong nay
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "LayHangHoa" });
        }

        private void LayDiaChi()
        {
            if (HangHoas.Count == 0)
                return;
            string listMaHieu = "";
            if (IsFast)
            {
                string addressDefault = "https://bccp.vnpost.vn/BCCP.aspx?act=MultiTrace&id=";
                foreach (HangHoaDetailModel hangHoa in HangHoas)
                {
                    if (!string.IsNullOrEmpty(hangHoa.Code))
                    {
                        if (string.IsNullOrEmpty(hangHoa.Address))
                            listMaHieu += hangHoa.Code + ",";
                    }
                }
                addressDefault += listMaHieu;
                WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "ListAddressFull", Content = addressDefault });
            }
            else
            {
                foreach (HangHoaDetailModel diNgoaiItem in HangHoas)
                {
                    if (!string.IsNullOrEmpty(diNgoaiItem.Code))
                    {
                        if (diNgoaiItem.Code.Length != 13)
                            continue;
                        if (!string.IsNullOrEmpty(diNgoaiItem.Address))
                            continue;

                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "LoadAddressTQWeb", Content = diNgoaiItem.Code });
                        break;
                    }
                }
            }
        }

        public ICommand LayTungDiaChiCommand { get; }

        private void LayDiaChiGui()
        {
            if (SelectedAddress != null)
            {
                if (!string.IsNullOrEmpty(SelectedAddress.Code))
                {
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "LoadAddressTQWeb", Content = SelectedAddress.Code });
                }
            }
        }

        private void Loc()
        {
            //thuc hien loc danh sach tu bd8
            if (HangHoas.Count == 0)
                return;

            foreach (HangHoaDetailModel diNgoaiItem in HangHoas)
            {
                if (!string.IsNullOrEmpty(diNgoaiItem.TuiHangHoa.SHTui))
                {
                    if (!string.IsNullOrEmpty(diNgoaiItem.Code))
                        continue;
                    if (diNgoaiItem.TuiHangHoa.SHTui.Length != 13)
                    {
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "GetCodeFromBD", Content = diNgoaiItem.TuiHangHoa.SHTui });
                        break;
                    }
                    else
                    {
                        //thuc hien trong nay
                        diNgoaiItem.Code = diNgoaiItem.TuiHangHoa.SHTui;
                    }
                }
            }
        }

        private void SendData()
        {
            string dataSend = "";
            foreach (var item in HangHoas)
            {
                if (item.IsTamQuan != "None")
                {
                    if (item.IsTamQuan == "TamQuan")
                    {
                        dataSend += item.TuiHangHoa.SHTui + "|" + item.TuiHangHoa.ToBC + "\n";
                    }
                    else if (item.IsTamQuan == "ChuaXacDinh")
                    {
                        dataSend += item.TuiHangHoa.SHTui + "|chuaxacdinh\n";
                    }
                }
            }
            MqttManager.Pulish("tamquanget1", dataSend, isRetain: true);
        }

        private void SetCountTamQuan()
        {
            var data = HangHoas.Where(m => m.IsTamQuan != "None");
            if (data != null)
            {
                CountTamQuan = data.Count();
            }
        }

        public int CountTamQuan
        {
            get { return _CountTamQuan; }
            set { SetProperty(ref _CountTamQuan, value); }
        }

        public ICommand ChuyenTamQuanVeLayCTCommand { get; }

        public ObservableCollection<HangHoaDetailModel> HangHoas
        {
            get { return _HangHoas; }
            set { SetProperty(ref _HangHoas, value); }
        }

        public ICommand LayDanhSachCommand { get; }
        public ICommand LayDiaChiCommand { get; }
        public ICommand LayDiaChiGuiCommand { get; }

        public ObservableCollection<string> LoaiAddress
        {
            get { return _LoaiAddress; }
            set { SetProperty(ref _LoaiAddress, value); }
        }

        public ICommand LocCommand { get; }

        public HangHoaDetailModel SelectedAddress
        {
            get { return _SelectedAddress; }
            set { SetProperty(ref _SelectedAddress, value); }
        }

        public ICommand SendDataCommand { get; }
        private int _CountTamQuan;
        private ObservableCollection<HangHoaDetailModel> _HangHoas;
        private ObservableCollection<string> _LoaiAddress;
        private HangHoaDetailModel _SelectedAddress;
    }
}