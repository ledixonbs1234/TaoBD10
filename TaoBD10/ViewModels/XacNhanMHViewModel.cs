using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class XacNhanMHViewModel : ObservableObject
    {
        public XacNhanMHViewModel()
        {
            _XacNhanInfo = new XacNhanInfoModel();
            TrangThais = new ObservableCollection<ThongTinTrangThaiModel>();
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
                App.Current.Dispatcher.Invoke(delegate // <--- HERE
                {
                    XacNhanMH = m.Value;
                    TrangThais.Clear();
                    foreach (var item in m.Value.ThongTins)
                    {
                        TrangThais.Add(item);
                    }
                    IsWaitingComplete = false;
                    if (IsAutoGoCT)
                        GoToCTCommand.Execute(null);
                });
            }
        });
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "ToXNMH_XacNhanList")
                {
                    IsAutoDoiKiem = true;
                    listDoiKiem = JsonConvert.DeserializeObject<List<string>>(m.Content);
                    MaHieu = listDoiKiem[0] + "\n";
                }
            });
        }

        private List<string> listDoiKiem = new List<string>();

        private bool IsAutoDoiKiem = false;

        private bool _IsAutoGoCT = true;

        public bool IsAutoGoCT
        {
            get { return _IsAutoGoCT; }
            set { SetProperty(ref _IsAutoGoCT, value); }
        }

        private XacNhanInfoModel _XacNhanInfo;

        private bool _IsWaitingComplete = false;

        public bool IsWaitingComplete
        {
            get { return _IsWaitingComplete; }
            set { SetProperty(ref _IsWaitingComplete, value); }
        }

        private void BackgroundWorkerXacNhan_DoWork(object sender, DoWorkEventArgs e)
        {
            try
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
                if (_XacNhanInfo.IsChieuDen)
                {
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
                    Thread.Sleep(500);
                    SendKeys.SendWait("{F6}");
                    Thread.Sleep(500);
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
                    if (datas.Length < 4)
                    {
                        APIManager.ShowSnackbar("Không có dữ liệu CT");
                        return;
                    }
                    if (datas[3].Trim() == _XacNhanInfo.SoCT.Trim() && datas[1].Trim() == _XacNhanInfo.MaBCDong.Trim())
                    {
                        SendKeys.SendWait("{F10}");
                        WindowInfo windows = APIManager.WaitingFindedWindow("xem chuyen thu chieu den");
                        if (windows == null)
                            return;
                        SendKeys.SendWait("A{BS}{BS}");
                        SendKeys.SendWait("{F4}");
                        windows = APIManager.WaitingFindedWindow("xac nhan chi tiet tui thu");
                        if (windows == null)
                        {
                            IsAutoDoiKiem = false;
                            return;
                        }
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "XacNhanChiTiet", Content = "True" });

                        if (IsAutoDoiKiem)
                        {
                            IsAutoDoiKiem = false;
                            WindowInfo currentWindow = APIManager.GetActiveWindowTitle();

                            List<TestAPIModel> listControl = APIManager.GetListControlText(currentWindow.hwnd);
                            string numberText = listControl[2].Text;
                            numberText = numberText.Replace("(cái)", "").Trim();
                            int number = int.Parse(numberText);

                            Thread.Sleep(1000);

                            //thuc hien xu ly lenh trong nay
                            foreach (var mahieu in listDoiKiem)
                            {
                                Thread.Sleep(500);
                                SendKeys.SendWait(mahieu);
                                SendKeys.SendWait("{ENTER}");
                            }
                            Thread.Sleep(700);
                            currentWindow = APIManager.GetActiveWindowTitle();
                            if (currentWindow.text.IndexOf("xac nhan chi tiet tui thu") != -1)
                            {
                                listControl = APIManager.GetListControlText(currentWindow.hwnd);
                                numberText = listControl[2].Text;
                                numberText = numberText.Replace("(cái)", "").Trim();
                                int numberLast = int.Parse(numberText);
                                APIManager.ShowSnackbar("number last :" + numberLast + " number: " + number + " count: " + listDoiKiem.Count);

                                if (number + listDoiKiem.Count == numberLast)
                                {
                                    //THuc hien send thanh cong
                                    FileManager.SendMessageNotification("Đã xác nhận thành công");
                                }
                            }
                            else
                            {
                                FileManager.SendMessageNotification("Đã xác nhận thành công");
                            }
                        }
                    }
                }
                else
                {
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
                    Thread.Sleep(500);
                    SendKeys.SendWait("{F6}");
                    Thread.Sleep(500);
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
                    if (datas.Length < 4)
                    {
                        APIManager.ShowSnackbar("Không có dữ liệu CT");
                        return;
                    }
                    if (datas[3].Trim() == _XacNhanInfo.SoCT.Trim() && datas[1].Trim() == _XacNhanInfo.MaBCDong.Trim())
                    {
                        SendKeys.SendWait("{F10}");
                        WindowInfo windows = APIManager.WaitingFindedWindow("xem chuyen thu chieu den");
                        if (windows == null)
                            return;
                        SendKeys.SendWait("A{BS}{BS}");
                        SendKeys.SendWait("{F4}");
                        windows = APIManager.WaitingFindedWindow("xac nhan chi tiet tui thu");
                        if (windows == null)
                        {
                            IsAutoDoiKiem = false;
                            return;
                        }
                        WeakReferenceMessenger.Default.Send(new ContentModel { Key = "XacNhanChiTiet", Content = "True" });

                        if (IsAutoDoiKiem)
                        {
                            IsAutoDoiKiem = false;
                            WindowInfo currentWindow = APIManager.GetActiveWindowTitle();

                            List<TestAPIModel> listControl = APIManager.GetListControlText(currentWindow.hwnd);
                            string numberText = listControl[2].Text;
                            numberText = numberText.Replace("(cái)", "").Trim();
                            int number = int.Parse(numberText);

                            Thread.Sleep(1000);

                            //thuc hien xu ly lenh trong nay
                            foreach (var mahieu in listDoiKiem)
                            {
                                Thread.Sleep(500);
                                SendKeys.SendWait(mahieu);
                                SendKeys.SendWait("{ENTER}");
                            }
                            Thread.Sleep(700);
                            currentWindow = APIManager.GetActiveWindowTitle();
                            if (currentWindow.text.IndexOf("xac nhan chi tiet tui thu") != -1)
                            {
                                listControl = APIManager.GetListControlText(currentWindow.hwnd);
                                numberText = listControl[2].Text;
                                numberText = numberText.Replace("(cái)", "").Trim();
                                int numberLast = int.Parse(numberText);
                                APIManager.ShowSnackbar("number last :" + numberLast + " number: " + number + " count: " + listDoiKiem.Count);

                                if (number + listDoiKiem.Count == numberLast)
                                {
                                    //THuc hien send thanh cong
                                    FileManager.SendMessageNotification("Đã xác nhận thành công");
                                }
                            }
                            else
                            {
                                FileManager.SendMessageNotification("Đã xác nhận thành công");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "XNMaHieu " + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
            }
        }

        private void ChuyenThuDen(List<TestAPIModel> controls)
        {
            List<TestAPIModel> EditControls = controls.Where(m => m.ClassName == "Edit").ToList();

            string loaiString = "";
            switch (_XacNhanInfo.LoaiCT.ToUpper())
            {
                case "C":
                    loaiString = "Bưu kiện - Parcel";
                    break;

                case "E":
                    loaiString = "EMS - Chuyển phát nhanh - Express Mail Service";
                    break;

                case "R":
                    loaiString = "Bưu phẩm bảo đảm - Registed Mail";
                    break;

                case "P":
                    loaiString = "Logistic";
                    break;

                default:
                    break;
            }
            APIManager.setTextControl(EditControls.LastOrDefault().Handle, loaiString);
            Thread.Sleep(2000);
            SendKeys.SendWait("{TAB}");

            Thread.Sleep(20);
            if (_XacNhanInfo.IsChieuDen)
            {
                APIManager.setTextControl(EditControls[EditControls.Count - 2].Handle, _XacNhanInfo.MaBCDong);
            }
            else
            {
                SendKeys.SendWait(_XacNhanInfo.MaBCNhan);
                //APIManager.setTextControl(EditControls[EditControls.Count - 2].Handle, _XacNhanInfo.MaBCNhan);
            }
            //SendKeys.SendWait(maBuuCucChuyenThuDen);
            SendKeys.SendWait("{TAB}");
            Thread.Sleep(20);
            SendKeys.SendWait("{TAB}");

            Thread.Sleep(20);
            TestAPIModel editSoCT = controls.Last(m => m.ClassName.ToLower().IndexOf(".edit.") != -1);
            APIManager.setTextControl(editSoCT.Handle, _XacNhanInfo.SoCT);
            if (_XacNhanInfo.IsChieuDen)
                SendKeys.SendWait("{TAB}");
            SendKeys.SendWait(_XacNhanInfo.Date[0]);
            SendKeys.SendWait("{RIGHT}");
            SendKeys.SendWait(_XacNhanInfo.Date[1]);

            Thread.Sleep(20);
            SendKeys.SendWait("{TAB}");

            Thread.Sleep(20);
            SendKeys.SendWait("{DOWN}");

            if (_XacNhanInfo.IsChieuDen)
            {
                SendKeys.SendWait("{F8}");
            }
            else
            {
                SendKeys.SendWait("{F5}");
            }
        }

        private void GoToCT()
        {
            _XacNhanInfo = new XacNhanInfoModel();
            if (XacNhanMH == null)
                return;
            //quy trinh thuc hien
            //kiem tra thu buu cuc nhan co 280 hay 230 ko
            // neu co thi chay vao 1 trong n2 cai do
            //sau do vao chuyen thu
            //sau do chay vao do dua vao thong tin cua no va dien len
            if (XacNhanMH.BuuCucNhan.IndexOf("593280") != -1)
            {
                _XacNhanInfo.IsChieuDen = true;
                _XacNhanInfo.Is280 = true;
                //thuc hien go to chuyen  thu
                XuLyThongTin();
                if (!APIManager.ThoatToDefault("593280", "quan ly chuyen thu chieu deen"))
                {
                    SendKeys.SendWait("3");
                    Thread.Sleep(200);
                    SendKeys.SendWait("3");
                }
                backgroundWorkerXacNhan.RunWorkerAsync();
            }
            else if (XacNhanMH.BuuCucNhan.IndexOf("593230") != -1)
            {
                _XacNhanInfo.Is280 = false;
                _XacNhanInfo.IsChieuDen = true;
                //thuc hien xu ly thong tin can thiet
                XuLyThongTin();
                if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu deen"))
                {
                    SendKeys.SendWait("1");
                    Thread.Sleep(200);
                    SendKeys.SendWait("3");
                }
                backgroundWorkerXacNhan.RunWorkerAsync();
            }
            else if (XacNhanMH.BuuCucDong.IndexOf("593280") != -1)
            {
                _XacNhanInfo.Is280 = true;
                _XacNhanInfo.IsChieuDen = false;
                //thuc hien go to chuyen  thu
                XuLyThongTin();
                if (!APIManager.ThoatToDefault("593280", "quan ly chuyen thu chieu dii"))
                {
                    SendKeys.SendWait("3");
                    Thread.Sleep(200);
                    SendKeys.SendWait("3");
                }
                backgroundWorkerXacNhan.RunWorkerAsync();
            }
            else if (XacNhanMH.BuuCucDong.IndexOf("593230") != -1)
            {
                _XacNhanInfo.Is280 = false;
                _XacNhanInfo.IsChieuDen = false;
                //thuc hien xu ly thong tin can thiet
                XuLyThongTin();
                if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu dii"))
                {
                    SendKeys.SendWait("1");
                    Thread.Sleep(200);
                    SendKeys.SendWait("2");
                }
                backgroundWorkerXacNhan.RunWorkerAsync();
            }
        }

        private ObservableCollection<ThongTinTrangThaiModel> _TrangThais;

        public ObservableCollection<ThongTinTrangThaiModel> TrangThais
        {
            get { return _TrangThais; }
            set { SetProperty(ref _TrangThais, value); }
        }

        private void OnEnterKey()
        {
            if (MaHieu.IndexOf('\n') != -1)
            {
                int soLuong = MaHieu.Where(m => m == '\n').Count();
                if (soLuong > 1)
                {
                    MaHieu = "";
                    return;
                }
                string temp = "";
                if (MaHieu.Length >= 13)
                {
                    temp = MaHieu.Substring(0, 13);
                }

                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "XacNhanMH", Content = temp });
                IsWaitingComplete = true;

                MaHieu = "";
            }
        }

        public ICommand CopyMHCommand { get; }

        private void CopyMH()
        {
            if (!string.IsNullOrEmpty(MaHieu))
            {
                Clipboard.SetText(MaHieu);
                APIManager.ShowSnackbar("Đã copy");
            }
        }

        private void Test()
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
            _XacNhanInfo.Date[0] = dates[0];
            _XacNhanInfo.Date[1] = dates[1];
            _XacNhanInfo.Date[2] = dates[2].Substring(0, 4);
            //xu ly ct
            string[] temps = XacNhanMH.TTCT.ToLower().Split('/');
            _XacNhanInfo.LoaiCT = temps[0].Replace("chuyến thư ", "");

            _XacNhanInfo.SoCT = temps[1].Replace("số ct ", "");
            _XacNhanInfo.MaBCDong = XacNhanMH.BuuCucDong.Substring(0, 6);
            _XacNhanInfo.MaBCNhan = XacNhanMH.BuuCucNhan.Substring(0, 6);
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

        private readonly string[] date = new string[3];
        private string _MaHieu;
        private string _TestText;
        private KiemTraModel _XacNhanMH;
        private readonly BackgroundWorker backgroundWorker;
        private readonly BackgroundWorker backgroundWorkerXacNhan;
    }
}