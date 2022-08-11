using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;
using TaoBD10.Views;

namespace TaoBD10.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private bool Is16Kg = false;
        public IRelayCommand<Window> CloseWindowCommand { get; }

        private bool _IsTopMost = true;

        private bool _IsXacNhanChiTieting = false;

        int lastNumberSuaBD = 0;

        public bool IsTopMost
        {
            get { return _IsTopMost; }
            set { SetProperty(ref _IsTopMost, value); }
        }

        private readonly BackgroundWorker backgroundWorkerRead;

        private bool _IsAutoF4 = false;

        public bool IsAutoF4
        {
            get { return _IsAutoF4; }
            set { SetProperty(ref _IsAutoF4, value); }
        }

        private void CloseWindow(Window window)
        {
            window.Close();
        }
        BackgroundWorker bwprintMaVach;
        List<FindItemModel> listFindItem;
        DispatcherTimer timer;

        public MainViewModel()
        {
            //Thuc hien Qua trinh su dung may
            //FileManager.onSetupFileManager();
            //FileManager.onSetupFileManager();
            bwprintMaVach = new BackgroundWorker();
            bwprintMaVach.DoWork += BwprintMaVach_DoWork;
            printTrangCuoi = new BackgroundWorker();
            printTrangCuoi.DoWork += PrintTrangCuoi_DoWork;
            LoadPageCommand = new RelayCommand<Window>(LoadPage);
            TabChangedCommand = new RelayCommand<System.Windows.Controls.TabControl>(TabChanged);
            OnCloseWindowCommand = new RelayCommand(OnCloseWindow);
            SmallerWindowCommand = new RelayCommand(SmallerWindow);
            DefaultWindowCommand = new RelayCommand<System.Windows.Controls.TabControl>(DefaultWindow);
            ToggleWindowCommand = new RelayCommand(ToggleWindow);
            TabTuiChangedCommand = new RelayCommand<System.Windows.Controls.TabControl>(TabTuiChanged);
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow);
            backgroundWorkerRead = new BackgroundWorker();
            backgroundWorkerRead.DoWork += BackgroundWorkerRead_DoWork;
            backgroundWorkerRead.RunWorkerAsync();
            bwPrintBD10 = new BackgroundWorker();
            bwPrintBD10.DoWork += BwPrintBD10_DoWork;
            bwPrintBanKe = new BackgroundWorker();
            bwPrintBanKe.DoWork += BwPrintBanKe_DoWork;
            bwRunPrints = new BackgroundWorker();
            bwRunPrints.WorkerSupportsCancellation = true;
            bwRunPrints.DoWork += BwRunPrints_DoWork;
            bwRunPrints.RunWorkerCompleted += BwRunPrints_RunWorkerCompleted;

            OptionViewCommand = new RelayCommand(OptionView);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 20, 0);
            timer.Tick += Timer_Tick;
            timer.Start();
            _keyboardHook = new Y2KeyboardHook();
            _keyboardHook.OnKeyPressed += OnKeyPress;
            _keyboardHook.HookKeyboard();
            SoundManager.SetUpDirectory();
            listFindItem = new List<FindItemModel>();
            FileManager.onSetupFileManager();


            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "Navigation")
                {
                    if (m.Content == "GoChiTiet")
                    {
                        IndexTabControl = 2;
                    }
                    else if (m.Content == "Center")
                    {
                        SetChiTietWindow();
                    }
                    else if (m.Content == "SmallRight")
                    {
                        SetRightHeigtTuiWindow();
                    }
                    else if (m.Content == "Web")
                    {
                        IndexTabTui = 1;
                    }
                    else if (m.Content == "GoTaoTui")
                    {
                        IndexTabTui = 6;
                    }
                    else if (m.Content == "TamQuan")
                    {
                        IndexTabTui = 8;
                        IndexTabControl = 5;
                    }
                }
                else if (m.Key == "Snackbar")
                {
                    MessageShow(m.Content);
                }
                else if (m.Key == "SetFalseKg")
                {
                    Is16Kg = false;
                }
                else if (m.Key == "TopMost")
                {
                    if (m.Content == "False")
                    {
                        IsTopMost = false;
                    }
                    else
                    {
                        IsTopMost = true;
                    }
                }
                else if (m.Key == "XacNhanChiTiet")
                {
                    if (m.Content == "True")
                    {
                        _IsXacNhanChiTieting = true;
                    }
                    else
                    {
                        _IsXacNhanChiTieting = false;
                    }
                }
            });

            WeakReferenceMessenger.Default.Register<WebContentModel>(this, (r, m) =>
            {
                if (m.Key == "AddressDongChuyenThu")
                {
                    string boDauAddress = LocHuyen(m.AddressReiceive);

                    if (maSoBuuCucCurrent == "590100")
                    {
                        if (loaiCurrent == "E" || loaiCurrent == "C")
                        {
                            if (m.BuuCucPhat == "59")
                            {
                                if (boDauAddress == "hoai nhon" && m.BuuCucGui != "59")
                                {

                                }
                                else
                                {
                                    SoundManager.playSound3(@"Number\error_sound.wav");
                                }
                            }
                        }
                        else if (loaiCurrent == "R")
                        {
                            if (m.BuuCucPhat == "59")
                            {
                                if (IsBoQuaHuyen)
                                {
                                    if (boDauAddress.IndexOf("van canh") != -1
                                    || boDauAddress.IndexOf("vinh thanh") != -1
                                    || boDauAddress.IndexOf("tay son") != -1
                                    || boDauAddress.IndexOf("phu my") != -1
                                    || boDauAddress.IndexOf("phu cat") != -1
                                    || boDauAddress.IndexOf("an nhon") != -1
                                    || boDauAddress.IndexOf("tuy phuoc") != -1
                                    || boDauAddress.IndexOf("quy nhon") != -1)
                                    {
                                    }
                                    else
                                    {
                                        SoundManager.playSound3(@"Number\error_sound.wav");
                                    }
                                }
                                else
                                {
                                    if (boDauAddress.IndexOf("van canh") != -1
                                    || boDauAddress.IndexOf("vinh thanh") != -1
                                    || boDauAddress.IndexOf("tay son") != -1
                                    || boDauAddress.IndexOf("tuy phuoc") != -1
                                    || boDauAddress.IndexOf("quy nhon") != -1)
                                    {
                                    }
                                    else
                                    {
                                        SoundManager.playSound3(@"Number\error_sound.wav");
                                    }
                                }
                            }
                            else
                            {
                                string fisrtchar = m.Code.Substring(0, 1).ToLower();
                                if (fisrtchar == "c" || fisrtchar == "e")
                                {
                                    SoundManager.playSound3(@"Number\error_sound.wav");
                                }
                            }
                        }
                    }
                    else if (maSoBuuCucCurrent == "592810")
                    {
                        if (boDauAddress.IndexOf("phu my") == -1)
                            SoundManager.playSound3(@"Number\error_sound.wav");
                    }
                    else if (maSoBuuCucCurrent == "592460")
                    {
                        if (boDauAddress.IndexOf("phu cat") == -1)
                            SoundManager.playSound3(@"Number\error_sound.wav");
                    }
                    else if (maSoBuuCucCurrent == "592020")
                    {
                        if (boDauAddress.IndexOf("an nhon") == -1)
                            SoundManager.playSound3(@"Number\error_sound.wav");
                    }
                    else if (maSoBuuCucCurrent == "591520" || maSoBuuCucCurrent == "591218")
                    {
                        if (boDauAddress.IndexOf("quy nhon") == -1)
                            SoundManager.playSound3(@"Number\error_sound.wav");
                    }
                    else if (maSoBuuCucCurrent == "593280")
                    {
                        if (boDauAddress.IndexOf("hoai nhon") == -1)
                            SoundManager.playSound3(@"Number\error_sound.wav");
                    }
                    else if (maSoBuuCucCurrent == "593330")
                    {
                        if (boDauAddress.IndexOf("hoai nhon") == -1)
                            SoundManager.playSound3(@"Number\error_sound.wav");
                    }
                    else if (maSoBuuCucCurrent == "593740" || maSoBuuCucCurrent == "593630")
                    {
                        if (boDauAddress.IndexOf("hoai an") == -1)
                            SoundManager.playSound3(@"Number\error_sound.wav");
                    }
                    else if (maSoBuuCucCurrent == "593850" || maSoBuuCucCurrent == "593880")
                    {
                        if (boDauAddress.IndexOf("an lao") == -1)
                            SoundManager.playSound3(@"Number\error_sound.wav");
                    }
                    else if (maSoBuuCucCurrent == "59G012")
                    {
                        string address = APIManager.BoDauAndToLower(m.AddressReiceive);
                        if (address.IndexOf("hoai my") != -1 ||
                        address.IndexOf("hoai huong") != -1 ||
                         address.IndexOf("hoai duc") != -1 ||
                          address.IndexOf("hoai xuan") != -1 ||
                           address.IndexOf("bong son") != -1 ||
                            address.IndexOf("hoai tan") != -1
                        )
                        {
                            SoundManager.playSound3(@"Number\error_sound.wav");
                        }
                    }
                    else if (maSoBuuCucCurrent == "59G003" || maSoBuuCucCurrent == "59G010")
                    {
                        string address = APIManager.BoDauAndToLower(m.AddressReiceive);
                        if (address.IndexOf("hoai my") != -1 ||
                        address.IndexOf("hoai hai") != -1 ||
                         address.IndexOf("hoai duc") != -1 ||
                          address.IndexOf("hoai xuan") != -1 ||
                           address.IndexOf("bong son") != -1 ||
                            address.IndexOf("hoai tan") != -1
                        )
                        {
                            SoundManager.playSound3(@"Number\error_sound.wav");
                        }
                    }
                    else if (maSoBuuCucCurrent == "59G011")
                    {
                        string address = APIManager.BoDauAndToLower(m.AddressReiceive);
                        if (address.IndexOf("hoai hai") != -1 ||
                        address.IndexOf("hoai huong") != -1 ||
                         address.IndexOf("hoai duc") != -1 ||
                          address.IndexOf("hoai xuan") != -1 ||
                           address.IndexOf("bong son") != -1 ||
                            address.IndexOf("hoai tan") != -1
                        )
                        {
                            SoundManager.playSound3(@"Number\error_sound.wav");
                        }
                    }
                }
            });
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (listFindItem.Count > 0)
            {
                List<FindItemModel> data = FileManager.LoadFindItemOnFirebase();
                if (data == null)
                {
                    data = new List<FindItemModel>();
                }
                data.AddRange(listFindItem);
                FileManager.SaveFindItemFirebase(data);
                listFindItem.Clear();
            }
        }

        private void BwprintMaVach_DoWork(object sender, DoWorkEventArgs e)
        {
            WindowInfo currentWindow = APIManager.WaitingFindedWindow("dong chuyen thu");
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window quan ly chuyen thu ");
                return;
            }

            APIManager.ClickButton(currentWindow.hwnd, "f7", isExactly: false);

            currentWindow = APIManager.WaitingFindedWindow("in an pham");
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window in an pham");
                return;
            }
            SendKeys.SendWait("{UP}");
            Thread.Sleep(50);
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(50);
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(50);
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(50);
            SendKeys.SendWait("{DOWN}");
            Thread.Sleep(50);
            SendKeys.SendWait("{DOWN}");
            Thread.Sleep(50);
            SendKeys.SendWait("{DOWN}");
            Thread.Sleep(50);
            SendKeys.SendWait(" ");
            Thread.Sleep(200);

            APIManager.ClickButton(currentWindow.hwnd, "f10", isExactly: false);

            currentWindow = APIManager.WaitingFindedWindow("hinh thuc sap xep");
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window ");
                return;
            }

            APIManager.ClickButton(currentWindow.hwnd, "f10", isExactly: false);


            printBanKeFromPrintDocument();
        }

        string LocHuyen(string address)
        {
            List<string> fillAddress = address.Split('-').Select(s => s.Trim()).ToList();
            if (fillAddress == null)
                return "";
            if (fillAddress.Count < 3)
                return "";
            string addressExactly = fillAddress[fillAddress.Count - 2];
            return APIManager.BoDauAndToLower(addressExactly);
        }

        private void PrintTrangCuoi_DoWork(object sender, DoWorkEventArgs e)
        {
            WindowInfo currentWindow = APIManager.WaitingFindedWindow("quan ly chuyen thu chieu di");
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window quan ly chuyen thu ");
                return;
            }

            APIManager.ClickButton(currentWindow.hwnd, "f7", isExactly: false);

            currentWindow = APIManager.WaitingFindedWindow("in an pham");
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window in an pham");
                return;
            }
            SendKeys.SendWait("{UP}");
            Thread.Sleep(50);
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(50);
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(50);
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(50);
            SendKeys.SendWait("{DOWN}");
            Thread.Sleep(50);
            SendKeys.SendWait("{DOWN}");
            Thread.Sleep(50);
            SendKeys.SendWait("{DOWN}");
            Thread.Sleep(50);
            SendKeys.SendWait(" ");
            Thread.Sleep(200);

            APIManager.ClickButton(currentWindow.hwnd, "f10", isExactly: false);

            currentWindow = APIManager.WaitingFindedWindow("hinh thuc sap xep");
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window ");
                return;
            }

            APIManager.ClickButton(currentWindow.hwnd, "f10", isExactly: false);



            currentWindow = APIManager.WaitingFindedWindow("print document");
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window print document");
                return;
            }
            Thread.Sleep(1500);

            APIManager.ClickButton(currentWindow.hwnd, "in an pham", isExactly: false);

            currentWindow = APIManager.WaitingFindedWindow("Print", isExactly: true);
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window print document");
                return;
            }
            List<TestAPIModel> controls = APIManager.GetListControlText(currentWindow.hwnd);
            TestAPIModel editControl = controls.Where(m => m.ClassName == "Edit").ToList()[3];
            string control = editControl.Text.Split('-')[1];

            string newText = control + "-" + control;
            APIManager.SendMessage(editControl.Handle, (int)0x000C, IntPtr.Zero, new StringBuilder(newText));

            SendKeys.SendWait("%{p}");
        }

        private void BwRunPrints_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsAutoF4)
            {
                if (isRunnedPrintBD)
                {
                    WindowInfo window = APIManager.WaitingFindedWindow("danh sach bd10 di");
                    if (window == null)
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    bwRunPrints.RunWorkerAsync();
                }
            }
        }

        private bool isRunnedPrintBD = false;

        private void BwRunPrints_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                isRunnedPrintBD = false;
                string lastcopy = "";
                string data = "null";
                APIManager.ClearClipboard();

                data = APIManager.GetCopyData();
                while (lastcopy != data)
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        return;
                    }
                    lastcopy = data;
                    //550910-VCKV - Đà Nẵng LT	08/06/2022	1	Ô tô	21	206,4	Đã đi
                    //590100-VCKV Nam Trung Bộ	08/06/2022	2	Ô tô	50	456,1	Khởi tạo
                    if ((data.IndexOf("550910") != -1
                        || data.IndexOf("550915") != -1
                        || data.IndexOf("590100") != -1
                        || data.IndexOf("592020") != -1
                        || data.IndexOf("592440") != -1
                        || data.IndexOf("592810") != -1
                        || data.IndexOf("560100") != -1
                        || data.IndexOf("570100") != -1)
                        && data.IndexOf("Khởi tạo") != -1)
                    {
                        isRunnedPrintBD = true;
                        WindowInfo window = APIManager.WaitingFindedWindow("danh sach bd10 di");
                        if (window == null)
                        {
                            return;
                        }
                        APIManager.ClickButton(window.hwnd, "Sửa");
                        window = APIManager.WaitingFindedWindow("sua thong tin bd10");
                        if (window == null)
                        {
                            return;
                        }
                        Thread.Sleep(500);
                        SendKeys.SendWait("{F6}");
                    }
                    else
                    {
                        SendKeys.SendWait("{DOWN}");
                        Thread.Sleep(100);
                        data = APIManager.GetCopyData();
                    }
                }
                APIManager.ShowSnackbar("Run print list bd 10 complete");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "MainViewModel " + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
            }
        }

        private void BwPrintBD10_DoWork(object sender, DoWorkEventArgs e)
        {
            WindowInfo currentWindow = APIManager.WaitingFindedWindow("sua thong tin bd10", "lap bd10");
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window bd 10");
                return;
            }

            APIManager.SetPrintBD10();
            SendKeys.SendWait("{F3}");
            Thread.Sleep(50);
            WindowInfo cuaSo = APIManager.WaitingFindedWindow("sua thong tin bd10", "lap bd10");
            while (currentWindow.hwnd == cuaSo.hwnd)
            {
                Thread.Sleep(50);
                cuaSo = APIManager.WaitingFindedWindow("sua thong tin bd10", "lap bd10");
            }
            APIManager.ClickButton(cuaSo.hwnd, "yes", isExactly: false);

            currentWindow = APIManager.WaitingFindedWindow("print document");
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window bd 10");
                return;
            }
            Thread.Sleep(500);
            APIManager.ClickButton(currentWindow.hwnd, "in an pham", isExactly: false);
            WindowInfo printWindow = APIManager.WaitingFindedWindow("Print", isExactly: true);
            //if (printWindow == null)
            //{
            //    MessageShow("Không tìm thấy window print document");
            //    return;
            //}
            //SendKeys.SendWait("%{r}");
            //currentWindow = APIManager.WaitingFindedWindow("printing preferences");

            //if (currentWindow == null)
            //{
            //    MessageShow("Không tìm thấy window print pre");
            //    return;
            //}

            //Thread.Sleep(50);
            //SendKeys.SendWait("{DOWN}");
            //Thread.Sleep(100);
            //APIManager.ClickButton(currentWindow.hwnd, "ok", isExactly: false);

            //currentWindow = APIManager.WaitingFindedWindow("Print", isExactly: true);
            //if (currentWindow == null)
            //{
            //    MessageShow("Không tìm thấy window print document");
            //    return;
            //}
            SendKeys.SendWait("%{c}");
            Thread.Sleep(50);
            SendKeys.SendWait("{Up}");
            if (_DefaultNumberPagePrint == 3)
            {
                Thread.Sleep(50);
                SendKeys.SendWait("{Up}");
            }
            Thread.Sleep(50);
            SendKeys.SendWait("%{o}");
            Thread.Sleep(50);
            SendKeys.SendWait("%{p}");
            Thread.Sleep(500);

            currentWindow = APIManager.WaitingFindedWindow("print document");
            if (currentWindow == null)
                return;
            APIManager.ClickButton(currentWindow.hwnd, "thoat", isExactly: false);
            APIManager.WaitingFindedWindow("sua thong tin bd10", "lap bd10");

            if (IsAutoF4)
            {
                WindowInfo window = APIManager.WaitingFindedWindow("danh sach bd10 di");
                if (window == null)
                {
                    return;
                }
                Thread.Sleep(500);
                SendKeys.SendWait("{F3}");
            }
        }

        void printBanKeFromPrintDocument()
        {
            WindowInfo currentWindow = APIManager.WaitingFindedWindow("print document");
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window print document");
                return;
            }

            Thread.Sleep(200);
            APIManager.ClickButton(currentWindow.hwnd, "in an pham", isExactly: false);

            currentWindow = APIManager.WaitingFindedWindow("Print", isExactly: true);
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window print document");
                return;
            }
            SendKeys.SendWait("%{r}");
            currentWindow = APIManager.WaitingFindedWindow("printing preferences");

            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window print pre");
                return;
            }

            Thread.Sleep(50);
            SendKeys.SendWait("%{u}");
            //SendKeys.SendWait("{DOWN}");
            //Thread.Sleep(100);
            APIManager.ClickButton(currentWindow.hwnd, "ok", isExactly: false);

            currentWindow = APIManager.WaitingFindedWindow("Print", isExactly: true);
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window print document");
                return;
            }
            SendKeys.SendWait("%{p}");

            Thread.Sleep(300);
            currentWindow = APIManager.WaitingFindedWindow("print document");
            if (currentWindow == null)
            {
                MessageShow("Không tìm thấy window print document");
                return;
            }

            APIManager.ClickButton(currentWindow.hwnd, "thoat", isExactly: false);

        }

        private void BwPrintBanKe_DoWork(object sender, DoWorkEventArgs e)
        {
            WindowInfo currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow.text.IndexOf("dong chuyen thu") == -1)
            {
                return;
            }
            APIManager.ClickButton(currentWindow.hwnd, "in an pham", isExactly: false);
            currentWindow = APIManager.WaitingFindedWindow("in an pham");
            if (currentWindow == null)
                return;

            SendKeys.SendWait("{UP}");
            SendKeys.SendWait(" ");
            SendKeys.SendWait("{DOWN}");
            SendKeys.SendWait(" ");

            APIManager.ClickButton(currentWindow.hwnd, "in an pham", isExactly: false);
            //Hình thức
            currentWindow = APIManager.WaitingFindedWindow("hinh thuc");
            if (currentWindow == null)
                return;
            APIManager.ClickButton(currentWindow.hwnd, "chap nhan", isExactly: false);

            printBanKeFromPrintDocument();

            currentWindow = APIManager.WaitingFindedWindow("hinh thuc");
            if (currentWindow == null)
                return;
            APIManager.ClickButton(currentWindow.hwnd, "chap nhan", isExactly: false);

            printBanKeFromPrintDocument();
        }

        private bool isReadDuRoi = false;
        private int lastConLai = 0;
        List<TestAPIModel> listControl;
        WindowInfo currentWindowRead;

        private void BackgroundWorkerRead_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                while (true)
                {

                    Thread.Sleep(200);
                    currentWindowRead = APIManager.GetActiveWindowTitle();
                    //thuc hien loc du lieu con
                    //listControl = APIManager.GetListControlText(currentWindowRead.hwnd);
                    //if (currentWindowRead.text == "")
                    //{
                    //    if (listControl.Count > 10)
                    //        continue;
                    //}

                    //TestText = DateTime.Now.ToString();
                    if (currentWindowRead.text.IndexOf("dong chuyen thu") != -1)
                    {
                        IsHaveError = false;
                        listControl = APIManager.GetListControlText(currentWindowRead.hwnd);
                        List<TestAPIModel> listWindowForm = listControl.Where(m => m.ClassName.IndexOf("WindowsForms10.EDIT") != -1).ToList();
                        if (listWindowForm.Count < 7)
                            continue;
                        TestAPIModel apiMaBuuCuc = listWindowForm[2];
                        TestAPIModel apiLoai;
                        TestAPIModel apiSoCT;
                        if (apiMaBuuCuc.Text.Length < 6)
                            continue;
                        if (!string.IsNullOrEmpty(apiMaBuuCuc.Text) && int.TryParse(apiMaBuuCuc.Text.Substring(0, 2), out int a))
                        {
                            if (apiMaBuuCuc.Text.Length < 6)
                                continue;
                            maSoBuuCucCurrent = apiMaBuuCuc.Text.Substring(0, 6);
                            apiLoai = listWindowForm[3];
                            apiSoCT = listWindowForm[6];
                            soCTCurrent = apiSoCT.Text;
                        }
                        else
                        {
                            apiMaBuuCuc = listWindowForm[3];
                            maSoBuuCucCurrent = apiMaBuuCuc.Text.Substring(0, 6);
                            apiLoai = listWindowForm[4];
                            apiSoCT = listWindowForm[7];
                            soCTCurrent = apiSoCT.Text;
                        }

                        string textLoai = APIManager.ConvertToUnSign3(apiLoai.Text).ToLower();
                        if (textLoai.IndexOf("buu kien") != -1)
                        {
                            loaiCurrent = "C";
                        }
                        else if (textLoai.IndexOf("ems") != -1)
                        {
                            loaiCurrent = "E";
                        }
                        else if (textLoai.IndexOf("buu pham") != -1)
                        {
                            loaiCurrent = "R";
                        }
                        else if (textLoai.IndexOf("logi") != -1)
                        {
                            loaiCurrent = "P";
                        }
                        else if (textLoai.IndexOf("phat hanh bao chi") != -1)
                        {
                            loaiCurrent = "B";
                        }

                        //kiem tra gr
                        TestAPIModel apiGr = listControl.FirstOrDefault(m => m.Text.IndexOf("gr") != -1);
                        if (apiGr == null)
                            continue;
                        string textGr = apiGr.Text.Replace("(gr)", "");
                        if (textGr.IndexOf('.') != -1)
                        {
                            bool isRight = double.TryParse(textGr, NumberStyles.Any, CultureInfo.InvariantCulture, out double numberGR);
                            if (isRight)
                            {
                                if (!Is16Kg)
                                {
                                    //txtInfo.Text = numberGR.ToString();
                                    if (numberGR > 16)
                                    {
                                        Is16Kg = true;
                                        SoundManager.playSound2(@"Number\tui16kg.wav");
                                    }
                                }
                            }
                        }
                        //kiem tra cai
                        TestAPIModel apiCai = listControl.FirstOrDefault(m => m.Text.IndexOf("cái") != -1);
                        if (apiCai == null)
                        {
                            continue;
                        }
                        //TestText += apiCai.Text + "\n";
                        int.TryParse(Regex.Match(apiCai.Text, @"\d+").Value, out numberRead);
                    }
                    else if (currentWindowRead.text.IndexOf("xac nhan chi tiet tui thu") != -1)
                    {
                        IsHaveError = false;
                        listControl = APIManager.GetListControlText(currentWindowRead.hwnd);

                        TestAPIModel apiCai = listControl.FirstOrDefault(m => m.Text.IndexOf("cái") != -1);
                        if (apiCai == null)
                        {
                            continue;
                        }
                        int.TryParse(Regex.Match(apiCai.Text, @"\d+").Value, out numberRead);
                    }
                    else if (currentWindowRead.text.IndexOf("xac nhan bd10 theo so hieu tui") != -1)
                    {
                        IsHaveError = false;
                        listControl = APIManager.GetListControlText(currentWindowRead.hwnd);
                        List<TestAPIModel> listWindowStatic = listControl.Where(m => m.ClassName.IndexOf("WindowsForms10.STATIC.app") != -1).ToList();

                        if (listWindowStatic.Count < 15)
                        {
                            continue;
                        }
                        TestAPIModel apiNumber = listWindowStatic[8];
                        TestAPIModel tongSoTui = listWindowStatic[15];
                        int.TryParse(Regex.Match(apiNumber.Text, @"\d+").Value, out numberRead);
                        int.TryParse(Regex.Match(tongSoTui.Text, @"\d+").Value, out int tongSoTuiN);
                        if (apiNumber.Text.Trim() == tongSoTui.Text.Trim() && tongSoTuiN != 0)
                        {
                            if (!isReadDuRoi)
                            {
                                isReadDuRoi = true;
                                new Thread(() =>
                                {
                                    Thread.CurrentThread.IsBackground = true;
                                    /* run your code here */
                                    Thread.Sleep(1000);

                                    SoundManager.playSound2(@"Number\dusoluong.wav");
                                }).Start();

                            }
                        }
                        else
                        {
                            isReadDuRoi = false;
                            if (tongSoTuiN != 0)
                            {
                                if (tongSoTuiN - numberRead < 5)
                                {
                                    if (lastConLai != tongSoTuiN - numberRead)
                                    {
                                        lastConLai = tongSoTuiN - numberRead;
                                        new Thread(() =>
                                        {
                                            Thread.CurrentThread.IsBackground = true;
                                            /* run your code here */
                                            Thread.Sleep(1000);

                                            SoundManager.playSound2(@"Number\" + lastConLai.ToString() + ".wav");
                                        }).Start();
                                    }
                                }
                            }
                        }
                    }
                    else if (currentWindowRead.text.IndexOf("lap bd10 theo duong thu") != -1)
                    {

                        IsHaveError = false;
                        listControl = APIManager.GetListControlText(currentWindowRead.hwnd);
                        List<TestAPIModel> listWindowStatic = listControl.Where(m => m.ClassName.IndexOf("WindowsForms10.STATIC.app") != -1).ToList();
                        if (listWindowStatic.Count < 8)
                        {
                            continue;
                        }
                        TestAPIModel apiNumber = listWindowStatic[8];
                        int.TryParse(Regex.Match(apiNumber.Text, @"\d+").Value, out numberRead);
                        if (numberRead != lastNumberSuaBD)
                        {
                            APIManager.currentNumberBD = numberRead;
                            lastNumberSuaBD = numberRead;
                        }
                    }
                    else if (currentWindowRead.text.IndexOf("xem chuyen thu chieu den") != -1)
                    {

                        IsHaveError = false;
                        if (_IsXacNhanChiTieting)
                        {
                            _IsXacNhanChiTieting = false;
                            SendKeys.SendWait("{F5}");
                            APIManager.ShowSnackbar("Dang kiem tra Auto Xac Nhan");
                            //Túi số  KL(kg) Loại túi    F xác nhận
                            //False   1   3,4 Ði ngoài(EMS)   True Cleared
                            //False   1   2,2 Ði ngoài(EMS)   False Selected
                            string dataCopped = APIManager.GetCopyData();
                            if (dataCopped.IndexOf("Selected") != -1)
                            {
                                SendKeys.SendWait("{F10}");
                                Thread.Sleep(200);
                                SendKeys.SendWait(" ");
                            }
                            else
                            {
                                SendKeys.SendWait("{F6}");
                            }
                        }
                    }
                    else if (currentWindowRead.text.IndexOf("sua thong tin bd10") != -1 || currentWindowRead.text.Trim() == "lap bd10")
                    {
                        IsHaveError = false;
                        listControl = APIManager.GetListControlText(currentWindowRead.hwnd);
                        List<TestAPIModel> listWindowStatic = listControl.Where(m => m.ClassName.IndexOf("WindowsForms10.STATIC.app") != -1).ToList();
                        if (listWindowStatic.Count <= 10)
                        {
                            continue;
                        }
                        TestAPIModel apiNumber = listWindowStatic[10];
                        //TestText += apiNumber.Text + "\n";
                        int.TryParse(Regex.Match(apiNumber.Text, @"\d+").Value, out numberRead);
                        if (numberRead != lastNumberSuaBD)
                        {
                            APIManager.currentNumberBD = numberRead;
                            lastNumberSuaBD = numberRead;
                        }
                    }

                    if (numberRead <= 300)
                    {
                        if (lastNumber != numberRead)
                        {
                            SoundManager.playSound(@"Number\" + numberRead.ToString() + ".wav");
                            lastNumber = numberRead;
                        }
                    }

                    //get error window
                    if (currentWindowRead.text.IndexOf("canh bao") != -1)
                    {
                        if (IsHaveError == false)
                        {
                            listControl = APIManager.GetListControlText(currentWindowRead.hwnd);
                            foreach (TestAPIModel apiContent in listControl)
                            {
                                //thuc hien lay text cua handle item
                                if (!string.IsNullOrEmpty(apiContent.Text))
                                {
                                    string textError = APIManager.ConvertToUnSign3(apiContent.Text).ToLower();
                                    if (textError.IndexOf("buu gui da duoc dong") != -1)
                                    {
                                        //thuc hien su ly trong nay
                                        Regex regex = new Regex(@"Số: ((\w||\W)+?)\r(\w||\W)+?Đến BC: ((\w||\W)+?)\r((\w||\W)+?)Dịch vụ: ((\w||\W)+?).");
                                        Match match = regex.Match(apiContent.Text);
                                        string tempSct = match.Groups[1].Value;
                                        string tempMaBuuCucNhan = match.Groups[4].Value;
                                        string tempLoai = match.Groups[8].Value;
                                        if (tempSct == soCTCurrent && tempMaBuuCucNhan == maSoBuuCucCurrent && tempLoai == loaiCurrent)
                                        {
                                            SoundManager.playSound2(@"Number\buiguiduocdong.wav");
                                            SendKeys.SendWait("{ENTER}");
                                            IsHaveError = true;
                                        }
                                        else
                                        {
                                            SoundManager.playSound2(@"Number\dacochuyenthukhac.wav");
                                            IsHaveError = true;
                                        }
                                    }
                                    else if (textError.IndexOf("khong them duoc tui") != -1)
                                    {
                                        SendKeys.SendWait("{ENTER}");
                                    }
                                    else
                                    if (textError.IndexOf("khong co buu gui") != -1)
                                    {
                                        SoundManager.playSound2(@"Number\khongcobuugui.wav");
                                        SendKeys.SendWait("{ENTER}");
                                        IsHaveError = true;
                                    }
                                    else
                                    if (textError.IndexOf("buu gui khong ton tai trong co so du lieu") != -1)
                                    {
                                        SoundManager.playSound2(@"Number\buuguikhongtontai.wav");
                                        SendKeys.SendWait("{ENTER}");
                                        IsHaveError = true;
                                    }
                                    else
                                    if (textError.IndexOf("khong tim thay tui thu co ma tui") != -1)
                                    {
                                        SoundManager.playSound2(@"Number\khongtimthaytuithucomanay.wav");
                                        SendKeys.SendWait("{ENTER}");
                                        IsHaveError = true;
                                    }
                                    else
                                    if (textError.IndexOf("buu gui da duoc giao cho buu ta") != -1)
                                    {
                                        SoundManager.playSound2(@"Number\buuguidaduocgiao.wav");
                                        SendKeys.SendWait("{ENTER}");
                                        IsHaveError = true;
                                    }
                                    else
                                    if (textError.IndexOf("buu gui da duoc xac nhan") != -1)
                                    {
                                        SoundManager.playSound2(@"Number\buuguidaduocxacnhan.wav");
                                        SendKeys.SendWait("{ENTER}");
                                        IsHaveError = true;
                                    }
                                    else if (textError.IndexOf("buu gui chua duoc xac nhan den") != -1)
                                    {
                                        SoundManager.playSound2(@"Number\buuguichuaduocxacnhan.wav");
                                        SendKeys.SendWait("{ENTER}");
                                        IsHaveError = true;
                                    }
                                    else
                                    if (textError.IndexOf("buu gui khong dung dich vu") != -1)
                                    {
                                        SoundManager.playSound2(@"Number\buuguikhongdungdichvu.wav");
                                        SendKeys.SendWait("{ENTER}");
                                        IsHaveError = true;
                                    }
                                    else if (textError.IndexOf("phat sinh su vu") != -1)
                                    {
                                        SoundManager.playSound2(@"Number\phatsinhsuvu.wav");
                                        IsHaveError = true;
                                    }
                                    else if (textError.IndexOf("an nut ok de bat dau") != -1)
                                    {
                                        Thread.Sleep(100);
                                        SendKeys.SendWait("{ENTER}");
                                        IsHaveError = true;
                                    }
                                    else if (textError.IndexOf("khong tao duoc tui thu") != -1)
                                    {
                                        Thread.Sleep(100);
                                        SendKeys.SendWait("{ENTER}");
                                        Thread.Sleep(100);
                                        SendKeys.SendWait("1");
                                        SendKeys.SendWait("{ENTER}");
                                        SendKeys.SendWait("{ENTER}");
                                        IsHaveError = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (currentWindowRead.text == "xac nhan")
                    {
                        if (IsHaveError == false)
                        {
                            listControl = APIManager.GetListControlText(currentWindowRead.hwnd);
                            foreach (TestAPIModel apiContent in listControl)
                            {
                                //thuc hien lay text cua handle item
                                if (!string.IsNullOrEmpty(apiContent.Text))
                                {
                                    string textError = APIManager.ConvertToUnSign3(apiContent.Text).ToLower();
                                    if (textError.IndexOf("ban co chac chan muon sua") != -1)
                                    {
                                        Thread.Sleep(100);
                                        APIManager.ClickButton(currentWindowRead.hwnd, "yes", isExactly: false);
                                        IsHaveError = true;
                                    }
                                    else if (textError.IndexOf("phat sinh su vu") != -1)
                                    {
                                        SoundManager.playSound2(@"Number\phatsinhsuvu.wav");
                                        IsHaveError = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (currentWindowRead.text.IndexOf("thong bao") != -1)
                    {
                        if (IsHaveError == false)
                        {
                            listControl = APIManager.GetListControlText(currentWindowRead.hwnd);
                            foreach (TestAPIModel apiContent in listControl)
                            {
                                //thuc hien lay text cua handle item
                                if (!string.IsNullOrEmpty(apiContent.Text))
                                {
                                    string textError = APIManager.ConvertToUnSign3(apiContent.Text).ToLower();
                                    if (textError.IndexOf("truyen thong tin chuyen thu") != -1)
                                    {
                                        Thread.Sleep(200);
                                        SendKeys.SendWait("{ENTER}");
                                        IsHaveError = true;
                                    }
                                    else if (textError.IndexOf("da truyen du lieu thanh cong") != -1)
                                    {
                                        Thread.Sleep(200);
                                        IsHaveError = true;
                                        SendKeys.SendWait("{ENTER}");
                                    }
                                }
                            }
                        }
                    }
                    else if (currentWindowRead.text == "loi")
                    {
                        if (IsHaveError == false)
                        {
                            listControl = APIManager.GetListControlText(currentWindowRead.hwnd);
                            foreach (TestAPIModel apiContent in listControl)
                            {
                                //thuc hien lay text cua handle item
                                if (!string.IsNullOrEmpty(apiContent.Text))
                                {
                                    string textError = APIManager.ConvertToUnSign3(apiContent.Text).ToLower();
                                    if (textError.IndexOf("tui khong duoc dong buu gui") != -1)
                                    {
                                        Thread.Sleep(200);
                                        IsHaveError = true;

                                        SoundManager.playSound2(@"Number\emsthuong.wav");
                                        SendKeys.SendWait("{ENTER}");
                                    }
                                    else if (textError.IndexOf("tui buu gui ems tmdt dg") != -1)
                                    {
                                        Thread.Sleep(200);
                                        IsHaveError = true;
                                        SoundManager.playSound2(@"Number\tmdt.wav");
                                        SendKeys.SendWait("{ENTER}");
                                    }
                                }
                            }
                        }
                    }
                    else if (string.IsNullOrEmpty(currentWindowRead.text))
                    {
                        listControl = APIManager.GetListControlText(currentWindowRead.hwnd);
                        if (listControl.Count > 10)
                        {
                            continue;
                        }
                        if (IsHaveError == false)
                        {
                            foreach (TestAPIModel apiContent in listControl)
                            {
                                //thuc hien lay text cua handle item
                                if (!string.IsNullOrEmpty(apiContent.Text))
                                {
                                    string textError = APIManager.ConvertToUnSign3(apiContent.Text).ToLower();
                                    if (textError.IndexOf("khong co ma tui nay") != -1)
                                    {
                                        SoundManager.playSound2(@"Number\khongcomatuinaytronghethong.wav");
                                        IsHaveError = true;
                                    }
                                    else if (textError.IndexOf("ma tui nay da duoc them") != -1)
                                    {
                                        SoundManager.playSound2(@"Number\nhanbd8.wav");
                                        SendKeys.SendWait("{ENTER}");
                                        IsHaveError = true;
                                    }
                                    else if (textError.IndexOf("da truyen bd10 di thanh cong") != -1)
                                    {
                                        SendKeys.SendWait("{ENTER}");
                                        SendKeys.SendWait("{ESC}");
                                        Thread.Sleep(100);
                                        SendKeys.SendWait("{ENTER}");
                                        IsHaveError = true;
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageShow(ex.Message);
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "MainViewModel " + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
            }
        }


        public ICommand OptionViewCommand { get; }

        void OptionView()
        {
            OptionView optionView = new OptionView();
            optionView.ShowDialog();
        }

        private string _TestText;

        public string TestText
        {
            get { return _TestText; }
            set { SetProperty(ref _TestText, value); }
        }



        public string CountInBD { get => _CountInBD; set => SetProperty(ref _CountInBD, value); }
        public IRelayCommand<System.Windows.Controls.TabControl> DefaultWindowCommand { get; }
        private bool _IsFindItem = true;
        private bool _IsBoQuaHuyen;

        public bool IsBoQuaHuyen
        {
            get { return _IsBoQuaHuyen; }
            set { SetProperty(ref _IsBoQuaHuyen, value); }
        }

        public bool IsFindItem
        {
            get { return _IsFindItem; }
            set { SetProperty(ref _IsFindItem, value); }
        }

        public int IndexTabControl
        {
            get { return _IndexTabControl; }
            set
            {
                SetProperty(ref _IndexTabControl, value);
                OnSelectedTabBD();
            }
        }

        private int _IndexTabKT;

        public int IndexTabKT
        {
            get { return _IndexTabKT; }
            set { SetProperty(ref _IndexTabKT, value); OnSelectedTabKT(); }
        }

        private void OnSelectedTabKT()
        {
            SetChiTietWindow();
            switch (IndexTabKT)
            {
                default:
                    break;
            }
        }

        private int _IndexTabOption;

        public int IndexTabOption
        {
            get { return _IndexTabOption; }
            set { SetProperty(ref _IndexTabOption, value); }
        }


        private int _IndexTabTui = 1;

        public int IndexTabTui
        {
            get { return _IndexTabTui; }
            set
            {
                SetProperty(ref _IndexTabTui, value);
                OnSelectedTabTui();
            }
        }

        private void OnSelectedTabTui()
        {
            switch (IndexTabTui)
            {
                case 0:
                    //thuc hien chuyen ve

                    SetDefaultWindowTui();
                    break;

                case 1:

                    SetDefaultWindowTui();
                    break;

                case 2:
                    SetRightHeigtTuiWindow();
                    break;

                case 3:

                    SetRightHeigtTuiWindow();
                    //DefaultWindowCommand.Execute(null);

                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Focus", Content = "Box" });
                    break;

                case 4:
                    SetDefaultWindowTui();
                    break;

                case 5:
                    SetDefaultWindowTui();
                    break;

                case 6:
                    SetDefaultWindowTui();
                    break;

                case 7:
                    SetDefaultWindowTui();
                    break;

                case 8:
                    OnSelectedTabBD();
                    break;

                case 9:
                    SetChiTietWindow();
                    break;

                case 10:
                    SetChiTietWindow();
                    break;

                case 11:
                    SetLayChuyenThuWindow();
                    break;

                case 12:
                    SetDefaultWindowTui();
                    break;

                default:
                    break;
            }
        }

        private void OnSelectedTabBD()
        {
            switch (IndexTabControl)
            {
                case 0:
                    //thuc hien chuyen ve
                    SetGetBD10Window();
                    break;

                case 1:
                    //SetDanhSachBD10Window();
                    SetChiTietWindow();
                    break;

                case 2:
                    SetChiTietWindow();
                    break;

                case 3:
                    SetLayChuyenThuWindow();
                    break;

                case 4:
                    SetChiTietWindow();
                    break;

                case 5:
                    break;

                default:
                    break;
            }
        }

        public ICommand LoadPageCommand { get; }

        public SnackbarMessageQueue MessageQueue
        {
            get { return _MessageQueue; }
            set { SetProperty(ref _MessageQueue, value); }
        }

        public ICommand OnCloseWindowCommand { get; }
        public ICommand SmallerWindowCommand { get; }
        public ICommand TabChangedCommand { get; }
        public ICommand TabTuiChangedCommand { get; }
        public ICommand ToggleWindowCommand { get; }

        private void CreateConnection()
        {
            //var pathDB = System.IO.Path.Combine(Environment.CurrentDirectory, "dulieu.sqlite");
            //string _strConnect = @"DataSource=" + pathDB + ";Version=3";
            //_con = new SqliteConnection(_strConnect);
            //_con.ConnectionString = _strConnect;
            //_con.Open();
        }

        private BackgroundWorker printTrangCuoi;

        private void DefaultWindow(System.Windows.Controls.TabControl tabControl)
        {
            //TabChanged(tabControl);
            SetDefaultWindowTui();
            isSmallWindow = false;
        }

        private void LoadPage(Window window)
        {
            _window = window;
            //SetRightWindow();
            SetDefaultWindowTui();
        }

        private void MessageShow(string content)
        {
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                if (MessageQueue == null)
                    MessageQueue = new SnackbarMessageQueue();
                MessageQueue.Enqueue(content, null, null, null, false, false, TimeSpan.FromSeconds(2));
                SoundManager.playSound2(@"Number\tingting.wav");
            });
        }

        private void OnCloseWindow()
        {
            _keyboardHook.UnHookKeyboard();
        }

        private string KeyData = "";
        private int _DefaultNumberPagePrint = 3;

        private void OnKeyPress(object sender, KeyPressedArgs e)
        {
            //thuc hien kiem tra cua so active hien tai

            try
            {
                switch (e.KeyPressed)
                {
                    case Key.D0:
                        KeyData += "0";
                        break;

                    case Key.D1:
                        KeyData += "1";
                        break;

                    case Key.D2:
                        KeyData += "2";
                        break;

                    case Key.D3:
                        KeyData += "3";
                        break;

                    case Key.D4:
                        KeyData += "4";
                        break;

                    case Key.D5:
                        KeyData += "5";
                        break;

                    case Key.D6:
                        KeyData += "6";
                        break;

                    case Key.D7:
                        KeyData += "7";
                        break;

                    case Key.D8:
                        KeyData += "8";
                        break;

                    case Key.D9:
                        KeyData += "9";
                        break;

                    case Key.F8:
                        //Thuc hien nay
                        WindowInfo currentWindow = APIManager.GetActiveWindowTitle();
                        if (currentWindow == null)
                            return;

                        if (currentWindow.text.IndexOf("xac nhan bd10 den") != -1)
                            WeakReferenceMessenger.Default.Send(new MessageManager("getData"));
                        break;

                    case Key.F3:
                        WindowInfo activeWindow1 = APIManager.GetActiveWindowTitle();
                        if (activeWindow1.text.IndexOf("danh sach bd10 di") != -1)
                        {
                            if (!bwRunPrints.IsBusy)
                            {
                                bwRunPrints.CancelAsync();
                                bwRunPrints.RunWorkerAsync();
                            }
                        }
                        break;

                    case Key.F5:
                        currentWindow = APIManager.GetActiveWindowTitle();
                        if (currentWindow == null)
                        {
                            return;
                        }

                        if (currentWindow.text.IndexOf("sua thong tin bd10") != -1 || currentWindow.text.IndexOf("lap bd10") != -1)
                        {
                            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "GuiTrucTiep", Content = "PrintDiNgoai" });
                        }
                        else if (currentWindow.text.IndexOf("thong tin buu gui") != -1)
                        {
                            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Navigation", Content = "PrintDiNgoai" });
                        }
                        else if (currentWindow.text.IndexOf("xac nhan chi tiet tui thu") != -1)
                        {
                            //thuc hien lenh trong nay
                            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "XacNhan", Content = "GetData" });
                        }
                        break;

                    case Key.F1:
                        currentWindow = APIManager.GetActiveWindowTitle();
                        if (currentWindow == null)
                            return;
                        if (currentWindow.text.IndexOf("khoi tao chuyen thu") != -1)
                            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "RunPrintDiNgoai", Content = "PrintDiNgoai" });
                        else if (currentWindow.text.IndexOf("dong chuyen thu") != -1)
                        {
                            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "Print" });
                        }
                        break;

                    case Key.F6:
                        currentWindow = APIManager.GetActiveWindowTitle();
                        if (currentWindow == null)
                            return;
                        if (currentWindow.text.IndexOf("sua thong tin bd10") != -1 || currentWindow.text.IndexOf("lap bd10") != -1)
                        {
                            _DefaultNumberPagePrint = 3;
                            bwPrintBD10.RunWorkerAsync();
                        }
                        break;
                    case Key.F7:
                        currentWindow = APIManager.GetActiveWindowTitle();
                        if (currentWindow == null)
                            return;
                        if (currentWindow.text.IndexOf("sua thong tin bd10") != -1 || currentWindow.text.IndexOf("lap bd10") != -1)
                        {
                            _DefaultNumberPagePrint = 2;
                            bwPrintBD10.RunWorkerAsync();
                        }
                        break;
                    case Key.Enter:

                        KeyData = KeyData.ToLower();
                        if (FileManager.listChuyenThu != null && FileManager.listChuyenThu.Count != 0)
                        {
                            if (KeyData.IndexOf(FileManager.listChuyenThu[0].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu0" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[1].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu1" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[2].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu2" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[3].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu3" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[4].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu4" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[5].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu5" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[6].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu6" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[7].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu7" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[8].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu8" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[9].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu9" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[10].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu10" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[11].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu11" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[12].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu12" });
                            }
                            else if (KeyData.IndexOf(FileManager.listChuyenThu[13].Barcode) != -1)
                            {
                                Thread.Sleep(700);
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "ChuyenThu13" });
                            }
                        }

                        if (KeyData.IndexOf("dong230") != -1)
                        {
                            Thread.Sleep(700);
                            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "KT" });
                        }
                        else if (KeyData.IndexOf("laydulieu") != -1)
                        {
                            Thread.Sleep(700);
                            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "LayDuLieu" });
                        }
                        else if (KeyData.IndexOf("inbd8") != -1)
                        {
                            Thread.Sleep(100);
                            currentWindow = APIManager.GetActiveWindowTitle();
                            if (currentWindow.text.IndexOf("dong chuyen thu") != -1)
                            {
                                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "Print" });
                            }
                        }
                        else if (KeyData.IndexOf("xnmh") != -1)
                        {
                            Thread.Sleep(100);
                            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Active", Content = "MainView" });
                            IndexTabTui = 3;
                            Thread.Sleep(100);
                            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Focus", Content = "Box" });
                        }
                        else if (KeyData.IndexOf("xoabg") != -1)
                        {
                            XoaBG();
                        }
                        else if (KeyData.ToLower().IndexOf("vn") != -1)
                        {
                            if (IsFindItem)
                            {
                                WindowInfo activeWindow = APIManager.GetActiveWindowTitle();

                                if (activeWindow.text.IndexOf("dong chuyen thu") != -1)
                                {
                                    var controls = APIManager.GetListControlText(activeWindow.hwnd);

                                    string keyData = KeyData.ToUpper();
                                    if (keyData.Length < 13)
                                    {
                                        KeyData = "";
                                        return;
                                    }
                                    var indexVN = keyData.IndexOf("VN");
                                    if (indexVN - 11 < 0)
                                    {
                                        KeyData = "";
                                        return;
                                    }
                                    if (maSoBuuCucCurrent.Length != 6)
                                    {
                                        KeyData = "";
                                        return;
                                    }

                                    string code = GetCodeFromString(KeyData.ToLower());

                                    //lay dia chi cua ma can tim

                                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "LoadAddressDong", Content = code });
                                }
                            }
                        }
                        else
                        {
                            activeWindow1 = APIManager.GetActiveWindowTitle();
                            if (activeWindow1.text.IndexOf("xac nhan bd10 theo") != -1)
                            {
                                if (KeyData.Length == 29 || KeyData.Length == 13 || KeyData.Length == 9)
                                {
                                    //thuc hien cong viec la luu du lieu len server
                                    listFindItem.Add(new FindItemModel(KeyData.ToUpper(), DateTime.Now.ToString()));
                                }
                            }
                        }
                        KeyData = "";
                        break;

                    case Key.LeftShift:
                        break;

                    case Key.F2:
                        //thuc hien copy du lieu sau do sang ben kia
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "TamQuanRun", Content = "" });
                        break;
                    case Key.F11:
                        WindowInfo activeWindows1 = APIManager.GetActiveWindowTitle();
                        if (activeWindows1.text.IndexOf("dong chuyen thu") != -1)
                        {
                            bwprintMaVach.RunWorkerAsync();
                        }
                        break;

                    case Key.F12:
                        WindowInfo activeWindows = APIManager.GetActiveWindowTitle();
                        if (activeWindows.text.IndexOf("dong chuyen thu") != -1)
                        {
                            bwPrintBanKe.RunWorkerAsync();
                        }
                        else if (activeWindows.text.IndexOf("quan ly chuyen thu chieu di") != -1)
                        {
                            printTrangCuoi.RunWorkerAsync();
                        }
                        break;

                    case Key.F4:

                        break;

                    default:
                        KeyData += e.KeyPressed.ToString();
                        break;
                }
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "loi Line MainViewModel " + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
            }
        }

        private readonly BackgroundWorker bwPrintBanKe;
        private readonly BackgroundWorker bwRunPrints;

        private string GetCodeFromString(string content)
        {
            int index = content.LastIndexOf("vn");
            return content.Substring(index - 11, 13);
        }

        private void XoaBG()
        {
            Thread.Sleep(2000);
            WindowInfo currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;
            if (currentWindow.text.IndexOf("dong chuyen thu") == -1)
            {
                return;
            }
            APIManager.ClickButton(currentWindow.hwnd, "xoa buu gui", isExactly: false);
            WindowInfo xacNhanWindow = APIManager.WaitingFindedWindow("xac nhan");
            if (xacNhanWindow == null)
            {
                return;
            }
            else
            {
                APIManager.ClickButton(xacNhanWindow.hwnd, "yes", isExactly: false);
                Thread.Sleep(1500);
                SoundManager.playSound2(@"\music\xoabg.wav");
            }
        }

        private void SetChiTietWindow()
        {
            if (_window == null)
                return;
            _window.Width = 1150;
            _window.Height = 630;
            double width = SystemParameters.PrimaryScreenWidth;
            double height = SystemParameters.PrimaryScreenHeight;
            _window.Left = (width - 1150) / 2;
            _window.Top = (height - 630) / 2;
        }

        private readonly BackgroundWorker bwPrintBD10;

        private void SetDefaultWindowTui()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 340;
            _window.Height = 480;
            _ = SystemParameters.PrimaryScreenHeight;
            double width = SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        private void SetGetBD10Window()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 470;
            _window.Height = 330;
            double width = SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        private void SetLayChuyenThuWindow()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 410;
            _window.Height = 670;
            double width = SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        private void SetRightHeigtTuiWindow()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 340;
            _window.Height = 630;
            double width = SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        private void SmallerWindow()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 85;
            _window.Height = 335;
            double width = SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        private void TabChanged(System.Windows.Controls.TabControl control)
        {
            if (control == null)
                return;
        }

        private void TabTuiChanged(System.Windows.Controls.TabControl tabControl)
        {
            if (tabControl == null)
                return;
            if (tabControl.SelectedIndex != lastSelectedTabTui)
            {
                lastSelectedTabTui = tabControl.SelectedIndex;
            }
            else
            {
                return;
            }
        }

        private double lastWidth = 0;
        private double lastHeight = 0;

        private void ToggleWindow()
        {
            if (isSmallWindow)
            {
                isSmallWindow = false;
                if (_window == null)
                    return;
                if (lastWidth == 1150)
                {
                    if (_window == null)
                        return;
                    _ = SystemParameters.WorkArea;
                    _window.Width = 1150;
                    _window.Height = 630;
                    double width = SystemParameters.PrimaryScreenWidth;
                    double height = SystemParameters.PrimaryScreenHeight;
                    _window.Left = (width - 1150) / 2;
                    _window.Top = (height - 630) / 2;
                }
                else
                {
                    Rect desktopWorkingArea = SystemParameters.WorkArea;
                    _window.Width = lastWidth;
                    _window.Height = lastHeight;
                    double width = SystemParameters.PrimaryScreenWidth;
                    // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
                    _window.Left = desktopWorkingArea.Left + width - _window.Width;
                    _window.Top = desktopWorkingArea.Top + 0;
                }
            }
            else
            {
                isSmallWindow = true;
                lastWidth = _window.Width;
                lastHeight = _window.Height;
                SmallerWindowCommand.Execute(null);
            }
        }

        private string _CountInBD;
        private int _IndexTabControl = 0;
        private readonly Y2KeyboardHook _keyboardHook;
        private SnackbarMessageQueue _MessageQueue;
        private Window _window;
        private bool _IsHaveError;

        public bool IsHaveError
        {
            get { return _IsHaveError; }
            set { SetProperty(ref _IsHaveError, value); }
        }


        private bool isSmallWindow = false;
        private int lastNumber = 0;
        private int lastSelectedTabTui = 0;
        private string loaiCurrent = "";
        private string maSoBuuCucCurrent = "";
        private int numberRead = 0;
        private string soCTCurrent = "";
    }
}