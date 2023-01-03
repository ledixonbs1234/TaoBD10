using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class GetBD10ViewModel : ObservableObject
    {
        private bool[] _BuoiArray = new bool[] { true, false, false, false };

        private string _NameBD = "";

        private int _AnimatedProgressInCard = 50;

        private bool _SoSanhSLTui = true;

        public bool SoSanhSLTui
        {
            get { return _SoSanhSLTui; }
            set { SetProperty(ref _SoSanhSLTui, value); }
        }

        private BackgroundWorker bwGetData;

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

        private readonly List<MaBD8Model> listMaBD8;

        private List<TuiHangHoa> tuiTempHangHoa;

        private BackgroundWorker bwGoToBd;

        public GetBD10ViewModel()
        {
            listMaBD8 = new List<MaBD8Model>();
            listMaBD8 = FileManager.GetMaBD8s();
            WeakReferenceMessenger.Default.Register<MessageManager>(this, ReceiveMessage);
            TestCommand = new RelayCommand(new Action(() =>
            {
            }));
            bwGoToBd = new BackgroundWorker();
            bwGoToBd.DoWork += BwGoToBd_DoWork;

            bwGetData = new BackgroundWorker();
            bwGetData.WorkerSupportsCancellation = true;
            bwGetData.DoWork += BwGetData_DoWork;
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "BD10BUOI")
                {
                    if (m.Content == "Toi")
                    {
                        BuoiArray[3] = true;
                    }
                }
                else if (m.Key == "ToGetBD_SaveBD")
                {
                    if (FileManager.IS_PHONE_IS_EXCUTTING)
                    {
                        FileManager.SendMessageNotification("Đang lưu BD");
                    }
                    string[] datas = m.Content.Split('|');
                    currentMaBuuCuc = datas[0];
                    currentLanLap = datas[1];
                    _BuoiArray = new bool[] { false, false, false, false };
                    _BuoiArray[int.Parse(datas[2])] = true;
                    bwGoToBd.RunWorkerAsync();
                }
            });
        }

        private string currentMaBuuCuc = "";
        private string currentLanLap = "";

        private void BwGoToBd_DoWork(object sender, DoWorkEventArgs e)
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
            bool isFinded = false;
            while (countSame <= 3)
            {
                string textClip = APIManager.GetCopyData();

                if (string.IsNullOrEmpty(textClip))
                {
                    APIManager.ShowSnackbar("Chạy Lại");
                    MqttManager.SendMessageToPhone("Chạy Lại");
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
                        if (listString[0].Substring(0, 6) == currentMaBuuCuc && listString[2] == currentLanLap)
                        {
                            isFinded = true;
                            break;
                        }
                    }
                    else
                    {
                        APIManager.ShowSnackbar("Lỗi! Không Copy Được");
                        MqttManager.SendMessageToPhone("Lỗi! Không Copy Được");
                        return;
                    }
                }
                SendKeys.SendWait("{DOWN}");
            }

            //thuc hien lenh
            if (!isFinded)
            {
                return;
            }
            APIManager.ClickButton(currentWindow.hwnd, "xac nhan", isExactly: false);
            APIManager.WaitingFindedWindow("xac nhan bd10 den");
            Thread.Sleep(1500);
            RunAutoGetData();
        }

        private void BwGetData_DoWork(object sender, DoWorkEventArgs e)
        {
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
                return;
            //System.Windows.Clipboard.Clear();

            //thuc hien lay thong tin cua bd nay
            List<TestAPIModel> childHandles = APIManager.GetListControlText(currentWindow.hwnd);
            string noiGuiBD = "";
            string ngayThangBD = "";
            string lanLapBD = "";
            int slTuiInBD = 0;
            noiGuiBD = childHandles.Where(m => m.ClassName.IndexOf("COMBOBOX.app") != -1).ToList()[7].Text;
            ngayThangBD = childHandles.Where(m => m.ClassName.IndexOf("SysDateTimePick32") != -1).ToList()[1].Text;
            lanLapBD = childHandles.Last(m => m.ClassName.IndexOf(".EDIT.app.0") != -1).Text;
            slTuiInBD = int.Parse(childHandles.Where(m => m.ClassName.IndexOf(".STATIC.app") != -1).ToList()[22].Text);
            if (SelectedBuoi == -1)
            {
                APIManager.ShowSnackbar("Bạn chưa chọn buổi trong ngày");
                MqttManager.SendMessageToPhone("Bạn chưa chọn buổi trong ngày");
                return;
            }
            //thuc hien xu ly ngay thang bd
            DateTime ngayThang = DateTime.Now;
            //DateTime.TryParseExact(ngayThangBD, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out ngayThang);

            var info = FileManager.list.Find(m => m.Name == noiGuiBD && m.LanLap == lanLapBD && m.NgayThangBD == ngayThangBD && m.TimeTrongNgay == (TimeSet)SelectedBuoi);
            if (info != null)
            {
                SoundManager.playSound(@"Number\trungbd.wav");
                MqttManager.SendMessageToPhone("Trùng BD");
                return;
            }

            NameBD = noiGuiBD;
            IsLoading = true;
            ValueLoading = 50;

            SendKeys.SendWait("{F3}");
            Thread.Sleep(300);
            SendKeys.SendWait("^{UP}");
            Thread.Sleep(300);

            String lastText = "";
            int countSame = 0;
            tuiTempHangHoa = new List<TuiHangHoa>();
            while (countSame <= 3)
            {
                string textClip = APIManager.GetCopyData();

                if (string.IsNullOrEmpty(textClip))
                {
                    NameBD = "Chạy Lại";
                    MqttManager.SendMessageToPhone("Chạy lại");
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
                    MqttManager.SendMessageToPhone("Lỗi! Không Copy Được");
                    NameBD = "Lỗi! Không Copy Được";
                    return;
                }
                //tuiHangHoas.Add(TuiHangHoa);
                SendKeys.SendWait("{DOWN}");
            }
            if (SoSanhSLTui)
                if (slTuiInBD != tuiTempHangHoa.Count)
                {
                    IsLoading = false;
                    ValueLoading = 0;

                    SoundManager.playSound(@"Number\chuadusoluong.wav");
                    MqttManager.SendMessageToPhone(" Chưa đủ số lượng");
                    return;
                }

            /// Kiem tra trong nay co trung so luong voi file khong neu khong thi hie nra thong bao loi

            //dgvMain.DataSource = tuiHangHoas;
            CountTui = tuiTempHangHoa.Count.ToString();

            //Thuc hien viec loc dua vao ma bat dau
            PhanLoaiSHTui(tuiTempHangHoa);

            //thuc hien vao xac nhan chi tiet trong nay
            SendKeys.SendWait("{F4}");
            WindowInfo window = APIManager.WaitingFindedWindow("xac nhan tui thu den");
            if (window == null)
            {
                APIManager.ShowSnackbar("Không tìm thấy window xác nhận");
            }
            Thread thread = new Thread(() => System.Windows.Clipboard.Clear());
            string maBDGui = noiGuiBD.Substring(0, 6);
            string dataCopyed = "";
            if (maBDGui == "593740" || maBDGui == "593850" || maBDGui == "593880" || maBDGui == "593630")
            {
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(50);
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(50);
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(50);
                SendKeys.SendWait("^(a)");
                Thread.Sleep(800);

                dataCopyed = APIManager.GetCopyData();
                if (!string.IsNullOrEmpty(dataCopyed))
                {
                    if (dataCopyed.Split('\n')[0].Split('\t').Length == 11)
                    {
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(50);
                    }
                    else
                    {
                        APIManager.ClickButton(window.hwnd, ">");
                        Thread.Sleep(500);
                        SendKeys.SendWait("+{TAB}");
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    SendKeys.SendWait("{TAB}");
                    Thread.Sleep(50);
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    SendKeys.SendWait("{TAB}");
                    Thread.Sleep(50);
                }
                //thuc hien ctrl a
            }

            SendKeys.SendWait("^(a)");
            Thread.Sleep(800);
            dataCopyed = APIManager.GetCopyData();
            if (string.IsNullOrEmpty(dataCopyed))
            {
                APIManager.ShowSnackbar("Không copy được");
                MqttManager.SendMessageToPhone("Không copy được");
                return;
            }
            PhanLoai(dataCopyed);
            IsLoading = false;
            ValueLoading = 100;

            FileManager.SaveBD10Offline(new BD10InfoModel(noiGuiBD, tuiTempHangHoa, ngayThang, (EnumAll.TimeSet)SelectedBuoi, lanLapBD));
            CountTui = tuiTempHangHoa.Count().ToString();
            SendKeys.SendWait("{ESC}");
            SoundManager.playSound2(@"Number\tingting.wav");
            APIManager.ShowSnackbar("OK");
            //MqttManager.SendMessageToPhone("OK");

            //MqttManager.Pulish(FileManager.FirebaseKey + "_luubd", currentMaBuuCuc + "|" + currentLanLap);
            if (FileManager.IS_PHONE_IS_EXCUTTING)
            {
                FileManager.SendMessage(new MessageToPhoneModel("luubd", currentMaBuuCuc + "|" + currentLanLap));
                //FileManager.client.Child(FileManager.FirebaseKey + "/message/").PutAsync(@"{""tophone"":""" + currentMaBuuCuc + "|" + currentLanLap + @"""}");
            }

            WeakReferenceMessenger.Default.Send("LoadBD10");
        }

        private void PhanLoaiSHTui(List<TuiHangHoa> list)
        {
            List<TuiThuModel> tuiThus = FileManager.LoadTuiThuOffline();

            foreach (var item in list)
            {
                if (item.SHTui.Length == 29)
                {
                    var findedTuiThu = tuiThus.FirstOrDefault(m => m.Ma == item.SHTui.Substring(13, 2).ToUpper());
                    if (findedTuiThu != null)
                    {
                        item.PhanLoai = findedTuiThu.Content;
                    }
                }
                else if (item.SHTui.Length == 13)
                {
                    item.PhanLoai = "Đi ngoài";
                }
            }
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

        private string _CountTui = "0";

        public string CountTui
        {
            get { return _CountTui; }
            set { SetProperty(ref _CountTui, value); }
        }

        public void RunAutoGetData()
        {
            bwGetData.RunWorkerAsync();
        }

        private void PhanLoai(string textXacNhan)
        {
            if (string.IsNullOrEmpty(textXacNhan))
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
                else if (countChiTiet == 0)
                {
                    if (item.SHTui.Length == 13)
                    {
                        item.PhanLoai = "Đi ngoài";
                    }
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