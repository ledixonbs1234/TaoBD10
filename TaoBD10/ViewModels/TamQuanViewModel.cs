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
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class TamQuanViewModel : ObservableObject
    {
        private string _MaBCP;

        public string MaBCP
        {
            get { return _MaBCP; }
            set { SetProperty(ref _MaBCP, value); }
        }

        BackgroundWorker bwTamQuanCheck;

        public ICommand FillMaBCCommand { get; }

        private List<string> LocBCP(string TextFill)
        {
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(TextFill))
                return null;
            string[] texts = TextFill.Split('\n');
            //2	ED594304497VN	59	593200	590000	EU	2800	0
            //3   RA596820153VN   59  593200  593330  R!  50  0
            foreach (var item in texts)
            {
                string[] splitTabTexts = item.Split(' ');
                if (splitTabTexts.Length >= 7)
                {
                    if (MaBCP == splitTabTexts[4])
                    {
                        list.Add(splitTabTexts[1]);
                    }
                }
            }
            return list;
        }

        private void FillMaBC()
        {
            if (string.IsNullOrEmpty(_MaBCP))
                return;

            TamQuans.Clear();
            List<string> list = LocBCP(TextFill);
            for (int i = 0; i < list.Count; i++)
            {
                string item = list[i];
                TamQuans.Add(new TamQuanModel(i + 1, list[i]));
            }
        }

        public TamQuanViewModel()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += Timer_Tick;
            bwTamQuanCheck = new BackgroundWorker();
            bwTamQuanCheck.DoWork += BwTamQuanCheck_DoWork;
            TamQuans = new ObservableCollection<TamQuanModel>();
            SendCommand = new RelayCommand(Send);
            FillMaHieuCommand = new RelayCommand(FillMaHieu);
            FillMaBCCommand = new RelayCommand(FillMaBC);
            SortCommand = new RelayCommand(Sort);
            Numbers = new ObservableCollection<int>();
            Numbers.Add(1);
            Numbers.Add(2);
            Numbers.Add(3);
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "TamQuan")
                {
                    TamQuans.Add(new TamQuanModel(TamQuans.Count + 1, m.Content.ToUpper()));
                }
                else if (m.Key == "TamQuanRun")
                {
                    if (!bwTamQuanCheck.IsBusy)
                        bwTamQuanCheck.RunWorkerAsync();
                }
            });

            WeakReferenceMessenger.Default.Register<TamQuansMessage>(this, (r, m) =>
            {
                if (m.Value != null)
                {
                    TamQuans.Clear();
                    foreach (TamQuanModel item in m.Value)
                    {
                        TamQuans.Add(item);
                    }
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Navigation", Content = "TamQuan" });
                }
            });
            WeakReferenceMessenger.Default.Register<ChuyenTamQuanMHMessage>(this, (r, m) =>
            {
                if (m.Value != null)
                {
                    TamQuans.Add(new TamQuanModel(TamQuans.Count + 1, m.Value.MaHieu.ToUpper(), m.Value.TrongLuong));
                    SoundManager.playSound(@"Number\" + TamQuans.Count.ToString() + ".wav");
                }
            });
        }

        private void BwTamQuanCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;

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
                    if (code.Length == 13)
                    {
                        //them du lieu vao
                        if (TamQuans.Count >= 1)
                        {
                            foreach (TamQuanModel item in TamQuans)
                            {
                                if (item.MaHieu == code.ToUpper())
                                {
                                    return;
                                }
                            }
                        }
                        App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                        {
                            if (isDoubleRight)
                            {
                                TamQuans.Add(new TamQuanModel(TamQuans.Count + 1, code.ToUpper(), klTemp));
                            }
                            else
                            {
                                TamQuans.Add(new TamQuanModel(TamQuans.Count + 1, code.ToUpper()));
                            }

                            SoundManager.playSound(@"Number\" + TamQuans.Count.ToString() + ".wav");
                        });


                    }
                }
            };
        }

        public ICommand SortCommand { get; }

        private void Sort()
        {
            if (TamQuans.Count > 0)
            {
                var tempList = TamQuans.ToList();
                tempList.Sort((m, n) => m.TrongLuong.CompareTo(n.TrongLuong));
                TamQuans.Clear();
                foreach (var item in tempList)
                {
                    TamQuans.Add(item);
                }
            }
        }

        private int _CurrentNumber = 2;

        public int CurrentNumber
        {
            get { return _CurrentNumber; }
            set { SetProperty(ref _CurrentNumber, value); }
        }

        private ObservableCollection<int> _Numbers;

        public ObservableCollection<int> Numbers
        {
            get { return _Numbers; }
            set { SetProperty(ref _Numbers, value); }
        }

        private void FillMaHieu()
        {
            TamQuans.Clear();
            foreach (string item in LocTextTho(TextFill))
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                string textChanged = item.Trim().ToUpper();
                if (textChanged.Length == 13 || textChanged.Length == 29)
                {
                    TamQuans.Add(new TamQuanModel(TamQuans.Count + 1, item));
                }
                else
                {
                    continue;
                }
            }
        }

        private List<string> LocTextTho(string textsRange)
        {
            List<string> list = new List<string>();
            var datas = textsRange.Split('\n');
            foreach (string data in datas)
            {
                if (data.Count() < 13)
                    continue;
                var indexVN = data.ToUpper().IndexOf("VN");
                if (indexVN - 11 < 0)
                    continue;
                list.Add(data.Substring(indexVN - 11, 13));
            }
            return list;
        }

        private void Send()
        {
            if (TamQuans.Count == 0)
            {
                return;
            }

            Thread.Sleep(3000);
            foreach (TamQuanModel item in TamQuans)
            {
                SendKeys.SendWait(item.MaHieu);
                SendKeys.SendWait("{ENTER}");
                Thread.Sleep(CurrentNumber * 500);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isBusyTamQuan)
            {
                return;
            }

            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;

            string textCo = APIManager.ConvertToUnSign3(currentWindow.text).ToLower();
            if (textCo.IndexOf("xem chuyen thu chieu den") != -1)
            {
                isBusyTamQuan = true;
                SendKeys.SendWait("^(c)");
                string data = Clipboard.GetText();

                //loc du lieu trong nay
                var temp = data.Split('\n');
                if (temp.Length == 2)
                {
                    string code = temp[1].Split('\t')[1];
                    bool isDoubleRight = double.TryParse(temp[1].Split('\t')[6], out double klTemp);

                    //xu ly du lieu trong nay
                    if (code.Length == 13)
                    {
                        //them du lieu vao
                        if (TamQuans.Count >= 1)
                        {
                            foreach (TamQuanModel item in TamQuans)
                            {
                                if (item.MaHieu == code.ToUpper())
                                {
                                    timer.Stop();
                                    isBusyTamQuan = false;
                                    return;
                                }
                            }
                        }
                        if (isDoubleRight)
                        {
                            TamQuans.Add(new TamQuanModel(TamQuans.Count + 1, code.ToUpper(), klTemp));
                        }
                        else
                        {
                            TamQuans.Add(new TamQuanModel(TamQuans.Count + 1, code.ToUpper()));
                        }

                        SoundManager.playSound(@"Number\" + TamQuans.Count.ToString() + ".wav");
                    }
                }

                timer.Stop();
                isBusyTamQuan = false;
            }
        }

        public ICommand FillMaHieuCommand { get; }
        public ICommand SendCommand { get; }

        public ObservableCollection<TamQuanModel> TamQuans
        {
            get { return _TamQuans; }
            set { SetProperty(ref _TamQuans, value); }
        }

        public string TextFill
        {
            get { return _TextFill; }
            set { SetProperty(ref _TextFill, value); }
        }

        private ObservableCollection<TamQuanModel> _TamQuans;
        private string _TextFill;
        private bool isBusyTamQuan = false;
        private DispatcherTimer timer;
    }
}