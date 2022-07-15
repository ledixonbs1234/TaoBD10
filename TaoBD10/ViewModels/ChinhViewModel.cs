using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;
using WindowsInput;
using WindowsInput.Native;

namespace TaoBD10.ViewModels
{
    public class ChinhViewModel : ObservableObject
    {
        public ICommand ChuyenThu13Command { get; }


        void ChuyenThu13()
        {
            if (ChuyenThus.Count >= 14)
            {
                currentChuyenThu = ChuyenThus[13];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        public ChinhViewModel()
        {
            ChuyenThus = new ObservableCollection<ChuyenThuModel>();
            System.Collections.Generic.List<ChuyenThuModel> cts = FileManager.LoadCT();
            if (cts != null)
                if (cts.Count != 0)
                {
                    foreach (ChuyenThuModel item in cts)
                    {
                        ChuyenThus.Add(item);
                    }
                }

            XuongCommand = new RelayCommand(Xuong);
            LenCommand = new RelayCommand(Len);
            //thuc hien lay du lieu tu web
            ChuyenThu4Command = new RelayCommand(ChuyenThu4);
            ChuyenThu3Command = new RelayCommand(ChuyenThu3);
            ChuyenThu0Command = new RelayCommand(ChuyenThu0);
            ChuyenThu1Command = new RelayCommand(ChuyenThu1);
            ChuyenThu2Command = new RelayCommand(ChuyenThu2);
            ChuyenThu8Command = new RelayCommand(ChuyenThu8);
            ChuyenThu9Command = new RelayCommand(ChuyenThu9);
            ChuyenThu12Command = new RelayCommand(ChuyenThu12);
            ChuyenThu11Command = new RelayCommand(ChuyenThu11);
            ChuyenThu10Command = new RelayCommand(ChuyenThu10);
            ChuyenThu7Command = new RelayCommand(ChuyenThu7);
            ChuyenThu6Command = new RelayCommand(ChuyenThu6);
            ChuyenThu5Command = new RelayCommand(ChuyenThu5);
            ChuyenThu13Command = new RelayCommand(ChuyenThu13);

            NewCTCommand = new RelayCommand(NewCT);
            SaveCTCommand = new RelayCommand(SaveCT);

            KTHNCommand = new RelayCommand(KTHN);

            AutoXacNhanCommand = new RelayCommand(AutoXacNhan);

            LayDuLieuCommand = new RelayCommand(LayDuLieu);
            BD10DiCommand = new RelayCommand(BD10Di);
            BD10DenCommand = new RelayCommand(BD10Den);
            D420Command = new RelayCommand(D420);
            PrintDefaultCommand = new RelayCommand(PrintDefault);
            bwCreateChuyenThu = new BackgroundWorker();
            bwCreateChuyenThu.DoWork += BackgroundCreateChuyenThu_DoWork;
            bwCreateChuyenThu.WorkerSupportsCancellation = true;
            bwPrint = new BackgroundWorker();
            bwPrint.DoWork += BwPrint_DoWork;

            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "Chinh")
                {
                    if (m.Content == "ChuyenThu0")
                    {
                        ChuyenThu0();
                    }
                    else if (m.Content == "ChuyenThu1")
                    {
                        ChuyenThu1();
                    }
                    else if (m.Content == "ChuyenThu2")
                    {
                        ChuyenThu2();
                    }
                    else if (m.Content == "ChuyenThu3")
                    {
                        ChuyenThu3();
                    }
                    else if (m.Content == "ChuyenThu4")
                    {
                        ChuyenThu4();
                    }
                    else if (m.Content == "ChuyenThu5")
                    {
                        ChuyenThu5();
                    }
                    else if (m.Content == "ChuyenThu6")
                    {
                        ChuyenThu6();
                    }
                    else if (m.Content == "ChuyenThu7")
                    {
                        ChuyenThu7();
                    }
                    else if (m.Content == "ChuyenThu8")
                    {
                        ChuyenThu8();
                    }
                    else if (m.Content == "ChuyenThu9")
                    {
                        ChuyenThu9();
                    }
                    else if (m.Content == "ChuyenThu10")
                    {
                        ChuyenThu10();
                    }
                    else if (m.Content == "ChuyenThu11")
                    {
                        ChuyenThu11();
                    }
                    else if (m.Content == "ChuyenThu12")
                    {
                        ChuyenThu12();
                    }
                    else if (m.Content == "ChuyenThu13")
                    {
                        ChuyenThu13();
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
                }
                else if (m.Key == "XN593200")
                {
                    XacNhanChiTiet200();
                }
            });
        }
        private int _SelectedIndexCT;

        public int SelectedIndexCT
        {
            get { return _SelectedIndexCT; }
            set { SetProperty(ref _SelectedIndexCT, value); }
        }


