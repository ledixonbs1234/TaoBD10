﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
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
        private bool isAutoAddMaHieuFrom200To230 = false;
        private List<string> mahieus200 = new List<string>();

        public ChinhViewModel()
        {
            ChuyenThus = new ObservableCollection<ChuyenThuModel>();
            System.Collections.Generic.List<ChuyenThuModel> cts = FileManager.LoadCTOffline();
            if (cts != null)
                if (cts.Count != 0)
                {
                    foreach (ChuyenThuModel item in cts)
                    {
                        ChuyenThus.Add(item);
                    }
                }

            GetDataFromCloudCommand = new RelayCommand(GetDataFromCloud);
            XuongCommand = new RelayCommand(Xuong);
            LenCommand = new RelayCommand(Len);
            PublishCommand = new RelayCommand(Publish);

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
            ChuyenThu14Command = new RelayCommand(ChuyenThu14);

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
                    else if (m.Content == "SetTrueAuto200")
                    {
                    }
                }
                else if (m.Key == "XN593200")
                {
                    XacNhanChiTiet200();
                }
                else if (m.Key == "xntd200")
                {
                    Thread.Sleep(1200);
                    AutoXacNhan();
                }
                else if (m.Key == "SetTrueAuto200")
                {
                    isAutoAddMaHieuFrom200To230 = true;
                    mahieus200 = JsonConvert.DeserializeObject<List<String>>(m.Content);
                }
            });
        }

        private void ShowData(List<ChuyenThuModel> data)
        {
            if (data != null)
                if (data.Count != 0)
                {
                    ChuyenThus.Clear();
                    foreach (ChuyenThuModel item in data)
                    {
                        ChuyenThus.Add(item);
                    }
                }
        }

        public ICommand GetDataFromCloudCommand { get; }

        private void GetDataFromCloud()
        {
            ShowData(FileManager.LoadCTOnFirebase());
        }

        private void AutoXacNhan()
        {
            IntPtr handleWindow = APIManager.SetToLastWindow("593200");
            if (handleWindow == IntPtr.Zero)
                return;
            WindowInfo currentWindow = APIManager.GetActiveWindowTitle();
            if (APIManager.BoDauAndToLower(currentWindow.text).IndexOf("dong chuyen thu") == -1)
                return;
            var controls = APIManager.GetListControlText(currentWindow.hwnd);
            TestAPIModel ct = controls.Last(m => m.ClassName.ToLower().IndexOf("edit") != -1);
            soCTCurrent = ct.Text;
            //thuc hien cong viec trong nay khong co gi la khong the neu chung ta khong lam duoc
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
            SendKeys.SendWait("{ESC}");
            Thread.Sleep(2000);

            //thuc hien nhan button get chuyen thu 593200;
            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Button593200" });
            DataManager.IsWaitingCompleteLayComplete = true;

            //khi con trong chuyen thu thuc hien tu dong dong chuyen thu va lay du lieu chuyen thu
            //tu dong thoat chuyen thu va truyen
            // qua 280 tu dong lay chuyen thu
            //tu dong xac nhan chi tiet tui thu
        }

        public ICommand TestCommand { get; }

        private void Test()
        {
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
                List<TestAPIModel> childControls = APIManager.GetListControlText(currentWindow.hwnd);
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
                if (isAutoAddMaHieuFrom200To230)
                {
                    isAutoAddMaHieuFrom200To230 = false;
                    currentWindow = APIManager.GetActiveWindowTitle();
                    List<TestAPIModel> listControl = APIManager.GetListControlText(currentWindow.hwnd);

                    TestAPIModel apiCai = listControl.FirstOrDefault(m => m.Text.IndexOf("cái") != -1);
                    //TestText += apiCai.Text + "\n";
                    int.TryParse(Regex.Match(apiCai.Text, @"\d+").Value, out int numberRead);

                    Thread.Sleep(1000);

                    //thuc hien xu ly lenh trong nay
                    foreach (var mahieu in mahieus200)
                    {
                        Thread.Sleep(500);
                        SendKeys.SendWait(mahieu);
                        SendKeys.SendWait("{ENTER}");
                    }
                    Thread.Sleep(700);
                    listControl = APIManager.GetListControlText(currentWindow.hwnd);

                    apiCai = listControl.FirstOrDefault(m => m.Text.IndexOf("cái") != -1);
                    int.TryParse(Regex.Match(apiCai.Text, @"\d+").Value, out int numberReadLast);

                    if (numberRead + mahieus200.Count == numberReadLast)
                    {
                        //THuc hien send thanh cong
                        FileManager.SendMessageNotification("Đang đóng ct qua 230");
                        AutoXacNhan();
                    }
                    else
                    {
                        FileManager.SendMessageNotification("Không khớp số liệu");
                    }
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
            if (!string.IsNullOrEmpty(FileManager.optionModel.GoFastBD10Den))
            {
                var temp = FileManager.optionModel.GoFastBD10Den.Split(',');
                APIManager.GoToWindow(FileManager.optionModel.MaKhaiThac, "danh sach bd10 den", temp[0], temp[1]);
                PrintDefault();
            }
        }

        private void BD10Di()
        {
            if (!string.IsNullOrEmpty(FileManager.optionModel.GoFastBD10Di))
            {
                var temp = FileManager.optionModel.GoFastBD10Di.Split(',');
                APIManager.GoToWindow(FileManager.optionModel.MaKhaiThac, "danh sach bd10 di", temp[0], temp[1]);
                PrintDefault();
                //var currentWindow = APIManager.GetActiveWindowTitle();
                //if(currentWindow.text.IndexOf("danh sach bd10 di")!= -1)
                //{
                //    //SysDateTimePick32
                //    List<TestAPIModel> listControl = APIManager.GetListControlText(currentWindow.hwnd);
                //     var controlSys =listControl.FirstOrDefault(m => m.ClassName.IndexOf("SysDateTimePick32") != -1);
                //    if (controlSys == null)
                //        return;
                //    APIManager.setTextControl(controlSys.Handle, "01/09/2022");
                //    APIManager.FocusHandle(controlSys.Handle);
                //    Thread.Sleep(100);
                //    APIManager.FocusHandle(controlSys.Handle);

                //}
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

            numberGR = double.Parse(textGr, CultureInfo.InvariantCulture);

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
                Thread.Sleep(50);
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

        private void ChuyenThu0()
        {
            ChuyenThuThu(0);
        }

        private void ChuyenThuThu(int number)
        {
            if (ChuyenThus.Count >= number + 1)
            {
                currentChuyenThu = ChuyenThus[number];
            }

            var temp = ChuyenThus[number].GoFastBCCP.Split(',');
            APIManager.GoToWindow(ChuyenThus[number].MaBCCP, "khoi tao chuyen thu", temp[0], temp[1]);
            if (!bwCreateChuyenThu.IsBusy)
            {
                bwCreateChuyenThu.CancelAsync();
                bwCreateChuyenThu.RunWorkerAsync();
            }
        }

        private void ChuyenThu1()
        {
            ChuyenThuThu(1);
        }

        private void ChuyenThu10()
        {
            ChuyenThuThu(10);
        }

        private void ChuyenThu11()
        {
            ChuyenThuThu(11);
        }

        private void ChuyenThu12()
        {
            ChuyenThuThu(12);
        }

        private void ChuyenThu13()
        {
            ChuyenThuThu(13);
        }

        private void ChuyenThu2()
        {
            ChuyenThuThu(2);
        }

        private void ChuyenThu3()
        {
            ChuyenThuThu(3);
        }

        private void ChuyenThu4()
        {
            ChuyenThuThu(4);
        }

        private void ChuyenThu5()
        {
            ChuyenThuThu(5);
        }

        private void ChuyenThu6()
        {
            ChuyenThuThu(6);
        }

        private void ChuyenThu7()
        {
            ChuyenThuThu(7);
        }

        private void ChuyenThu8()
        {
            ChuyenThuThu(8);
        }

        private void ChuyenThu9()
        {
            ChuyenThuThu(9);
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
            if (!string.IsNullOrEmpty(FileManager.optionModel.LayDuLieu))
            {
                FileManager.SendMessageNotification("Đang bắt đầu lấy dữ liệu");
                Thread.Sleep(400);
                var temp = FileManager.optionModel.LayDuLieu.Split(',');
                APIManager.GoToWindow(FileManager.optionModel.MaBuuCucLayDuLieu, "Default", temp[0], temp[1]);
                var window = APIManager.WaitingFindedWindow("danh sach buu gui le");
                if (window != null)
                {
                    SendKeys.SendWait("{F4}");
                    Thread.Sleep(1000);
                    SendKeys.SendWait("{F10}");
                    FileManager.SendMessageNotification("Đã lấy dữ liệu thành công");
                }
                else
                {
                    FileManager.SendMessageNotification("Không tìm thấy cửa sổ",disablePhone:true);
                }
            }
        }

        private void Len()
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

        private void PrintDefault()
        {
            APIManager.SetPrintBD10();
        }

        public ICommand PublishCommand { get; }

        private void Publish()
        {
            if (ChuyenThus.Count != 0)
            {
                FileManager.SaveCTOnFirebase(ChuyenThus.ToList());
            }
        }

        private void SaveCT()
        {
            if (ChuyenThus.Count != 0)
            {
                FileManager.SaveCTOffline(ChuyenThus.ToList());
            }
        }

        private void XacNhanChiTiet200()
        {
            SendKeys.SendWait("{F6}");
            Thread.Sleep(500);

            string copyedData = APIManager.GetCopyData();
            if (string.IsNullOrEmpty(copyedData))
                return;
            //1	593200	C	2350	03/07/2022	THỦY BỘ	1	8	29,7	03/07/2022 18:04:49
            if (copyedData.ToLower().IndexOf("stt") != -1)
            {
                string[] splitEnter = copyedData.Split('\n');
                if (splitEnter.Length >= 2)
                {
                    copyedData = splitEnter[1];
                }
                else
                {
                    return;
                }
            }

            string[] splitString = copyedData.Split('\t');
            if (splitString[1] == "593200" && splitString[3] == soCTCurrent)
            {
                WindowInfo currentWindow = APIManager.GetActiveWindowTitle();
                APIManager.ClickButton(currentWindow.hwnd, "f10", isExactly: false);

                currentWindow = APIManager.WaitingFindedWindow("xem chuyen thu chieu den");
                APIManager.ClickButton(currentWindow.hwnd, "xac nhan chi tiet", isExactly: false);
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
                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "XacNhanChiTiet", Content = "True" });
                SendKeys.SendWait("{ESC}");
            }
        }

        public ICommand ChuyenThu14Command { get; }

        private void ChuyenThu14()
        {
            ChuyenThuThu(14);
        }

        private void Xuong()
        {
            if (SelectedIndexCT == -1)
                return;
            if (SelectedIndexCT == ChuyenThus.Count - 1)
                return;
            int tempSelected = SelectedIndexCT;
            ChuyenThuModel currentCT = ChuyenThus[SelectedIndexCT];
            ChuyenThuModel tempCT = ChuyenThus[SelectedIndexCT + 1];
            ChuyenThus[SelectedIndexCT + 1] = currentCT;
            ChuyenThus[SelectedIndexCT] = tempCT;
            SelectedIndexCT = tempSelected + 1;
        }

        private readonly BackgroundWorker bwCreateChuyenThu;
        private readonly BackgroundWorker bwPrint;
        private ObservableCollection<ChuyenThuModel> _ChuyenThus;
        private int _SelectedIndexCT;
        private ChuyenThuModel currentChuyenThu;
        private string soCTCurrent = "";
        public ICommand AutoXacNhanCommand { get; }
        public ICommand BCPHNCommand { get; }
        public ICommand BD10DenCommand { get; }
        public ICommand BD10DiCommand { get; }
        public ICommand ChuyenThu0Command { get; }
        public ICommand ChuyenThu10Command { get; }
        public ICommand ChuyenThu11Command { get; }
        public ICommand ChuyenThu12Command { get; }
        public ICommand ChuyenThu13Command { get; }
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

        public ICommand LenCommand { get; }

        public ICommand NewCTCommand { get; }

        public ICommand PrintDefaultCommand { get; }

        public ICommand SaveCTCommand { get; }

        public int SelectedIndexCT
        {
            get { return _SelectedIndexCT; }
            set { SetProperty(ref _SelectedIndexCT, value); }
        }

        public ICommand XuongCommand { get; }
    }
}