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
            timerRead.Interval = new TimeSpan(2000);
            timerRead.Tick += TimerRead_Tick;


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
                    }else if(m.Content == "Center")
                    {
                        SetChiTietWindow();

                    }else if (m.Content == "SmallRight")
                    {
                        SetRightHeigtTuiWindow();
                    }
                }
                else if (m.Key == "Snackbar")
                {
                    MessageShow(m.Content);
                }            });
        }
        bool isHaveError = false;
        string maSoBuuCucCurrent = "";
        string loaiCurrent = "";
        string soCTCurrent = "";
        int numberRead = 0;
        int lastNumber = 0;
        bool isErrorSay = true;
        int countError = 1;
        bool isNewNumber = true;
        bool isReading = true;

        public ICommand ToggleWindowCommand { get; }


        bool isSmallWindow = false;
        void ToggleWindow()
        {
            if (isSmallWindow)
            {
                isSmallWindow = false;
                DefaultWindowCommand.Execute(null);
            }else
            {
                isSmallWindow = true;
                SmallerWindowCommand.Execute(null);

            }

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
            var allChild = APIManager.GetAllChildHandles(activeWindow.hwnd);

            if (activeWindow.text.IndexOf("dong chuyen thu") != -1)
            {
                isHaveError = false;
                int countGr = 0;
                int countDongChuyenThu = 0;
                int count = 0;
                foreach (var item in allChild)
                {
                    //thuc hien lay text cua handle item
                    String text = APIManager.GetControlText(item);
                    count++;

                    string className = APIManager.GetWindowClass(item);
                    if (className.IndexOf("WindowsForms10.EDIT.app.0.1e6fa8e") != -1)
                    {
                        countDongChuyenThu++;
                        if (countDongChuyenThu == 3)
                        {
                            if (!string.IsNullOrEmpty(text))
                            {
                                maSoBuuCucCurrent = text.Substring(0, 6);
                            }
                            else
                            {
                                countDongChuyenThu--;
                            }
                        }
                        if (countDongChuyenThu == 4)
                        {
                            string temLow = APIManager.convertToUnSign3(text).ToLower();
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
                        if (countDongChuyenThu == 7)
                        {
                            soCTCurrent = text;
                        }
                    }

                    if (text.IndexOf("gr") != -1 && countGr == 0)
                    {
                        countGr++;
                        text = text.Replace("(gr)", "");
                        if (text.IndexOf('.') != -1)
                        {
                            bool isRight = double.TryParse(text, out double numberGR);
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
                    }

                    //loc name
                    if (text.IndexOf("cái") != -1)
                    {
                        //thuc hien cai thu 2
                        //regex get number
                        string resultString = Regex.Match(text, @"\d+").Value;
                        bool isRight = int.TryParse(resultString, out numberRead);

                        if (!isRight)
                        {
                            timerRead.Stop();
                            System.Windows.MessageBox.Show("Không phải số. \n Vui lòng mở lại chương trình.");
                        }

                        //thuc hien doc so
                    }
                }
            }
            else
            if (activeWindow.text.IndexOf("xac nhan chi tiet tui thu") != -1)
            {
                isHaveError = false;
                foreach (var item in allChild)
                {
                    //thuc hien lay text cua handle item
                    String text = APIManager.GetControlText(item);

                    //loc name
                    if (text.IndexOf("cái") != -1)
                    {
                        //thuc hien cai thu 2
                        //regex get number
                        string resultString = Regex.Match(text, @"\d+").Value;
                        bool isRight = int.TryParse(resultString, out numberRead);
                        if (!isRight)
                        {
                            timerRead.Stop();
                            System.Windows.MessageBox.Show("Không phải số. \n Vui lòng mở lại chương trình.");
                        }
                        break;
                        //thuc hien doc so
                    }
                }
            }
            else if (activeWindow.text.IndexOf("xac nhan bd10 theo so hieu tui") != -1)
            {
                isHaveError = false;
                int count = 0;
                foreach (var item in allChild)
                {
                    string cWindow = APIManager.GetWindowClass(item);
                    if (cWindow.IndexOf("WindowsForms10.STATIC.app.0.1e6fa8e") != -1)
                    {
                        if (count == 8)
                        {
                            //thuc hien lay class nay
                            String text = APIManager.GetControlText(item);
                            string resultString = Regex.Match(text, @"\d+").Value;
                            bool isRight = int.TryParse(resultString, out numberRead);
                            if (!isRight)
                            {
                                return;
                                //timerRead.Stop();

                                //MessageBox.Show("Không phải số. \n Vui lòng mở lại chương trình.");
                            }
                            break;
                        }
                        count++;
                    }
                }
            }
            else if (activeWindow.text.IndexOf("lap bd10 theo duong thu") != -1)
            {
                isHaveError = false;
                int count = 0;
                foreach (var item in allChild)
                {
                    string cWindow = APIManager.GetWindowClass(item);
                    if (cWindow.IndexOf("WindowsForms10.STATIC.app.0.1e6fa8e") != -1)
                    {
                        if (count == 7)
                        {
                            //thuc hien lay class nay
                            String text = APIManager.GetControlText(item);
                            string resultString = Regex.Match(text, @"\d+").Value;
                            bool isRight = int.TryParse(resultString, out numberRead);
                            if (!isRight)
                            {
                                return;
                            }
                            break;
                        }
                        count++;
                    }
                }
            }
            else if (activeWindow.text.IndexOf("sua thong tin bd10") != -1)
            {
                isHaveError = false;
                int count = 0;
                int countEdit = 0;
                foreach (var item in allChild)
                {
                    string cWindow = APIManager.GetWindowClass(item);
                    if (cWindow.IndexOf("WindowsForms10.STATIC.app.0.1e6fa8e") != -1)
                    {
                        if (count == 9)
                        {
                            //thuc hien lay class nay
                            String text = APIManager.GetControlText(item);
                            string resultString = Regex.Match(text, @"\d+").Value;
                            bool isRight = int.TryParse(resultString, out numberRead);
                            if (!isRight)
                            {
                                timerRead.Stop();
                                System.Windows.MessageBox.Show("Không phải số. \n Vui lòng mở lại chương trình.");
                            }
                        }
                        count++;
                    }
                    if (cWindow == "Edit")
                    {
                        if (countEdit == 3)
                        {
                            string content = APIManager.GetControlText(item);
                            if (content.IndexOf("590100") != -1)
                            {
                                //txtInfo.Text = "Dang mo BD Nam Trung Bo";
                            }
                            else if (content.IndexOf("593330") != -1)
                            {
                                //txtInfo.Text = "Dang mo BD Tam Quan";
                            }
                        }
                        countEdit++;
                    }
                }
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

        DispatcherTimer timerRead;

        void ReadNumber()
        {

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

        private CurrentTab currentTab = CurrentTab.GetBD10;


        public string CountInBD { get => _CountInBD; set => SetProperty(ref _CountInBD, value); }
        public int IndexTabControl
        {
            get { return _IndexTabControl; }
            set { SetProperty(ref _IndexTabControl, value); }
        }

        public ICommand LoadPageCommand { get; }
        public SnackbarMessageQueue MessageQueue
        {
            get { return _MessageQueue; }
            set { SetProperty(ref _MessageQueue, value); }
        }

        public ICommand OnCloseWindowCommand { get; }
        public ICommand TabChangedCommand { get; }
        private void createConnection()
        {
            //var pathDB = System.IO.Path.Combine(Environment.CurrentDirectory, "dulieu.sqlite");
            //string _strConnect = @"DataSource=" + pathDB + ";Version=3";
            //_con = new SqliteConnection(_strConnect);
            //_con.ConnectionString = _strConnect;
            //_con.Open();
        }
        bool isSmallerWindow = false;

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

        public ICommand SmallerWindowCommand { get; }


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

        public IRelayCommand<System.Windows.Controls.TabControl> DefaultWindowCommand { get; }


        void DefaultWindow(System.Windows.Controls.TabControl tabControl)
        {
            //TabChanged(tabControl);
            SetDefaultWindowTui();
            isSmallWindow = false;

        }

        public ICommand TabTuiChangedCommand { get; }

        int lastSelectedTabTui = 0;
        void TabTuiChanged(System.Windows.Controls.TabControl tabControl)
        {
            if (tabControl == null)
                return;
            if(tabControl.SelectedIndex != lastSelectedTabTui)
            {
                lastSelectedTabTui = tabControl.SelectedIndex;
            }else
            {
                return;
            }

            switch (tabControl.SelectedIndex)
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
                    break;

                default:
                    break;
            }
        }




        private void OnKeyPress(object sender, KeyPressedArgs e)
        {
            CountInBD += e.KeyPressed.ToString();
            switch (e.KeyPressed)
            {
                case Key.F8:
                    //Thuc hien nay
                    WeakReferenceMessenger.Default.Send<MessageManager>(new MessageManager("getData"));
                    break;
                case Key.F5:
                    if (currentTab == CurrentTab.ThuGon)
                    {
                        //thuc hien liet ke danh sach 

                    }
                    else if (currentTab == CurrentTab.LayChuyenThu)
                    {
                        WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "PrintDiNgoai" });
                    }
                    break;
                case Key.F1:
                    WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "RunPrintDiNgoai", Content = "PrintDiNgoai" });
                    break;

                default:
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

        private void SetGetBD10Window()
        {
            if (_window == null)
                return;
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            _window.Width = 440;
            _window.Height = 300;
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
            switch (control.SelectedIndex)
            {
                case 0:
                    //thuc hien chuyen ve
                    SetGetBD10Window();
                    currentTab = CurrentTab.GetBD10;

                    break;

                case 1:
                    SetDanhSachBD10Window();
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
                    break;
                    currentTab = CurrentTab.LocTui;
                    SetChiTietWindow();
                case 6:
                    break;

                default:
                    break;
            }
        }

        private string _CountInBD;
        private int _IndexTabControl = 0;
        private Y2KeyboardHook _keyboardHook;
        private SnackbarMessageQueue _MessageQueue;
        private Window _window;
    }
}