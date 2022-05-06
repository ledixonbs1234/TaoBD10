using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
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

            XeXaHoiCommand = new RelayCommand(XeXaHoi);
            KienDaNangCommand = new RelayCommand(KienDaNang);
            EMSDaNangCommand = new RelayCommand(EMSDaNang);
            NamTrungBoCommand = new RelayCommand(NamTrungBo);
            QuangNamCommand = new RelayCommand(QuangNam);
            QuangNgaiCommand = new RelayCommand(QuangNgai);
            PhuMyCommand = new RelayCommand(PhuMy);
            PhuCatCommand = new RelayCommand(PhuCat);
            AnNhonCommand = new RelayCommand(AnNhon);
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
        void AnNhon()
        {
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

        void EMSDaNang()
        {
            maBuuCuc = "550915";
            tenDuongThu = "Bình Định - Đà Nẵng";
            countDuongThu = 4;
            countChuyen = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }

        void KienDaNang()
        {
            maBuuCuc = "550910";
            tenDuongThu = "Bình Định - Đà Nẵng";
            countDuongThu = 4;
            countChuyen = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }

        void NamTrungBo()
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

        void PhuCat()
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

        void PhuMy()
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

        void QuangNam()
        {
            maBuuCuc = "560100";
            tenDuongThu = "Bình Định - Đà Nẵng";
            countDuongThu = 4;
            countChuyen = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }

        void QuangNgai()
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

        void XeXaHoi()
        {
            maBuuCuc = "590100";
            tenDuongThu = "Tam Quan - Quy Nhơn (Xe XH)";
            countDuongThu = 9;
            countChuyen = 2;
            stateTaoBd10 = StateTaoBd10.DanhSachBD10;
            timerTaoBD.Start();
        }

        int countChuyen = 0;
        int countDuongThu = 0;
        bool isWaiting = false;
        string maBuuCuc = "0";
        StateTaoBd10 stateTaoBd10;
        string tenDuongThu = "";
        DispatcherTimer timerTaoBD;
    }
}
