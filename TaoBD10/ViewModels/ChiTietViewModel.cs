using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class ChiTietViewModel : ObservableObject
    {
        public ICommand GopBDCommand { get; }

        private void GopBD()
        {
            //thuc hien gop bd dua vao danh sach hien co
            //bo bd phu my phu cat an nhon
            int countPhuMy = currentListHangHoa.Where(m => m.PhanLoai == PhanLoaiTinh.PhuMy).Count();
            int countPhuCat = currentListHangHoa.Where(m => m.PhanLoai == PhanLoaiTinh.PhuCat).Count();
            int countAnNhon = currentListHangHoa.Where(m => m.PhanLoai == PhanLoaiTinh.AnNhon).Count();
        }

        private ObservableCollection<TinhHuyenModel> _ShowTinhs;

        public ObservableCollection<TinhHuyenModel> ShowTinhs
        {
            get { return _ShowTinhs; }
            set { SetProperty(ref _ShowTinhs, value); }
        }


        public ICommand UpdateBuuCucChuyenThuCommand { get; }

        void UpdateBuuCucChuyenThu()
        {
            if (MaKT.Length == 6)
            {
                LocKhaiThac = new LocBDInfoModel
                {
                    DanhSachHuyen = MaKT,
                    TenBD = MaKT
                };
            }
            if (MaBCP.Length == 6)
            {
                LocBCP = new LocBDInfoModel
                {
                    DanhSachHuyen = MaBCP,
                    TenBD = MaBCP
                };
            }
            List<LocBDInfoModel> list = new List<LocBDInfoModel>();
            list.Add(LocKhaiThac);
            list.Add(LocBCP);
            FileManager.SaveLocKTBCPOffline(list);

        }



        private LocBDInfoModel _ConLai;

        public LocBDInfoModel ConLai
        {
            get { return _ConLai; }
            set { SetProperty(ref _ConLai, value); }
        }

        private LocBDInfoModel _LocKhaiThac;

        public LocBDInfoModel LocKhaiThac
        {
            get { return _LocKhaiThac; }
            set { SetProperty(ref _LocKhaiThac, value); }
        }
        private LocBDInfoModel _LocBCP;

        public LocBDInfoModel LocBCP
        {
            get { return _LocBCP; }
            set { SetProperty(ref _LocBCP, value); }
        }



        private string _MaKT;

        public string MaKT
        {
            get { return _MaKT; }
            set { SetProperty(ref _MaKT, value); }
        }
        private string _MaBCP;

        public string MaBCP
        {
            get { return _MaBCP; }
            set { SetProperty(ref _MaBCP, value); }
        }















        public ChiTietViewModel()
        {
            ConLai = new LocBDInfoModel();
            ConLai.TenBD = "Còn Lại";
            GopBDCommand = new RelayCommand(GopBD);
            bwChiTiet = new BackgroundWorker();
            bwChiTiet.DoWork += BwChiTiet_DoWork;
            taoBDWorker = new BackgroundWorker();
            taoBDWorker.DoWork += TaoBDWorker_DoWork;
            LocBDs = new ObservableCollection<LocBDInfoModel>();
            AddBDTinhCommand = new RelayCommand<string>(AddBDTinh);
            ShowTinhs = new ObservableCollection<TinhHuyenModel>();
            UpdateBuuCucChuyenThuCommand = new RelayCommand(UpdateBuuCucChuyenThu);
            SaveLocBDCommand = new RelayCommand(SaveLocBD);
            XuongLocCommand = new RelayCommand(XuongLoc);
            LenLocCommand = new RelayCommand(LenLoc);
            SaveTinhToSelectedLocBDCommand = new RelayCommand(SaveTinhToSelectedLocBD);
            DeleteTinhCommand = new RelayCommand(DeleteTinh);

            LoadLoc();
            WeakReferenceMessenger.Default.Register<BD10Message>(this, (r, m) =>
            {
                //Thuc Hien Trong ngay
                if (m.Value != null)
                {
                    currentListHangHoa = new List<HangHoaDetailModel>();
                    ListShowHangHoa = new ObservableCollection<HangHoaDetailModel>();
                    foreach (var BD10 in m.Value)
                    {
                        foreach (TuiHangHoa tuiHangHoa in BD10.TuiHangHoas)
                        {
                            currentListHangHoa.Add(new HangHoaDetailModel(tuiHangHoa, EnumAll.PhanLoaiTinh.None));
                        }
                    }
                    //FillData();
                    FillLocBD();
                    //ResetAndCount();

                    UpdateEnableButtonLoc();
                    //ShowTest();
                }
            });

            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "Navigation" && m.Content == "PrintDiNgoai")
                {
                    PrintDiNgoai();
                }
                else if (m.Key == "LayHangHoa")
                {
                    //thuc hien lay hang hoa trong nay
                    if (currentListHangHoa == null)
                        return;
                    List<HangHoaDetailModel> tempData = currentListHangHoa.FindAll(n => n.PhanLoai == PhanLoaiTinh.KTHN || n.PhanLoai == PhanLoaiTinh.BCPHN);
                    if (tempData == null)
                        return;
                    List<HangHoaDetailModel> data = new List<HangHoaDetailModel>();

                    foreach (HangHoaDetailModel hangHoa in tempData)
                    {
                        string temp = APIManager.ConvertToUnSign3(hangHoa.TuiHangHoa.DichVu).ToLower();
                        string temp1 = APIManager.ConvertToUnSign3(hangHoa.TuiHangHoa.PhanLoai).ToLower();
                        if (temp.IndexOf("phat hanh") != -1 || temp1.IndexOf("tui") != -1)
                        {
                            continue;
                        }
                        data.Add(hangHoa);
                    }

                    WeakReferenceMessenger.Default.Send<TuiHangHoaMessage>(new TuiHangHoaMessage(data));
                }
                else if (m.Key == "GuiTrucTiep")
                {
                    GuiTrucTiep();
                }
            });

            XeXaHoiCommand = new RelayCommand(XeXaHoi);
            timerTaoBD = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 200)
            };
            timerTaoBD.Tick += TimerTaoBD_Tick;

            SelectedTinhCommand = new RelayCommand<string>(SelectedTinh);

            SelectionCommand = new RelayCommand<HangHoaDetailModel>(Selection);

            CopySHTuiCommand = new RelayCommand(CopySHTui, () =>
            {
                if (SelectedTui != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            var listTemp = FileManager.LoadTinhThanh();
            if (listTemp != null)
            {
                ShowTinhs = new ObservableCollection<TinhHuyenModel>();
                foreach (TinhHuyenModel item in listTemp)
                {
                    ShowTinhs.Add(item);
                }
            }

            //Lay Du Lieu LocBD10
            var listLoc = FileManager.LoadLocBD10sOffline();
            if (listLoc != null)
            {
                LocBDs = new ObservableCollection<LocBDInfoModel>();
                foreach (LocBDInfoModel item in listLoc)
                {
                    if (item.TaoBDs.Count == 0)
                    {
                        item.TaoBDs.Add(new TaoBdInfoModel());
                    }
                    LocBDs.Add(item);
                }
            }

            //thuc hien viec xoa Thong Tin tu Tinh Thanh;
        }
        void LoadLoc()
        {
            var list = FileManager.LoadLocKTBCPOffline();
            LocKhaiThac = list[0];
            LocBCP = list[1];
        }
        void ClearHangHoa()
        {
            foreach (var locBD in LocBDs)
            {
                locBD.IsEnabledButton = false;
                locBD.HangHoas.Clear();
            }
            ConLai.HangHoas.Clear();
            ConLai.IsEnabledButton = false;
        }

        void UpdateEnableButtonLoc()
        {
            foreach (var locBD in LocBDs)
            {
                locBD.IsEnabledButton = false;
                if (locBD.HangHoas.Count > 0)
                {
                    locBD.IsEnabledButton = true;
                }
            }
            if (ConLai.HangHoas.Count > 0)
            {
                ConLai.IsEnabledButton = true;
            }

        }

        void FillLocBD()
        {
            ClearHangHoa();
            //thuc hien viec clear Hang Hoa
            foreach (LocBDInfoModel locBD in LocBDs)
            {
                if (!string.IsNullOrEmpty(locBD.DanhSachHuyen))
                {
                    foreach (var item in FillThang(locBD.DanhSachHuyen))
                    {
                        IEnumerable<HangHoaDetailModel> listFilledHuyen = currentListHangHoa.Where(m => m.TuiHangHoa.ToBC.IndexOf(item) != -1);
                        foreach (HangHoaDetailModel temp in listFilledHuyen.ToList())
                        {
                            if (!string.IsNullOrEmpty(locBD.PhanLoais))
                            {
                                foreach (string phanLoai in FillThang(locBD.PhanLoais))
                                {
                                    if (temp.TuiHangHoa.PhanLoai.IndexOf(phanLoai) != -1)
                                    {
                                        locBD.HangHoas.Add(temp);
                                        currentListHangHoa.Remove(temp);
                                        break;
                                    }
                                }

                            }
                            if (!string.IsNullOrEmpty(locBD.DichVus))
                            {
                                foreach (string phanLoai in FillThang(locBD.DichVus))
                                {
                                    if (temp.TuiHangHoa.DichVu.IndexOf(phanLoai) != -1)
                                    {
                                        locBD.HangHoas.Add(temp);
                                        currentListHangHoa.Remove(temp);
                                        break;
                                    }
                                }

                            }
                            if (string.IsNullOrEmpty(locBD.PhanLoais) && string.IsNullOrEmpty(locBD.DichVus))
                            {
                                locBD.HangHoas.Add(temp);
                                currentListHangHoa.Remove(temp);
                            }
                            //temp.Key = locBD.TenBD;
                        }

                    }
                }
                if (locBD.DanhSachTinh.Count > 0)
                {
                    foreach (TinhHuyenModel item in locBD.DanhSachTinh)
                    {
                        IEnumerable<HangHoaDetailModel> listTinh = currentListHangHoa.Where(m => m.TuiHangHoa.ToBC.Substring(0, 2) == item.Ma);
                        foreach (HangHoaDetailModel temp in listTinh.ToList())
                        {
                            if (!string.IsNullOrEmpty(locBD.PhanLoais))
                            {
                                foreach (string phanLoai in FillThang(locBD.PhanLoais))
                                {
                                    if (temp.TuiHangHoa.PhanLoai.IndexOf(phanLoai) != -1)
                                    {
                                        locBD.HangHoas.Add(temp);
                                        currentListHangHoa.Remove(temp);
                                        break;
                                    }
                                }

                            }
                            if (!string.IsNullOrEmpty(locBD.DichVus))
                            {
                                foreach (string phanLoai in FillThang(locBD.DichVus))
                                {
                                    if (temp.TuiHangHoa.DichVu.IndexOf(phanLoai) != -1)
                                    {
                                        locBD.HangHoas.Add(temp);
                                        currentListHangHoa.Remove(temp);
                                        break;
                                    }
                                }

                            }
                        }
                    }
                }
            }

            //Xu ly ben kt bcp
            //KT
            if (!string.IsNullOrEmpty(LocKhaiThac.DanhSachHuyen))
            {

                IEnumerable<HangHoaDetailModel> listFill = currentListHangHoa.Where(m => m.TuiHangHoa.ToBC.IndexOf(LocKhaiThac.DanhSachHuyen) != -1);
                foreach (var item in listFill.ToList())
                {
                    LocKhaiThac.HangHoas.Add(item);
                    currentListHangHoa.Remove(item);
                }
            }

            //BCP
            if (!string.IsNullOrEmpty(LocBCP.DanhSachHuyen))
            {

                IEnumerable<HangHoaDetailModel> listFill = currentListHangHoa.Where(m => m.TuiHangHoa.ToBC.IndexOf(LocBCP.DanhSachHuyen) != -1);
                foreach (var item in listFill.ToList())
                {
                    LocBCP.HangHoas.Add(item);
                    currentListHangHoa.Remove(item);
                }
            }


            //Con Lai
            if (currentListHangHoa.Count > 0)
            {
                foreach (HangHoaDetailModel item in currentListHangHoa.ToList())
                {
                    ConLai.HangHoas.Add(item);
                    currentListHangHoa.Remove(item);
                }
            }
        }
        List<string> FillThang(string text)
        {
            List<string> temp = text.Split('|').ToList();

            return temp;
        }





        public ICommand XuongLocCommand { get; }

        void XuongLoc()
        {
            int indexLoc = LocBDs.IndexOf(SelectedLocBD);
            if (indexLoc == -1)
                return;
            if (indexLoc == LocBDs.Count - 1)
                return;
            LocBDInfoModel tempCT = LocBDs[indexLoc + 1];
            LocBDs[indexLoc + 1] = SelectedLocBD;
            LocBDs[indexLoc] = tempCT;
            SelectedLocBD = LocBDs[indexLoc + 1];
        }


        void DeleteTinhWhenUnCheck()
        {
            foreach (LocBDInfoModel item in LocBDs)
            {
                if (item.DanhSachTinh.Count > 0)
                {
                    foreach (var tinh in item.DanhSachTinh.ToList())
                    {
                        if (!tinh.IsChecked)
                            item.DanhSachTinh.Remove(tinh);
                    }
                }
            }
        }

        private TinhHuyenModel _SelectTinh;

        public TinhHuyenModel SelectTinh
        {
            get { return _SelectTinh; }
            set { SetProperty(ref _SelectTinh, value); }
        }



        public ICommand SaveLocBDCommand { get; }

        void SaveLocBD()
        {
            //DeleteTinhWhenUnCheck();
            FileManager.SaveLocBD10Offline(LocBDs.ToList());
        }



        public ICommand SaveTinhToSelectedLocBDCommand { get; }

        void SaveTinhToSelectedLocBD()
        {
            if (SelectedLocBD == null)
                return;

            if (SelectedLocBD.IsTinh)
            {
                foreach (TinhHuyenModel item in ShowTinhs)
                {
                    if (item.IsChecked)
                    {
                        TinhHuyenModel isHave = SelectedLocBD.DanhSachTinh.FirstOrDefault(m => m.Ma == item.Ma);
                        if (isHave == null)
                        {
                            SelectedLocBD.DanhSachTinh.Add(item);
                        }
                    }

                }
            }

        }




        private LocBDInfoModel _SelectedLocBD;

        public LocBDInfoModel SelectedLocBD
        {
            get { return _SelectedLocBD; }
            set
            {
                SetProperty(ref _SelectedLocBD, value);

            }
        }



        TaoBdInfoModel _taoBDAdd;
        private void AddBDTinh(string Name)
        {
            if (string.IsNullOrEmpty(Name))
                return;
            LocBDInfoModel loc = LocBDs.FirstOrDefault(m => m.TenBD == Name);
            if (loc == null) return;
            _taoBDAdd = loc.TaoBDs[0];

            taoBDWorker.RunWorkerAsync();

        }

        private ObservableCollection<LocBDInfoModel> _LocBDs;

        public ObservableCollection<LocBDInfoModel> LocBDs
        {
            get { return _LocBDs; }
            set { SetProperty(ref _LocBDs, value); }
        }

        private TaoBdInfoModel _CurrenTaoBD;

        public TaoBdInfoModel CurrenTaoBD
        {
            get { return _CurrenTaoBD; }
            set { SetProperty(ref _CurrenTaoBD, value); }
        }





        private void AnNhon()
        {
            var time = DateTime.Now;
            if (time.Hour > 12)
                countChuyen = 2;
            else
                countChuyen = 1;

            maBuuCuc = "592020";
            tenDuongThu = "Đà Nẵng - Bình Định";
            countDuongThu = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }
        private void BwChiTiet_DoWork(object sender, DoWorkEventArgs e)
        {
            WindowInfo window = APIManager.WaitingFindedWindow("quan ly chuyen thu");
            if (window == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy quản lý chuyến thư");
                return;
            }

            List<TestAPIModel> controls = APIManager.GetListControlText(window.hwnd);
            if (controls.Count == 0)
            {
                APIManager.ShowSnackbar("Window lỗi");
                return;
            }
            List<TestAPIModel> listCombobox = controls.Where(m => m.ClassName.ToLower().IndexOf("combobox") != -1).ToList();
            IntPtr comboHandle = listCombobox[3].Handle;

            APIManager.SendMessage(comboHandle, 0x0007, 0, 0);
            APIManager.SendMessage(comboHandle, 0x0007, 0, 0);

            SendKeys.SendWait("{F3}");
            SendKeys.SendWait(CurrentSelectedHangHoaDetail.TuiHangHoa.SHTui);
            SendKeys.SendWait("{ENTER}");
            window = APIManager.WaitingFindedWindow("xac nhan chi tiet tui thu", "xem chuyen thu chieu den");
            if (window == null)
                return;
            Thread.Sleep(500);
            window = APIManager.WaitingFindedWindow("xac nhan chi tiet tui thu", "xem chuyen thu chieu den");
            if (window == null)
                return;
            if (window.text.IndexOf("xac nhan chi tiet tui thu") != -1)
            {
                SendKeys.SendWait("{ESC}");
            }
            else if (window.text.IndexOf("xem chuyen thu chieu den") != -1)
            {
            }
        }

        private BD10DiInfoModel ConvertBD10Di(string content)
        {
            if (string.IsNullOrEmpty(content))
                return null;
            string[] splitString = content.Split('\t');

            //550910-VCKV - Đà Nẵng LT	08/06/2022	1	Ô tô	21	206,4	Đã đi
            BD10DiInfoModel bD10DiInfoModel = new BD10DiInfoModel(splitString[0], splitString[1], int.Parse(splitString[2]), int.Parse(splitString[4]), splitString[6]);

            return bD10DiInfoModel;
        }

        private void CopySHTui()
        {
            //thuc hien lenh trong nay
            if (SelectedTui != null)
            {
                System.Windows.Clipboard.SetDataObject(SelectedTui.TuiHangHoa.SHTui);
                //SendMessage
                WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = " Đã Copy" });
            }
        }

        /// <summary>
        /// Dua thong tin vao sua thong tin bd
        /// Kiem tra va luu du lieu da dang ky
        ///
        /// </summary>
        private void RunLietKeDataToSuaThongTin(PhanLoaiTinh tinh)
        {
            var currentWindow = APIManager.WaitingFindedWindow("sua thong tin bd 10");
            if (currentWindow == null)
                return;

            ////////////////////////////////////////////////////
            if (_ListShowHangHoa.Count == 0)
            {
                APIManager.ShowSnackbar("Chưa có dữ liệu");
                return;
            }

            //thuc hien kiem tra thu hien tai dang dung cai nao
            var handles = APIManager.GetAllChildHandles(currentWindow.hwnd);
            string textHandleName = "WindowsForms10.COMBOBOX.app.0.1e6fa8e";
            string textSoLuongTui = "WindowsForms10.STATIC.app.0.1e6fa8e";
            int countTemp = 0;
            string classNameComBoBox = "";

            ///Kiem tra so tui hien tai
            int countCurrentTui = 0;

            bool isGone = false;
            if (currentTinh != PhanLoaiTinh.HA_AL)
            {
                var controls = APIManager.GetListControlText(currentWindow.hwnd);
                foreach (var item in controls)
                {
                    if (item.ClassName.IndexOf(textHandleName) != -1)
                    {
                        classNameComBoBox = item.Text;
                    }
                    else if (item.ClassName.IndexOf(textSoLuongTui) != -1)
                    {
                        if (countTemp == 10)
                        {
                            countCurrentTui = int.Parse(item.Text);
                        }
                        countTemp++;
                    }

                    //tim cai o cua sh tui
                    //focus no
                    //xong roi dien vao va nhan enter thoi
                }
            }
            else
            {
                isGone = true;
            }

            bool isRightBD10 = true;
            switch (currentTinh)
            {
                case PhanLoaiTinh.None:
                    break;

                case PhanLoaiTinh.HA_AL:
                    isGone = true;
                    break;

                case PhanLoaiTinh.TamQuan:
                    if (classNameComBoBox.IndexOf("593330") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.KienDaNang:
                    if (classNameComBoBox.IndexOf("550910") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.EMSDaNang:
                    if (classNameComBoBox.IndexOf("550915") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.QuangNam:
                    if (classNameComBoBox.IndexOf("560100") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.QuangNgai:
                    if (classNameComBoBox.IndexOf("570100") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.DiNgoaiNamTrungBo:
                case PhanLoaiTinh.TuiNTB:
                case PhanLoaiTinh.KT1:
                    if (classNameComBoBox.IndexOf("590100") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.PhuMy:
                    if (classNameComBoBox.IndexOf("592810") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.PhuCat:
                    if (classNameComBoBox.IndexOf("592440") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.AnNhon:
                    if (classNameComBoBox.IndexOf("592020") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                default:
                    break;
            }

            if (!isRightBD10)
            {
                //thuc hien doc roi thoat
                SoundManager.playSound(@"Number\nhapkhongdung.wav");
                return;
            }
            //thuc hien kiem tra ngay trong nay

            Thread.Sleep(200);

            //Xu Ly trong nay
            List<HangHoaDetailModel> listFall = new List<HangHoaDetailModel>();

            int currentCount = 0;
            int numberRead = 0;
            foreach (HangHoaDetailModel hangHoa in ListShowHangHoa)
            {
                SendKeys.SendWait(hangHoa.TuiHangHoa.SHTui);
                SendKeys.SendWait("{ENTER}");
                //cho number tang len neu khong tang len thi se hien thong bao
                bool isWaiting = true;
                while (isWaiting)
                {
                    Thread.Sleep(50);
                    var window = APIManager.GetActiveWindowTitle();
                    if (window.text.IndexOf("sua thong tin bd10") != -1)
                    {
                        List<TestAPIModel> listWindowStatic = APIManager.GetListControlText(currentWindow.hwnd).Where(m => m.ClassName.IndexOf("WindowsForms10.STATIC.app") != -1).ToList();
                        if (listWindowStatic.Count <= 10)
                        {
                            continue;
                        }
                        TestAPIModel apiNumber = listWindowStatic[10];
                        //TestText += apiNumber.Text + "\n";
                        int.TryParse(Regex.Match(apiNumber.Text, @"\d+").Value, out numberRead);
                        if (currentCount != numberRead)
                        {
                            currentCount = numberRead;
                            isWaiting = false;
                        }
                    }
                    else
                    {
                        //thuc hien luu du lieu sai vao
                        listFall.Add(hangHoa);
                        Thread.Sleep(200);
                        SendKeys.SendWait("{Enter}");
                        Thread.Sleep(200);

                        isWaiting = false;
                    }
                }
            }

            if (!isGone)
            {
                //kiem tra so luong tui hien tai co bang
                int lastCountTuiHienTai = 0;
                countTemp = 0;
                foreach (var item in handles)
                {
                    string classText = APIManager.GetWindowClass(item);

                    if (classText.IndexOf(textSoLuongTui) != -1)
                    {
                        if (countTemp == 10)
                        {
                            lastCountTuiHienTai = int.Parse(APIManager.GetControlText(item));
                        }
                        countTemp++;
                    }
                    //tim cai o cua sh tui
                    //focus no
                    //xong roi dien vao va nhan enter thoi
                }

                if (lastCountTuiHienTai - countCurrentTui != ListShowHangHoa.Count)
                {
                    SoundManager.playSound(@"Number\khongkhopsolieu.wav");
                    return;
                }
                else
                {
                    SoundManager.playSound2(@"Number\tingting.wav");
                    return;
                }
            }
            ////////////////////////////////////////////////////
        }

        private void EMSDaNang()
        {
            maBuuCuc = "550915";
            tenDuongThu = "Đà Nẵng - Bình Định";
            countDuongThu = 2;
            var time = DateTime.Now;
            if (time.Hour < 18 && time.Hour > 9)
                countChuyen = 2;
            else
                countChuyen = 1;

            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }


        public ICommand DeleteTinhCommand { get; }

        void DeleteTinh()
        {
            if (SelectedLocBD != null && SelectTinh != null)
            {
                SelectedLocBD.DanhSachTinh.Remove(SelectTinh);
            }
        }


        public ICommand LenLocCommand { get; }

        void LenLoc()
        {
            int indexLoc = LocBDs.IndexOf(SelectedLocBD);
            if (indexLoc == -1)
                return;
            if (indexLoc == 0)
                return;
            LocBDInfoModel tempCT = LocBDs[indexLoc - 1];
            LocBDs[indexLoc - 1] = SelectedLocBD;
            LocBDs[indexLoc] = tempCT;
            SelectedLocBD = LocBDs[indexLoc - 1];
        }





        private void GuiTrucTiep()
        {
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;
            _ = APIManager.ConvertToUnSign3(currentWindow.text).ToLower();
            if (_ListShowHangHoa.Count == 0)
            {
                APIManager.ShowSnackbar("Chưa có dữ liệu");
                return;
            }

            //thuc hien kiem tra thu hien tai dang dung cai nao
            var handles = APIManager.GetAllChildHandles(currentWindow.hwnd);
            string textHandleName = "WindowsForms10.COMBOBOX.app.0.1e6fa8e";
            string textSoLuongTui = "WindowsForms10.STATIC.app.0.1e6fa8e";
            int countTemp = 0;
            string classNameComBoBox = "";
            int countCurrentTui = 0;

            bool isGone = false;
            if (currentTinh != PhanLoaiTinh.HA_AL)
            {
                List<TestAPIModel> controlsHandle = APIManager.GetListControlText(currentWindow.hwnd);
                foreach (var item in controlsHandle)
                {
                    if (item.ClassName.IndexOf(textHandleName) != -1)
                    {
                        classNameComBoBox = item.Text;
                    }
                    else if (item.ClassName.IndexOf(textSoLuongTui) != -1)
                    {
                        if (countTemp == 10)
                        {
                            countCurrentTui = int.Parse(item.Text);
                        }
                        countTemp++;
                    }

                    //tim cai o cua sh tui
                    //focus no
                    //xong roi dien vao va nhan enter thoi
                }
            }
            else
            {
                isGone = true;
            }

            bool isRightBD10 = true;
            switch (currentTinh)
            {
                case PhanLoaiTinh.None:
                    break;

                case PhanLoaiTinh.HA_AL:
                    isGone = true;
                    break;

                case PhanLoaiTinh.TamQuan:
                    if (classNameComBoBox.IndexOf("593330") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.KienDaNang:
                    if (classNameComBoBox.IndexOf("550910") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.EMSDaNang:
                    if (classNameComBoBox.IndexOf("550915") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.QuangNam:
                    if (classNameComBoBox.IndexOf("560100") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.QuangNgai:
                    if (classNameComBoBox.IndexOf("570100") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.DiNgoaiNamTrungBo:
                case PhanLoaiTinh.TuiNTB:
                case PhanLoaiTinh.KT1:
                    if (classNameComBoBox.IndexOf("590100") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.PhuMy:
                    if (classNameComBoBox.IndexOf("592810") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.PhuCat:
                    if (classNameComBoBox.IndexOf("592440") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                case PhanLoaiTinh.AnNhon:
                    if (classNameComBoBox.IndexOf("592020") == -1)
                    {
                        isRightBD10 = false;
                    }
                    break;

                default:
                    break;
            }

            if (!isRightBD10)
            {
                //thuc hien doc roi thoat
                SoundManager.playSound(@"Number\nhapkhongdung.wav");
                return;
            }
            //thuc hien kiem tra ngay trong nay

            Thread.Sleep(200);
            WindowInfo suaBDInfo = APIManager.GetActiveWindowTitle();
            ///////////////////////  
            TestAPIModel textBDHandle = null;

            var controls = APIManager.GetListControlText(suaBDInfo.hwnd);
            if (suaBDInfo.text.IndexOf("sua thong tin bd10") != -1 || suaBDInfo.text == "lap bd10")
            {
                int iShTuiHandle = controls.FindIndex(m => m.Text == "SH túi");
                textBDHandle = controls[iShTuiHandle + 1];
            }
            else if (suaBDInfo.text.IndexOf("lap bd10 theo") != -1)
            {
                int iShTuiHandle = controls.FindIndex(m => m.Text == "SH túi");
                textBDHandle = controls[iShTuiHandle - 1];
            }
            if (textBDHandle == null)
            {
                return;
            }


            ///////////////////////////////////////////
            //txtStateSend.Text = "Đang Gửi Trực Tiếp";
            double delayTime = Convert.ToDouble(SelectedTime);
            foreach (var hangHoa in ListShowHangHoa)
            {
                //SendKeys.SendWait(hangHoa.TuiHangHoa.SHTui);
                //SendKeys.SendWait("{ENTER}");

                ////////////////////////////////

                APIManager.setTextControl(textBDHandle.Handle, hangHoa.TuiHangHoa.SHTui);
                SendKeys.SendWait("{ENTER}");
                //lenh la cho khi so thay doi hoac hien thong bao toi da la 1 s

                /////////////////////////////////

                Thread.Sleep(Convert.ToInt32(Math.Round(delayTime * 1000, 0)));
            }

            if (!isGone)
            {
                //kiem tra so luong tui hien tai co bang
                int lastCountTuiHienTai = 0;
                countTemp = 0;
                foreach (var item in handles)
                {
                    string classText = APIManager.GetWindowClass(item);

                    if (classText.IndexOf(textSoLuongTui) != -1)
                    {
                        if (countTemp == 10)
                        {
                            lastCountTuiHienTai = int.Parse(APIManager.GetControlText(item));
                        }
                        countTemp++;
                    }
                    //tim cai o cua sh tui
                    //focus no
                    //xong roi dien vao va nhan enter thoi
                }

                if (lastCountTuiHienTai - countCurrentTui != ListShowHangHoa.Count)
                {
                    SoundManager.playSound(@"Number\khongkhopsolieu.wav");
                    return;
                }
                else
                {
                    SoundManager.playSound2(@"Number\tingting.wav");
                    APIManager.ShowSnackbar("OK");
                    return;
                }
            }
        }
        private void PrintDiNgoai()
        {
            WindowInfo info = APIManager.WaitingFindedWindow("thong tin buu gui");
            if (info == null)
                return;

            APIManager.SetPrintBD10();
            List<TestAPIModel> listControl = APIManager.GetListControlText(info.hwnd);
            List<TestAPIModel> listWindowForm = listControl.Where(m => m.ClassName.IndexOf("10.Window.8.ap") != -1).ToList();

            //thuc hien kiem tra ma trong nay
            SendKeys.SendWait("^(a)");

            SendKeys.SendWait("+{TAB}");
            SendKeys.SendWait("+{TAB}");
            SendKeys.SendWait("+{TAB}");
            SendKeys.SendWait("^(a)");
            Thread.Sleep(50);
            SendKeys.SendWait("^(c)");
            Thread.Sleep(50);
            string clipboard = System.Windows.Clipboard.GetText();
            Thread.Sleep(50);
            if (string.IsNullOrEmpty(clipboard))
            {
                return;
            }
            if (clipboard.IndexOf("BĐ1 Bis") == -1)
            {
                return;
            }
            foreach (string item in clipboard.Split('\n'))
            {
                var datas = item.Split('\t');
                if (datas[1].IndexOf("BĐ1 Bis") != -1)
                {
                    SendKeys.SendWait(" ");
                    break;
                }
                if (datas[4].IndexOf("BĐ1 Bis") != -1)
                {
                    SendKeys.SendWait("{RIGHT}");
                    Thread.Sleep(50);
                    SendKeys.SendWait("{RIGHT}");
                    Thread.Sleep(50);
                    SendKeys.SendWait("{RIGHT}");
                    Thread.Sleep(50);
                    SendKeys.SendWait(" ");
                    Thread.Sleep(100);
                    break;
                }
                SendKeys.SendWait("{DOWN}");
            }

            APIManager.ClickButton(info.hwnd, "in an pham", isExactly: false);
            WindowInfo currentWindow = APIManager.WaitingFindedWindow("print document");
            if (currentWindow == null)
            {
                return;
            }
            Thread.Sleep(500);
            APIManager.ClickButton(currentWindow.hwnd, "in an pham", isExactly: false);

            WindowInfo infoPrint = APIManager.WaitingFindedWindow("Print", isExactly: true);
            if (infoPrint == null)
                return;

            SendKeys.SendWait("%(p)");
            Thread.Sleep(500);

            WindowInfo infoPrintDocument = APIManager.WaitingFindedWindow("Print Document", isExactly: true);
            if (infoPrintDocument == null)
                return;

            if (CurrentSelectedHangHoaDetail != null)
            {
                CurrentSelectedHangHoaDetail.TrangThaiBD = TrangThaiBD.DaIn;
            }
            APIManager.ClickButton(currentWindow.hwnd, "thoat", isExactly: false);

            WindowInfo infoThongTin = APIManager.WaitingFindedWindow("thong tin buu gui");
            if (infoThongTin == null)
                return;
            APIManager.ClickButton(infoThongTin.hwnd, "thoat", isExactly: false);
        }




        private void SelectedTinh(string Name)
        {
            if (string.IsNullOrEmpty(Name))
                return;
            if (Name == ConLai.TenBD)
            {
                if (ConLai.HangHoas.Count == 0)
                    return;

                ListShowHangHoa = new ObservableCollection<HangHoaDetailModel>();

                foreach (HangHoaDetailModel hangHoa in ConLai.HangHoas)
                {
                    ListShowHangHoa.Add(hangHoa);
                }
                //thuc hien show Ten Tinh
                NameTinhCurrent = ConLai.TenBD;
            }
            else if (Name == LocKhaiThac.TenBD || Name == LocBCP.TenBD)
            {
                LocBDInfoModel currenLoc;
                if (Name == LocKhaiThac.TenBD)
                {
                    currenLoc = LocKhaiThac;
                    currentBuuCuc = BuuCuc.KT;
                }
                else
                {
                    currenLoc = LocBCP;
                    currentBuuCuc = BuuCuc.BCP;
                }

                if (currenLoc.HangHoas.Count == 0)
                    return;

                ListShowHangHoa = new ObservableCollection<HangHoaDetailModel>();

                foreach (HangHoaDetailModel hangHoa in currenLoc.HangHoas)
                {
                    string temp = APIManager.ConvertToUnSign3(hangHoa.TuiHangHoa.DichVu).ToLower();
                    string temp1 = APIManager.ConvertToUnSign3(hangHoa.TuiHangHoa.PhanLoai).ToLower();
                    if (temp.IndexOf("phat hanh") != -1 || temp1.IndexOf("tui") != -1)
                    {
                        continue;
                    }
                    ListShowHangHoa.Add(hangHoa);
                }

                //thuc hien show Ten Tinh
                NameTinhCurrent = currenLoc.TenBD;

            }
            else
            {
                LocBDInfoModel LocBD = LocBDs.FirstOrDefault(m => m.TenBD == Name);
                if (LocBD == null)
                    return;
                if (LocBD.HangHoas.Count == 0)
                    return;

                ListShowHangHoa = new ObservableCollection<HangHoaDetailModel>();

                foreach (HangHoaDetailModel hangHoa in LocBD.HangHoas)
                {

                    ListShowHangHoa.Add(hangHoa);
                }
                //thuc hien show Ten Tinh
                NameTinhCurrent = LocBD.TenBD;
                //ShowNameTinh(LocBD.TenBD);
            }

        }

        private void Selection(HangHoaDetailModel selected)
        {
            if (selected == null)
                return;
            CurrentSelectedHangHoaDetail = selected;
            if (selected.TrangThaiBD == TrangThaiBD.ChuaChon)
            {
                selected.TrangThaiBD = TrangThaiBD.DaChon;
            }

            switch (currentBuuCuc)
            {
                case BuuCuc.None:
                    return;

                case BuuCuc.KT:
                    string[] temp = FileManager.optionModel.GoFastQLCTCDKT.Split('|');
                    APIManager.GoToWindow(FileManager.optionModel.MaKhaiThac, "quan ly chuyen thu chieu den", temp[0], temp[1]);
                    break;

                case BuuCuc.BCP:
                    string[] temp1 = FileManager.optionModel.GoFastQLCTCDBCP.Split('|');
                    APIManager.GoToWindow(FileManager.optionModel.MaBuuCucPhat, "quan ly chuyen thu chieu den", temp1[0], temp1[1]);
                    break;

                default:
                    break;
            }
            bwChiTiet.RunWorkerAsync();
        }

        private void ShowNameTinh(PhanLoaiTinh phanLoaiTinh)
        {
            string textTemp = "";
            switch (phanLoaiTinh)
            {
                case PhanLoaiTinh.None:
                    textTemp = "Còn Lại";
                    break;

                case PhanLoaiTinh.HA_AL:
                    textTemp = "Hoài Ân - An Lão";
                    break;

                case PhanLoaiTinh.TamQuan:
                    textTemp = "Tam Quan";
                    break;

                case PhanLoaiTinh.KienDaNang:
                    textTemp = "Kiện Đà Nẵng";
                    break;

                case PhanLoaiTinh.EMSDaNang:
                    textTemp = "EMS Đà Nẵng";
                    break;

                case PhanLoaiTinh.QuangNam:
                    textTemp = "Quảng Nam";
                    break;

                case PhanLoaiTinh.QuangNgai:
                    textTemp = "Quảng Ngãi";
                    break;

                case PhanLoaiTinh.DiNgoaiNamTrungBo:
                    textTemp = "Kiện Nam Trung Bộ";
                    break;

                case PhanLoaiTinh.TuiNTB:
                    textTemp = "Tui Nam Trung Bộ";
                    break;

                case PhanLoaiTinh.PhuMy:
                    textTemp = "Phù Mỹ";
                    break;

                case PhanLoaiTinh.PhuCat:
                    textTemp = "Phù Cát";
                    break;

                case PhanLoaiTinh.AnNhon:
                    textTemp = "An Nhơn";
                    break;

                case PhanLoaiTinh.KT1:
                    textTemp = "KT1";
                    break;

                case PhanLoaiTinh.KTHN:
                    textTemp = "Khai Thác Hoài Nhơn";
                    TextCurrentChuyenThu = "593230";
                    currentBuuCuc = BuuCuc.KT;
                    break;

                case PhanLoaiTinh.BCPHN:
                    textTemp = "Bưu Cục Phát Hoài Nhơn";
                    TextCurrentChuyenThu = "593280";
                    currentBuuCuc = BuuCuc.BCP;
                    break;

                default:
                    break;
            }
            NameTinhCurrent = textTemp;
        }

        private void TaoBDWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var temp = FileManager.optionModel.GoFastBD10Di.Split(',');
            APIManager.GoToWindow(FileManager.optionModel.MaKhaiThac, "danh sach bd10 di", temp[0], temp[1]);

            WindowInfo currentWindow = APIManager.WaitingFindedWindow("danh sach bd10 di");
            if (currentWindow == null)
                return;

            SendKeys.SendWait("{F1}");
            currentWindow = APIManager.WaitingFindedWindow("lap bd10");
            if (currentWindow == null)
                return;
            System.Collections.Generic.List<Model.TestAPIModel> controls = APIManager.GetListControlText(currentWindow.hwnd);
            Model.TestAPIModel controlDuongThu = controls.Where(m => m.ClassName == "WindowsForms10.COMBOBOX.app.0.1e6fa8e").ToList()[2];
            Model.TestAPIModel controlChuyen = controls.Where(m => m.ClassName == "WindowsForms10.COMBOBOX.app.0.1e6fa8e").ToList()[1];
            Model.TestAPIModel controlBCNhan = controls.Where(m => m.ClassName == "WindowsForms10.COMBOBOX.app.0.1e6fa8e").ToList()[3];
            TestAPIModel editDuongThu = controls.Where(m => m.ClassName == "Edit").ToList()[2];
            TestAPIModel editChuyen = controls.Where(m => m.ClassName == "Edit").ToList()[1];
            TestAPIModel editBCNhan = controls.Where(m => m.ClassName == "Edit").ToList()[3];
            //const int CB_SETCURSEL = 0x014E;
            APIManager.FocusHandle(controlDuongThu.Handle);
            APIManager.setTextControl(editDuongThu.Handle, _taoBDAdd.DuongThu);
            //APIManager.SendMessage(controlDuongThu.Handle, CB_SETCURSEL, countDuongThu, 0);
            //SendKeys.SendWait("{TAB}");
            APIManager.setTextControl(editChuyen.Handle, _taoBDAdd.Chuyen1);
            //SendKeys.SendWait("{TAB}");
            APIManager.setTextControl(editBCNhan.Handle, _taoBDAdd.BCNhan);

            Thread.Sleep(200);
            SendKeys.SendWait("{TAB}");
            Thread.Sleep(2000);
            APIManager.setTextControl(editChuyen.Handle, _taoBDAdd.Chuyen1);
            //SendKeys.SendWait("{TAB}");
            APIManager.setTextControl(editBCNhan.Handle, _taoBDAdd.BCNhan);


            //APIManager.SendMessage(controlChuyen.Handle, CB_SETCURSEL, countChuyen, 0);
            //SendKeys.SendWait("{ENTER}");
            //APIManager.SendMessage(controlBCNhan.Handle, CB_SETCURSEL, 10, 0);
            //SendKeys.SendWait("{ENTER}");
        }

        private void ThuHep()
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "SmallRight" });
        }

        private void TimerTaoBD_Tick(object sender, EventArgs e)
        {
            if (isWaiting)
                return;
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;

            switch (stateTaoBd10)
            {
                case StateTaoBd10.DanhSachBD10:
                    if (currentWindow.text.IndexOf("danh sach bd10 di") != -1)
                    {
                        SendKeys.SendWait("{F1}");
                        stateTaoBd10 = StateTaoBd10.LapBD10;
                    }
                    break;

                case StateTaoBd10.LapBD10:
                    if (currentWindow.text.IndexOf("lap bd10") != -1)
                    {
                        isWaiting = true;
                        for (int i = 0; i < countDuongThu; i++)
                        {
                            SendKeys.SendWait("{DOWN}");
                            Thread.Sleep(50);
                        }
                        SendKeys.SendWait("^(c)");
                        Thread.Sleep(100);
                        string clip = Clipboard.GetText();
                        if (clip.IndexOf(tenDuongThu) == -1)
                        {
                            isWaiting = false;
                            timerTaoBD.Stop();
                        }
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(100);
                        for (int i = 0; i < countChuyen; i++)
                        {
                            SendKeys.SendWait("{DOWN}");
                            Thread.Sleep(50);
                        }
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(100);
                        SendKeys.SendWait(maBuuCuc);
                        Thread.Sleep(100);
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(100);
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(100);
                        isWaiting = false;
                        timerTaoBD.Stop();
                    }

                    break;

                default:
                    break;
            }
        }

        private void XeXaHoi()
        {
            maBuuCuc = "590100";
            tenDuongThu = "Tam Quan - Quy Nhơn (Xe XH)";
            countDuongThu = 11;
            countChuyen = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }
        public IRelayCommand<string> AddBDTinhCommand { get; }

        public IRelayCommand CopySHTuiCommand { get; }


        public ObservableCollection<HangHoaDetailModel> ListShowHangHoa
        {
            get { return _ListShowHangHoa; }
            set { SetProperty(ref _ListShowHangHoa, value); }
        }

        public string NameTinhCurrent
        {
            get { return _NameTinhCurrent; }
            set { SetProperty(ref _NameTinhCurrent, value); }
        }

        public string SelectedTime
        {
            get { return _SelectedTime; }
            set { SetProperty(ref _SelectedTime, value); }
        }

        public ICommand SelectedTinhCommand { get; }

        public HangHoaDetailModel SelectedTui
        {
            get { return _SelectedTui; }
            set
            {
                SetProperty(ref _SelectedTui, value);
                CopySHTuiCommand.NotifyCanExecuteChanged();
            }
        }

        public IRelayCommand<HangHoaDetailModel> SelectionCommand { get; }
        public string TextCurrentChuyenThu
        {
            get { return _TextCurrentChuyenThu; }
            set { SetProperty(ref _TextCurrentChuyenThu, value); }
        }

        public ICommand XeXaHoiCommand { get; }
        private readonly BackgroundWorker bwChiTiet;
        private readonly BackgroundWorker taoBDWorker;
        private readonly DispatcherTimer timerTaoBD;
        private ObservableCollection<HangHoaDetailModel> _ListShowHangHoa;
        private string _NameTinhCurrent;
        private string _SelectedTime = "0.5";
        private HangHoaDetailModel _SelectedTui;
        private string _TextCurrentChuyenThu;
        private int countChuyen = 0;
        private int countDuongThu = 0;
        private BuuCuc currentBuuCuc = BuuCuc.None;
        private List<HangHoaDetailModel> currentListHangHoa;
        private HangHoaDetailModel CurrentSelectedHangHoaDetail = null;
        private PhanLoaiTinh currentTinh = PhanLoaiTinh.None;
        private bool isWaiting = false;
        private string maBuuCuc = "0";
        private StateTaoBd10 stateTaoBd10;
        private string tenDuongThu = "";
    }
}