using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
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
        private ObservableCollection<HangHoaDetailModel> _ListShowHangHoa;
        private string _NAnNhon;
        private string _NBCP_HN;
        private string _NEMSDaNang;
        private string _NHA_AL;
        private string _NKienDaNang;
        private string _NKNTB;
        private string _NKT_HN;
        private string _NKT1;
        private string _NPhuCat;
        private string _NPhuMy;
        private string _NQuangNam;
        private string _NQuangNgai;
        private string _NTamQuan;
        private string _NTNTB;
        private string _NameTinhCurrent;
        private string _NConLai;
        private DispatcherTimer timer;

        private BuuCuc currentBuuCuc = BuuCuc.None;

        public string NConLai
        {
            get { return _NConLai; }
            set { SetProperty(ref _NConLai, value); }
        }


        public string NameTinhCurrent
        {
            get { return _NameTinhCurrent; }
            set { SetProperty(ref _NameTinhCurrent, value); }
        }

        private List<HangHoaDetailModel> currentListHangHoa;
        private string[] fillDaNang = new string[] { "26", "27", "23", "22", "55", "38", "40", "10", "48", "17", "18", "16", "31", "39", "24", "33", "42", "46", "43", "51", "20", "52", "36", "41", "25", "44", "31", "53", "30", "32", "29", "28", "35", "13" };
        private string[] fillNoiTinh = new string[] { "591218", "591520", "591720", "591760", "591900", "592100", "592120", "592220", "594080", "594090", "594210", "594220", "594300", "594350", "594560", "594610", "590100" };
        private string[] fillNTB = new string[] { "88", "79", "96", "93", "82", "83", "80", "97", "90", "63", "64", "81", "87", "60", "91", "70", "65", "92", "58", "67", "85", "66", "62", "95", "84", "86", "94", "89", "74" };

        private HangHoaDetailModel _SelectedTui;

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

        private string currentSHTui = "";
        void Selection(HangHoaDetailModel selected)
        {
            if (selected == null)
                return;
            currentSHTui = selected.TuiHangHoa.SHTui;
            selected.TrangThaiBD = TrangThaiBD.DaChon;

            switch (currentBuuCuc)
            {
                case BuuCuc.None:
                    return;
                case BuuCuc.KT:

                    if (!APIManager.ThoatToDefault("593230", "Quản lý chuyến thư chiều đến"))
                    {
                        SendKeys.SendWait("1");
                        Thread.Sleep(200);
                        SendKeys.SendWait("3");
                    }
                    break;
                case BuuCuc.BCP:
                    if (!APIManager.ThoatToDefault("593280", "Quản lý chuyến thư chiều đến"))
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
            timer.Start();
        }


        public IRelayCommand CopySHTuiCommand { get; }


        void CopySHTui()
        {
            //thuc hien lenh trong nay
            if (SelectedTui != null)
            {
                System.Windows.Clipboard.SetDataObject(SelectedTui.TuiHangHoa.SHTui);
                //SendMessage 
                WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = " Đã Copy" });
            }

        }


        public ChiTietViewModel()
        {

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
                if(m.Key == "navigation")
                {
                    PrintDiNgoai();
                }
            });

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;


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

        void PrintDiNgoai()
        {
            WindowInfo info = WaitingFindedWindow("thong tin buu gui", 1);
            if (info == null)
                return;

            SendKeys.SendWait("+{TAB}");
            SendKeys.SendWait("+{TAB}");
            SendKeys.SendWait("+{TAB}");
            SendKeys.SendWait("^(a)");
            Thread.Sleep(200);
            SendKeys.SendWait("^(c)");
            Thread.Sleep(200);
            string clipboard = System.Windows.Clipboard.GetText();
            Thread.Sleep(200);
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


            WindowInfo infoDocument = WaitingFindedWindow("document", 3);
            if (infoDocument == null)
                return;

            Thread.Sleep(600);
            List<IntPtr> childs = APIManager.GetAllChildHandles(infoDocument.hwnd);
            int countButton = 0;
            foreach (var item in childs)
            {
                string a = APIManager.GetWindowClass(item);

                if (a.IndexOf("WindowsForms10.BUTTON.app.0.1e6fa8e") != -1)
                {
                    countButton++;
                    if (countButton == 2)
                    {
                        SendKeys.SendWait("{RIGHT}");
                        SendKeys.SendWait(" ");
                        break;
                    }
                }
            }


            Thread.Sleep(200);
            SendKeys.SendWait("%(p)");


            WindowInfo infoPrintDocument = WaitingFindedWindow("print document", 3);
            if (infoPrintDocument == null) 
            return;


            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(50);
            SendKeys.SendWait(" ");

            WindowInfo infoThongTin = WaitingFindedWindow("thong tin buu gui", 3);
            if (infoThongTin == null)
                return;
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(100);
            SendKeys.SendWait(" ");
        }



        WindowInfo WaitingFindedWindow(string title, int time)
        {
            WindowInfo currentWindow = null;
            string titleWindow = "";
            time *= 5;
            while (titleWindow.IndexOf(title) == -1)
            {
                time--;
                if (time <= 0)
                    return null;

                Thread.Sleep(200);
                currentWindow = APIManager.GetActiveWindowTitle();
                if (currentWindow == null)
                    return null;

titleWindow = APIManager.convertToUnSign3(currentWindow.text).ToLower();

            }
            Thread.Sleep(100);
            return currentWindow;
        }


        bool isBusyXacNhan = false;
        int countDown = 0;
        private void Timer_Tick(object sender, System.EventArgs e)
        {
            if (isBusyXacNhan)
            {
                return;
            }
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;

            string textCo = APIManager.convertToUnSign3(currentWindow.text).ToLower();

            if (textCo.IndexOf("quan ly chuyen thu") != -1)
            {
                isBusyXacNhan = true;
                var childHandles3 = APIManager.GetAllChildHandles(currentWindow.hwnd);
                int countCombobox = 0;
                IntPtr combo = IntPtr.Zero;
                foreach (var item in childHandles3)
                {
                    string className = APIManager.GetWindowClass(item);
                    string classDefault = "WindowsForms10.COMBOBOX.app.0.1e6fa8e";
                    //string classDefault = "WindowsForms10.COMBOBOX.app.0.141b42a_r8_ad1";
                    if (className == classDefault)
                    {
                        if (countCombobox == 3)
                        {
                            //road = item;
                            combo = item;
                            break;
                        }
                        countCombobox++;
                    }
                }
                APIManager.SendMessage(combo, 0x0007, 0, 0);
                APIManager.SendMessage(combo, 0x0007, 0, 0);

                SendKeys.SendWait("{F3}");
                SendKeys.SendWait(currentSHTui);
                SendKeys.SendWait("{ENTER}");
                timer.Stop();
                isBusyXacNhan = false;


                //var handles = GetAllChildHandles(currentWindow.hwnd);
                //string textHandleName = "WindowsForms10.EDIT.app.0.1e6fa8e";
                //foreach (var item in handles)
                //{
                //    string classText = GetWindowClass(item);

                //    if (classText.IndexOf(textHandleName) != -1)
                //    {
                //        SendMessage(item, 0x0007, 0, 0);
                //        Thread.Sleep(1000);
                //        SendKeys.SendWait("chao");
                //        timerXacNhan.Stop();
                //        isBusyXacNhan = false;
                //        break;
                //    }
                //    //tim cai o cua sh tui
                //    //focus no
                //    //xong roi dien vao va nhan enter thoi
                //}

            }
            else
            {
                countDown--;
                if (countDown <= 0)
                {
                    countDown = 10;
                    isBusyXacNhan = false;
                    timer.Stop();
                }

            }
        }

        private void SelectedTinh(PhanLoaiTinh phanLoaiTinh)
        {
            var data = currentListHangHoa.FindAll(m => m.PhanLoai == phanLoaiTinh);
            if (data != null)
            {

                ListShowHangHoa = new ObservableCollection<HangHoaDetailModel>();
                foreach (HangHoaDetailModel hangHoa in data)
                {
                    ListShowHangHoa.Add(hangHoa);
                }
                //thuc hien show Ten Tinh
                ShowNameTinh(phanLoaiTinh);
            }
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
                    currentBuuCuc = BuuCuc.KT;
                    break;

                case PhanLoaiTinh.BCPHN:
                    textTemp = "Bưu Cục Phát Hoài Nhơn";
                    currentBuuCuc = BuuCuc.BCP;
                    break;

                default:
                    break;
            }
            NameTinhCurrent = textTemp;
        }

        public ICommand SelectedTinhCommand { get; }

        public ObservableCollection<HangHoaDetailModel> ListShowHangHoa
        {
            get { return _ListShowHangHoa; }
            set { SetProperty(ref _ListShowHangHoa, value); }
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
                        else if (APIManager.convertToUnSign3(hangHoa.TuiHangHoa.PhanLoai).ToLower().IndexOf("ngoai") != -1)
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
                        if (APIManager.convertToUnSign3(hangHoa.TuiHangHoa.PhanLoai).ToLower().IndexOf("ngoai") != -1)
                        {
                            currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.DiNgoaiNamTrungBo;
                        }
                    }
                    else
                    {
                        string temp1 = fillDaNang.FirstOrDefault(m => m == maSoTinh);
                        if (!string.IsNullOrEmpty(temp1))
                        {
                            if (hangHoa.TuiHangHoa.DichVu.IndexOf("Bưu") != -1 || hangHoa.TuiHangHoa.DichVu.IndexOf("Logi") != -1)
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
            NTamQuan = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.TamQuan).Count.ToString();
            NKienDaNang = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.KienDaNang).Count.ToString();
            NEMSDaNang = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.EMSDaNang).Count.ToString();
            NQuangNam = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.QuangNam).Count.ToString();
            NQuangNgai = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.QuangNgai).Count.ToString();
            NKNTB = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.DiNgoaiNamTrungBo).Count.ToString();
            NTNTB = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.TuiNTB).Count.ToString();
            NKT_HN = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.KTHN).Count.ToString();
            NBCP_HN = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.BCPHN).Count.ToString();
            NPhuMy = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.PhuMy).Count.ToString();
            NPhuCat = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.PhuCat).Count.ToString();
            NAnNhon = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.AnNhon).Count.ToString();
            NKT1 = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.KT1).Count.ToString();
            NConLai = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.None).Count.ToString();
        }
    }
}