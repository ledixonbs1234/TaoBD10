﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;

namespace TaoBD10.ViewModels
{
    public class LayBDHAViewModel : ObservableObject
    {
        private DispatcherTimer timer;
        private string maBuuCuc = "";
        private bool isWating = false;
        BackgroundWorker bwLayBD;

        public LayBDHAViewModel()
        {
            AnLaoCommand = new RelayCommand(AnLao);
            HoaiAnCommand = new RelayCommand(HoaiAn);
            AnMyCommand = new RelayCommand(AnMy);
            AnHoaCommand = new RelayCommand(AnHoa);
            LayToanBoCommand = new RelayCommand(LayToanBo);
            bwLayBD = new BackgroundWorker();
            NamTrungBoCommand = new RelayCommand(NamTrungBo);
            DaNangCommand = new RelayCommand(DaNang);
            bwLayBD.DoWork += BwLayBD_DoWork;
            bwLayBD.RunWorkerCompleted += BwLayBD_RunWorkerCompleted;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += Timer_Tick;
        }
        private readonly bool[] _LanLapArray = new bool[] { true, false, false, false,false };
        public bool[] LanLapArray
        {
            get { return _LanLapArray; }
        }

        public int SelectedLanLap
        {
            get { return Array.IndexOf(_LanLapArray, true); }
        }

        private void BwLayBD_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isRunComplete = true;
        }

        public ICommand DaNangCommand { get; }

        
        public ICommand NamTrungBoCommand { get; }

        

        void NamTrungBo()
        {
            maBuuCuc = "590100";
            indexBuuCuc = 515;
            //timer.Start();
            bwLayBD.RunWorkerAsync();
        }

        void DaNang()
        {
            maBuuCuc = "550910";
            indexBuuCuc = 446;
            //timer.Start();
            bwLayBD.RunWorkerAsync();
        }


        const uint WM_SETTEXT = 0x000C;
        private void BwLayBD_DoWork(object sender, DoWorkEventArgs e)
        {
            WindowInfo currentWindow = APIManager.WaitingFindedWindow("danh sach bd10 den");
            if (currentWindow == null)
            {
                return;
            }
            System.Collections.Generic.List<Model.TestAPIModel> childControl = APIManager.GetListControlText(currentWindow.hwnd);
            int countCombobox = 0;
            IntPtr combo = IntPtr.Zero;

            string classDefault = "WindowsForms10.COMBOBOX.app.0.1e6fa8e";
            string classEditDefault = "WindowsForms10.EDIT.app.0.1e6fa8e";
            string classButtonDefault = "WindowsForms10.BUTTON.app.0.1e6fa8e";

            if (childControl.Count < 5)
            {
                return;
            }
            Model.TestAPIModel combobox = childControl.FindAll(m => m.ClassName == classDefault)[1];
            Model.TestAPIModel editControl = childControl.FindAll(m => m.ClassName == classEditDefault)[1];
            Model.TestAPIModel buttonFindControl = childControl.FindAll(m => m.ClassName == classButtonDefault)[5];
            Model.TestAPIModel buttonGetControl = childControl.FindAll(m => m.ClassName == classButtonDefault)[4];
            const int CB_SETCURSEL = 0x014E;
            APIManager.SendMessage(combobox.Handle, CB_SETCURSEL, indexBuuCuc, 0);
            APIManager.SendMessage(editControl.Handle, WM_SETTEXT, IntPtr.Zero, new StringBuilder((SelectedLanLap+1).ToString()));
            APIManager.ClickButton(currentWindow.hwnd, buttonGetControl.Handle);
            APIManager.ClickButton(currentWindow.hwnd, buttonFindControl.Handle);
        }
        bool isRunComplete = false;
        public ICommand LayToanBoCommand { get; }


        void LayToanBo()
        {
            AnMy();
            HoaiAn();
            AnLao();
            AnHoa();

        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isWating)
                return;

            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
            {
                return;
            }
            if (currentWindow.text.IndexOf("danh sach bd10 den") != -1)
            {
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
                        if (countCombobox == 1)
                        {
                            //road = item;
                            combo = item;
                            break;
                        }
                        countCombobox++;
                    }
                }
                isWating = true;
                APIManager.SendMessage(combo, 0x0007, 0, 0);
                APIManager.SendMessage(combo, 0x0007, 0, 0);
                SendKeys.SendWait(maBuuCuc);
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(200);
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(200);
                //thuc hien ctrl A
                SendKeys.SendWait("^(a){BS}");
                SendKeys.SendWait("1");
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(200);
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(500);
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(200);
                SendKeys.SendWait(" ");
                timer.Stop();
                isWating = false;
                //isWaitingRead = true;
                //SendKeys.SendWait(numberTinh.ToString() + "{TAB}");
                //isWaitingRead = false;
            }
        }

        public ICommand AnHoaCommand { get; }
        public ICommand AnLaoCommand { get; }
        public ICommand AnMyCommand { get; }
        public ICommand HoaiAnCommand { get; }

        private void AnHoa()
        {
            maBuuCuc = "593880";
            indexBuuCuc = 552;
            //timer.Start();
            bwLayBD.RunWorkerAsync();
        }
        int indexBuuCuc = 0;
        private void AnLao()
        {
            maBuuCuc = "593850";
            indexBuuCuc = 550;
            //timer.Start();
            bwLayBD.RunWorkerAsync();
        }

        private void AnMy()
        {
            maBuuCuc = "593630";
            indexBuuCuc = 545;
            //timer.Start();
            bwLayBD.RunWorkerAsync();
        }

        private void HoaiAn()
        {
            maBuuCuc = "593740";
            indexBuuCuc = 547;
            //timer.Start();
            bwLayBD.RunWorkerAsync();
        }
    }
}