using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class LayChuyenThuViewModel : ObservableObject
    {
        public LayChuyenThuViewModel()
        {
            SaveBuuCucCommand = new RelayCommand(SaveBuuCuc);
            BuuCuc0Command = new RelayCommand(BuuCuc0);
            BuuCuc3Command = new RelayCommand(BuuCuc3);
            BuuCuc4Command = new RelayCommand(BuuCuc4);
            BuuCuc5Command = new RelayCommand(BuuCuc5);
            BuuCuc6Command = new RelayCommand(BuuCuc6);
            BuuCuc7Command = new RelayCommand(BuuCuc7);
            BuuCuc8Command = new RelayCommand(BuuCuc8);
            BuuCuc9Command = new RelayCommand(BuuCuc9);
            PublishCommand = new RelayCommand(Publish);

            GetDataFromCloudCommand = new RelayCommand(GetDataFromCloud);
            bwLayCT = new BackgroundWorker();
            bwLayCT.WorkerSupportsCancellation = true;
            bwLayCT.DoWork += BwLayCT_DoWork;
            BCPCommand = new RelayCommand(BCP);
            BuuCuc1Command = new RelayCommand(BuuCuc1);
            BuuCuc2Command = new RelayCommand(BuuCuc2);
            BuuCucs = new ObservableCollection<BuuCucModel>();
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "Button593200")
                {
                    Btn593200();
                }
            });
            ShowData(FileManager.LoadBuuCucOffline());
        }

        public ICommand PublishCommand { get; }

        private bool _IsLayDuLieu = true;

        public bool IsLayDuLieu
        {
            get { return _IsLayDuLieu; }
            set { SetProperty(ref _IsLayDuLieu, value); }
        }

        private void Publish()
        {
            FileManager.SaveBuuCuc(BuuCucs.ToList());
        }

        private void ShowData(List<BuuCucModel> data)
        {
            if (data != null)
                if (data.Count != 0)
                {
                    BuuCucs.Clear();
                    foreach (BuuCucModel item in data)
                    {
                        BuuCucs.Add(item);
                    }
                }
        }

        public ICommand GetDataFromCloudCommand { get; }

        private void GetDataFromCloud()
        {
            ShowData(FileManager.LoadBuuCucOnFirebase());
        }

        public void Btn593200()
        {
            maBuuCucChuyenThuDen = "593200";
            isBaoDamChuyenThuDen = false;

            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(50);
                SendKeys.SendWait("3");
            }
            bwLayCT.RunWorkerAsync();
        }

        private void BCP()
        {
            maBuuCucChuyenThuDen = "593280";
            isBaoDamChuyenThuDen = false;
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(50);
                SendKeys.SendWait("3");
            }
            bwLayCT.RunWorkerAsync();
        }

        private void BuuCuc0()
        {
            if (BuuCucs.Count < 1)
            {
                return;
            }
            int i = 0;
            maBuuCucChuyenThuDen = BuuCucs[i].MaBuuCuc;
            isBaoDamChuyenThuDen = BuuCucs[i].IsBaoDam;
            var temp = BuuCucs[i].GoFastBCCP.Split(',');
            if (!APIManager.ThoatToDefault(BuuCucs[i].MaBCCP, "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait(temp[0]);
                Thread.Sleep(50);
                SendKeys.SendWait(temp[1]);
            }
            if (!bwLayCT.IsBusy)
            {
                bwLayCT.CancelAsync();
                bwLayCT.RunWorkerAsync();
            }
        }

        private void BuuCuc1()
        {
            if (BuuCucs.Count < 2)
            {
                return;
            }

            int i = 1;
            maBuuCucChuyenThuDen = BuuCucs[i].MaBuuCuc;
            isBaoDamChuyenThuDen = BuuCucs[i].IsBaoDam;
            var temp = BuuCucs[i].GoFastBCCP.Split(',');
            if (!APIManager.ThoatToDefault(BuuCucs[i].MaBCCP, "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait(temp[0]);
                Thread.Sleep(50);
                SendKeys.SendWait(temp[1]);
            }
            if (!bwLayCT.IsBusy)
            {
                bwLayCT.CancelAsync();
                bwLayCT.RunWorkerAsync();
            }
        }

        private void BuuCuc2()
        {
            if (BuuCucs.Count < 3)
            {
                return;
            }
            int i = 2;
            maBuuCucChuyenThuDen = BuuCucs[i].MaBuuCuc;
            isBaoDamChuyenThuDen = BuuCucs[i].IsBaoDam;
            var temp = BuuCucs[i].GoFastBCCP.Split(',');
            if (!APIManager.ThoatToDefault(BuuCucs[i].MaBCCP, "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait(temp[0]);
                Thread.Sleep(50);
                SendKeys.SendWait(temp[1]);
            }
            if (!bwLayCT.IsBusy)
            {
                bwLayCT.CancelAsync();
                bwLayCT.RunWorkerAsync();
            }
        }

        private void BuuCuc3()
        {
            if (BuuCucs.Count < 4)
            {
                return;
            }
            int i = 3;
            maBuuCucChuyenThuDen = BuuCucs[i].MaBuuCuc;
            isBaoDamChuyenThuDen = BuuCucs[i].IsBaoDam;
            var temp = BuuCucs[i].GoFastBCCP.Split(',');
            if (!APIManager.ThoatToDefault(BuuCucs[i].MaBCCP, "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait(temp[0]);
                Thread.Sleep(50);
                SendKeys.SendWait(temp[1]);
            }
            if (!bwLayCT.IsBusy)
            {
                bwLayCT.CancelAsync();
                bwLayCT.RunWorkerAsync();
            }
        }

        private void BuuCuc4()
        {
            if (BuuCucs.Count < 5)
            {
                return;
            }
            int i = 4;
            maBuuCucChuyenThuDen = BuuCucs[i].MaBuuCuc;
            isBaoDamChuyenThuDen = BuuCucs[i].IsBaoDam;
            var temp = BuuCucs[i].GoFastBCCP.Split(',');
            if (!APIManager.ThoatToDefault(BuuCucs[i].MaBCCP, "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait(temp[0]);
                Thread.Sleep(50);
                SendKeys.SendWait(temp[1]);
            }
            if (!bwLayCT.IsBusy)
            {
                bwLayCT.CancelAsync();
                bwLayCT.RunWorkerAsync();
            }
        }

        private void BuuCuc5()
        {
            if (BuuCucs.Count < 6)
            {
                return;
            }
            int i = 5;
            maBuuCucChuyenThuDen = BuuCucs[i].MaBuuCuc;
            isBaoDamChuyenThuDen = BuuCucs[i].IsBaoDam;
            var temp = BuuCucs[i].GoFastBCCP.Split(',');
            if (!APIManager.ThoatToDefault(BuuCucs[i].MaBCCP, "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait(temp[0]);
                Thread.Sleep(50);
                SendKeys.SendWait(temp[1]);
            }
            if (!bwLayCT.IsBusy)
            {
                bwLayCT.CancelAsync();
                bwLayCT.RunWorkerAsync();
            }
        }

        private void BuuCuc6()
        {
            if (BuuCucs.Count < 7)
            {
                return;
            }
            int i = 6;
            maBuuCucChuyenThuDen = BuuCucs[i].MaBuuCuc;
            isBaoDamChuyenThuDen = BuuCucs[i].IsBaoDam;
            var temp = BuuCucs[i].GoFastBCCP.Split(',');
            if (!APIManager.ThoatToDefault(BuuCucs[i].MaBCCP, "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait(temp[0]);
                Thread.Sleep(50);
                SendKeys.SendWait(temp[1]);
            }
            if (!bwLayCT.IsBusy)
            {
                bwLayCT.CancelAsync();
                bwLayCT.RunWorkerAsync();
            }
        }

        private void BuuCuc7()
        {
            if (BuuCucs.Count < 8)
            {
                return;
            }
            int i = 7;
            maBuuCucChuyenThuDen = BuuCucs[i].MaBuuCuc;
            isBaoDamChuyenThuDen = BuuCucs[i].IsBaoDam;
            var temp = BuuCucs[i].GoFastBCCP.Split(',');
            if (!APIManager.ThoatToDefault(BuuCucs[i].MaBCCP, "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait(temp[0]);
                Thread.Sleep(50);
                SendKeys.SendWait(temp[1]);
            }
            if (!bwLayCT.IsBusy)
            {
                bwLayCT.CancelAsync();
                bwLayCT.RunWorkerAsync();
            }
        }

        private void BuuCuc8()
        {
            if (BuuCucs.Count < 9)
            {
                return;
            }
            int i = 8;
            maBuuCucChuyenThuDen = BuuCucs[i].MaBuuCuc;
            isBaoDamChuyenThuDen = BuuCucs[i].IsBaoDam;
            var temp = BuuCucs[i].GoFastBCCP.Split(',');
            if (!APIManager.ThoatToDefault(BuuCucs[i].MaBCCP, "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait(temp[0]);
                Thread.Sleep(50);
                SendKeys.SendWait(temp[1]);
            }
            if (!bwLayCT.IsBusy)
            {
                bwLayCT.CancelAsync();
                bwLayCT.RunWorkerAsync();
            }
        }

        public ICommand BuuCuc9Command { get; }

        private void BuuCuc9()
        {
            int i = 9;
            if (BuuCucs.Count < (i + 1))
            {
                return;
            }

            maBuuCucChuyenThuDen = BuuCucs[i].MaBuuCuc;
            isBaoDamChuyenThuDen = BuuCucs[i].IsBaoDam;
            var temp = BuuCucs[i].GoFastBCCP.Split(',');
            if (!APIManager.ThoatToDefault(BuuCucs[i].MaBCCP, "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait(temp[0]);
                Thread.Sleep(50);
                SendKeys.SendWait(temp[1]);
            }
            if (!bwLayCT.IsBusy)
            {
                bwLayCT.CancelAsync();
                bwLayCT.RunWorkerAsync();
            }
        }

        private void BwLayCT_DoWork(object sender, DoWorkEventArgs e)
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

            ChuyenThuDen(controls);
            Thread.Sleep(200);
            SendKeys.SendWait("{ENTER}");

            window = APIManager.WaitingFindedWindow("canh bao", "thong bao", time: 20);
            if (window == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy cảnh báo");
                return;
            }

            SendKeys.SendWait("{ENTER}");
            Thread.Sleep(500);
            SendKeys.SendWait("{F5}");
            if (DataManager.IsWaitingCompleteLayComplete)
            {
                DataManager.IsWaitingCompleteLayComplete = false;
                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "XN593200" });
            }
        }

        private void ChuyenThuDen(List<TestAPIModel> controls)
        {
            List<TestAPIModel> EditControls = controls.Where(m => m.ClassName == "Edit").ToList();
            if (isBaoDamChuyenThuDen)
            {
                //SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}");
                //buu pham bao dam
                //SendKeys.SendWait("{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}");
                APIManager.setTextControl(EditControls.LastOrDefault().Handle, "Bưu phẩm bảo đảm - Registed Mail");
            }
            else
            {
                //SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}{UP}");
                //buu pham bao dam
                //SendKeys.SendWait("{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}{DOWN}");
                APIManager.setTextControl(EditControls.LastOrDefault().Handle, "Bưu kiện - Parcel");
            }
            SendKeys.SendWait("{TAB}");
            APIManager.setTextControl(EditControls[EditControls.Count - 2].Handle, maBuuCucChuyenThuDen);
            //SendKeys.SendWait(maBuuCucChuyenThuDen);
            SendKeys.SendWait("{TAB}");

            TestAPIModel editSoCT = controls.Last(m => m.ClassName.ToLower().IndexOf(".edit.") != -1);
            APIManager.setTextControl(editSoCT.Handle, "");

            if (IsLayDuLieu)
                SendKeys.SendWait("{F8}");
        }

        private void SaveBuuCuc()
        {
            FileManager.SaveBuuCucOffline(BuuCucs.ToList());
        }

        private readonly BackgroundWorker bwLayCT;
        private ObservableCollection<BuuCucModel> _BuuCucs;
        private int _SelectedIndexBC;
        private bool isBaoDamChuyenThuDen = false;
        private string maBuuCucChuyenThuDen = "";

        public ICommand BCPCommand { get; }

        public ICommand BongSonCommand { get; }

        public ICommand BuuCuc0Command { get; }

        public ICommand BuuCuc1Command { get; }

        public ICommand BuuCuc2Command { get; }

        public ICommand BuuCuc3Command { get; }

        public ICommand BuuCuc4Command { get; }

        public ICommand BuuCuc5Command { get; }

        public ICommand BuuCuc6Command { get; }

        public ICommand BuuCuc7Command { get; }

        public ICommand BuuCuc8Command { get; }

        public ObservableCollection<BuuCucModel> BuuCucs
        {
            get { return _BuuCucs; }
            set { SetProperty(ref _BuuCucs, value); }
        }

        public ICommand GiaoDichCommand { get; }
        public ICommand HoaiDucCommand { get; }
        public ICommand HoaiHaiCommand { get; }
        public ICommand HoaiHuongCommand { get; }
        public ICommand HoaiMyCommand { get; }
        public ICommand HoaiTanCommand { get; }
        public ICommand HoaiXuanCommand { get; }
        public ICommand SaveBuuCucCommand { get; }

        public int SelectedIndexBC
        {
            get { return _SelectedIndexBC; }
            set { SetProperty(ref _SelectedIndexBC, value); }
        }

        public ICommand TestCommand { get; }
    }
}