        void Len()
        {
            if (SelectedIndexCT == -1)
                return;
            if (SelectedIndexCT == 0)
                return;
            int tempSelected = SelectedIndexCT;
            ChuyenThuModel currentCT = ChuyenThus[SelectedIndexCT];
            ChuyenThuModel tempCT = ChuyenThus[SelectedIndexCT - 1];
            ChuyenThus[SelectedIndexCT - 1] = currentCT;
            ChuyenThus[SelectedIndexCT] = tempCT;
            SelectedIndexCT = tempSelected - 1;
        }
        public ICommand XuongCommand { get; }
        public ICommand LenCommand { get; }


        void Xuong()
        {
            if (SelectedIndexCT == -1)
                return;
            if (SelectedIndexCT == ChuyenThus.Count -1)
                return;
            int tempSelected = SelectedIndexCT;
            ChuyenThuModel currentCT = ChuyenThus[SelectedIndexCT];
            ChuyenThuModel tempCT = ChuyenThus[SelectedIndexCT + 1];
            ChuyenThus[SelectedIndexCT + 1] = currentCT;
            ChuyenThus[SelectedIndexCT] = tempCT;
            SelectedIndexCT = tempSelected + 1;
        }





        private void AutoXacNhan()
        {
            IntPtr handleWindow = APIManager.SetToLastWindow("593200");
            if (handleWindow == IntPtr.Zero)
                return;
            WindowInfo currentWindow = APIManager.GetActiveWindowTitle();
            if (APIManager.BoDauAndToLower(currentWindow.text).IndexOf("dong chuyen thu") == -1)
                return;
            //thuc hien cong viec trong nay khong co gi la khong the neu chung ta khong lam duoc
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
            SendKeys.SendWait("{ESC}");

            //thuc hien nhan button get chuyen thu 593200;
            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Button593200" });

            //khi con trong chuyen thu thuc hien tu dong dong chuyen thu va lay du lieu chuyen thu
            //tu dong thoat chuyen thu va truyen
            // qua 280 tu dong lay chuyen thu
            //tu dong xac nhan chi tiet tui thu
        }

