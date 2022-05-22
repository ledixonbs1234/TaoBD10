using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public TamQuanViewModel()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += Timer_Tick;
            TamQuans = new ObservableCollection<TamQuanModel>();
            SendCommand = new RelayCommand(Send);
            FillMaHieuCommand = new RelayCommand(FillMaHieu);

            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "TamQuan")
                {
                    TamQuans.Add(new TamQuanModel(TamQuans.Count + 1, m.Content.ToUpper()));
                }
                else if (m.Key == "TamQuanRun")
                {
                    timer.Start();
                }
            });

            WeakReferenceMessenger.Default.Register<TamQuansMessage>(this, (r, m) => { 
                if(m.Value!= null)
                {
                    TamQuans.Clear();
                    foreach (TamQuanModel item in m.Value)
                    {
                        TamQuans.Add(item);
                    }
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "Navigation", Content = "TamQuan" });

                }
            
            });
        }

        void FillMaHieu()
        {

            TamQuans.Clear();
            foreach (string item in LocTextTho(TextFill))
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                string textChanged = item.Trim().ToUpper();
                if (textChanged.Length != 13)
                {
                    continue;
                }
                TamQuans.Add(new TamQuanModel(TamQuans.Count + 1, item));
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
                Thread.Sleep(1500);
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
                        TamQuans.Add(new TamQuanModel(TamQuans.Count + 1, code.ToUpper()));

                        SoundManager.playSound(@"Number\tamquan.wav");
                        Thread.Sleep(500);

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