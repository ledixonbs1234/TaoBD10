using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
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
        private int lanLap = 1;
        private bool isWating = false;

        public LayBDHAViewModel()
        {
            AnLaoCommand = new RelayCommand(AnLao);
            HoaiAnCommand = new RelayCommand(HoaiAn);
            AnMyCommand = new RelayCommand(AnMy);
            AnHoaCommand = new RelayCommand(AnHoa);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += Timer_Tick;
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
            timer.Start();
        }

        private void AnLao()
        {
            maBuuCuc = "593850";
            timer.Start();
        }

        private void AnMy()
        {
            maBuuCuc = "593630";
            timer.Start();
        }

        private void HoaiAn()
        {
            maBuuCuc = "593740";
            timer.Start();
        }
    }
}