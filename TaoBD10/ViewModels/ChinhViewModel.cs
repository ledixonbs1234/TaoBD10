using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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

        DispatcherTimer timerPrint;
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

            timerPrint = new DispatcherTimer();
            timerPrint.Interval = new TimeSpan(0, 0, 0, 0, 200);
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

                    }else if (m.Content == "LayDuLieu")
                    {
                        LayDuLieu();
                    }else if (m.Content == "KT")
                    {
                        KTHN();
                    }
                    else if(m.Content == "Print")
                    {
                        timerPrint.Stop();
                        timerPrint.Start();
                    }
                }

            });




        }

        PrintState printState = PrintState.CheckF;
        bool isWaitingPrint = false;

       bool isRunFirst = false;

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
                        foreach (var item in allChild)
                        {
                            int count = 0;

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
                        string clipboard = Clipboard.GetText();
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
                        SoundManager.playSync(@"music\thoatthu.wav");
                        timerPrint.Stop();
                    }
                    timerPrint.Stop();

                    break;

                default:
                    break;
            }
        }

        public ICommand LayDuLieuCommand { get; }


        void LayDuLieu()
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


        void TamQuan()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "Tam Quan Tổng Hợp";
            chuyenThu.NumberTinh = "593330";
            chuyenThu.TextLoai = "Bưu phẩm bảo";
            chuyenThu.TextTui = "Tổng hợp (Túi";
            chuyenThu.CheckTinh = "593330";
            chuyenThu.CheckLoai = "buu pham bao dam";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "tuitamquanth";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();
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
        void AnHoa()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "An Hòa Tổng Hợp";
            chuyenThu.NumberTinh = "593880";
            chuyenThu.TextLoai = "Bưu phẩm bảo";
            chuyenThu.TextTui = "Tổng hợp (Túi";
            chuyenThu.CheckTinh = "593880";
            chuyenThu.CheckLoai = "buu pham bao dam";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "anhoath";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();
        }

        void AnLao()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "An Lão Tổng Hợp";
            chuyenThu.NumberTinh = "593850";
            chuyenThu.TextLoai = "Bưu phẩm bảo";
            chuyenThu.TextTui = "Tổng hợp (Túi";
            chuyenThu.CheckTinh = "593850";
            chuyenThu.CheckLoai = "buu pham bao dam";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "anlaoth";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();
        }

        void AnMy()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "Ân Mỹ Tổng Hợp";
            chuyenThu.NumberTinh = "593630";
            chuyenThu.TextLoai = "Bưu phẩm bảo";
            chuyenThu.TextTui = "Tổng hợp (Túi";
            chuyenThu.CheckTinh = "593630";
            chuyenThu.CheckLoai = "buu pham bao dam";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "anmyth";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();
        }

        void AnNhon()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "An Nhơn Tổng Hợp";
            chuyenThu.NumberTinh = "592020";
            chuyenThu.TextLoai = "Bưu phẩm bảo";
            chuyenThu.TextTui = "Tổng hợp (Túi";
            chuyenThu.CheckTinh = "592020";
            chuyenThu.CheckLoai = "buu pham bao dam";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "annhonth";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();

        }

        void BCPHN()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "Khai Thác Hoài Nhơn";
            chuyenThu.NumberTinh = "593280";
            chuyenThu.TextLoai = "Bưu kiện";
            chuyenThu.TextTui = "";
            chuyenThu.CheckTinh = "593280";
            chuyenThu.CheckLoai = "buu kien";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "buucucphatk";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();


        }

        void CreateChuyenThu()
        {
            //thuc hien cong viec trong nay
            WindowInfo currentWindow = APIManager.GetActiveWindowTitle(true);
            if (currentWindow == null)
            {
                return;
            }
            int countTempReturn = 0;

            while (currentWindow.text.IndexOf("Khởi tạo chuyến thư") == -1)
            {
                currentWindow = APIManager.GetActiveWindowTitle(true);
                if (currentWindow == null)
                {
                    return;
                }
                countTempReturn++;
                if (countTempReturn > 50)
                {
                    return;
                }
                Thread.Sleep(100);
            }


            Thread.Sleep(200);

            var childsHandle = APIManager.GetAllChildHandles(currentWindow.hwnd);
            //thuc hien lay vi tri nao do
            int indexTinh = 14;
            IntPtr Tinh = childsHandle[indexTinh];
            Thread.Sleep(100);


            APIManager.SendMessage(Tinh, 0x0007, 0, 0);
            APIManager.SendMessage(Tinh, 0x0007, 0, 0);            //thuc hien nhap vao
            var inputImulator = new InputSimulator();
            inputImulator.Keyboard.TextEntry(currentChuyenThu.NumberTinh);
            inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);

            inputImulator.Keyboard.TextEntry(currentChuyenThu.TextLoai);
            inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
            inputImulator.Keyboard.KeyPress(VirtualKeyCode.F10);
            countTempReturn = 0;

            while (currentWindow.text.IndexOf("Khởi tạo chuyến thư") != -1)
            {
                currentWindow = APIManager.GetActiveWindowTitle(true);
                countTempReturn++;
                if (countTempReturn > 30)
                {
                    return;
                }
                Thread.Sleep(100);
            }
            Thread.Sleep(200);


            currentWindow = APIManager.GetActiveWindowTitle(true);
            if (currentWindow == null)
            {
                return;
            }
            if (currentWindow.text.IndexOf("Đóng chuyến thư") != -1)
            {

                int countInTui = 0;
                childsHandle = APIManager.GetAllChildHandles(currentWindow.hwnd);
                string title = APIManager.GetControlText(childsHandle[3]);
                //kiem tra so luong la bao nhieu
                string resultString = Regex.Match(title, @"\d+").Value;
                bool isRight = int.TryParse(resultString, out countInTui);

                if (countInTui > 0)
                {

                }
                else
                {
                    //thuc hien cho tao tui 10 lan
                    countTempReturn = 0;
                    while (currentWindow.text.IndexOf("Tạo túi") == -1)
                    {
                        currentWindow = APIManager.GetActiveWindowTitle(true);
                        if (currentWindow == null)
                        {
                            return;
                        }
                        countTempReturn++;
                        if (countTempReturn > 10)
                        {
                            return;
                        }
                        Thread.Sleep(200);
                    }

                    //thuc hien co tui trong nay
                    var childTaoTuiHandle = APIManager.GetAllChildHandles(currentWindow.hwnd);
                    IntPtr loai1 = IntPtr.Zero;

                    if (currentChuyenThu.Ten == "EMS Nam Trung Bộ" || currentChuyenThu.Ten == "Kiện Nam Trung Bộ")
                    { }
                    else
                    {
                        APIManager.SendMessage(childTaoTuiHandle[8], 0x0007, 0, 0);
                        APIManager.SendMessage(childTaoTuiHandle[8], 0x0007, 0, 0);
                        inputImulator.Keyboard.TextEntry(currentChuyenThu.TextTui);
                        inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
                    }
                    inputImulator.Keyboard.KeyPress(VirtualKeyCode.F10);
                }

            }
            else
            if (currentWindow.text.IndexOf("Tạo túi") != -1)
            {
                var childTaoTuiHandle = APIManager.GetAllChildHandles(currentWindow.hwnd);
                IntPtr loai1 = IntPtr.Zero;

                if (currentChuyenThu.Ten == "EMS Nam Trung Bộ" || currentChuyenThu.Ten == "Kiện Nam Trung Bộ")
                { }
                else
                {
                    APIManager.SendMessage(childTaoTuiHandle[8], 0x0007, 0, 0);
                    APIManager.SendMessage(childTaoTuiHandle[8], 0x0007, 0, 0);
                    inputImulator.Keyboard.TextEntry(currentChuyenThu.TextTui);
                    inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);
                }
                inputImulator.Keyboard.KeyPress(VirtualKeyCode.F10);
            }
            countTempReturn = 0;
            if (currentWindow == null)
            {
                return;
            }
            while (currentWindow.text.IndexOf("Đóng chuyến thư") == -1)
            {
                currentWindow = APIManager.GetActiveWindowTitle(true);
                countTempReturn++;
                if (countTempReturn > 30)
                {
                    return;
                }
                Thread.Sleep(100);
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
                String text = APIManager.convertToUnSign3(temp).ToLower();

                string classDefault = "WindowsForms10.EDIT.app.0.1e6fa8e";
                if (className == classDefault)
                {
                    if (countEdit == 2)
                    {
                        tempCheckTinh = text;
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
                SoundManager.playSound(@"\music\" + currentChuyenThu.NameMusic + ".wav");
            }
            else
            {
                //Kiem tra lai
            }
        }

        void EMS()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "EMS Nam Trung Bộ";
            chuyenThu.NumberTinh = "590100";
            chuyenThu.TextLoai = "EMS";
            chuyenThu.TextTui = "EMS H";
            chuyenThu.CheckTinh = "590100";
            chuyenThu.CheckLoai = "chuyen phat nhanh";
            chuyenThu.CheckThuyBo = "ems thuong";
            chuyenThu.NameMusic = "emsntbth";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();

        }

        void HoaiAn()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "Hoài Ân Tổng Hợp";
            chuyenThu.NumberTinh = "593740";
            chuyenThu.TextLoai = "Bưu phẩm bảo";
            chuyenThu.TextTui = "Tổng hợp (Túi";
            chuyenThu.CheckTinh = "593740";
            chuyenThu.CheckLoai = "buu pham bao dam";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "hoaianth";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();
        }

        void Kien()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "Kiện Nam Trung Bộ";
            chuyenThu.NumberTinh = "590100";
            chuyenThu.TextLoai = "Bưu kiện";
            chuyenThu.TextTui = "BK (";
            chuyenThu.CheckTinh = "590100";
            chuyenThu.CheckLoai = "buu kien";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "kiennamtrungboth";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();

        }

        void KTHN()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "Khai Thác Hoài Nhơn";
            chuyenThu.NumberTinh = "593230";
            chuyenThu.TextLoai = "Bưu kiện";
            chuyenThu.TextTui = "";
            chuyenThu.CheckTinh = "593230";
            chuyenThu.CheckLoai = "buu kien";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "khaithack";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593200", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("3");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();

        }

        void PhuCat()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "Phù Cát Tổng Hợp";
            chuyenThu.NumberTinh = "592460";
            chuyenThu.TextLoai = "Bưu phẩm bảo";
            chuyenThu.TextTui = "Tổng hợp (Túi";
            chuyenThu.CheckTinh = "592460";
            chuyenThu.CheckLoai = "buu pham bao dam";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "phucatth";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();

        }

        void PhuMy()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "Phù Mỹ Tổng Hợp";
            chuyenThu.NumberTinh = "592810";
            chuyenThu.TextLoai = "Bưu phẩm bảo";
            chuyenThu.TextTui = "Tổng hợp (Túi";
            chuyenThu.CheckTinh = "592810";
            chuyenThu.CheckLoai = "buu pham bao dam";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "phumyth";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();

        }

        void QuiNhon1()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "Quy Nhơn 1 Tổng Hợp";
            chuyenThu.NumberTinh = "591520";
            chuyenThu.TextLoai = "Bưu phẩm bảo";
            chuyenThu.TextTui = "Tổng hợp (Túi";
            chuyenThu.CheckTinh = "591520";
            chuyenThu.CheckLoai = "buu pham bao dam";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "qn1th";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();

        }

        void QuyNhon2()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "Quy Nhơn 2 Tổng Hợp";
            chuyenThu.NumberTinh = "591218";
            chuyenThu.TextLoai = "Bưu phẩm bảo";
            chuyenThu.TextTui = "Tổng hợp (Túi";
            chuyenThu.CheckTinh = "591218";
            chuyenThu.CheckLoai = "buu pham bao dam";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "tuiqn2th";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();

        }

        void TongHop()
        {
            ChuyenThuModel chuyenThu = new ChuyenThuModel();
            chuyenThu.Ten = "Nam Trung Bộ TH";
            chuyenThu.NumberTinh = "590100";
            chuyenThu.TextLoai = "Bưu phẩm bảo";
            chuyenThu.TextTui = "Tổng hợp (Túi";
            chuyenThu.CheckTinh = "590100";
            chuyenThu.CheckLoai = "buu pham bao dam";
            chuyenThu.CheckThuyBo = "thuy bo";
            chuyenThu.NameMusic = "ntbth";
            currentChuyenThu = chuyenThu;

            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            CreateChuyenThu();
        }

        private ChuyenThuModel currentChuyenThu;
    }
}
