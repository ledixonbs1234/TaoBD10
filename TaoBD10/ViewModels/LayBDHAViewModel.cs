using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExcelLibrary.BinaryFileFormat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class LayBDHAViewModel : ObservableObject
    {
        public LayBDHAViewModel()
        {
            Button1Command = new RelayCommand(Button1);
            SaveLayBDOfflineCommand = new RelayCommand(SaveLayBDOffline);

            Button6Command = new RelayCommand(Button6);
            SaveLayBDCommand = new RelayCommand(SaveLayBD);
            Button5Command = new RelayCommand(Button5);
            Button0Command = new RelayCommand(Button0);
            Button4Command = new RelayCommand(Button4);
            Button3Command = new RelayCommand(Button3);
            Button2Command = new RelayCommand(Button2);
            LayToanBoCommand = new RelayCommand(LayToanBo);
            BD10Infos = new ObservableCollection<LayBD10Info>();
            bwLayBD = new BackgroundWorker();
            bwLayBD.DoWork += BwLayBD_DoWork;
            bwLayBD.RunWorkerCompleted += BwLayBD_RunWorkerCompleted;
            GetDataFromCloudCommand = new RelayCommand(GetDataFromCloud);

            bwGetDanhSachBD = new BackgroundWorker();
            bwGetDanhSachBD.DoWork += BwGetDanhSachBD_DoWork;
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 500)
            };
            timer.Tick += Timer_Tick;
            ShowData(FileManager.LoadLayBDOffline());

            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "Button")
                {
                    if (m.Content == "AnMy")
                    {
                        selectionBDIndex = 1;
                        Button3();
                    }
                    else if (m.Content == "HoaiAn")
                    {

                        selectionBDIndex = 2;
                        Button2();
                    }
                    else if (m.Content == "AnLao")
                    {

                        selectionBDIndex = 3;
                        Button4();
                    }
                    else if (m.Content == "AnHoa")
                    {

                        selectionBDIndex = 4;
                        Button5();
                    }
                }
                else if (m.Key == "ToLayBDHA_LayDanhSach")
                {
                    bwGetDanhSachBD.RunWorkerAsync();
                }
                else if (m.Key == "ToLayHAAL_LayBD")
                {
                    isGetDataFromPhone = true;
                    string[] datas = m.Content.Split('|');
                    string bdIndex = datas[0];
                    string lanLap = datas[1];
                    _LanLapArray = new bool[] { false, false, false, false, false };
                    _LanLapArray[int.Parse(lanLap) - 1] = true;
                    switch (bdIndex)
                    {

                        case "1":
                            Button0();

                            break;
                        case "2":

                            Button1();
                            break;
                        case "3":

                            Button2();
                            break;
                        case "4":

                            Button3();
                            break;
                        case "5":

                            Button4();
                            break;
                        case "6":

                            Button5();
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        private void BwGetDanhSachBD_DoWork(object sender, DoWorkEventArgs e)
        {
            var temp = FileManager.optionModel.GoFastBD10Den.Split(',');
            APIManager.GoToWindow(FileManager.optionModel.MaKhaiThac, "danh sach bd14", temp[0], temp[1]);
            WindowInfo currentWindow = APIManager.WaitingFindedWindow("danh sach bd10 den");
            if (currentWindow == null)
            {
                return;
            }
            Thread.Sleep(100);
            SendKeys.SendWait("{TAB}");
            Thread.Sleep(100);
            SendKeys.SendWait("^{UP}");
            Thread.Sleep(200);

            string lastText = "";
            int countSame = 0;
            List<BD10DenInfo> bD10Dens = new List<BD10DenInfo>();
            string test ="";
            while (countSame <= 3)
            {
                string textClip = APIManager.GetCopyData();

                if (string.IsNullOrEmpty(textClip))
                {
                    APIManager.ShowSnackbar("Chạy Lại");
                    return;
                }
                //593880-An Hòa	14/09/2022	1	Ô tô	5	18,7	Đã nhận
                if (lastText == textClip)
                {
                    countSame++;
                }
                else
                {
                    lastText = textClip;
                    countSame = 0;
                    List<string> listString = textClip.Split('\t').ToList();
                    if (listString.Count >= 6)
                    {
                        test += listString[0] + '\n';
                        
                        //bD10Dens.Add(new BD10DenInfo(APIManager.ConvertToUnSign3(listString[0]), listString[2], listString[4], listString[5], APIManager.ConvertToUnSign3(listString[6])));
bD10Dens.Add(new BD10DenInfo(listString[0], listString[2], listString[4], listString[5], listString[6]));
                    }
                    else
                    {
                        APIManager.ShowSnackbar("Lỗi! Không Copy Được");
                        return;
                    }
                }

                //tuiHangHoas.Add(TuiHangHoa);
                SendKeys.SendWait("{DOWN}");
            }
            if (bD10Dens.Count > 0)
            {

                        APIManager.ShowSnackbar(bD10Dens[0].Name);
                APIManager.OpenNotePad(test, "df");
                string jsonText = JsonConvert.SerializeObject(bD10Dens, Formatting.Indented);
                MqttManager.Pulish(FileManager.MQTTKEY + "_laydanhsachbd", jsonText);


            }
        }

        int selectionBDIndex = 0;


        void ShowData(List<LayBD10Info> data)
        {
            if (data != null)
                if (data.Count != 0)
                {
                    BD10Infos.Clear();
                    foreach (LayBD10Info item in data)
                    {
                        BD10Infos.Add(item);
                    }
                }
        }
        public ICommand GetDataFromCloudCommand { get; }

        void GetDataFromCloud()
        {
            ShowData(FileManager.LoadLayBDOnFirebase());
        }



        private void AnHoa()
        {
            maBuuCuc = "593880";
            indexBuuCuc = 552;
            //timer.Start();
            bwLayBD.RunWorkerAsync();
        }

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

        void Button0()
        {
            int i = 0;
            if (BD10Infos.Count < (i + 1))
            {
                return;
            }
            maBuuCuc = BD10Infos[i].MaBuuCuc;
            indexBuuCuc = BD10Infos[i].IndexBuuCuc;
            bwLayBD.RunWorkerAsync();
        }

        void Button1()
        {
            int i = 1;
            if (BD10Infos.Count < (i + 1))
            {
                return;
            }
            maBuuCuc = BD10Infos[i].MaBuuCuc;
            indexBuuCuc = BD10Infos[i].IndexBuuCuc;
            bwLayBD.RunWorkerAsync();
        }

        void Button2()
        {
            int i = 2;
            if (BD10Infos.Count < (i + 1))
            {
                return;
            }
            maBuuCuc = BD10Infos[i].MaBuuCuc;
            indexBuuCuc = BD10Infos[i].IndexBuuCuc;
            bwLayBD.RunWorkerAsync();
        }

        void Button3()
        {
            int i = 3;
            if (BD10Infos.Count < (i + 1))
            {
                return;
            }
            maBuuCuc = BD10Infos[i].MaBuuCuc;
            indexBuuCuc = BD10Infos[i].IndexBuuCuc;
            bwLayBD.RunWorkerAsync();
        }

        void Button4()
        {
            int i = 4;
            if (BD10Infos.Count < (i + 1))
            {
                return;
            }
            maBuuCuc = BD10Infos[i].MaBuuCuc;
            indexBuuCuc = BD10Infos[i].IndexBuuCuc;
            if (!bwLayBD.IsBusy)
                bwLayBD.RunWorkerAsync();
        }

        void Button5()
        {
            int i = 5;
            if (BD10Infos.Count < (i + 1))
            {
                return;
            }
            maBuuCuc = BD10Infos[i].MaBuuCuc;
            indexBuuCuc = BD10Infos[i].IndexBuuCuc;
            bwLayBD.RunWorkerAsync();
        }

        void Button6()
        {
            int i = 6;
            if (BD10Infos.Count < (i + 1))
            {
                return;
            }
            maBuuCuc = BD10Infos[i].MaBuuCuc;
            indexBuuCuc = BD10Infos[i].IndexBuuCuc;
            bwLayBD.RunWorkerAsync();
        }

        BackgroundWorker bwGetDanhSachBD;


        private void BwLayBD_DoWork(object sender, DoWorkEventArgs e)
        {
            var temp = FileManager.optionModel.GoFastBD10Den.Split(',');
            APIManager.GoToWindow(FileManager.optionModel.MaKhaiThac, "danh sach bd10 den", temp[0], temp[1]);
            WindowInfo currentWindow = APIManager.WaitingFindedWindow("danh sach bd10 den");
            if (currentWindow == null)
            {
                return;
            }
            System.Collections.Generic.List<Model.TestAPIModel> childControl = APIManager.GetListControlText(currentWindow.hwnd);
            IntPtr combo = IntPtr.Zero;

            string classDefault = "WindowsForms10.COMBOBOX.app.0.1e6fa8e";
            string classEditDefault = "WindowsForms10.EDIT.app.0.1e6fa8e";
            string classButtonDefault = "WindowsForms10.BUTTON.app.0.1e6fa8e";

            if (childControl.Count < 5)
            {
                return;
            }
            TestAPIModel combobox = childControl.FindAll(m => m.ClassName == classDefault)[1];
            TestAPIModel editControl = childControl.FindAll(m => m.ClassName == classEditDefault)[1];
            TestAPIModel buttonFindControl = childControl.FindAll(m => m.ClassName == classButtonDefault)[5];
            TestAPIModel buttonGetControl = childControl.FindAll(m => m.ClassName == classButtonDefault)[4];
            //const int CB_SETCURSEL = 0x014E;
            //APIManager.SendMessage(combobox.Handle, CB_SETCURSEL, indexBuuCuc, 0);
            APIManager.SendMessage(combobox.Handle, 0x0007, 0, 0);
            APIManager.SendMessage(combobox.Handle, 0x0007, 0, 0);

            APIManager.SendMessage(childControl[20].Handle, WM_SETTEXT, IntPtr.Zero, new StringBuilder(maBuuCuc));
            SendKeys.SendWait("{TAB}");
            Thread.Sleep(200);

            APIManager.SendMessage(editControl.Handle, WM_SETTEXT, IntPtr.Zero, new StringBuilder((SelectedLanLap + 1).ToString()));
            APIManager.ClickButton(currentWindow.hwnd, buttonGetControl.Handle);
            APIManager.ClickButton(currentWindow.hwnd, buttonFindControl.Handle);
            SendKeys.SendWait("{TAB}");
            Thread.Sleep(200);
            var textBoxHandle = childControl.FirstOrDefault(m => m.ClassName.IndexOf(".EDIT.") != -1);
            if (textBoxHandle == null)
                return;
            APIManager.FocusHandle(textBoxHandle.Handle);
            SendKeys.SendWait("{TAB}");
            //string data = APIManager.GetCopyData();

            if (isGetDataFromPhone)
            {
                APIManager.ShowSnackbar("Chay");
                isGetDataFromPhone = false;
                bwGetDanhSachBD.RunWorkerAsync();
            }
            //int countEnter = data.Split('\n').Length;
            //if (countEnter == 2)
            //{
            //    data = data.Split('\n')[1];
            //    //thuc hien cong viec trong nay
            //    string[] texts = data.Split('\t');
            //    if (texts[0].Substring(0, 6) != maBuuCuc)
            //    {
            //        return;
            //    }
            //    string slTui = texts[4];
            //    if (MqttManager.IsConnected)
            //    {
            //        MqttManager.Pulish(FileManager.MQTTKEY + "_data", "soluong|" + selectionBDIndex.ToString() + "|" + slTui);
            //    }
            //}


        }

        private void BwLayBD_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {


        }

        private void DaNang()
        {
            maBuuCuc = "550910";
            indexBuuCuc = 446;
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

        private void LayToanBo()
        {
            AnMy();
            HoaiAn();
            AnLao();
            AnHoa();
        }

        private void NamTrungBo()
        {
            maBuuCuc = "590100";
            indexBuuCuc = 515;
            //timer.Start();
            bwLayBD.RunWorkerAsync();
        }

        void SaveLayBD()
        {
            FileManager.SaveLayBDFirebase(BD10Infos.ToList());
        }

        public ICommand SaveLayBDOfflineCommand { get; }


        void SaveLayBDOffline()
        {
            FileManager.SaveLayBDOffline(BD10Infos.ToList());
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

        private const uint WM_SETTEXT = 0x000C;
        private bool[] _LanLapArray = new bool[] { true, false, false, false, false };
        private readonly BackgroundWorker bwLayBD;
        private readonly DispatcherTimer timer;
        private ObservableCollection<LayBD10Info> _BD10Infos;
        private int _IndexBD10Infos;
        private int indexBuuCuc = 0;
        private bool isWating = false;
        private string maBuuCuc = "";
        private bool isGetDataFromPhone;

        public ICommand AnHoaCommand { get; }
        public ICommand AnLaoCommand { get; }
        public ICommand AnMyCommand { get; }
        public ObservableCollection<LayBD10Info> BD10Infos
        {
            get { return _BD10Infos; }
            set { SetProperty(ref _BD10Infos, value); }
        }

        public ICommand Button0Command { get; }
        public ICommand Button1Command { get; }
        public ICommand Button2Command { get; }
        public ICommand Button3Command { get; }
        public ICommand Button4Command { get; }
        public ICommand Button5Command { get; }
        public ICommand Button6Command { get; }
        public ICommand DaNangCommand { get; }
        public ICommand HoaiAnCommand { get; }
        public int IndexBD10Infos
        {
            get { return _IndexBD10Infos; }
            set { SetProperty(ref _IndexBD10Infos, value); }
        }

        public bool[] LanLapArray
        {
            get { return _LanLapArray; }
        }

        public ICommand LayToanBoCommand { get; }
        public ICommand NamTrungBoCommand { get; }
        public ICommand SaveLayBDCommand { get; }
        public int SelectedLanLap
        {
            get { return Array.IndexOf(_LanLapArray, true); }
        }

    }
}