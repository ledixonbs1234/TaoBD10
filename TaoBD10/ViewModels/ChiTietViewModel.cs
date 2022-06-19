using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    internal class ChiTietViewModel : ObservableObject
    {

        BackgroundWorker bwChiTiet;

        public RelayCommand<PhanLoaiTinh> AddBDTinhCommand { get; }


        void AddBDTinh(PhanLoaiTinh phanLoaiTinh)
        {
            switch (phanLoaiTinh)
            {
                case PhanLoaiTinh.None:
                    break;
                case PhanLoaiTinh.HA_AL:
                    break;
                case PhanLoaiTinh.TamQuan:
                    break;
                case PhanLoaiTinh.KienDaNang:
                    KienDaNang();
                    break;
                case PhanLoaiTinh.EMSDaNang:
                    EMSDaNang();
                    break;
                case PhanLoaiTinh.QuangNam:
                    QuangNam();
                    break;
                case PhanLoaiTinh.QuangNgai:
                    QuangNgai();
                    break;
                case PhanLoaiTinh.DiNgoaiNamTrungBo:
                    NamTrungBo();
                    break;
                case PhanLoaiTinh.TuiNTB:
                    NamTrungBo();
                    break;
                case PhanLoaiTinh.PhuMy:
                    PhuMy();
                    break;
                case PhanLoaiTinh.PhuCat:
                    PhuCat();
                    break;
                case PhanLoaiTinh.AnNhon:
                    AnNhon();
                    break;
                case PhanLoaiTinh.KT1:
                    NamTrungBo();
                    break;
                case PhanLoaiTinh.KTHN:
                    break;
                case PhanLoaiTinh.BCPHN:
                    break;
                default:
                    break;
            }

        }

        public ChiTietViewModel()
        {
            bwChiTiet = new BackgroundWorker();
            bwChiTiet.DoWork += BwChiTiet_DoWork;
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
                    FillData();
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
            AddBDTinhCommand = new RelayCommand<PhanLoaiTinh>(AddBDTinh);
            timerTaoBD = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 200)
            };
            timerTaoBD.Tick += TimerTaoBD_Tick;

            #region Command Create

            SelectedTinhCommand = new RelayCommand<PhanLoaiTinh>(SelectedTinh);

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

            #endregion Command Create
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

        private void FillData()
        {
            if (currentListHangHoa.Count == 0)
                return;
            //Thuc hien loc tung cai 1
            int countForeach = 0;

            foreach (var hangHoa in currentListHangHoa.ToList())
            {
                string maSoTinh = hangHoa.TuiHangHoa.ToBC.Substring(0, 2);
                if (hangHoa.TuiHangHoa.ToBC.IndexOf("593740") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("593630") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("593850") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("593880") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("593760") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("593870") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.HA_AL;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("593330") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.TamQuan;
                }
                else
               if (maSoTinh == "56")
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.QuangNam;
                }
                else if (maSoTinh == "57")
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.QuangNgai;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("592810") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("592850") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.PhuMy;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("592440") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("592460") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.PhuCat;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("592020") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("592040") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.AnNhon;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("590900") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.KT1;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("593230") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.KTHN;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("593280") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.BCPHN;
                }
                else if (maSoTinh == "59")
                {
                    string temp = fillNoiTinh.FirstOrDefault(m => m.IndexOf(hangHoa.TuiHangHoa.ToBC) != -1);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (hangHoa.TuiHangHoa.PhanLoai.IndexOf("Túi") != -1)
                        {
                            currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.TuiNTB;
                        }
                        else if (APIManager.ConvertToUnSign3(hangHoa.TuiHangHoa.PhanLoai).ToLower().IndexOf("ngoai") != -1)
                        {
                            currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.DiNgoaiNamTrungBo;
                        }
                    }
                }
                else
                {
                    string temp = fillNTB.FirstOrDefault(m => m == maSoTinh);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (APIManager.ConvertToUnSign3(hangHoa.TuiHangHoa.PhanLoai).ToLower().IndexOf("ngoai") != -1)
                        {
                            currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.DiNgoaiNamTrungBo;
                        }
                    }
                    else
                    {
                        string temp1 = fillDaNang.FirstOrDefault(m => m == maSoTinh);
                        if (!string.IsNullOrEmpty(temp1))
                        {
                            if (hangHoa.TuiHangHoa.DichVu.IndexOf("Parcel") != -1 || hangHoa.TuiHangHoa.DichVu.IndexOf("Logi") != -1)
                            {
                                currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.KienDaNang;
                            }
                            else if (hangHoa.TuiHangHoa.DichVu.IndexOf("EMS") != -1)
                            {
                                currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.EMSDaNang;
                            }
                        }
                    }
                }
                countForeach++;
            }
            ResetAndCount();
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
            //txtStateSend.Text = "Đang Gửi Trực Tiếp";
            double delayTime = Convert.ToDouble(SelectedTime);
            foreach (var hangHoa in ListShowHangHoa)
            {
                SendKeys.SendWait(hangHoa.TuiHangHoa.SHTui);
                SendKeys.SendWait("{ENTER}");

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
                    return;
                }
            }
        }

        private void KienDaNang()
        {
            maBuuCuc = "550910";
            tenDuongThu = "Bình Định - Đà Nẵng";
            countDuongThu = 4;
            countChuyen = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }

        private void NamTrungBo()
        {
            var time = DateTime.Now;
            if (time.Hour > 12)
                countChuyen = 2;
            else
                countChuyen = 1;

            maBuuCuc = "590100";
            tenDuongThu = "Đà Nẵng - Bình Định";
            countDuongThu = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }

        private void PhuCat()
        {
            var time = DateTime.Now;
            if (time.Hour > 12)
                countChuyen = 2;
            else
                countChuyen = 1;

            maBuuCuc = "592440";
            tenDuongThu = "Đà Nẵng - Bình Định";
            countDuongThu = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }

        private void PhuMy()
        {
            var time = DateTime.Now;
            if (time.Hour > 12)
                countChuyen = 2;
            else
                countChuyen = 1;

            maBuuCuc = "592810";
            tenDuongThu = "Đà Nẵng - Bình Định";
            countDuongThu = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }
        private string _TestText;

        public string TestText
        {
            get { return _TestText; }
            set { SetProperty(ref _TestText, value); }
        }


        private void PrintDiNgoai()
        {
            WindowInfo info = APIManager.WaitingFindedWindow("thong tin buu gui");
            if (info == null)
                return;
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

            SendKeys.SendWait("^{TAB}");
            Thread.Sleep(50);
            SendKeys.SendWait(" ");

            WindowInfo printD = new WindowInfo();
            while (printD.text.IndexOf("print document") == -1)
            {
                printD = APIManager.GetActiveWindowTitle();
                if (printD == null)
                {
                    APIManager.ShowSnackbar("Null Title");
                    return;
                }
                TestText += printD.text + "\n";
                Thread.Sleep(100);
            }
            Thread.Sleep(200);
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait(" ");

            WindowInfo infoPrint = APIManager.WaitingFindedWindow("Print", isExactly: true);
            if (infoPrint == null)
                return;

            SendKeys.SendWait("%(p)");

            WindowInfo infoPrintDocument = APIManager.WaitingFindedWindow("Print Document", isExactly: true);
            if (infoPrintDocument == null)
                return;
            WindowInfo printD1 = new WindowInfo();
            while (printD1.text.IndexOf("printing") == -1)
            {
                printD1 = APIManager.GetActiveWindowTitle();
                Thread.Sleep(100);
            }
            while (printD1.text.IndexOf("printing") != -1)
            {
                printD1 = APIManager.GetActiveWindowTitle();
                Thread.Sleep(100);
            }
            if (CurrentSelectedHangHoaDetail != null)
            {
                CurrentSelectedHangHoaDetail.TrangThaiBD = TrangThaiBD.DaIn;
            }

            Thread.Sleep(200);
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(50);
            SendKeys.SendWait(" ");

            WindowInfo infoThongTin = APIManager.WaitingFindedWindow("thong tin buu gui");
            if (infoThongTin == null)
                return;
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(100);
            SendKeys.SendWait(" ");
        }

        private void QuangNam()
        {
            maBuuCuc = "560100";
            tenDuongThu = "Bình Định - Đà Nẵng";
            countDuongThu = 4;
            countChuyen = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }

        private void QuangNgai()
        {
            maBuuCuc = "570100";
            tenDuongThu = "Bình Định - Đà Nẵng";
            countDuongThu = 4;
            countChuyen = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }

        private void ResetAndCount()
        {
            NHA_AL = "0";
            NTamQuan = "0";
            NKienDaNang = "0";
            NEMSDaNang = "0";
            NQuangNam = "0";
            NQuangNgai = "0";
            NKNTB = "0";
            NTNTB = "0";
            NKT_HN = "0";
            NBCP_HN = "0";
            NPhuMy = "0";
            NPhuCat = "0";
            NAnNhon = "0";
            NKT1 = "0";
            NConLai = "0";

            NHA_AL = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.HA_AL).Count.ToString();
            if (NHA_AL != "0") IsEnHAAL = true; else IsEnHAAL = false;
            NTamQuan = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.TamQuan).Count.ToString();
            if (NTamQuan != "0") IsEnTamQuan = true; else IsEnTamQuan = false;
            NKienDaNang = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.KienDaNang).Count.ToString();
            if (NKienDaNang != "0") IsEnKDN = true; else IsEnKDN = false;
            NEMSDaNang = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.EMSDaNang).Count.ToString();
            if (NEMSDaNang != "0") IsEnEDN = true; else IsEnEDN = false;
            NQuangNam = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.QuangNam).Count.ToString();
            if (NQuangNam != "0") IsEnQNam = true; else IsEnQNam = false;
            NQuangNgai = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.QuangNgai).Count.ToString();
            if (NQuangNgai != "0") IsEnQNgai = true; else IsEnQNgai = false;
            NKNTB = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.DiNgoaiNamTrungBo).Count.ToString();
            if (NKNTB != "0") IsEnKNTB = true; else IsEnKNTB = false;
            NTNTB = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.TuiNTB).Count.ToString();
            if (NTNTB != "0") IsEnTNTB = true; else IsEnTNTB = false;
            NKT_HN = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.KTHN).Count.ToString();
            NBCP_HN = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.BCPHN).Count.ToString();
            NPhuMy = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.PhuMy).Count.ToString();
            if (NPhuMy != "0") IsEnPM = true; else IsEnPM = false;
            NPhuCat = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.PhuCat).Count.ToString();
            if (NPhuCat != "0") IsEnPC = true; else IsEnPC = false;
            NAnNhon = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.AnNhon).Count.ToString();
            if (NAnNhon != "0") IsEnAN = true; else IsEnAN = false;
            NKT1 = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.KT1).Count.ToString();
            if (NKT1 != "0") IsEnKT1 = true; else IsEnKT1 = false;
            NConLai = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.None).Count.ToString();
            if (NConLai != "0") IsEnConLai = true; else IsEnConLai = false;
        }

        private void SelectedTinh(PhanLoaiTinh phanLoaiTinh)
        {
            if (currentListHangHoa == null)
            {
                return;
            }
            var data = currentListHangHoa.FindAll(m => m.PhanLoai == phanLoaiTinh);
            if (data != null)
            {
                currentTinh = phanLoaiTinh;

                ListShowHangHoa = new ObservableCollection<HangHoaDetailModel>();

                foreach (HangHoaDetailModel hangHoa in data)
                {
                    if (phanLoaiTinh == PhanLoaiTinh.KTHN || phanLoaiTinh == PhanLoaiTinh.BCPHN)
                    {
                        string temp = APIManager.ConvertToUnSign3(hangHoa.TuiHangHoa.DichVu).ToLower();
                        string temp1 = APIManager.ConvertToUnSign3(hangHoa.TuiHangHoa.PhanLoai).ToLower();
                        if (temp.IndexOf("phat hanh") != -1 || temp1.IndexOf("tui") != -1)
                        {
                            continue;
                        }
                    }
                    ListShowHangHoa.Add(hangHoa);
                }
                //thuc hien show Ten Tinh
                ShowNameTinh(phanLoaiTinh);
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

                    if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
                    {
                        SendKeys.SendWait("1");
                        Thread.Sleep(200);
                        SendKeys.SendWait("3");
                    }
                    break;

                case BuuCuc.BCP:
                    if (!APIManager.ThoatToDefault("593280", "quan ly chuyen thu chieu den"))
                    {
                        SendKeys.SendWait("1");
                        Thread.Sleep(200);
                        SendKeys.SendWait("3");
                    }
                    break;

                default:
                    break;
            }
            countDown = 60;
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

        public IRelayCommand CopySHTuiCommand { get; }
        public bool IsEnAN
        {
            get { return _IsEnAN; }
            set { SetProperty(ref _IsEnAN, value); }
        }

        public bool IsEnConLai
        {
            get { return _IsEnConLai; }
            set { SetProperty(ref _IsEnConLai, value); }
        }

        public bool IsEnEDN
        {
            get { return _IsEnEDN; }
            set { SetProperty(ref _IsEnEDN, value); }
        }

        public bool IsEnHAAL
        {
            get { return _IsEnHAAL; }
            set { SetProperty(ref _IsEnHAAL, value); }
        }

        public bool IsEnKDN
        {
            get { return _IsEnKDN; }
            set { SetProperty(ref _IsEnKDN, value); }
        }

        public bool IsEnKNTB
        {
            get { return _IsEnKNTB; }
            set { SetProperty(ref _IsEnKNTB, value); }
        }

        public bool IsEnKT1
        {
            get { return _IsEnKT1; }
            set { SetProperty(ref _IsEnKT1, value); }
        }

        public bool IsEnPC
        {
            get { return _IsEnPC; }
            set { SetProperty(ref _IsEnPC, value); }
        }

        public bool IsEnPM
        {
            get { return _IsEnPM; }
            set { SetProperty(ref _IsEnPM, value); }
        }

        public bool IsEnQNam
        {
            get { return _IsEnQNam; }
            set { SetProperty(ref _IsEnQNam, value); }
        }

        public bool IsEnQNgai
        {
            get { return _IsEnQNgai; }
            set { SetProperty(ref _IsEnQNgai, value); }
        }

        public bool IsEnTamQuan
        {
            get { return _IsEnTamQuan; }
            set { SetProperty(ref _IsEnTamQuan, value); }
        }

        public bool IsEnTNTB
        {
            get { return _IsEnTNTB; }
            set { SetProperty(ref _IsEnTNTB, value); }
        }

        public bool IsTaoBD
        {
            get { return _IsTaoBD; }
            set { SetProperty(ref _IsTaoBD, value); }
        }

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

        public string NAnNhon
        {
            get { return _NAnNhon; }
            set { SetProperty(ref _NAnNhon, value); }
        }

        public string NBCP_HN
        {
            get { return _NBCP_HN; }
            set { SetProperty(ref _NBCP_HN, value); }
        }

        public string NConLai
        {
            get { return _NConLai; }
            set { SetProperty(ref _NConLai, value); }
        }

        public string NEMSDaNang
        {
            get { return _NEMSDaNang; }
            set { SetProperty(ref _NEMSDaNang, value); }
        }

        public string NHA_AL
        {
            get { return _NHA_AL; }
            set { SetProperty(ref _NHA_AL, value); }
        }

        public string NKienDaNang
        {
            get { return _NKienDaNang; }
            set { SetProperty(ref _NKienDaNang, value); }
        }

        public string NKNTB
        {
            get { return _NKNTB; }
            set { SetProperty(ref _NKNTB, value); }
        }

        public string NKT_HN
        {
            get { return _NKT_HN; }
            set { SetProperty(ref _NKT_HN, value); }
        }

        public string NKT1
        {
            get { return _NKT1; }
            set { SetProperty(ref _NKT1, value); }
        }

        public string NPhuCat
        {
            get { return _NPhuCat; }
            set { SetProperty(ref _NPhuCat, value); }
        }

        public string NPhuMy
        {
            get { return _NPhuMy; }
            set { SetProperty(ref _NPhuMy, value); }
        }

        public string NQuangNam
        {
            get { return _NQuangNam; }
            set { SetProperty(ref _NQuangNam, value); }
        }

        public string NQuangNgai
        {
            get { return _NQuangNgai; }
            set { SetProperty(ref _NQuangNgai, value); }
        }

        public string NTamQuan
        {
            get { return _NTamQuan; }
            set { SetProperty(ref _NTamQuan, value); }
        }

        public string NTNTB
        {
            get { return _NTNTB; }
            set { SetProperty(ref _NTNTB, value); }
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
        private bool _IsEnAN = true;
        private bool _IsEnConLai = true;
        private bool _IsEnEDN = true;
        private bool _IsEnHAAL = true;
        private bool _IsEnKDN = true;
        private bool _IsEnKNTB = true;
        private bool _IsEnKT1 = true;
        private bool _IsEnPC = true;
        private bool _IsEnPM = true;
        private bool _IsEnQNam = true;
        private bool _IsEnQNgai = true;
        private bool _IsEnTamQuan = true;
        private bool _IsEnTNTB = true;
        private bool _IsTaoBD = false;
        private ObservableCollection<HangHoaDetailModel> _ListShowHangHoa;
        private string _NameTinhCurrent;
        private string _NAnNhon = "0";
        private string _NBCP_HN = "0";
        private string _NConLai = "0";
        private string _NEMSDaNang = "0";
        private string _NHA_AL = "0";
        private string _NKienDaNang = "0";
        private string _NKNTB = "0";
        private string _NKT_HN = "0";
        private string _NKT1 = "0";
        private string _NPhuCat = "0";
        private string _NPhuMy = "0";
        private string _NQuangNam = "0";
        private string _NQuangNgai = "0";
        private string _NTamQuan = "0";
        private string _NTNTB = "0";
        private string _SelectedTime = "0.5";
        private HangHoaDetailModel _SelectedTui;
        private string _TextCurrentChuyenThu;
        private int countChuyen = 0;
        private int countDown = 0;
        private int countDuongThu = 0;
        private BuuCuc currentBuuCuc = BuuCuc.None;
        private List<HangHoaDetailModel> currentListHangHoa;
        private HangHoaDetailModel CurrentSelectedHangHoaDetail = null;
        private PhanLoaiTinh currentTinh = PhanLoaiTinh.None;
        private readonly string[] fillDaNang = new string[] { "26", "27", "23", "22", "55", "38", "40", "10", "48", "17", "18", "16", "31", "39", "24", "33", "42", "46", "43", "51", "20", "52", "36", "41", "25", "44", "31", "53", "30", "32", "29", "28", "35", "13" };
        private readonly string[] fillNoiTinh = new string[] { "591218", "591520", "591720", "591760", "591900", "592100", "592120", "592220", "594080", "594090", "594210", "594220", "594300", "594350", "594560", "594610", "590100" };
        private readonly string[] fillNTB = new string[] { "88", "79", "96", "93", "82", "83", "80", "97", "90", "63", "64", "81", "87", "60", "91", "70", "65", "92", "58", "67", "85", "66", "62", "95", "84", "86", "94", "89", "74" };
        private bool isBusyXacNhan = false;
        private bool isWaiting = false;
        private string maBuuCuc = "0";
        private StateTaoBd10 stateTaoBd10;
        private string tenDuongThu = "";
        private readonly DispatcherTimer timer;
        private readonly DispatcherTimer timerTaoBD;
    }
}