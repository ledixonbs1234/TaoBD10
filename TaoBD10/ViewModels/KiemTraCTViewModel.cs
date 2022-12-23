using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class KiemTraCTViewModel : ObservableObject
    {
        private BackgroundWorker bwRunCheck;
        private bool _IsCancelWorker = false;

        public KiemTraCTViewModel()
        {
            bwRunCheck = new BackgroundWorker();
            bwRunCheck.DoWork += BwRunCheck_DoWork;
            bwRunCheck.WorkerSupportsCancellation = true;
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "ToKTCT")
                {
                    if (m.Content == "RunCheckCT")
                    {
                        if (!bwRunCheck.IsBusy)
                        {
                            _IsCancelWorker = false;
                            bwRunCheck.RunWorkerAsync();
                        }
                        else
                        {
                            _IsCancelWorker = true;
                            APIManager.ShowSnackbar("Da cancel");
                            bwRunCheck.CancelAsync();
                        }
                    }
                }
            });
        }

        private List<ChuyenThuInQuanLyModel> cts = new List<ChuyenThuInQuanLyModel>();

        private List<string> MaHieus = new List<string>();

        private void BwRunCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            APIManager.ShowSnackbar("sdf");
            //thuc hien trong nay
            var currentWindow = APIManager.WaitingFindedWindow("quan ly chuyen thu chieu den");
            if (currentWindow == null)
            {
                return;
            }
            Thread.Sleep(100);
            string ctTemp = "dsfsd";
            while (true)
            {
                if (_IsCancelWorker)
                {
                    //thuc hien send to mahieu
                    List<HangTonModel> list = new List<HangTonModel>();
                    foreach (var item in MaHieus)
                    {
                        list.Add(new HangTonModel() { MaHieu = item });
                    }
                    WeakReferenceMessenger.Default.Send(new HangTonMessage(list));
                    break;
                }
                ctTemp = APIManager.GetCopyData();
                if (string.IsNullOrEmpty(ctTemp))
                {
                    break;
                }
                //if (lastCopy == ctTemp)
                //{
                //    break;
                //}
                //else
                //{
                //    lastCopy = ctTemp;
                //}
                string[] ctTempSplited = ctTemp.Split('\t');
                var chuyenThu = new ChuyenThuInQuanLyModel(ctTempSplited[1], ctTempSplited[2], ctTempSplited[3], ctTempSplited[4], ctTempSplited[5], ctTempSplited[6], ctTempSplited[7]);
                var isExistCT = cts.Exists(m => m.SLTui == chuyenThu.SLTui && m.SLBuuGui == chuyenThu.SLBuuGui && m.BuuCucDong == chuyenThu.BuuCucDong && m.DichVu == chuyenThu.DichVu &&
                m.KL == chuyenThu.KL && m.DateCreate == chuyenThu.DateCreate);
                if (!isExistCT)
                {
                    cts.Add(chuyenThu);
                    //Thuc hien vao chuyen thu trong nay
                    GoToCT();
                    Thread.Sleep(1000);
                }
                else
                {
                    SendKeys.SendWait("{DOWN}");
                    Thread.Sleep(120);
                }
            }

            APIManager.ShowSnackbar("end");
            //thuc hien tuan tu chay len xuong cac chuyen thu tu quan ly
        }

        private void GoToCT()
        {
            SendKeys.SendWait("{F10}");
            var currentWindow = APIManager.WaitingFindedWindow("xem chuyen thu chieu den");
            if (currentWindow == null)
            {
                return;
            }

            SendKeys.SendWait("{F5}");
            SendKeys.SendWait("^{UP}");
            Thread.Sleep(100);
            string lastCopied = "";
            bool daTimThay = false;
            while (true)
            {
                var code = layMaHieuTrongDongCT();
                if (code == null)
                {
                    return;
                }
                if (MaHieus.IndexOf(code) < 0)
                {
                    MaHieus.Add(code);
                }
                else
                {
                    //thuc hien thoat trong nay
                    SendKeys.SendWait("{ESC}");
                    break;
                }

                SendKeys.SendWait("{F5}");
                Thread.Sleep(50);
                SendKeys.SendWait("{DOWN}");
                Thread.Sleep(50);
            }
        }

        private string layMaHieuTrongDongCT()
        {
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return null;

            if (currentWindow.text.IndexOf("xem chuyen thu chieu den") != -1)
            {
                SendKeys.SendWait("{F6}");
                Thread.Sleep(100);
                SendKeys.SendWait("{TAB}{TAB}");
                Thread.Sleep(100);

                SendKeys.SendWait("^(c)");
                string data = APIManager.GetCopyData();

                //loc du lieu trong nay
                //STT	Số hiệu	Tỉnh gốc	BC gốc	BC phát	Loại	KL (gr)	QĐ (gr)	Số hiệu lô	Ghi chú
                // 1   CH214294910VN   10  157870  593280  C * 10000   0
                var temp = data.Split('\n');
                if (temp.Length == 2)
                {
                    string code = temp[1].Split('\t')[1];
                    bool isDoubleRight = double.TryParse(temp[1].Split('\t')[6], out double klTemp);

                    //xu ly du lieu trong nay
                    if (code.Length == 13 && isDoubleRight)
                    {
                        return code;
                    }
                }
            }
            return null;
        }
    }
}