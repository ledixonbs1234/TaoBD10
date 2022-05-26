using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;
using WindowsInput;
using WindowsInput.Native;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class ChinhViewModel : ObservableObject
    {
        private readonly DispatcherTimer timerPrint;

        private bool _IsChecking = true;

        public bool IsChecking
        {
            get { return _IsChecking; }
            set
            {
                SetProperty(ref _IsChecking, value);
                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "TopMost", Content = value.ToString() });

            }
        }
        BackgroundWorker bwPrint;

        public ChinhViewModel()
        {
            KTHNCommand = new RelayCommand(KTHN);
            TongHopCommand = new RelayCommand(TongHop);
            AnNhonCommand = new RelayCommand(AnNhon);
            PhuCatCommand = new RelayCommand(PhuCat);
            PhuMyCommand = new RelayCommand(PhuMy);
            QuyNhon2Command = new RelayCommand(QuyNhon2);
            QuiNhon1Command = new RelayCommand(QuiNhon1);
            EMSCommand = new RelayCommand(EMS);
            KienCommand = new RelayCommand(Kien);
            BCPHNCommand = new RelayCommand(BCPHN);
            HoaiAnCommand = new RelayCommand(HoaiAn);
            AnMyCommand = new RelayCommand(AnMy);
            AnLaoCommand = new RelayCommand(AnLao);
            AnHoaCommand = new RelayCommand(AnHoa);
            TamQuanCommand = new RelayCommand(TamQuan);
            LayDuLieuCommand = new RelayCommand(LayDuLieu);
            BD10DiCommand = new RelayCommand(BD10Di);
            BD10DenCommand = new RelayCommand(BD10Den);
            D420Command = new RelayCommand(D420);
            PrintDefaultCommand = new RelayCommand(PrintDefault);
            bwCreateChuyenThu = new BackgroundWorker();
            bwCreateChuyenThu.DoWork += BackgroundCreateChuyenThu_DoWork;
            bwPrint = new BackgroundWorker();
            bwPrint.DoWork += BwPrint_DoWork;
            timerPrint = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 200)
            };
            timerPrint.Tick += TimerPrint_Tick;

            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "Chinh")
                {
                    if (m.Content == "Kien")
                    {
                        Kien();
                    }
                    else if (m.Content == "EMS")
                    {
                        EMS();
                    }
                    else if (m.Content == "TamQuan")
                    {
                        TamQuan();
                    }
                    else if (m.Content == "QuiNhon1")
                    {
                        QuiNhon1();
                    }
                    else if (m.Content == "QuiNhon2")
                    {
                        QuyNhon2();
                    }
                    else if (m.Content == "PhuMy")
                    {
                        PhuMy();
                    }
                    else if (m.Content == "PhuCat")
                    {
                        PhuCat();
                    }
                    else if (m.Content == "AnNhon")
                    {
                        AnNhon();
                    }
                    else if (m.Content == "TongHop")
                    {
                        TongHop();
                    }
                    else if (m.Content == "HoaiAn")
                    {
                        HoaiAn();
                    }
                    else if (m.Content == "AnLao")
                    {
                        AnLao();
                    }
                    else if (m.Content == "AnMy")
                    {
                        AnMy();
                    }
                    else if (m.Content == "AnHoa")
                    {
                        AnHoa();
                    }
                    else if (m.Content == "LayDuLieu")
                    {
                        LayDuLieu();
                    }
                    else if (m.Content == "KT")
                    {
                        KTHN();
                    }
                    else if (m.Content == "BCP")
                    {
                        BCPHN();
                    }
                    else if (m.Content == "Print")
                    {
                        bwPrint.RunWorkerAsync();
                    }
                    else if (m.Content == "True")
                    {
                        IsChecking = true;

                    }
                    else if (m.Content == "False")
                    {
                        IsChecking = false;
                    }
                }
            });
        }

        private void BwPrint_DoWork(object sender, DoWorkEventArgs e)
        {
            var currentWindow = APIManager.WaitingFindedWindow("dong chuyen thu");
            if (currentWindow == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy window đóng chuyến thư");
                return;
            }

            System.Collections.Generic.List<TestAPIModel> allChild = APIManager.GetListControlText(currentWindow.hwnd);
            TestAPIModel grControl = allChild.FirstOrDefault(m => m.Text.IndexOf("gr") != -1);
            if (grControl == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy label gr");

                return;
            }
            string textGr = grControl.Text.Replace("(gr)", "");
            double numberGR = 0;
            if (textGr.IndexOf('.') == -1)
            {
                int.TryParse(textGr, out int numberGr);
                if (numberGr < 100)
                {
                    textGr = "0.2";
                    numberGR = 0.1;
                }
                else
                    textGr = "0." + textGr;
            }

            numberGR = double.Parse(textGr);

            SendKeys.SendWait("{F6}");

            SendKeys.SendWait("{F5}");
            SendKeys.SendWait("{LEFT}");
            SendKeys.SendWait("{LEFT}");
            SendKeys.SendWait("{LEFT}");
            SendKeys.SendWait("{LEFT}");
            SendKeys.SendWait("{LEFT}");
            SendKeys.SendWait("{LEFT}");
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(50);
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(50);
            SendKeys.SendWait((numberGR + 0.1).ToString());
            Thread.Sleep(50);
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(50);
            SendKeys.SendWait("{RIGHT}");
            Thread.Sleep(50);
            SendKeys.SendWait(" ");

            Thread.Sleep(500);
            SendKeys.SendWait("{F4}");
            Thread.Sleep(500);
            currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow.text.IndexOf("nhap thong tin khoi luong") != -1)
            {
                SendKeys.SendWait("{F10}");
                Thread.Sleep(200);
                SendKeys.SendWait("{ENTER}");
                Thread.Sleep(200);
            }

            Thread.Sleep(500);

            currentWindow = APIManager.GetActiveWindowTitle();

            if (currentWindow.text.IndexOf("tao tui") != -1)
            {
                return;
            }

            isRunFirst = false;
            isWaitingPrint = false;


            currentWindow = APIManager.GetActiveWindowTitle();

            if (currentWindow.text.IndexOf("dong chuyen thu") != -1)
            {

                bool isClosedTui = false;
                int countReturn = 50;
                while (!isClosedTui)
                {
                    countReturn--;
                    if (countReturn <= 0)
                        return;
                    Thread.Sleep(50);
                    allChild = APIManager.GetListControlText(currentWindow.hwnd);

                    TestAPIModel dongTuiControl = allChild.First(m => m.Text.IndexOf("F4") != -1);
                    if (dongTuiControl.Text.IndexOf("Mở túi") != -1)
                    {
                        isClosedTui = true;
                    }
                }

                if (!isClosedTui)
                {
                    return;
                }
                Thread.Sleep(100);


                SendKeys.SendWait("{F6}");
                SendKeys.SendWait("{F5}");
                SendKeys.SendWait("{F5}");
                SendKeys.SendWait("{DOWN}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");

                SendKeys.SendWait(" ");
                Thread.Sleep(200);
            }
            else
            {
                return;
            }

            SendKeys.SendWait("{F6}");
            Thread.Sleep(100);
            SendKeys.SendWait("{F7}");

            currentWindow = APIManager.WaitingFindedWindow("in an pham");
            if (currentWindow == null)
            {
                return;
            }
            APIManager.SetZ420Print();
            SendKeys.SendWait("{TAB}");
            Thread.Sleep(100);
            SendKeys.SendWait("{TAB}");
            Thread.Sleep(100);
            Thread thread = new Thread(() => System.Windows.Clipboard.Clear());
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();

            SendKeys.SendWait("^(a)");
            SendKeys.SendWait("^(c)");
            string clipboard = "";
            for (int i = 0; i < 10; i++)
            {
                thread = new Thread(() => clipboard = System.Windows.Clipboard.GetText());
                thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                thread.Start();
                thread.Join(); //Wait for the thread to end
                if (!string.IsNullOrEmpty(clipboard))
                {
                    break;
                }
                SendKeys.SendWait("^(c)");
                Thread.Sleep(50);

            }
            if (string.IsNullOrEmpty(clipboard))
                return;
            if (clipboard.IndexOf("BĐ8") == -1)
                return;

            foreach (string item in clipboard.Split('\n'))
            {
                var datas = item.Split('\t');
                if (datas[1].IndexOf("BĐ8") != -1)
                {
                    SendKeys.SendWait(" ");
                    break;
                }
                if (datas[4].IndexOf("BĐ8") != -1)
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

            currentWindow = APIManager.GetActiveWindowTitle();
            var childHandlesIn = APIManager.GetAllChildHandles(currentWindow.hwnd);
            IntPtr buttonThoat = IntPtr.Zero;
            foreach (var item in childHandlesIn)
            {
                string className = APIManager.GetWindowClass(item);
                string classDefault = "WindowsForms10.BUTTON.app.0.1e6fa8e";
                //string classDefault = "WindowsForms10.COMBOBOX.app.0.141b42a_r8_ad1";
                if (className == classDefault)
                {
                    buttonThoat = item;
                    break;
                }
            }

            SendKeys.SendWait("{F10}");
            if (buttonThoat != IntPtr.Zero)
            {
                APIManager.SendMessage(buttonThoat, 0x00F5, 0, 0);
            }
            APIManager.WaitingFindedWindow("dong chuyen thu");

            SendKeys.SendWait("{F10}");
            Thread.Sleep(200);
            SendKeys.SendWait("{F10}");
            Thread.Sleep(200);
            SendKeys.SendWait("{ENTER}");
            currentWindow = APIManager.WaitingFindedWindow("khoi tao chuyen thu");
            if (currentWindow == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy window khởi tạo chuyến thư");
                return;
            }
            System.Collections.Generic.List<TestAPIModel> childControls = APIManager.GetListControlText(currentWindow.hwnd);
            //thuc hien lay vi tri nao do

            APIManager.SendMessage(childControls[14].Handle, 0x0007, 0, 0);
            APIManager.SendMessage(childControls[14].Handle, 0x0007, 0, 0);

        }

        private void BackgroundCreateChuyenThu_DoWork(object sender, DoWorkEventArgs e)
        {
            //thuc hien cong viec trong nay
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "SetFalseKg", Content = "" });

            WindowInfo currentWindow = APIManager.WaitingFindedWindow("khoi tao chuyen thu");
            if (currentWindow == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy window khởi tạo chuyến thư");
                return;
            }
            System.Collections.Generic.List<TestAPIModel> childControls = APIManager.GetListControlText(currentWindow.hwnd);
            //thuc hien lay vi tri nao do

            APIManager.SendMessage(childControls[14].Handle, 0x0007, 0, 0);
            APIManager.SendMessage(childControls[14].Handle, 0x0007, 0, 0);            //thuc hien nhap vao
            var inputImulator = new InputSimulator();
            inputImulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
            inputImulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
            inputImulator.Keyboard.TextEntry(currentChuyenThu.NumberTinh);
            Thread.Sleep(50);
            inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);

            inputImulator.Keyboard.TextEntry(currentChuyenThu.TextLoai);
            inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
            inputImulator.Keyboard.KeyPress(VirtualKeyCode.F10);


            currentWindow = APIManager.WaitingFindedWindow("dong chuyen thu", "tao tui");
            if (currentWindow == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy window Đóng chuyến thư");
                return;
            }

            if (currentWindow.text.IndexOf("dong chuyen thu") != -1)
            {

                Thread.Sleep(500);
                //thuc hien cho tao tui 10 lan
                currentWindow = APIManager.GetActiveWindowTitle();
                if (currentWindow.text.IndexOf("tao tui") != -1)
                {
                    IntPtr handleTaoTui = APIManager.GetListControlText(currentWindow.hwnd)[8].Handle;


                    APIManager.SendMessage(handleTaoTui, 0x0007, 0, 0);
                    APIManager.SendMessage(handleTaoTui, 0x0007, 0, 0);
                    inputImulator.Keyboard.TextEntry(currentChuyenThu.TextTui);
                    inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
                    APIManager.ShowSnackbar(currentChuyenThu.TextTui);

                    inputImulator.Keyboard.KeyPress(VirtualKeyCode.F10);
                }
                else
                {

                }

                //thuc hien co tui trong nay

            }
            else if (currentWindow.text.IndexOf("tao tui") != -1)
            {
                Thread.Sleep(200);
                var childTaoTuiHandle = APIManager.GetAllChildHandles(currentWindow.hwnd);

                APIManager.SendMessage(childTaoTuiHandle[8], 0x0007, 0, 0);
                APIManager.SendMessage(childTaoTuiHandle[8], 0x0007, 0, 0);
                inputImulator.Keyboard.TextEntry(currentChuyenThu.TextTui);
                inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);

                inputImulator.Keyboard.KeyPress(VirtualKeyCode.F10);
            }

            currentWindow = APIManager.WaitingFindedWindow("dong chuyen thu");
            if (currentWindow == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy window Đóng chuyến thư");
                return;
            }

            var childHandles = APIManager.GetAllChildHandles(currentWindow.hwnd);
            int countEdit = 0;
            string tempCheckTinh = "";
            string tempCheckLoai = "";
            string tempCheckThuyBo = "";
            foreach (var item in childHandles)
            {
                string className = APIManager.GetWindowClass(item);
                string temp = APIManager.GetControlText(item);
                String text = APIManager.ConvertToUnSign3(temp).ToLower();

                string classDefault = "WindowsForms10.EDIT.app.0.1e6fa8e";
                if (className == classDefault)
                {
                    if (countEdit == 2)
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            tempCheckTinh = text;
                        }
                        else
                        {
                            countEdit = 1;
                        }

                    }
                    else if (countEdit == 3)
                    {
                        tempCheckLoai = text;
                    }
                    else if (countEdit == 4)
                    {
                        tempCheckThuyBo = text;
                        break;
                    }
                    countEdit++;
                }
            }
            if (tempCheckTinh.IndexOf(currentChuyenThu.CheckTinh) != -1 && tempCheckLoai.IndexOf(currentChuyenThu.CheckLoai) != -1 && tempCheckThuyBo.IndexOf(currentChuyenThu.CheckThuyBo) != -1)
            {
                SendKeys.SendWait("A{BS}{BS}");
                Thread.Sleep(700);
                SoundManager.playSound2(@"\music\" + currentChuyenThu.NameMusic + ".wav");
            }
            else
            {
                //Kiem tra lai
                SoundManager.playSound2(@"\Number\khongkhopsolieu.wav");
            }
        }

        public ICommand BD10DiCommand { get; }

        private void BD10Di()
        {
            if (!APIManager.ThoatToDefault("593230", "danh sach bd10 di"))
            {
                SendKeys.SendWait("3");
                Thread.Sleep(200);
                SendKeys.SendWait("2");
                PrintDefault();
            }
        }

        public ICommand BD10DenCommand { get; }

        private void BD10Den()
        {
            if (!APIManager.ThoatToDefault("593230", "danh sach bd10 den"))
            {
                SendKeys.SendWait("3");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
                PrintDefault();
            }
        }

        public ICommand D420Command { get; }

        private void D420()
        {
            APIManager.SetZ420Print();
        }

        public ICommand PrintDefaultCommand { get; }

        private void PrintDefault()
        {
            APIManager.SetDefaultPrint();
        }

        private PrintState printState = PrintState.CheckF;
        private bool isWaitingPrint = false;

        private bool isRunFirst = false;

        private void TimerPrint_Tick(object sender, EventArgs e)
        {
            if (isWaitingPrint)
            {
                return;
            }

            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;

            if (currentWindow == null)
            {
                return;
            }

            switch (printState)
            {
                case PrintState.CheckF:
                    if (currentWindow.text.IndexOf("dong chuyen thu") != -1)
                    {
                        //thuc hien loc du lieu con
                        var allChild = APIManager.GetAllChildHandles(currentWindow.hwnd);
                        int countGr = 0;
                        double numberGR = 0;
                        foreach (IntPtr item in allChild)
                        {

                            //thuc hien lay text cua handle item
                            String text = APIManager.GetControlText(item);
                            if (text.IndexOf("gr") != -1)
                            {
                                countGr++;
                                text = text.Replace("(gr)", "");
                                if (text.IndexOf('.') == -1)
                                {
                                    int.TryParse(text, out int number);
                                    if (number < 100)
                                    {
                                        text = "0.2";
                                        numberGR = 0.1;
                                    }
                                    else
                                        text = "0." + text;
                                }

                                if (double.TryParse(text, out numberGR))
                                {
                                    break;
                                }
                            }
                        }

                        SendKeys.SendWait("{F6}");

                        isWaitingPrint = true;
                        SendKeys.SendWait("{F5}");
                        SendKeys.SendWait("{LEFT}");
                        SendKeys.SendWait("{LEFT}");
                        SendKeys.SendWait("{LEFT}");
                        SendKeys.SendWait("{LEFT}");
                        SendKeys.SendWait("{LEFT}");
                        SendKeys.SendWait("{LEFT}");
                        SendKeys.SendWait("{RIGHT}");
                        Thread.Sleep(50);
                        SendKeys.SendWait("{RIGHT}");
                        Thread.Sleep(50);
                        SendKeys.SendWait((numberGR + 0.1).ToString());
                        Thread.Sleep(50);
                        SendKeys.SendWait("{RIGHT}");
                        Thread.Sleep(50);
                        SendKeys.SendWait("{RIGHT}");
                        Thread.Sleep(50);
                        SendKeys.SendWait(" ");

                        Thread.Sleep(500);
                        SendKeys.SendWait("{F4}");

                        printState = PrintState.DongTui;
                        isWaitingPrint = false;
                    }

                    break;

                case PrintState.DongTui:
                    Thread.Sleep(500);
                    if (currentWindow.text.IndexOf("nhap thong tin khoi luong") != -1)
                    {
                        isWaitingPrint = true;

                        SendKeys.SendWait("{F10}");
                        Thread.Sleep(200);
                        SendKeys.SendWait("{ENTER}");
                        Thread.Sleep(200);
                        printState = PrintState.CheckTaoTui;
                        isWaitingPrint = false;
                    }
                    else
                    {
                        printState = PrintState.CheckTaoTui;
                    }
                    break;

                case PrintState.CheckTaoTui:
                    isWaitingPrint = true;
                    Thread.Sleep(500);

                    if (currentWindow.text.IndexOf("tao tui") != -1)
                    {
                        timerPrint.Stop();
                    }
                    else
                    {
                        printState = PrintState.CheckButton;
                    }
                    isRunFirst = false;
                    isWaitingPrint = false;
                    break;

                case PrintState.CheckButton:
                    if (currentWindow.text.IndexOf("dong chuyen thu") != -1)
                    {
                        if (!isRunFirst)
                        {
                            isRunFirst = true;
                            return;
                        }

                        var allChild = APIManager.GetAllChildHandles(currentWindow.hwnd);
                        bool isClosedTui = false;
                        foreach (var item in allChild)
                        {
                            String text = APIManager.GetControlText(item);
                            if (text.IndexOf("F4") != -1)
                            {
                                if (text.IndexOf("Mở túi") != -1)
                                {
                                    isClosedTui = true;
                                    break;
                                }
                            }
                        }
                        if (!isClosedTui)
                        {
                            return;
                        }

                        isWaitingPrint = true;
                        Thread.Sleep(100);

                        SendKeys.SendWait("{F6}");
                        SendKeys.SendWait("{F5}");
                        SendKeys.SendWait("{F5}");
                        SendKeys.SendWait("{DOWN}");
                        SendKeys.SendWait("{LEFT}");
                        SendKeys.SendWait("{LEFT}");
                        SendKeys.SendWait("{LEFT}");
                        SendKeys.SendWait("{LEFT}");
                        SendKeys.SendWait("{LEFT}");

                        SendKeys.SendWait(" ");
                        Thread.Sleep(200);
                        printState = PrintState.ChuanBiInBD8;
                        isWaitingPrint = false;

                        break;
                    }
                    break;

                case PrintState.ChuanBiInBD8:
                    isWaitingPrint = true;
                    SendKeys.SendWait("{F6}");
                    Thread.Sleep(100);
                    SendKeys.SendWait("{F7}");
                    printState = PrintState.PrintBD8;
                    isWaitingPrint = false;
                    break;

                case PrintState.PrintBD8:
                    if (currentWindow.text.IndexOf("in an pham") != -1)
                    {
                        APIManager.SetZ420Print();
                        isWaitingPrint = true;
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(100);
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(100);

                        SendKeys.SendWait("^(a)");
                        SendKeys.SendWait("^(c)");
                        Thread.Sleep(200);
                        string clipboard = System.Windows.Clipboard.GetText();
                        if (string.IsNullOrEmpty(clipboard))
                        {
                            timerPrint.Stop();
                            isWaitingPrint = false;
                            return;
                        }
                        if (clipboard.IndexOf("BĐ8") == -1)
                        {
                            timerPrint.Stop();
                            isWaitingPrint = false;
                            return;
                        }
                        foreach (string item in clipboard.Split('\n'))
                        {
                            var datas = item.Split('\t');
                            if (datas[1].IndexOf("BĐ8") != -1)
                            {
                                SendKeys.SendWait(" ");
                                break;
                            }
                            if (datas[4].IndexOf("BĐ8") != -1)
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

                        var childHandlesIn = APIManager.GetAllChildHandles(currentWindow.hwnd);
                        IntPtr buttonThoat = IntPtr.Zero;
                        foreach (var item in childHandlesIn)
                        {
                            string className = APIManager.GetWindowClass(item);
                            string classDefault = "WindowsForms10.BUTTON.app.0.1e6fa8e";
                            //string classDefault = "WindowsForms10.COMBOBOX.app.0.141b42a_r8_ad1";
                            if (className == classDefault)
                            {
                                buttonThoat = item;
                                break;
                            }
                        }

                        SendKeys.SendWait("{F10}");
                        if (buttonThoat != IntPtr.Zero)
                        {
                            APIManager.SendMessage(buttonThoat, 0x00F5, 0, 0);
                        }
                        printState = PrintState.DongChuyenThu;
                        isRunFirst = false;
                        isWaitingPrint = false;
                    }
                    break;

                case PrintState.DongChuyenThu:
                    if (currentWindow.text.IndexOf("dong chuyen thu") != -1)
                    {
                        if (!isRunFirst)
                        {
                            isRunFirst = true;
                            return;
                        }

                        isWaitingPrint = true;
                        SendKeys.SendWait("{F10}");
                        Thread.Sleep(200);
                        SendKeys.SendWait("{F10}");
                        Thread.Sleep(200);
                        SendKeys.SendWait("{ENTER}");
                        printState = PrintState.DaThoat;
                        isWaitingPrint = false;
                    }

                    break;

                case PrintState.DaThoat:
                    if (currentWindow.text.IndexOf("khoi tao chuyen") != -1)
                    {
                        var childHandles3 = APIManager.GetAllChildHandles(currentWindow.hwnd);
                        int countCombobox = 0;
                        IntPtr tinh = IntPtr.Zero;
                        foreach (var item in childHandles3)
                        {
                            string className = APIManager.GetWindowClass(item);
                            string classDefault = "WindowsForms10.COMBOBOX.app.0.1e6fa8e";
                            //string classDefault = "WindowsForms10.COMBOBOX.app.0.141b42a_r8_ad1";
                            if (className == classDefault)
                            {
                                if (countCombobox == 2)
                                {
                                    tinh = item;
                                    break;
                                }
                                countCombobox++;
                            }
                        }
                        APIManager.SendMessage(tinh, 0x0007, 0, 0);
                        APIManager.SendMessage(tinh, 0x0007, 0, 0);
                        timerPrint.Stop();
                    }
                    timerPrint.Stop();

                    break;

                default:
                    break;
            }
        }

        public ICommand LayDuLieuCommand { get; }

        private void LayDuLieu()
        {
            APIManager.ThoatToDefault("593200", "Default");
            SendKeys.SendWait("1");
            Thread.Sleep(200);
            SendKeys.SendWait("9");
            Thread.Sleep(200);
            SendKeys.SendWait("{F4}");
            Thread.Sleep(1000);
            SendKeys.SendWait("{F10}");
        }

        public ICommand TamQuanCommand { get; }

        private void TamQuan()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "Tam Quan Tổng Hợp",
                NumberTinh = "593330",
                TextLoai = "Bưu phẩm bảo đảm - Registed Mail",
                TextTui = "Tổng hợp (Túi)",
                CheckTinh = "593330",
                CheckLoai = "buu pham bao dam",
                CheckThuyBo = "thuy bo",
                NameMusic = "tuitamquanth"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        public ICommand AnHoaCommand { get; }
        public ICommand AnLaoCommand { get; }
        public ICommand AnMyCommand { get; }
        public ICommand AnNhonCommand { get; }
        public ICommand BCPHNCommand { get; }
        public ICommand EMSCommand { get; }
        public ICommand HoaiAnCommand { get; }
        public ICommand KienCommand { get; }
        public ICommand KTHNCommand { get; }
        public ICommand PhuCatCommand { get; }
        public ICommand PhuMyCommand { get; }
        public ICommand QuiNhon1Command { get; }
        public ICommand QuyNhon2Command { get; }
        public ICommand TongHopCommand { get; }

        private void AnHoa()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "An Hòa Tổng Hợp",
                NumberTinh = "593880",
                TextLoai = "Bưu phẩm bảo đảm - Registed Mail",
                TextTui = "Tổng hợp (Túi)",
                CheckTinh = "593880",
                CheckLoai = "buu pham bao dam",
                CheckThuyBo = "thuy bo",
                NameMusic = "anhoath"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private void AnLao()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "An Lão Tổng Hợp",
                NumberTinh = "593850",
                TextLoai = "Bưu phẩm bảo đảm - Registed Mail",
                TextTui = "Tổng hợp (Túi)",
                CheckTinh = "593850",
                CheckLoai = "buu pham bao dam",
                CheckThuyBo = "thuy bo",
                NameMusic = "anlaoth"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private void AnMy()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "Ân Mỹ Tổng Hợp",
                NumberTinh = "593630",
                TextLoai = "Bưu phẩm bảo đảm - Registed Mail",
                TextTui = "Tổng hợp (Túi)",
                CheckTinh = "593630",
                CheckLoai = "buu pham bao dam",
                CheckThuyBo = "thuy bo",
                NameMusic = "anmyth"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private void AnNhon()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "An Nhơn Tổng Hợp",
                NumberTinh = "592020",
                TextLoai = "Bưu phẩm bảo đảm - Registed Mail",
                TextTui = "Tổng hợp (Túi)",
                CheckTinh = "592020",
                CheckLoai = "buu pham bao dam",
                CheckThuyBo = "thuy bo",
                NameMusic = "annhonth"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private void BCPHN()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "Khai Thác Hoài Nhơn",
                NumberTinh = "593280",
                TextLoai = "Bưu kiện - Parcel",
                TextTui = "",
                CheckTinh = "593280",
                CheckLoai = "buu kien",
                CheckThuyBo = "thuy bo",
                NameMusic = "buucucphatk"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        BackgroundWorker bwCreateChuyenThu;

        private void EMS()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "EMS Nam Trung Bộ",
                NumberTinh = "590100",
                TextLoai = "EMS - Chuyển phát nhanh - Express Mail Service",
                TextTui = "EMS Hàng hóa (Túi)",
                CheckTinh = "590100",
                CheckLoai = "chuyen phat nhanh",
                CheckThuyBo = "ems thuong",
                NameMusic = "emsntbth"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private void HoaiAn()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "Hoài Ân Tổng Hợp",
                NumberTinh = "593740",
                TextLoai = "Bưu phẩm bảo đảm - Registed Mail",
                TextTui = "Tổng hợp (Túi)",
                CheckTinh = "593740",
                CheckLoai = "buu pham bao dam",
                CheckThuyBo = "thuy bo",
                NameMusic = "hoaianth"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private void Kien()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "Kiện Nam Trung Bộ",
                NumberTinh = "590100",
                TextLoai = "Bưu kiện - Parcel",
                TextTui = "BK (Túi)",
                CheckTinh = "590100",
                CheckLoai = "buu kien",
                CheckThuyBo = "thuy bo",
                NameMusic = "kiennamtrungboth"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private void KTHN()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "Khai Thác Hoài Nhơn",
                NumberTinh = "593230",
                TextLoai = "Bưu kiện - Parcel",
                TextTui = "",
                CheckTinh = "593230",
                CheckLoai = "buu kien",
                CheckThuyBo = "thuy bo",
                NameMusic = "khaithack"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593200", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("3");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private void PhuCat()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "Phù Cát Tổng Hợp",
                NumberTinh = "592460",
                TextLoai = "Bưu phẩm bảo đảm - Registed Mail",
                TextTui = "Tổng hợp (Túi)",
                CheckTinh = "592460",
                CheckLoai = "buu pham bao dam",
                CheckThuyBo = "thuy bo",
                NameMusic = "phucatth"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private void PhuMy()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "Phù Mỹ Tổng Hợp",
                NumberTinh = "592810",
                TextLoai = "Bưu phẩm bảo đảm - Registed Mail",
                TextTui = "Tổng hợp (Túi)",
                CheckTinh = "592810",
                CheckLoai = "buu pham bao dam",
                CheckThuyBo = "thuy bo",
                NameMusic = "phumyth"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private void QuiNhon1()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "Quy Nhơn 1 Tổng Hợp",
                NumberTinh = "591520",
                TextLoai = "Bưu phẩm bảo đảm - Registed Mail",
                TextTui = "Tổng hợp (Túi)",
                CheckTinh = "591520",
                CheckLoai = "buu pham bao dam",
                CheckThuyBo = "thuy bo",
                NameMusic = "qn1th"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private void QuyNhon2()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "Quy Nhơn 2 Tổng Hợp",
                NumberTinh = "591218",
                TextLoai = "Bưu phẩm bảo đảm - Registed Mail",
                TextTui = "Tổng hợp (Túi)",
                CheckTinh = "591218",
                CheckLoai = "buu pham bao dam",
                CheckThuyBo = "thuy bo",
                NameMusic = "tuiqn2th"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private void TongHop()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel
            {
                Ten = "Nam Trung Bộ TH",
                NumberTinh = "590100",
                TextLoai = "Bưu phẩm bảo đảm - Registed Mail",
                TextTui = "Tổng hợp (Túi)",
                CheckTinh = "590100",
                CheckLoai = "buu pham bao dam",
                CheckThuyBo = "thuy bo",
                NameMusic = "ntbth"
            };
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            bwCreateChuyenThu.RunWorkerAsync();
        }

        private ChuyenThuModel currentChuyenThu;
    }
}