        private void BackgroundCreateChuyenThu_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //thuc hien cong viec trong nay
                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "SetFalseKg", Content = "" });

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
                    Thread.Sleep(300);
                    //thuc hien cho tao tui 10 lan
                    currentWindow = APIManager.GetActiveWindowTitle();
                    if (currentWindow.text.IndexOf("tao tui") != -1)
                    {
                        IntPtr handleTaoTui = APIManager.GetListControlText(currentWindow.hwnd)[8].Handle;

                        APIManager.SendMessage(handleTaoTui, 0x0007, 0, 0);
                        APIManager.SendMessage(handleTaoTui, 0x0007, 0, 0);
                        inputImulator.Keyboard.TextEntry(currentChuyenThu.TextTui);
                        inputImulator.Keyboard.KeyPress(VirtualKeyCode.TAB);

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
                    string text = APIManager.ConvertToUnSign3(temp).ToLower();

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
                    SendKeys.SendWait("T{BS}{BS}");
                    SoundManager.playSound2(@"\music\" + currentChuyenThu.NameMusic + ".wav");
                }
                else
                {
                    //Kiem tra lai
                    SoundManager.playSound2(@"\Number\khongkhopsolieu.wav");
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
                APIManager.OpenNotePad(ex.Message + '\n' + "loi Line  ChinhViewModel" + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
            }
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
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

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
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{DOWN}");

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
            APIManager.SetPrintBD8();
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
            //Chinh sua trong nay
            SendKeys.SendWait("{F10}");
            if (buttonThoat != IntPtr.Zero)
            {
                APIManager.SendMessage(buttonThoat, 0x00F5, 0, 0);
            }

            currentWindow = APIManager.WaitingFindedWindow("dong chuyen thu");

            if (currentWindow == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy window đóng chuyến thư");
                return;
            }

            SendKeys.SendWait("{F10}");
            Thread.Sleep(200);
            currentWindow = APIManager.WaitingFindedWindow("xac nhan");
            if (currentWindow == null)
                return;
            APIManager.ClickButton(currentWindow.hwnd, "yes", isExactly: false);
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

        void ChuyenThu0()
        {
            if (ChuyenThus.Count >= 1)
            {
                currentChuyenThu = ChuyenThus[0];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        void ChuyenThu1()
        {
            if (ChuyenThus.Count >= 2)
            {
                currentChuyenThu = ChuyenThus[1];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        void ChuyenThu10()
        {
            if (ChuyenThus.Count >= 11)
            {
                currentChuyenThu = ChuyenThus[10];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        void ChuyenThu11()
        {
            if (ChuyenThus.Count >= 12)
            {
                currentChuyenThu = ChuyenThus[11];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        void ChuyenThu12()
        {
            if (ChuyenThus.Count >= 13)
            {
                currentChuyenThu = ChuyenThus[12];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        void ChuyenThu2()
        {
            if (ChuyenThus.Count >= 3)
            {
                currentChuyenThu = ChuyenThus[2];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        void ChuyenThu3()
        {
            if (ChuyenThus.Count >= 4)
            {
                currentChuyenThu = ChuyenThus[3];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        void ChuyenThu4()
        {
            if (ChuyenThus.Count >= 5)
            {
                currentChuyenThu = ChuyenThus[4];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        void ChuyenThu5()
        {
            if (ChuyenThus.Count >= 6)
            {
                currentChuyenThu = ChuyenThus[5];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        void ChuyenThu6()
        {
            if (ChuyenThus.Count >= 7)
            {
                currentChuyenThu = ChuyenThus[6];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        void ChuyenThu7()
        {
            if (ChuyenThus.Count >= 8)
            {
                currentChuyenThu = ChuyenThus[7];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        void ChuyenThu8()
        {
            if (ChuyenThus.Count >= 9)
            {
                currentChuyenThu = ChuyenThus[8];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        void ChuyenThu9()
        {
            if (ChuyenThus.Count >= 10)
            {
                currentChuyenThu = ChuyenThus[9];
            }
            if (!APIManager.ThoatToDefault("593230", "khoi tao chuyen thu"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("1");
            }
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        private void D420()
        {
            APIManager.SetPrintBD8();
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
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

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

        void NewCT()
        {


        }


        private void PrintDefault()
        {
            APIManager.SetPrintBD10();
        }


        void SaveCT()
        {
            if (ChuyenThus.Count != 0)
            {
                FileManager.SaveCT(ChuyenThus.ToList());
            }

        }

        private void XacNhanChiTiet200()
        {
            SendKeys.SendWait("{F6}");
            string copyedData = APIManager.GetCopyData();
            if (string.IsNullOrEmpty(copyedData))
                return;
            //1	593200	C	2350	03/07/2022	THỦY BỘ	1	8	29,7	03/07/2022 18:04:49
            string[] splitString = copyedData.Split('\t');
            if (splitString.Length < 11)
                return;
            if (splitString[1] == "593200" && splitString[3] == soCTCurrent)
            {
                WindowInfo currentWindow = APIManager.GetActiveWindowTitle();
                var controls = APIManager.GetListControlText(currentWindow.hwnd);
                APIManager.ClickButton(currentWindow.hwnd, "Xem và xác nhận CT (F10)");

                currentWindow = APIManager.WaitingFindedWindow("xem chuyen thu chieu den");
                APIManager.ClickButton(currentWindow.hwnd, "Xác nhận chi tiết túi thư");
                currentWindow = APIManager.WaitingFindedWindow("xac nhan chi tiet tui thu");
                if (currentWindow == null)
                    return;
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(50);
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(50);
                SendKeys.SendWait("^(a)");
                Thread.Sleep(600);

                APIManager.ClickButton(currentWindow.hwnd, "Đối kiểm");
                Thread.Sleep(50);
                SendKeys.SendWait("{ESC}");
                currentWindow = APIManager.WaitingFindedWindow("xem chuyen thu chieu den");
                //	Túi số	KL (kg)	Loại túi	F	xác nhận
                //False   1   11,8    Ði ngoài(BK)    True Cleared
                SendKeys.SendWait("{F5}");
                Thread.Sleep(50);
                SendKeys.SendWait("^(a)");

                copyedData = APIManager.GetCopyData();
                if (string.IsNullOrEmpty(copyedData))
                    return;
                if (copyedData.IndexOf("Selected") == -1)
                    return;
                APIManager.ClickButton(currentWindow.hwnd, "xac nhan chuyen thu", isExactly: false);
                currentWindow = APIManager.WaitingFindedWindow("xac nhan");
                APIManager.ClickButton(currentWindow.hwnd, "yes", isExactly: false);
                //Xong
            }
        }


        public ICommand AutoXacNhanCommand { get; }
        public ICommand BCPHNCommand { get; }
        public ICommand BD10DenCommand { get; }
        public ICommand BD10DiCommand { get; }
        public ICommand ChuyenThu0Command { get; }
        public ICommand ChuyenThu10Command { get; }
        public ICommand ChuyenThu11Command { get; }
        public ICommand ChuyenThu12Command { get; }
        public ICommand ChuyenThu1Command { get; }
        public ICommand ChuyenThu2Command { get; }
        public ICommand ChuyenThu3Command { get; }
        public ICommand ChuyenThu4Command { get; }
        public ICommand ChuyenThu5Command { get; }
        public ICommand ChuyenThu6Command { get; }
        public ICommand ChuyenThu7Command { get; }
        public ICommand ChuyenThu8Command { get; }
        public ICommand ChuyenThu9Command { get; }
        public ObservableCollection<ChuyenThuModel> ChuyenThus
        {
            get { return _ChuyenThus; }
            set { SetProperty(ref _ChuyenThus, value); }
        }

        public ICommand D420Command { get; }

        public ICommand KTHNCommand { get; }
        public ICommand LayDuLieuCommand { get; }
        public ICommand NewCTCommand { get; }

        public ICommand PrintDefaultCommand { get; }

        public ICommand SaveCTCommand { get; }

        private readonly BackgroundWorker bwCreateChuyenThu;
        private readonly BackgroundWorker bwPrint;
        private ObservableCollection<ChuyenThuModel> _ChuyenThus;

        private ChuyenThuModel currentChuyenThu;
        private string soCTCurrent = "";
    }
}