using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class XacNhanMHViewModel : ObservableObject
    {
        public XacNhanMHViewModel()
        {
            TestCommand = new RelayCommand(Test);
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += Woker_DoWork;
            backgroundWorkerXacNhan = new BackgroundWorker();
            backgroundWorkerXacNhan.DoWork += BackgroundWorkerXacNhan_DoWork;


            GoToCTCommand = new RelayCommand(GoToCT);
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(2000)
            };
            timer.Tick += Timer_Tick;
            WeakReferenceMessenger.Default.Register<KiemTraMessage>(this, (r, m) =>
        {
            if (m.Value.Key == "XacNhanMH")
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    XacNhanMH = m.Value;
                });

            }
        });
        }

        private void BackgroundWorkerXacNhan_DoWork(object sender, DoWorkEventArgs e)
        {
            WindowInfo window = APIManager.WaitingFindedWindow("quan ly chuyen thu");
            if (window == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy quản lý chuyến thư");
                return;
            }

            List<TestAPIModel> controls = APIManager.GetListControlText(window.hwnd);
            if (controls.Count == 0)
            {
                APIManager.ShowSnackbar("Window lỗi");
                return;
            }
            List<TestAPIModel> listCombobox = controls.Where(m => m.ClassName.ToLower().IndexOf("combobox") != -1).ToList();
            IntPtr comboHandle = listCombobox[3].Handle;

            APIManager.SendMessage(comboHandle, 0x0007, 0, 0);
            APIManager.SendMessage(comboHandle, 0x0007, 0, 0);
            ChuyenThuDen();
            Thread.Sleep(200);
            SendKeys.SendWait("{ENTER}");

            window = APIManager.WaitingFindedWindow("canh bao","thong bao");
            if (window == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy cảnh báo");
                return;
            }

            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(500);
            SendKeys.SendWait("{F5}");
            Thread.Sleep(500);
            SendKeys.SendWait("{F6}");
            SendKeys.SendWait("{UP}");
            SendKeys.SendWait("{UP}");
            SendKeys.SendWait("{UP}");
            SendKeys.SendWait("{UP}");
            string dataCopyed =APIManager.GetCopyData();
            if (string.IsNullOrEmpty(dataCopyed))
                return;
            string[] datas = dataCopyed.Split('\t');
            if(datas[3].Trim() == soCT.Trim() && datas[1].Trim() == buuCucDong.Trim())
            {
                SendKeys.SendWait("{F10}");
                WindowInfo windows = APIManager.WaitingFindedWindow("xem chuyen thu chieu den");
                if (windows == null)
                    return;
                SendKeys.SendWait("A{BS}{BS}");
                SendKeys.SendWait("{F4}");
            }

        }

        private void ChuyenThuDen()
        {
            if (isR)
            {
                SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}");
                //buu pham bao dam
                SendKeys.SendWait("{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}");
            }
            else
            {
                if (!Is280)
                {
                    SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}");
                    //buu pham bao dam
                    SendKeys.SendWait("{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}");
                }
                else
                {
                    SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}");
                    //buu pham bao dam
                    SendKeys.SendWait("{DOWN}");
                }
            }
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait(buuCucDong);
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait(soCT);
            SendKeys.SendWait("{TAB}");
            SendKeys.SendWait(date[0]);
            SendKeys.SendWait("{RIGHT}");
            SendKeys.SendWait(date[1]);
            SendKeys.SendWait("{TAB}");

            SendKeys.SendWait("{F8}");
        }

        void GoToCT()
        {
            //quy trinh thuc hien 
            //kiem tra thu buu cuc nhan co 280 hay 230 ko
            // neu co thi chay vao 1 trong n2 cai do 
            //sau do vao chuyen thu
            //sau do chay vao do dua vao thong tin cua no va dien len
            if (XacNhanMH.BuuCucNhan.IndexOf("593280") != -1)
            {
                Is280 = true;
                //thuc hien go to chuyen  thu
                XuLyThongTin();
                if (!APIManager.ThoatToDefault("593280", "quan ly chuyen thu chieu deen"))
                {
                    SendKeys.SendWait("1");
                    Thread.Sleep(200);
                    SendKeys.SendWait("3");
                }
                //timer.Stop();
                //isWaitingChuyenThuChieuDen = false;
                //stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
                //timer.Start();
                backgroundWorkerXacNhan.RunWorkerAsync();
            }
            else if (XacNhanMH.BuuCucNhan.IndexOf("593230") != -1)
            {
                Is280 = false;
                //thuc hien xu ly thong tin can thiet
                XuLyThongTin();
                if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu deen"))
                {
                    SendKeys.SendWait("1");
                    Thread.Sleep(200);
                    SendKeys.SendWait("3");
                }

                //timer.Stop();
                //isWaitingChuyenThuChieuDen = false;
                //stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
                //timer.Start();

                backgroundWorkerXacNhan.RunWorkerAsync();
            }


        }

        private void OnEnterKey()
        {
            
            if (MaHieu.IndexOf('\n') != -1)
            {
                MaHieu = MaHieu.Trim();
                if (MaHieu.Length != 13)
                {
                    MaHieu = "";
                    return;
                }
                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "XacNhanMH", Content = _MaHieu.Trim().ToLower() });

                MaHieu = "";
            }
        }

        void Test()
        {
            //backgroundWorker.RunWorkerAsync();
            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isWaitingChuyenThuChieuDen)
                return;
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;

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
                        APIManager.SendMessage(combo, 0x0007, 0, 0);
                        APIManager.SendMessage(combo, 0x0007, 0, 0);

                        ChuyenThuDen();
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

        private void Woker_DoWork(object sender, DoWorkEventArgs e)
        {
            TestText += "Lan 1" + '\n';
            Thread.Sleep(1000);
            TestText += "Lan 2" + '\n';
            Thread.Sleep(2000);
            SendKeys.SendWait("khong co gi");
            TestText += "Lan 3" + '\n';
            SendKeys.SendWait("khong ddco gi");
            Thread.Sleep(2000);
            TestText += "Lan 4" + '\n';
        }

        private void XuLyThongTin()
        {
            //xu ly date
            var dates = XacNhanMH.Date.Split('/');
            date[0] = dates[0];
            date[1] = dates[1];
            date[2] = dates[2].Substring(0, 4);
            //xu ly ct
            string[] temps = XacNhanMH.TTCT.ToLower().Split('/');
            string loaiCT = temps[0].Replace("chuyến thư ", "");
            if (loaiCT == "r")
            {
                isR = true;
            }

            soCT = temps[1].Replace("số ct ", "");
            buuCucDong = XacNhanMH.BuuCucDong.Substring(0, 6);
        }

        public ICommand GoToCTCommand { get; }
        public string MaHieu
        {
            get { return _MaHieu; }
            set { SetProperty(ref _MaHieu, value); OnEnterKey(); }
        }

        public ICommand TestCommand { get; }
        public string TestText
        {
            get { return _TestText; }
            set { SetProperty(ref _TestText, value); }
        }

        public KiemTraModel XacNhanMH
        {
            get { return _XacNhanMH; }
            set { SetProperty(ref _XacNhanMH, value); }
        }

        readonly string[] date = new string[3];
        private readonly DispatcherTimer timer;
        private string _MaHieu;
        private string _TestText;
        private KiemTraModel _XacNhanMH;
        BackgroundWorker backgroundWorker;
        BackgroundWorker backgroundWorkerXacNhan;
        string buuCucDong = "";
        bool Is280 = false;
        bool isR = false;
        private bool isWaitingChuyenThuChieuDen = false;
        string soCT = "0";
        private StateChuyenThuChieuDen stateChuyenThuChieuDen = StateChuyenThuChieuDen.GetData;
    }
}
