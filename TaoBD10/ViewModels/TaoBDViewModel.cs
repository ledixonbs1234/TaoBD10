using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
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
    public class TaoBDViewModel : ObservableObject
    {
        public TaoBDViewModel()
        {
            timerTaoBD = new DispatcherTimer();
            timerTaoBD.Interval = new TimeSpan(0, 0, 0, 0, 200);
            timerTaoBD.Tick += TimerTaoBD_Tick;
            taoBDWorker = new BackgroundWorker();
            taoBDWorker.DoWork += TaoBDWorker_DoWork;

            AutoGetBD10Command = new RelayCommand(AutoGetBD10);
            XeXaHoiCommand = new RelayCommand(XeXaHoi);
            KienDaNangCommand = new RelayCommand(KienDaNang);
            EMSDaNangCommand = new RelayCommand(EMSDaNang);
            NamTrungBoCommand = new RelayCommand(NamTrungBo);
            QuangNamCommand = new RelayCommand(QuangNam);
            QuangNgaiCommand = new RelayCommand(QuangNgai);
            PhuMyCommand = new RelayCommand(PhuMy);
            PhuCatCommand = new RelayCommand(PhuCat);
            AnNhonCommand = new RelayCommand(AnNhon);
            AddBDTinhCommand = new RelayCommand(AddBDTinh);
        }

        public ICommand AutoGetBD10Command { get; }

        private void AutoGetBD10()
        {
            if (!APIManager.ThoatToDefault("593230", "danh sach bd10 di"))
            {
                SendKeys.SendWait("3");
                Thread.Sleep(200);
                SendKeys.SendWait("2");
            }
            WindowInfo activeWindows = APIManager.GetActiveWindowTitle();
            if (activeWindows.text.IndexOf("danh sach bd10 di") == -1)
                return;

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
                SendKeys.SendWait("{DOWN}");
                Thread.Sleep(50);
                data = APIManager.GetCopyData();
                //550910-VCKV - Đà Nẵng LT	08/06/2022	1	Ô tô	21	206,4	Đã đi
                //590100-VCKV Nam Trung Bộ	08/06/2022	2	Ô tô	50	456,1	Khởi tạo
                //if ((data.IndexOf("550910") != -1
                //    || data.IndexOf("550915") != -1
                //    || data.IndexOf("590100") != -1
                //    || data.IndexOf("592020") != -1
                //    || data.IndexOf("592440") != -1
                //    || data.IndexOf("592810") != -1
                //    || data.IndexOf("560100") != -1
                //    || data.IndexOf("570100") != -1)
                //    && data.IndexOf("Khởi tạo") != -1)
                //{
                //    WindowInfo window = APIManager.WaitingFindedWindow("danh sach bd10 di");
                //    if (window == null)
                //    {
                //        return;
                //    }
                //    APIManager.ClickButton(window.hwnd, "Sửa");
                //    window = APIManager.WaitingFindedWindow("sua thong tin bd10");
                //    if (window == null)
                //    {
                //        return;
                //    }
                //    Thread.Sleep(500);
                //    SendKeys.SendWait("{F6}");
                //}
                //else
                //{
                //    SendKeys.SendWait("{DOWN}");
                //    Thread.Sleep(100);
                //    data = APIManager.GetCopyData();
                //}
            }
            APIManager.ShowSnackbar("Run print list bd 10 complete");
        }

        private void TaoBDWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!APIManager.ThoatToDefault("593230", "danh sach bd10 di"))
            {
                SendKeys.SendWait("3");
                Thread.Sleep(200);
                SendKeys.SendWait("2");
            }
            WindowInfo currentWindow = APIManager.WaitingFindedWindow("danh sach bd10 di");
            if (currentWindow == null)
                return;

            SendKeys.SendWait("{F1}");
            currentWindow = APIManager.WaitingFindedWindow("lap bd10");
            if (currentWindow == null)
                return;
            System.Collections.Generic.List<Model.TestAPIModel> controls = APIManager.GetListControlText(currentWindow.hwnd);
            Model.TestAPIModel controlDuongThu = controls.Where(m => m.ClassName == "WindowsForms10.COMBOBOX.app.0.1e6fa8e").ToList()[2];
            Model.TestAPIModel controlChuyen = controls.Where(m => m.ClassName == "WindowsForms10.COMBOBOX.app.0.1e6fa8e").ToList()[1];
            Model.TestAPIModel controlBCNhan = controls.Where(m => m.ClassName == "WindowsForms10.COMBOBOX.app.0.1e6fa8e").ToList()[3];
            const int CB_SETCURSEL = 0x014E;
            APIManager.SendMessage(controlDuongThu.Handle, CB_SETCURSEL, countDuongThu, 0);
            APIManager.SendMessage(controlChuyen.Handle, CB_SETCURSEL, countChuyen, 0);
            APIManager.SendMessage(controlBCNhan.Handle, CB_SETCURSEL, 10, 0);
        }

        private BackgroundWorker taoBDWorker;
        public ICommand AddBDTinhCommand { get; }
        TaoBdInfoModel taoBDInfo;
        private void AddBDTinh()
        {
        }

        public ICommand AnNhonCommand { get; }
        public ICommand EMSDaNangCommand { get; }
        public ICommand KienDaNangCommand { get; }
        public ICommand NamTrungBoCommand { get; }
        public ICommand PhuCatCommand { get; }
        public ICommand PhuMyCommand { get; }
        public ICommand QuangNamCommand { get; }
        public ICommand QuangNgaiCommand { get; }
        public ICommand XeXaHoiCommand { get; }

        private void AnNhon()
        {
            taoBDInfo=
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

        private void EMSDaNang()
        {
            maBuuCuc = "550915";
            tenDuongThu = "Bình Định - Đà Nẵng";
            countDuongThu = 4;
            countChuyen = 2;
            taoBDWorker.RunWorkerAsync();
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
            countDuongThu = 9;
            countChuyen = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }

        private int countChuyen = 0;
        private int countDuongThu = 0;
        private bool isWaiting = false;
        private string maBuuCuc = "0";
        private StateTaoBd10 stateTaoBd10;
        private string tenDuongThu = "";
        private DispatcherTimer timerTaoBD;
    }
}