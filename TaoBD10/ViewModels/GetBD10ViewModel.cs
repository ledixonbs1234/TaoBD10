using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class GetBD10ViewModel : ObservableObject
    {
        private bool[] _BuoiArray = new bool[] { true, false, false, false };

        private string _NameBD = "0";

        private bool isWaitingGetData = false;

        private int _AnimatedProgressInCard = 50;

        public int AnimatedProgressInCard
        {
            get => _AnimatedProgressInCard;
            set => SetProperty(ref _AnimatedProgressInCard, value);
        }

        private bool _IsLoading;

        public bool IsLoading
        {
            get { return _IsLoading; }
            set { SetProperty(ref _IsLoading, value); }
        }

        private int _ValueLoading;

        public int ValueLoading
        {
            get { return _ValueLoading; }
            set { SetProperty(ref _ValueLoading, value); }
        }

        private DispatcherTimer timer;

        private List<TuiHangHoa> tuiTempHangHoa;

        public GetBD10ViewModel()
        {
            WeakReferenceMessenger.Default.Register<MessageManager>(this, ReceiveMessage);
            TestCommand = new RelayCommand(new Action(() =>
            {
            }));
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;
        }

        public bool[] BuoiArray
        {
            get { return _BuoiArray; }
        }

        public int SelectedBuoi
        {
            get { return Array.IndexOf(_BuoiArray, true); }
        }

        public ICommand TestCommand { get; }

        public string NameBD
        {
            get { return _NameBD; }
            set { SetProperty(ref _NameBD, value); }
        }

        private string _CountTui;

        public string CountTui
        {
            get { return _CountTui; }
            set { SetProperty(ref _CountTui, value); }
        }

        public void RunAutoGetData()
        {
            timer.Start();
        }

        private void PhanLoai(string textXacNhan)
        {
            if (String.IsNullOrEmpty(textXacNhan))
            {
                System.Windows.MessageBox.Show("Bạn chưa nhập phần xác nhận chi tiết");
                return;
            }

            //Lay va xu ly du lieu phan chi tiet
            var tuiXacNhans = new List<TuiXacNhanModel>();

            List<string> listTemp = textXacNhan.Split('\n').ToList();
            if (listTemp[0].IndexOf("STT") != -1)
            {
                listTemp.RemoveAt(0);
            }
            foreach (string item in listTemp)
            {
                string[] listSubTemp = item.Split('\t');
                tuiXacNhans.Add(new TuiXacNhanModel(listSubTemp[1], listSubTemp[3], listSubTemp[5], listSubTemp[2], listSubTemp[6]));
            }

            //sau khi loc du lieu xong
            //bat dau ghep vao
            if (tuiTempHangHoa.Count == 0)
            {
                System.Windows.MessageBox.Show("Chua co du lieu");
                return;
            }
            foreach (var item in tuiTempHangHoa)
            {
                int countChiTiet = 0;
                TuiXacNhanModel tempChiTiet = null;
                foreach (TuiXacNhanModel chiTiet in tuiXacNhans)
                {
                    if (item.SCT == chiTiet.SCT && item.KhoiLuong == chiTiet.KhoiLuong && item.TuiSo == chiTiet.TuiSo)
                    {
                        tempChiTiet = chiTiet;
                        countChiTiet++;
                    }
                }
                if (countChiTiet == 1)
                {
                    item.PhanLoai = tempChiTiet.DVChiTiet;
                }
            }
        }

        private void ReceiveMessage(object content, MessageManager message)
        {
            if (message.Value == "getData")
            {
                RunAutoGetData();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isWaitingGetData)
            {
                return;
            }
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;

            string textCo = APIManager.convertToUnSign3(currentWindow.text).ToLower();

            if (textCo.IndexOf("xac nhan bd10 den") != -1)
            {
                isWaitingGetData = true;
                System.Windows.Clipboard.Clear();

                //thuc hien lay thong tin cua bd nay
                var handles = APIManager.GetAllChildHandles(currentWindow.hwnd);
                string textHandleName = "WindowsForms10.COMBOBOX.app.0.1e6fa8e";
                string textDateTime = "WindowsForms10.SysDateTimePick32.app.0.1e6fa8e";
                string textEdit = "WindowsForms10.EDIT.app.0.1e6fa8e";
                string textSoLuongTui = "WindowsForms10.STATIC.app.0.1e6fa8e";
                int countTuiInBD = 0;
                int slTuiInBD = 0;
                int countComBoBox = 0;
                int countDateTime = 0;
                string noiGuiBD = "";
                string ngayThangBD = "";
                string lanLapBD = "";
                foreach (var item in handles)
                {
                    string classText = APIManager.GetWindowClass(item);

                    if (classText.IndexOf(textHandleName) != -1)
                    {
                        if (countComBoBox == 7)
                        {
                            noiGuiBD = APIManager.GetControlText(item);
                            countComBoBox++;
                        }
                        else
                        {
                            countComBoBox++;
                        }
                    }
                    else
                    if (classText.IndexOf(textDateTime) != -1)
                    {
                        if (countDateTime == 1)
                        {
                            ngayThangBD = APIManager.GetControlText(item);

                            countDateTime++;
                        }
                        else
                        {
                            countDateTime++;
                        }
                    }
                    else if (classText.IndexOf(textEdit) != -1)
                    {
                        lanLapBD = APIManager.GetControlText(item);
                    }
                    else if (classText.IndexOf(textSoLuongTui) != -1)
                    {
                        if (countTuiInBD == 22)
                        {
                            slTuiInBD = int.Parse(APIManager.GetControlText(item));
                        }
                        countTuiInBD++;
                    }
                    //tim cai o cua sh tui
                    //focus no
                    //xong roi dien vao va nhan enter thoi
                }

                //thuc hien xu ly ngay thang bd
                DateTime ngayThang = DateTime.Now;
                DateTime.TryParseExact(ngayThangBD, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ngayThang);

                var info = FileManager.list.Find(m => m.Name == noiGuiBD && m.LanLap == lanLapBD && m.DateCreateBD10.DayOfYear == ngayThang.DayOfYear);
                if (info != null)
                {
                    isWaitingGetData = false;
                    timer.Stop();

                    SoundManager.playSound(@"Number\trungbd.wav");
                    return;
                }
                NameBD = noiGuiBD;
                IsLoading = true;
                ValueLoading = 50;

                SendKeys.SendWait("{F3}");
                Thread.Sleep(200);
                SendKeys.SendWait("^{UP}");
                Thread.Sleep(200);

                String lastText = "";
                int countSame = 0;
                tuiTempHangHoa = new List<TuiHangHoa>();
                while (countSame <= 3)
                {
                    SendKeys.SendWait("^C");
                    Thread.Sleep(100);
                    string textClip;
                    try
                    {
                        textClip = System.Windows.Clipboard.GetText() + '\n';
                    }
                    catch (Exception)
                    {
                        NameBD = "Chạy Lại";
                        isWaitingGetData = false;
                        timer.Stop();
                        return;
                    }

                    if (textClip.IndexOf("STT") != -1)
                    {
                        textClip = textClip.Split('\n')[1];
                    }
                    if (lastText == textClip)
                    {
                        countSame++;
                    }
                    else
                    {
                        lastText = textClip;
                        countSame = 0;
                    }

                    List<string> listString = textClip.Split('\t').ToList();
                    if (listString.Count == 11)
                    {
                        if (tuiTempHangHoa.FindIndex(m => m.SHTui == listString[9]) == -1)
                        {
                            tuiTempHangHoa.Add(new TuiHangHoa(listString[0], listString[1], listString[3], listString[2], listString[4], listString[5], listString[6], listString[8], listString[9]));
                        }
                    }
                    else
                    {
                        isWaitingGetData = false;
                        NameBD = "Lỗi! Không Copy Được";
                        timer.Stop();
                        return;
                    }
                    //tuiHangHoas.Add(TuiHangHoa);
                    SendKeys.SendWait("{DOWN}");
                }
                if (slTuiInBD != tuiTempHangHoa.Count)
                {
                    isWaitingGetData = false;
                    timer.Stop();
                    IsLoading = false;
                    ValueLoading = 0;

                    SoundManager.playSound(@"Number\chuadusoluong.wav");
                    return;
                }

                /// Kiem tra trong nay co trung so luong voi file khong neu khong thi hie nra thong bao loi

                //dgvMain.DataSource = tuiHangHoas;
                CountTui = tuiTempHangHoa.Count.ToString();

                //thuc hien vao xac nhan chi tiet trong nay
                SendKeys.SendWait("{F4}");
                Thread.Sleep(500);
                for (int i = 0; i < 4; i++)
                {
                    SendKeys.SendWait("{TAB}");
                    Thread.Sleep(50);
                }
                //thuc hien ctrl a
                SendKeys.SendWait("^(a)");
                Thread.Sleep(700);

                //thuc hien ctrl c
                SendKeys.SendWait("^(c)");
                Thread.Sleep(100);

                String textXacNhan = System.Windows.Clipboard.GetText();
                if (string.IsNullOrEmpty(textXacNhan))
                    return;
                PhanLoai(textXacNhan);
                IsLoading = false;
                ValueLoading = 100;

                FileManager.SaveData(new BD10InfoModel(noiGuiBD, tuiTempHangHoa, ngayThang, (EnumAll.TimeSet)SelectedBuoi, lanLapBD));
                CountTui = tuiTempHangHoa.Count().ToString();
                SendKeys.SendWait("{ESC}");
                isWaitingGetData = false;
                timer.Stop();
            }
        }

        //var bd10Info = new BD10InfoModel();
        //bd10Info.Name = noiGuiBD;
        //    bd10Info.TuiHangHoas = tuiTempHangHoa;
        //    bd10Info.CountTui = tuiTempHangHoa.Count.ToString();
        //    bd10Info.DateCreateBD10 = ngayThang;
        //    bd10Info.LanLap = lanLapBD;

        //    bd10Info.TimeTrongNgay = (EnumAll.TimeSet) SelectedBuoi;

        ////thuc hien save BD Nay
        //FileManager.SaveData(bd10Info);

        //NgayThang CreateNgayThang(string date)
        //{
        //    using (DataProvider context = new DataProvider())
        //    {
        //        var content = context.NgayThangs.FirstOrDefault(m => m.Day == date);
        //        if (content == null || context.NgayThangs.Count() == 0)
        //        {
        //            context.NgayThangs.Add(new NgayThang { Day = date });
        //            context.SaveChanges();
        //        }
        //        return context.NgayThangs.FirstOrDefault(m => m.Day == date);
        //    }
        //}

        //public BD10 createBD10(string name, string lanLap, int ngayThangID)
        //{
        //    using (DataProvider context = new DataProvider())
        //    {
        //        var content = context.BD10s.FirstOrDefault(m => m.DisplayName == name && m.LanLap == lanLap);
        //        if (content == null || context.BD10s.Count() == 0)
        //        {
        //            context.BD10s.Add(new BD10 { DisplayName = name, LanLap = lanLap, IdNgayThang = ngayThangID });
        //            context.SaveChanges();
        //        }
        //        return context.BD10s.FirstOrDefault(m => m.DisplayName == name && m.LanLap == lanLap);
        //    }
        //}
    }
}