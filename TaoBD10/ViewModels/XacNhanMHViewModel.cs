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

            CopyMHCommand = new RelayCommand(CopyMH);

            GoToCTCommand = new RelayCommand(GoToCT);

            WeakReferenceMessenger.Default.Register<KiemTraMessage>(this, (r, m) =>
        {
            if (m.Value.Key == "XacNhanMH")
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    XacNhanMH = m.Value;
                    IsWaitingComplete = false;
                    GoToCTCommand.Execute(null);
                });

            }
        });
        }
        private bool _IsWaitingComplete = false;

        public bool IsWaitingComplete
        {
            get { return _IsWaitingComplete; }
            set { SetProperty(ref _IsWaitingComplete, value); }
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

            window = APIManager.WaitingFindedWindow("canh bao", "thong bao");
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
            string dataCopyed = APIManager.GetCopyData();
            if (string.IsNullOrEmpty(dataCopyed))
                return;
            List<string> listTemp = dataCopyed.Split('\n').ToList();
            if (listTemp[0].IndexOf("STT") != -1)
            {
                listTemp.RemoveAt(0);
            }

            string[] datas = listTemp[0].Split('\t');
            if (datas[3].Trim() == soCT.Trim() && datas[1].Trim() == buuCucDong.Trim())
            {
                SendKeys.SendWait("{F10}");
                WindowInfo windows = APIManager.WaitingFindedWindow("xem chuyen thu chieu den");
                if (windows == null)
                    return;
                SendKeys.SendWait("A{BS}{BS}");
                SendKeys.SendWait("{F4}");
                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "XacNhanChiTiet", Content = "True" });
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
                IsWaitingComplete = true;

                MaHieu = "";
            }
        }

        public ICommand CopyMHCommand { get; }


        void CopyMH()
        {
            if (!string.IsNullOrEmpty(MaHieu))
            {
                Clipboard.SetText(MaHieu);
                APIManager.ShowSnackbar("Đã copy");
            }

        }


        void Test()
        {
            //backgroundWorker.RunWorkerAsync();

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
            if (loaiCT.ToLower() == "r")
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
        private string _MaHieu;
        private string _TestText;
        private KiemTraModel _XacNhanMH;
        readonly BackgroundWorker backgroundWorker;
        readonly BackgroundWorker backgroundWorkerXacNhan;
        string buuCucDong = "";
        bool Is280 = false;
        bool isR = false;
        string soCT = "0";
    }
}
