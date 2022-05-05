using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class MainViewModel : ObservableObject
    {

        public MainViewModel()
        {
            LoadPageCommand = new RelayCommand<Window>(LoadPage);
            TabChangedCommand = new RelayCommand<System.Windows.Controls.TabControl>(TabChanged);
            OnCloseWindowCommand = new RelayCommand(OnCloseWindow);
            SmallerWindowCommand = new RelayCommand(SmallerWindow);
            DefaultWindowCommand = new RelayCommand<System.Windows.Controls.TabControl>(DefaultWindow);
            ToggleWindowCommand = new RelayCommand(ToggleWindow);
            TabTuiChangedCommand = new RelayCommand<System.Windows.Controls.TabControl>(TabTuiChanged);

            timerRead = new DispatcherTimer();
            timerRead.Interval = new TimeSpan(0, 0, 0, 0, 200);
            timerRead.Tick += TimerRead_Tick;
            timerRead.Start();


            _keyboardHook = new Y2KeyboardHook();
            _keyboardHook.OnKeyPressed += OnKeyPress;
            _keyboardHook.HookKeyboard();
            createConnection();
            SoundManager.SetUpDirectory();

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
                        if (tabTuiControl != null)
                            tabTuiControl.SelectedIndex = 1;
                    }
                }
                else if (m.Key == "Snackbar")
                {
                    MessageShow(m.Content);
                }
            });
        }
        public string CountInBD { get => _CountInBD; set => SetProperty(ref _CountInBD, value); }
        public IRelayCommand<System.Windows.Controls.TabControl> DefaultWindowCommand { get; }
        public int IndexTabControl
        {
            get { return _IndexTabControl; }
            set
            {
                SetProperty(ref _IndexTabControl, value);
                OnSelectedTabBD();

            }
        }

        private int _IndexTabTui;

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
                    //currentTab = CurrentTab.GetBD10;

                    SetDefaultWindowTui();
                    break;

                case 1:
                    //currentTab = CurrentTab.DanhSach;

                    SetDefaultWindowTui();
                    break;

                case 2:
                    //currentTab = CurrentTab.ChiTiet;

                    SetRightHeigtTuiWindow();
                    break;

                case 3:
                    //currentTab = CurrentTab.ThuGon;

                    DefaultWindowCommand.Execute(null);
                    break;
                case 4:
                    //currentTab = CurrentTab.LayChuyenThu;
                    break;

                case 5:
                    break;
                //currentTab = CurrentTab.LocTui;
                case 6:
                    OnSelectedTabBD();
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
                    currentTab = CurrentTab.GetBD10;

                    break;

                case 1:
                    //SetDanhSachBD10Window();
                    SetChiTietWindow();
                    currentTab = CurrentTab.DanhSach;
                    break;

                case 2:
                    SetChiTietWindow();
                    currentTab = CurrentTab.ChiTiet;
                    break;

                case 3:
                    SetThuGonWindow();
                    currentTab = CurrentTab.ThuGon;
                    break;
                case 4:
                    SetLayChuyenThuWindow();
                    currentTab = CurrentTab.LayChuyenThu;
                    break;

                case 5:
                    currentTab = CurrentTab.LocTui;
                    SetChiTietWindow();
                    break;
                case 6:
                    SetChiTietWindow();
                    currentTab = CurrentTab.Address;
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
        private void createConnection()
        {
            //var pathDB = System.IO.Path.Combine(Environment.CurrentDirectory, "dulieu.sqlite");
            //string _strConnect = @"DataSource=" + pathDB + ";Version=3";
            //_con = new SqliteConnection(_strConnect);
            //_con.ConnectionString = _strConnect;
            //_con.Open();
        }

        void DefaultWindow(System.Windows.Controls.TabControl tabControl)
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

        void MessageShow(string content)
        {
            if (MessageQueue == null)
                MessageQueue = new SnackbarMessageQueue();
            MessageQueue.Enqueue(content, null, null, null, false, false, TimeSpan.FromSeconds(1));

        }

        private void OnCloseWindow()
        {
            _keyboardHook.UnHookKeyboard();
        }

        private string KeyData = "";

        private void OnKeyPress(object sender, KeyPressedArgs e)
        {
            //thuc hien kiem tra cua so active hien tai


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
                    WeakReferenceMessenger.Default.Send<MessageManager>(new MessageManager("getData"));
                    break;
                case Key.F5:
                    var currentWindow = APIManager.GetActiveWindowTitle();
                    if (currentWindow == null)
                    {
                        return;
                    }

                    string textCo = APIManager.convertToUnSign3(currentWindow.text).ToLower();

                    if (textCo.IndexOf("sua thong tin bd10") != -1 || textCo.IndexOf("lap bd10") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "GuiTrucTiep", Content = "PrintDiNgoai" });
                    }
                    else if (currentWindow.text.IndexOf("thong tin buu gui") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "PrintDiNgoai" });
                    }
                    break;
                case Key.F1:
                    var currentWindow1 = APIManager.GetActiveWindowTitle();
                    if (currentWindow1 == null)
                        return;
                    if (currentWindow1.text.IndexOf("khoi tao chuyen thu") != -1)
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "RunPrintDiNgoai", Content = "PrintDiNgoai" });
                    else if (currentWindow1.text.IndexOf("dong chuyen thu") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "Print" });
                    }
                    break;
                case Key.Enter:
                    KeyData = KeyData.ToLower();

                    if (KeyData.IndexOf("mokntb") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "Kien" });
                    }
                    else if (KeyData.IndexOf("moentb") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "EMS" });
                    }
                    else
                   if (KeyData.IndexOf("moqnmth") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "QuiNhon1" });
                    }
                    else if (KeyData.IndexOf("moqnhth") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "QuiNhon2" });
                    }
                    else if (KeyData.IndexOf("mopmth") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "PhuMy" });
                    }
                    else
                   if (KeyData.IndexOf("mopcth") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "PhuCat" });
                    }
                    else if (KeyData.IndexOf("moanth") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "AnNhon" });
                    }
                    else if (KeyData.IndexOf("mohath") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "HoaiAn" });
                    }
                    else if (KeyData.IndexOf("moamth") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "AnMy" });
                    }
                    else if (KeyData.IndexOf("moalth") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "AnLao" });
                    }
                    else if (KeyData.IndexOf("moahth") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "AnHoa" });
                    }
                    //else if (KeyData.IndexOf("thoattui") != -1)
                    //{
                    //    trangThaiThoat = TrangThaiThoat.CanhBao;
                    //    timerThoat.Start();
                    //}
                    else if (KeyData.IndexOf("montbth") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "TongHop" });
                    }
                    else if (KeyData.IndexOf("motqth") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "TamQuan" });
                    }
                    //else if (KeyData.IndexOf("inbdd8") != -1)
                    //{
                    //    Thread.Sleep(500);
                    //    SendKeys.SendWait("{F6}");
                    //    BtnInBD8_Click(null, null);
                    //}
                    else if (KeyData.IndexOf("dong230") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "KT" });
                    }
                    else if (KeyData.IndexOf("laydulieu") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "LayDuLieu" });
                    }
                    else if (KeyData.IndexOf("dong280") != -1)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Chinh", Content = "BCP" });
                    }
                    KeyData = "";

                    break;

                case Key.LeftShift:
                    break;

                default:
                    KeyData += e.KeyPressed.ToString();
                    break;
            }
        }

        private void SetChiTietWindow()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 1150;
            _window.Height = 600;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            _window.Left = (width - 1150) / 2;
            _window.Top = (height - 600) / 2;
        }

        private void SetDanhSachBD10Window()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 570;
            _window.Height = 500;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        void SetDefaultWindowTui()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 360;
            _window.Height = 450;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
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
            _window.Height = 300;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
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
            _window.Height = 640;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        private void SetRightHeigtTuiWindow()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 360;
            _window.Height = 600;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        private void SetThuGonWindow()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 350;
            _window.Height = 550;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        void SmallerWindow()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 100;
            _window.Height = 335;
            double height = System.Windows.SystemParameters.PrimaryScreenHeight;
            double width = System.Windows.SystemParameters.PrimaryScreenWidth;
            // use 'Screen.AllScreens[1].WorkingArea' for secondary screen
            _window.Left = desktopWorkingArea.Left + width - _window.Width;
            _window.Top = desktopWorkingArea.Top + 0;
        }

        private void TabChanged(System.Windows.Controls.TabControl control)
        {
            if (control == null)
                return;
            tabControl = control;


        }

        void TabTuiChanged(System.Windows.Controls.TabControl tabControl)
        {
            if (tabControl == null)
                return;
            tabTuiControl = tabControl;
            if (tabControl.SelectedIndex != lastSelectedTabTui)
            {
                lastSelectedTabTui = tabControl.SelectedIndex;
            }
            else
            {
                return;
            }


        }

        //tu khoa handle
        string TextCurrentActive = "";
        string CaiChiTiet = "";
        List<IntPtr> allChild;


        private void TimerRead_Tick(object sender, EventArgs e)
        {
            WindowInfo activeWindow = APIManager.GetActiveWindowTitle();

            if (activeWindow == null)
            {
                return;
            }

            //class compare

            //thuc hien loc du lieu con

            if (activeWindow.text.IndexOf("dong chuyen thu") != -1)
            {
                if (TextCurrentActive != "dong chuyen thu")
                {
                    TextCurrentActive = "dong chuyen thu";
                    allChild = APIManager.GetAllChildHandles(activeWindow.hwnd);
                    maSoBuuCucCurrent = APIManager.GetControlText(allChild[25]);
                    string temLow = APIManager.convertToUnSign3(APIManager.GetControlText(allChild[25])).ToLower();
                    soCTCurrent = APIManager.GetControlText(allChild[29]);

                    if (temLow.IndexOf("buu kien") != -1)
                    {
                        loaiCurrent = "C";
                    }
                    else if (temLow.IndexOf("ems") != -1)
                    {
                        loaiCurrent = "E";
                    }
                    else if (temLow.IndexOf("buu pham") != -1)
                    {
                        loaiCurrent = "R";
                    }
                    else if (temLow.IndexOf("logi") != -1)
                    {
                        loaiCurrent = "P";
                    }
                }
                isHaveError = false;
                //trong luong // can kiem tra lai
                string grText = APIManager.GetControlText(allChild[14]);
                grText = grText.Replace("(gr)", "");
                if (grText.IndexOf('.') != -1)
                {
                    bool isRight = double.TryParse(grText, out double numberGR);
                    if (isRight)
                    {
                        //if (!chinhViewModel.is16Kg)
                        //{
                        //    if (numberGR > 16)
                        //    {
                        //        chinhViewModel.is16Kg = true;
                        //        SoundManager.playSync(@"Number\tui16kg.wav");
                        //    }
                        //}
                    }
                }
                string caiText = APIManager.GetControlText(allChild[3]);

                if (caiText.IndexOf("cái") != -1)
                {
                    int.TryParse(Regex.Match(caiText, @"\d+").Value, out numberRead);
                }

            }
            else
            if (activeWindow.text.IndexOf("xac nhan chi tiet tui thu") != -1)
            {
                if (TextCurrentActive != "Xac Nhan Chi Tiet")
                {
                    TextCurrentActive = "Xac Nhan Chi Tiet";
                    allChild = APIManager.GetAllChildHandles(activeWindow.hwnd);
                    CaiChiTiet = APIManager.GetControlText(allChild[2]);
                }
                isHaveError = false;

                //thuc hien cai thu 2
                //regex get number
                int.TryParse(Regex.Match(CaiChiTiet, @"\d+").Value, out numberRead);
                //thuc hien doc so


            }
            else if (activeWindow.text.IndexOf("xac nhan bd10 theo so hieu tui") != -1)
            {
                isHaveError = false;
                if (TextCurrentActive != "Xac Nhan BD Theo Tui")
                {
                    TextCurrentActive = "Xac Nhan BD Theo Tui";
                    allChild = APIManager.GetAllChildHandles(activeWindow.hwnd);
                }
                int.TryParse(Regex.Match(APIManager.GetControlText(allChild[24]), @"\d+").Value, out numberRead);
            }
            else if (activeWindow.text.IndexOf("lap bd10 theo duong thu") != -1)
            {

                if (TextCurrentActive != "Lap BD10 Theo DT")
                {
                    TextCurrentActive = "Lap BD10 Theo DT";
                    allChild = APIManager.GetAllChildHandles(activeWindow.hwnd);
                }


                isHaveError = false;
                int.TryParse(Regex.Match(APIManager.GetControlText(allChild[25]), @"\d+").Value, out numberRead);
            }
            else if (activeWindow.text.IndexOf("sua thong tin bd10") != -1)
            {
                if (TextCurrentActive != "Sua Thong Tin BD")
                {
                    TextCurrentActive = "Sua Thong Tin BD";
                    allChild = APIManager.GetAllChildHandles(activeWindow.hwnd);

                }
                isHaveError = false;


                List<TestAPIModel> list = APIManager.GetListControlText(activeWindow.hwnd);
                string a = "";
                foreach (var item in list)
                {
                    a += item.Index.ToString() + "|" + item.Text + "|" + item.ClassName + "\n";
                }

                int.TryParse(Regex.Match(APIManager.GetControlText(allChild[22]), @"\d+").Value, out numberRead);
            }
            if (numberRead <= 300)
            {
                if (lastNumber != numberRead)
                {
                    isNewNumber = true;
                    isReading = true;

                    SoundManager.playSound(@"Number\" + numberRead.ToString() + ".wav");
                    isReading = false;
                    lastNumber = numberRead;
                }
            }

            //get error window
            if (activeWindow.text.IndexOf("canh bao") != -1)
            {
                //thuc hien viec truy xuat thong tin trong nay

                if (isHaveError == false)
                {
                    //thuc hien loc du lieu con
                    List<IntPtr> _allChild = APIManager.GetAllChildHandles(activeWindow.hwnd);

                    foreach (var item in allChild)
                    {
                        //thuc hien lay text cua handle item
                        String text = APIManager.GetControlText(item);
                        if (!String.IsNullOrEmpty(text))
                        {
                            isErrorSay = true;
                            countError = 1;
                            string textError = APIManager.convertToUnSign3(text).ToLower();
                            if (textError.IndexOf("buu gui da duoc dong") != -1)
                            {
                                //thuc hien su ly trong nay
                                Regex regex = new Regex(@"Số: ((\w||\W)+?)\r(\w||\W)+?Đến BC: ((\w||\W)+?)\r((\w||\W)+?)Dịch vụ: ((\w||\W)+?).");
                                var match = regex.Match(text);
                                string tempSct = match.Groups[1].Value;
                                string tempMaBuuCucNhan = match.Groups[4].Value;
                                string tempLoai = match.Groups[8].Value;
                                if (tempSct == soCTCurrent && tempMaBuuCucNhan == maSoBuuCucCurrent && tempLoai == loaiCurrent)
                                {
                                    SoundManager.playSound(@"Number\buiguiduocdong.wav");
                                    SendKeys.SendWait("{ENTER}");
                                    isHaveError = true;
                                }
                                else
                                {
                                    SoundManager.playSound(@"Number\dacochuyenthukhac.wav");
                                    isHaveError = true;
                                }
                            }
                            else if (textError.IndexOf("khong them duoc tui") != -1)
                            {
                                SendKeys.SendWait("{ENTER}");
                            }
                            else
                            if (textError.IndexOf("khong co buu gui") != -1)
                            {
                                SoundManager.playSound(@"Number\khongcobuugui.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else
                            if (textError.IndexOf("buu gui khong ton tai trong co so du lieu") != -1)
                            {
                                SoundManager.playSound(@"Number\buuguikhongtontai.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else
                            if (textError.IndexOf("khong tim thay tui thu co ma tui") != -1)
                            {
                                SoundManager.playSound(@"Number\khongtimthaytuithucomanay.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else
                            if (textError.IndexOf("buu gui da duoc giao cho buu ta") != -1)
                            {
                                SoundManager.playSound(@"Number\buuguidaduocgiao.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else
                            if (textError.IndexOf("buu gui da duoc xac nhan") != -1)
                            {
                                SoundManager.playSound(@"Number\buuguidaduocxacnhan.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else if (textError.IndexOf("buu gui chua duoc xac nhan den") != -1)
                            {
                                SoundManager.playSound(@"Number\buuguichuaduocxacnhan.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else
                            if (textError.IndexOf("buu gui khong dung dich vu") != -1)
                            {
                                SoundManager.playSound(@"Number\buuguikhongdungdichvu.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else if (textError.IndexOf("phat sinh su vu") != -1)
                            {
                                SoundManager.playSound(@"Number\phatsinhsuvu.wav");
                                isHaveError = true;
                            }
                        }
                    }
                }
            }
            else if (activeWindow.text == "xac nhan")
            {
                if (isHaveError == false)
                {
                    //thuc hien loc du lieu con
                    List<IntPtr> _allChildError = APIManager.GetAllChildHandles(activeWindow.hwnd);

                    foreach (var item in allChild)
                    {
                        //thuc hien lay text cua handle item
                        String text = APIManager.GetControlText(item); if (!String.IsNullOrEmpty(text))
                        {
                            isErrorSay = true;
                            countError = 1;
                            string textError = APIManager.convertToUnSign3(text).ToLower();
                            if (textError.IndexOf("ban co chac muon") != -1)
                            {
                            }
                            else if (textError.IndexOf("phat sinh su vu") != -1)
                            {
                                SoundManager.playSound(@"Number\phatsinhsuvu.wav");
                                isHaveError = true;
                            }
                        }
                    }
                }
            }
            else
            if (String.IsNullOrEmpty(activeWindow.text))
            {
                if (isHaveError == false)
                {
                    //thuc hien loc du lieu con
                    var allChildError = APIManager.GetAllChildHandles(activeWindow.hwnd);

                    foreach (var item in allChild)
                    {
                        //thuc hien lay text cua handle item
                        String text = APIManager.GetControlText(item);
                        if (!String.IsNullOrEmpty(text))
                        {
                            string textError = APIManager.convertToUnSign3(text).ToLower();
                            if (textError.IndexOf("khong co ma tui nay") != -1)
                            {
                                SoundManager.playSound(@"Number\khongcomatuinaytronghethong.wav");
                                isHaveError = true;
                            }
                        }
                    }
                }
            }
        }

        void ToggleWindow()
        {
            if (isSmallWindow)
            {
                isSmallWindow = false;
                DefaultWindowCommand.Execute(null);
            }
            else
            {
                isSmallWindow = true;
                SmallerWindowCommand.Execute(null);

            }

        }

        private string _CountInBD;
        private int _IndexTabControl = 0;
        private Y2KeyboardHook _keyboardHook;
        private SnackbarMessageQueue _MessageQueue;
        private Window _window;
        int countError = 1;
        private CurrentTab currentTab = CurrentTab.GetBD10;
        bool isErrorSay = true;
        bool isHaveError = false;
        bool isNewNumber = true;
        bool isReading = true;
        bool isSmallerWindow = false;
        bool isSmallWindow = false;
        int lastNumber = 0;
        int lastSelectedTabTui = 0;
        string loaiCurrent = "";
        string maSoBuuCucCurrent = "";
        int numberRead = 0;
        string soCTCurrent = "";
        System.Windows.Controls.TabControl tabControl;
        private System.Windows.Controls.TabControl tabTuiControl;
        DispatcherTimer timerRead;
    }
}