using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace TaoBD10.ViewModels
{
    public class LocTuiViewModel : ObservableObject
    {
        private ObservableCollection<TuiHangHoa> _ListHangHoa;
        private bool _IsGopVeNTB;

        public bool IsGopVeNTB
        {
            get { return _IsGopVeNTB; }
            set { SetProperty(ref _IsGopVeNTB, value); }
        }

        public ObservableCollection<TuiHangHoa> ListHangHoa
        {
            get { return _ListHangHoa; }
            set
            {
                SetProperty(ref _ListHangHoa, value);
            }
        }

        private string _NameBD;

        public string NameBD
        {
            get { return _NameBD; }
            set
            {
                SetProperty(ref _NameBD, value);
                ClearCommand.NotifyCanExecuteChanged();
            }
        }

        public ICommand LayBCNhanCommand { get; }

        // Lấy địa chỉ toàn bộ và Xử Lý BC để lấy BC Nhận
        private void LayBCNhan()
        {
            string listMaHieu = "";
            foreach (var hangHoa in ListHangHoa)
            {
                if (hangHoa.SHTui.Length == 13)
                {
                    listMaHieu += hangHoa.SHTui + ",";
                }
            }
            if (!string.IsNullOrEmpty(listMaHieu))
            {
                string addressDefault = "https://bccp.vnpost.vn/BCCP.aspx?act=MultiTrace&id=";
                addressDefault += listMaHieu;
                WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "ToWeb_LocTui", Content = addressDefault });
            }
        }

        public IRelayCommand CapNhatCommand { get; }

        private void CapNhat()
        {
            //cap nhat ma khong co ten thi se chuyen qua chi tiet de kiem tra lai
            BD10InfoModel bd10 = new BD10InfoModel
            {
                Name = "Tam Thoi",
                DateCreateBD10 = DateTime.Now,
                LanLap = "1",
                TimeTrongNgay = EnumAll.TimeSet.Sang,
                TuiHangHoas = ListHangHoa.ToList()
            };

            List<BD10InfoModel> listBD10 = new List<BD10InfoModel>
                {
                    bd10
                };

            WeakReferenceMessenger.Default.Send<BD10Message>(new BD10Message(listBD10));
            if (string.IsNullOrEmpty(NameBD))
            {
                FileManager.SaveBD10Offline(new BD10InfoModel("Tam Thoi " + DateTime.Now.ToShortTimeString(), ListHangHoa.ToList(), DateTime.Now, EnumAll.TimeSet.Sang, "1"));
                WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = "Đã Tạo BĐ 10 với tên : Tam Thoi" });
            }
            else
            {
                FileManager.SaveBD10Offline(new BD10InfoModel(NameBD + " " + DateTime.Now.ToShortTimeString(), ListHangHoa.ToList(), DateTime.Now, EnumAll.TimeSet.Sang, "1"));
                WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = "Đã Tạo BĐ 10 với tên : " + NameBD });
            }
            WeakReferenceMessenger.Default.Send<string>("LoadBD10");
            //thuc hien navigate to chi tiet
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "GoTaoTui" });

            //MessageShow("Đã Tạo BĐ 10 với tên : " + NameBD);
        }

        public IRelayCommand ClearCommand { get; }

        private void ClearData()
        {
            ListHangHoa.Clear();
        }

        private string _TextBD;

        public string TextBD
        {
            get { return _TextBD; }
            set
            {
                if (value != _TextBD)
                {
                    SetProperty(ref _TextBD, value);
                    //Kiem tra thu Text trong nay
                    CheckEnterKey();
                }
            }
        }

        private void CheckEnterKey()
        {
            if (TextBD.IndexOf('\n') != -1)
            {
                TextBD = TextBD.Trim();
                if (TextBD.Length != 29 && TextBD.Length != 13)
                {
                    TextBD = "";
                    return;
                }
                //kiem tra trung khong
                if (ListHangHoa.Count == 0)
                {
                    ListHangHoa.Add(new TuiHangHoa((ListHangHoa.Count + 1).ToString(), TextBD));

                    CapNhatCommand.NotifyCanExecuteChanged();
                    ClearCommand.NotifyCanExecuteChanged();
                    TextBD = "";
                }
                else
                {
                    foreach (TuiHangHoa item in ListHangHoa)
                    {
                        if (item.SHTui == TextBD)
                        {
                            TextBD = "";
                            return;
                        }
                    }
                    ListHangHoa.Add(new TuiHangHoa((ListHangHoa.Count + 1).ToString(), TextBD));
                    CapNhatCommand.NotifyCanExecuteChanged();
                    ClearCommand.NotifyCanExecuteChanged();
                    TextBD = "";
                }
            }
        }

        public LocTuiViewModel()
        {
            LayBCNhanCommand = new RelayCommand(LayBCNhan);
            ClearCommand = new RelayCommand(ClearData, () =>
            {
                if (ListHangHoa.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            CapNhatCommand = new RelayCommand(CapNhat, () =>
            {
                if (ListHangHoa.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            ListHangHoa = new ObservableCollection<TuiHangHoa>();

            WeakReferenceMessenger.Default.Register<ChiTietTuiMessage>(this, (r, m) =>
            {
                if (m.Value != null)
                {
                    if (m.Value.Key == "LocTui")
                    {
                        List<ChiTietTuiModel> chiTietTuis = m.Value.ChiTietTuis;

                        foreach (ChiTietTuiModel chiTietTui in chiTietTuis)
                        {
                            var HaveHangHoa = ListHangHoa.FirstOrDefault(s => s.SHTui.ToUpper() == chiTietTui.MaHieu.ToUpper());
                            if (HaveHangHoa != null)
                            {
                                HaveHangHoa.ToBC = XuLyDiaChi(HaveHangHoa.SHTui, chiTietTui.Address.Trim());
                                HaveHangHoa.PhanLoai = "Đi ngoài";
                                HaveHangHoa.DichVu = SetDichVu(HaveHangHoa.SHTui);
                                //HaveHangHoa.BuuCucChapNhan = chiTietTui.BCChapNhan;
                            }
                        }
                    }
                }
            });
        }

        private string SetDichVu(string maHieu)
        {
            switch (maHieu.Substring(0, 1).ToUpper())
            {
                case "C":
                    return "Bưu kiện - Parcel";

                case "E":
                    return "EMS - Chuyển phát nhanh";

                case "U":
                    return "Bưu phẩm bảo đảm - Registed Mail";

                case "P":
                    return "Logistic";

                default:
                    return "Chả biết";
            }
        }

        private string XuLyDiaChi(string shTui, string address)
        {
            string maTinh = DataManager.AutoSetTinh(address);
            string loai = shTui.Substring(0, 1);
            if (string.IsNullOrEmpty(maTinh))
                return "000000";

            //thuc hien lay loai buu gui
            if (maTinh == "59")
            {
                List<string> fillAddress = address.Split('-').Select(s => s.Trim()).ToList();
                if (fillAddress == null)
                    return "000000";
                if (fillAddress.Count < 3)
                    return "000000";
                string addressExactly = fillAddress[fillAddress.Count - 2];
                string huyenUn = APIManager.BoDauAndToLower(addressExactly);
                if (huyenUn.IndexOf("phu my") != -1)
                {
                    return "592810";
                }
                else if (huyenUn.IndexOf("phu cat") != -1)
                {
                    return "592460";
                }
                else if (huyenUn.IndexOf("an nhon") != -1)
                {
                    return "592020";
                }
                else if (huyenUn.IndexOf("tay son") != -1)
                {
                    return "594210";
                }
                else if (huyenUn.IndexOf("van canh") != -1)
                {
                    return "594560";
                }
                else if (huyenUn.IndexOf("vinh thanh") != -1)
                {
                    return "594080";
                }
                else if (huyenUn.IndexOf("tuy phuoc") != -1)
                {
                    return "591720";
                }
                else if (huyenUn.IndexOf("quy nhon") != -1)
                {
                    string xaUn = APIManager.BoDauAndToLower(fillAddress[fillAddress.Count - 3]);
                    if (xaUn.Contains("bui thi xuan") || xaUn.Contains("nhon binh") || xaUn.Contains("nhon phu") || xaUn.Contains("phuoc my") || xaUn.Contains("tran quang dieu"))
                    {
                        return "591218";
                    }
                    else
                    {
                        return "591520";
                    }
                }
                else if (huyenUn.IndexOf("hoai an") != -1)
                {
                    string xaUn = APIManager.BoDauAndToLower(fillAddress[fillAddress.Count - 3]);
                    if (xaUn.Contains("an my") || xaUn.Contains("an tin") || xaUn.IndexOf("an hao") != -1)
                    {
                        return "593630";
                    }
                    else
                    {
                        return "593740";
                    }
                }
            }
            else if (maTinh == "70")
            {
                if (loai == "C")
                {
                    return "700920";
                }
                else if (loai == "E")
                {
                    return "701000";
                }
            }
            else if (maTinh == "10")
            {
                if (loai == "C")
                {
                    return "100920";
                }
                else if (loai == "E")
                {
                    return "101000";
                }
            }
            else if (maTinh == "55")
            {
                if (loai == "C")
                {
                    return "550920";
                }
                else if (loai == "E")
                {
                    return "550100";
                }
            }
            else
            {
                //thuc hien lay dia chi
                return maTinh + "0000";
                //foreach (string item in listBuuCuc)
                //{
                //    if (boDauAndToLower(addressExactly).IndexOf(boDauAndToLower(item)) != -1)
                //    {
                //        diNgoai.TenBuuCuc = item;
                //        diNgoai.MaBuuCuc = item.Substring(0, 6);
                //        break;
                //    }
                //}
            }
            return "000000";
        }
    }
}