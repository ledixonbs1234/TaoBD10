using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class MainViewModel : ObservableObject
    {
        private bool Is16Kg = false;

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
            timerRead.Interval = new TimeSpan(0, 0, 0, 0, 1000);
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
                        IndexTabTui = 1;
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

        private int _IndexTabTui = 0;

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

                    DefaultWindowCommand.Execute(null);
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
                    SetChiTietWindow();
                    break;

                case 6:
                    SetLayChuyenThuWindow();
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
                MessageQueue.Enqueue(content, null, null, null, false, false, TimeSpan.FromSeconds(1));
            });

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
                    WeakReferenceMessenger.Default.Send(new MessageManager("getData"));
                    break;

                case Key.F5:
                    var currentWindow = APIManager.GetActiveWindowTitle();
                    if (currentWindow == null)
                    {
                        return;
                    }

                    string textCo = APIManager.ConvertToUnSign3(currentWindow.text).ToLower();

                    if (textCo.IndexOf("sua thong tin bd10") != -1 || textCo.IndexOf("lap bd10") != -1)
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
                    var currentWindow1 = APIManager.GetActiveWindowTitle();
                    if (currentWindow1 == null)
                        return;
                    if (currentWindow1.text.IndexOf("khoi tao chuyen thu") != -1)
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "RunPrintDiNgoai", Content = "PrintDiNgoai" });
                    else if (currentWindow1.text.IndexOf("dong chuyen thu") != -1)
                    {
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "Print" });
                    }
                    break;

                case Key.F6:
                    PrintBD10();

                    break;

                case Key.Enter:
                    KeyData = KeyData.ToLower();

                    if (KeyData.IndexOf("mokntb") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "Kien" });
                    }
                    else if (KeyData.IndexOf("moentb") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "EMS" });
                    }
                    else
                   if (KeyData.IndexOf("moqnmth") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "QuiNhon1" });
                    }
                    else if (KeyData.IndexOf("moqnhth") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "QuiNhon2" });
                    }
                    else if (KeyData.IndexOf("mopmth") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "PhuMy" });
                    }
                    else
                   if (KeyData.IndexOf("mopcth") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "PhuCat" });
                    }
                    else if (KeyData.IndexOf("moanth") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "AnNhon" });
                    }
                    else if (KeyData.IndexOf("mohath") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "HoaiAn" });
                    }
                    else if (KeyData.IndexOf("moamth") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "AnMy" });
                    }
                    else if (KeyData.IndexOf("moalth") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "AnLao" });
                    }
                    else if (KeyData.IndexOf("moahth") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "AnHoa" });
                    }
                    //else if (KeyData.IndexOf("thoattui") != -1)
                    //{
                    //    trangThaiThoat = TrangThaiThoat.CanhBao;
                    //    timerThoat.Start();
                    //}
                    else if (KeyData.IndexOf("montbth") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "TongHop" });
                    }
                    else if (KeyData.IndexOf("motqth") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "TamQuan" });
                    }
                    //else if (KeyData.IndexOf("inbdd8") != -1)
                    //{
                    //    Thread.Sleep(500);
                    //    SendKeys.SendWait("{F6}");
                    //    BtnInBD8_Click(null, null);
                    //}
                    else if (KeyData.IndexOf("dong230") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "KT" });
                    }
                    else if (KeyData.IndexOf("laydulieu") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "LayDuLieu" });
                    }
                    else if (KeyData.IndexOf("dong280") != -1)
                    {
                        Thread.Sleep(700);
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Chinh", Content = "BCP" });
                    }
                    KeyData = "";

                    break;

                case Key.LeftShift:
                    break;

                case Key.F2:
                    //thuc hien copy du lieu sau do sang ben kia
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "TamQuanRun", Content = "" });
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

        void PrintBD10()
        {
            var danhSach = APIManager.GetActiveWindowTitle();
            if (danhSach == null)
                return;
            if (danhSach.text.IndexOf("sua thong tin bd10") != -1 || danhSach.text.IndexOf("lap bd10") != -1)
            {
                List<TestAPIModel> datas = APIManager.GetListControlText(danhSach.hwnd);
                SendKeys.SendWait("{F3}");
                Thread.Sleep(500);
                SendKeys.SendWait("{Enter}");

                WindowInfo printD = danhSach;
                while (printD.text.IndexOf("print document") == -1)
                {
                    printD = APIManager.GetActiveWindowTitle();
                    Thread.Sleep(200);
                }
                Thread.Sleep(200);
                SendKeys.SendWait("{TAB}");
                SendKeys.SendWait(" ");
                Thread.Sleep(1000);
                SendKeys.SendWait("%{c}");
                Thread.Sleep(50);
                SendKeys.SendWait("{Up}");
                Thread.Sleep(50);
                SendKeys.SendWait("{Up}");
                Thread.Sleep(50);
                SendKeys.SendWait("%{o}");
                Thread.Sleep(50);
                SendKeys.SendWait("%{p}");
                Thread.Sleep(500);
                WindowInfo printD1 = danhSach;
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
                Thread.Sleep(100);
                SendKeys.SendWait("{Right}");
                SendKeys.SendWait(" ");
                Thread.Sleep(500);
                while (printD1.text.IndexOf("sua thong tin bd10") != -1)
                {
                    printD1 = APIManager.GetActiveWindowTitle();
                    Thread.Sleep(100);
                }
                Thread.Sleep(500);
                SendKeys.SendWait("{Enter}");
                SendKeys.SendWait("{Esc}");
                Thread.Sleep(100);
                SendKeys.SendWait("{Enter}");
            }
        }

        private void SetDefaultWindowTui()
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

        private void SmallerWindow()
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

        private void TabTuiChanged(System.Windows.Controls.TabControl tabControl)
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

        private string _TestText;

        public string TestText
        {
            get { return _TestText; }
            set { SetProperty(ref _TestText, value); }
        }



        private void TimerRead_Tick(object sender, EventArgs e)
        {
            WindowInfo activeWindow = APIManager.GetActiveWindowTitle();

            if (activeWindow == null)
            {
                return;
            }

            //class compare

            //thuc hien loc du lieu con
            List<TestAPIModel> listControl = APIManager.GetListControlText(activeWindow.hwnd);

            if (activeWindow.text.IndexOf("dong chuyen thu") != -1)
            {
                isHaveError = false;

                List<TestAPIModel> listWindowForm = listControl.Where(m => m.ClassName.IndexOf("WindowsForms10.EDIT") != -1).ToList();
                TestAPIModel apiMaBuuCuc = listWindowForm[2];
                TestAPIModel apiLoai;
                TestAPIModel apiSoCT;
                if (!string.IsNullOrEmpty(apiMaBuuCuc.Text))
                {
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

                //kiem tra gr
                TestAPIModel apiGr = listControl.First(m => m.Text.IndexOf("gr") != -1);
                string textGr = apiGr.Text.Replace("(gr)", "");
                if (textGr.IndexOf('.') != -1)
                {
                    bool isRight = double.TryParse(textGr, out double numberGR);
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
                TestAPIModel apiCai = listControl.First(m => m.Text.IndexOf("cái") != -1);
                TestText += apiCai.Text + "\n";
                int.TryParse(Regex.Match(apiCai.Text, @"\d+").Value, out numberRead);
            }
            else if (activeWindow.text.IndexOf("xac nhan chi tiet tui thu") != -1)
            {
                isHaveError = false;
                TestAPIModel apiCai = listControl.First(m => m.Text.IndexOf("cái") != -1);
               
                int.TryParse(Regex.Match(apiCai.Text, @"\d+").Value, out numberRead);
            }
            else if (activeWindow.text.IndexOf("xac nhan bd10 theo so hieu tui") != -1)
            {
                isHaveError = false;

                List<TestAPIModel> listWindowStatic = listControl.Where(m => m.ClassName.IndexOf("WindowsForms10.STATIC.app") != -1).ToList();
                if(listWindowStatic.Count ==0)
                {
                    return;
                }
                TestAPIModel apiNumber = listWindowStatic[8];
                int.TryParse(Regex.Match(apiNumber.Text, @"\d+").Value, out numberRead);
            }
            else if (activeWindow.text.IndexOf("lap bd10 theo duong thu") != -1)
            {
                isHaveError = false;

                List<TestAPIModel> listWindowStatic = listControl.Where(m => m.ClassName.IndexOf("WindowsForms10.STATIC.app") != -1).ToList();
                if (listWindowStatic.Count == 0)
                {
                    return;
                }
                TestAPIModel apiNumber = listWindowStatic[7];
                int.TryParse(Regex.Match(apiNumber.Text, @"\d+").Value, out numberRead);
            }
            else if (activeWindow.text.IndexOf("sua thong tin bd10") != -1)
            {
                isHaveError = false;

                List<TestAPIModel> listWindowStatic = listControl.Where(m => m.ClassName.IndexOf("WindowsForms10.STATIC.app") != -1).ToList();
                if (listWindowStatic.Count == 0)
                {
                    return;
                }
                TestAPIModel apiNumber = listWindowStatic[9];
                TestText += apiNumber.Text + "\n";
                int.TryParse(Regex.Match(apiNumber.Text, @"\d+").Value, out numberRead);
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
            if (activeWindow.text.IndexOf("canh bao") != -1)
            {
                if (isHaveError == false)
                {
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
                                    isHaveError = true;
                                }
                                else
                                {
                                    SoundManager.playSound2(@"Number\dacochuyenthukhac.wav");
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
                                SoundManager.playSound2(@"Number\khongcobuugui.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else
                            if (textError.IndexOf("buu gui khong ton tai trong co so du lieu") != -1)
                            {
                                SoundManager.playSound2(@"Number\buuguikhongtontai.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else
                            if (textError.IndexOf("khong tim thay tui thu co ma tui") != -1)
                            {
                                SoundManager.playSound2(@"Number\khongtimthaytuithucomanay.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else
                            if (textError.IndexOf("buu gui da duoc giao cho buu ta") != -1)
                            {
                                SoundManager.playSound2(@"Number\buuguidaduocgiao.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else
                            if (textError.IndexOf("buu gui da duoc xac nhan") != -1)
                            {
                                SoundManager.playSound2(@"Number\buuguidaduocxacnhan.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else if (textError.IndexOf("buu gui chua duoc xac nhan den") != -1)
                            {
                                SoundManager.playSound2(@"Number\buuguichuaduocxacnhan.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else
                            if (textError.IndexOf("buu gui khong dung dich vu") != -1)
                            {
                                SoundManager.playSound2(@"Number\buuguikhongdungdichvu.wav");
                                SendKeys.SendWait("{ENTER}");
                                isHaveError = true;
                            }
                            else if (textError.IndexOf("phat sinh su vu") != -1)
                            {
                                SoundManager.playSound2(@"Number\phatsinhsuvu.wav");
                                isHaveError = true;
                            }
                            else if (textError.IndexOf("an nut ok de bat dau") != -1)
                            {
                                Thread.Sleep(100);
                                SendKeys.SendWait("{ENTER}");
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
                    foreach (TestAPIModel apiContent in listControl)
                    {
                        //thuc hien lay text cua handle item
                        if (!string.IsNullOrEmpty(apiContent.Text))
                        {
                            string textError = APIManager.ConvertToUnSign3(apiContent.Text).ToLower();
                            if (textError.IndexOf("ban co chac muon") != -1)
                            {
                            }
                            else if (textError.IndexOf("phat sinh su vu") != -1)
                            {
                                SoundManager.playSound2(@"Number\phatsinhsuvu.wav");
                                isHaveError = true;
                            }
                        }
                    }
                }
            }
            else if (activeWindow.text.IndexOf("thong bao") != -1)
            {
                if (isHaveError == false)
                {
                    foreach (TestAPIModel apiContent in listControl)
                    {
                        //thuc hien lay text cua handle item
                        if (!string.IsNullOrEmpty(apiContent.Text))
                        {
                            string textError = APIManager.ConvertToUnSign3(apiContent.Text).ToLower();
                            if (textError.IndexOf("truyen thong tin chuyen thu") != -1)
                            {
                                Thread.Sleep(200);
                                isHaveError = true;
                                SendKeys.SendWait("{ENTER}");
                            }
                        }
                    }
                }
            }
            else if (string.IsNullOrEmpty(activeWindow.text))
            {
                if (isHaveError == false)
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
                                isHaveError = true;
                            }
                        }
                    }
                }
            }
        }
        double lastWidth = 0;
        double lastHeight = 0;
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
                    var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
                    _window.Width = 1150;
                    _window.Height = 600;
                    double width = System.Windows.SystemParameters.PrimaryScreenWidth;
                    double height = System.Windows.SystemParameters.PrimaryScreenHeight;
                    _window.Left = (width - 1150) / 2;
                    _window.Top = (height - 600) / 2;
                }
                else
                {
                    var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
                    _window.Width = lastWidth;
                    _window.Height = lastHeight;
                    double height = System.Windows.SystemParameters.PrimaryScreenHeight;
                    double width = System.Windows.SystemParameters.PrimaryScreenWidth;
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
        private Y2KeyboardHook _keyboardHook;
        private SnackbarMessageQueue _MessageQueue;
        private Window _window;
        private bool isHaveError = false;
        private bool isSmallWindow = false;
        private int lastNumber = 0;
        private int lastSelectedTabTui = 0;
        private string loaiCurrent = "";
        private string maSoBuuCucCurrent = "";
        private int numberRead = 0;
        private string soCTCurrent = "";
        private System.Windows.Controls.TabControl tabControl;
        private System.Windows.Controls.TabControl tabTuiControl;
        private DispatcherTimer timerRead;
    }
}