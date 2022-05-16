using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class LayChuyenThuViewModel : ObservableObject
    {
        public LayChuyenThuViewModel()
        {
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(2000)
            };
            timer.Tick += Timer_Tick;
            BongSonCommand = new RelayCommand(BongSon);
            HoaiXuanCommand = new RelayCommand(HoaiXuan);
            HoaiTanCommand = new RelayCommand(HoaiTan);
            HoaiDucCommand = new RelayCommand(HoaiDuc);
            GiaoDichCommand = new RelayCommand(GiaoDich);
            HoaiHuongCommand = new RelayCommand(HoaiHuong);
            HoaiHaiCommand = new RelayCommand(HoaiHai);
            BCPCommand = new RelayCommand(BCP);
            HoaiMyCommand = new RelayCommand(HoaiMy);
        }

        public ICommand BCPCommand { get; }
        public ICommand BongSonCommand { get; }
        public ICommand GiaoDichCommand { get; }
        public ICommand HoaiDucCommand { get; }
        public ICommand HoaiHaiCommand { get; }
        public ICommand HoaiHuongCommand { get; }
        public ICommand HoaiMyCommand { get; }
        public ICommand HoaiTanCommand { get; }
        public ICommand HoaiXuanCommand { get; }

        public void Btn593200()
        {
            maBuuCucChuyenThuDen = "593200";
            isBaoDamChuyenThuDen = false;
            stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;

            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            timer.Start();
        }

        private void BCP()
        {
            maBuuCucChuyenThuDen = "593280";
            stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
            isBaoDamChuyenThuDen = false;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            timer.Start();
        }

        private void BongSon()
        {
            maBuuCucChuyenThuDen = "593522";
            stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            timer.Start();
        }

        private void ChuyenThuDen(bool isBaoDam, string mabuucuc)
        {
            if (isBaoDam)
            {
                SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}");
                //buu pham bao dam
                SendKeys.SendWait("{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}");
            }
            else
            {
                SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}");
                //buu pham bao dam
                SendKeys.SendWait("{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}");
            }
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait(mabuucuc);
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{F8}");
        }

        private void GiaoDich()
        {
            maBuuCucChuyenThuDen = "593200";
            stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
            isBaoDamChuyenThuDen = false;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            timer.Start();
        }

        private void HoaiDuc()
        {
            maBuuCucChuyenThuDen = "593550";
            stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            timer.Start();
        }

        private void HoaiHai()
        {
            maBuuCucChuyenThuDen = "593260";
            stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            timer.Start();
        }

        private void HoaiHuong()
        {
            maBuuCucChuyenThuDen = "593270";
            stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            timer.Start();
        }

        private void HoaiMy()
        {
            maBuuCucChuyenThuDen = "593240";
            stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            timer.Start();
        }

        private void HoaiTan()
        {
            maBuuCucChuyenThuDen = "593370";
            stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            timer.Start();
        }

        private void HoaiXuan()
        {
            maBuuCucChuyenThuDen = "593220";
            stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
            isBaoDamChuyenThuDen = true;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isWaitingChuyenThuChieuDen)
                return;
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;
            APIManager.ShowTest("1");

            switch (stateChuyenThuChieuDen)
            {
                case StateChuyenThuChieuDen.GetData:
                    if (currentWindow.text.IndexOf("quan ly chuyen thu") != -1)
                    {
                        Thread.Sleep(200);
                        isWaitingChuyenThuChieuDen = true;

                        //lay combobox hien tai
                        var childHandles3 = APIManager.GetAllChildHandles(currentWindow.hwnd);
                        int countCombobox = 0;
                        IntPtr combo = IntPtr.Zero;
                        foreach (var item in childHandles3)
                        {
                            string className = APIManager.GetWindowClass(item);
                            string classDefault = "WindowsForms10.COMBOBOX.app.0.1e6fa8e";
                            //string classDefault = "WindowsForms10.COMBOBOX.app.0.141b42a_r8_ad1";
                            if (className == classDefault)
                            {
                                if (countCombobox == 3)
                                {
                                    //road = item;
                                    combo = item;
                                    break;
                                }
                                countCombobox++;
                            }
                        }
                        APIManager.ShowTest("2");
                        APIManager.SendMessage(combo, 0x0007, 0, 0);
                        APIManager.SendMessage(combo, 0x0007, 0, 0);

                        ChuyenThuDen(isBaoDamChuyenThuDen, maBuuCucChuyenThuDen);
                        stateChuyenThuChieuDen = StateChuyenThuChieuDen.ShowInfo;
                        Thread.Sleep(200);
                        SendKeys.SendWait("{ENTER}");

                        isWaitingChuyenThuChieuDen = false;
                    }

                    break;

                case StateChuyenThuChieuDen.ShowInfo:
                    APIManager.ShowTest("3");
                    if (currentWindow.text.IndexOf("canh bao") != -1)
                    {
                        isWaitingChuyenThuChieuDen = true;
                        SendKeys.SendWait("{ENTER}");
                        Thread.Sleep(500);
                        SendKeys.SendWait("{F5}");
                        timer.Stop();
                        isWaitingChuyenThuChieuDen = false;
                    }
                    else if (currentWindow.text.IndexOf("thong bao") != -1)
                    {
                        isWaitingChuyenThuChieuDen = true;
                        SendKeys.SendWait("{ENTER}");
                        Thread.Sleep(500);
                        SendKeys.SendWait("{F5}");
                        timer.Stop();
                        isWaitingChuyenThuChieuDen = false;
                    }
                    APIManager.ShowTest("4");

                    break;

                case StateChuyenThuChieuDen.Find:
                    break;

                default:
                    break;
            }
        }

        private bool isBaoDamChuyenThuDen = false;
        private bool isWaitingChuyenThuChieuDen = false;
        private string maBuuCucChuyenThuDen = "";
        private StateChuyenThuChieuDen stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
        private readonly DispatcherTimer timer;
    }
}