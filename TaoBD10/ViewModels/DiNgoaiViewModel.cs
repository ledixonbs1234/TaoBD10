using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ExcelLibrary.BinaryFileFormat;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using TaoBD10.Manager;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class DiNgoaiViewModel : ObservableObject
    {
        public DiNgoaiViewModel()
        {
            DiNgoais = new ObservableCollection<DiNgoaiItemModel>();
            BuuCucs = new ObservableCollection<string>();

            timerPrint = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 50)
            };
            timerPrint.Tick += TimerPrint_Tick;
            SelectionCommand = new RelayCommand<DiNgoaiItemModel>(Selection);
            SelectionChiTietCommand = new RelayCommand(SelectionChiTiet);
            bwKhoiTao = new BackgroundWorker();
            bwKhoiTao.WorkerSupportsCancellation = true;
            bwKhoiTao.DoWork += BwKhoiTao_DoWork;
            bwPrintDiNgoai = new BackgroundWorker();
            ShowDataCommand = new RelayCommand(ShowData);

            bwPrintDiNgoai.DoWork += BwPrintDiNgoai_DoWork;
            bwPrintDiNgoai.WorkerSupportsCancellation = true;
            SortTinhCommand = new RelayCommand(SortTinh);

            XoaCommand = new RelayCommand(Xoa);
            ClearCommand = new RelayCommand(Clear);
            MoRongCommand = new RelayCommand(MoRong);
            ClearDiNgoaiCommand = new RelayCommand(ClearDiNgoai);
            AddRangeCommand = new RelayCommand(AddRange);
            XoaDiNgoaiCommand = new RelayCommand(XoaDiNgoai);
            CreateAddressCommand = new RelayCommand(CreateAddress);
            SetMaTinhGuiCommand = new RelayCommand(SetMaTinhGui);
            AddFastCommand = new RelayCommand(AddFast);
            SortCommand = new RelayCommand(Sort);
            SetTinhCommand = new RelayCommand(SetTinhs);
            StopDiNgoaiCommand = new RelayCommand(StopDiNgoai);

            AddAddressCommand = new RelayCommand(AddAddress);
            WeakReferenceMessenger.Default.Register<WebContentModel>(this, (r, m) =>
            {
                if (m.Key != "DiNgoaiAddress")
                    return;
                DiNgoaiItemModel diNgoai = DiNgoais.FirstOrDefault(c => m.Code.IndexOf(c.Code.ToUpper()) != -1);
                if (diNgoai != null)
                {
                    diNgoai.Address = m.AddressReiceive.Trim();
                    diNgoai.MaTinh = m.BuuCucPhat;
                    diNgoai.AddressSend = m.AddressSend;
                    diNgoai.BuuCucGui = m.BuuCucGui;

                    AutoSetBuuCuc(diNgoai);
                }
                if (!string.IsNullOrEmpty(m.AddressReiceive))
                {
                    AddAddress();
                }
                else
                {
                    APIManager.ShowSnackbar("Không có địa chỉ");
                }
            });

            WeakReferenceMessenger.Default.Register<ChiTietTuiMessage>(this, (r, m) =>
            {
                if (m.Value != null)
                {
                    if (m.Value.Key == "DiNgoai")
                    {
                        List<ChiTietTuiModel> chiTietTuis = m.Value.ChiTietTuis;

                        foreach (ChiTietTuiModel chiTietTui in chiTietTuis)
                        {
                            DiNgoaiItemModel have = DiNgoais.FirstOrDefault(s => s.Code.ToUpper() == chiTietTui.MaHieu.ToUpper());
                            if (have != null)
                            {
                                have.Address = chiTietTui.Address.Trim();
                                have.MaTinh = AutoSetTinh(have.Address);
                                AutoSetBuuCuc(have);
                            }
                        }
                        if (FileManager.IS_PHONE_IS_EXCUTTING)
                        {
                            foreach (var diNgoai in DiNgoais)
                            {
                                diNgoai.danhSachBuuCuc = GetListBuuCucFromTinh(diNgoai.MaTinh);
                            }
                            FileManager.SendMessageNotification("Đã lấy bưu cục thành công", disablePhone: true);

                            ChuyenDataDiNgoaiToPhone();
                        }
                        if (isAutoRunDiNgoai)
                        {
                            isAutoRunDiNgoai = false;
                            var diNgoai = DiNgoais[0];
                            if (diNgoai == null)
                                return;
                            var temp = FileManager.optionModel.GoFastKTCTKT.Split(',');
                            APIManager.GoToWindow(FileManager.optionModel.MaKhaiThac, "khoi tao chuyen thu", temp[0], temp[1]);
                            SelectedSimple = diNgoai;
                            Selection(diNgoai);
                        }
                    }
                }
            }
           );

            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "RunPrintDiNgoai")
                {
                    if (!bwPrintDiNgoai.IsBusy)
                    {
                        bwPrintDiNgoai.CancelAsync();
                        bwPrintDiNgoai.RunWorkerAsync();
                    }
                }
                else if (m.Key == "DiNgoaiTuDongNext")
                {
                    DiNgoaiTuDongNext();
                }
                else if (m.Key == "ToDiNgoai_GetAddressPhone")
                {
                    FileManager.SendMessageNotification("Đang lấy bưu cục");
                    //thuc hien dejson
                    var list = JsonConvert.DeserializeObject<List<string>>(m.Content);
                    if (list != null)
                    {
                        App.Current.Dispatcher.Invoke(delegate // <--- HERE
                        {
                            DiNgoais.Clear();

                            foreach (var item in list)
                            {
                                AddMaHieu(item);
                            }
                            AddFast();
                        });
                    }
                    else
                    {
                        FileManager.SendMessageNotification("Lỗi! Không có nội dung bên trong", disablePhone: true);
                    }
                }
                else if (m.Key == "ToDiNgoai")
                {
                    if (m.Content == "AddFast")
                    {
                        AddFast();
                        isAutoRunDiNgoai = true;
                    }
                    else if (m.Content == "Clear")
                    {
                        if (DiNgoais != null)
                        {
                            DiNgoais.Clear();
                        }
                    }
                }
                else if (m.Key == "ToDiNgoai_InDiNgoai")
                {
                    //thuc hien in di ngoai trong nay
                    var diNgoai = DiNgoais.FirstOrDefault(n => n.Code.ToLower() == m.Content.ToLower());
                    if (diNgoai == null)
                        return;
                    var temp = FileManager.optionModel.GoFastKTCTKT.Split(',');
                    APIManager.GoToWindow(FileManager.optionModel.MaKhaiThac, "khoi tao chuyen thu", temp[0], temp[1]);
                    SelectedSimple = diNgoai;
                    Selection(diNgoai);
                }
                else if (m.Key == "ToDiNgoai_ChinhSuaLai")
                {
                    List<string> list = JsonConvert.DeserializeObject<List<string>>(m.Content);
                    foreach (string item in list)
                    {
                        string[] phanChia = item.Split('_');
                        var diNgoai = DiNgoais.FirstOrDefault(n => n.Code == phanChia[0].ToUpper());
                        if (diNgoai == null) continue;
                        diNgoai.MaBuuCuc = phanChia[1];
                        if (!string.IsNullOrEmpty(diNgoai.MaBuuCuc))
                            diNgoai.TenBuuCuc = diNgoai.danhSachBuuCuc.First(d => d.IndexOf(diNgoai.MaBuuCuc) != -1);
                    }
                    ChuyenDataDiNgoaiToPhone();
                }
                else if (m.Key == "ToDiNgoai_SapXepDiNgoai")
                {
                    SortTinh();
                    ChuyenDataDiNgoaiToPhone();
                }
            });

            listBuuCuc = FileManager.LoadBuuCucsOffline();
            listBuuCucTuDong = FileManager.LoadBuuCucTuDongsOffline();

            tinhs = FileManager.LoadTinhThanhOffline();
        }

        private void AddAddress()
        {
            if (DiNgoais.Count == 0)
                return;

            foreach (DiNgoaiItemModel diNgoaiItem in DiNgoais)
            {
                if (string.IsNullOrEmpty(diNgoaiItem.MaTinh))
                {
                    WeakReferenceMessenger.Default.Send(new ContentModel { Key = "LoadAddressWeb", Content = diNgoaiItem.Code });
                    break;
                }
            }
        }

        private void AddFast()
        {
            //thuc hien them dia chi 1 cach nhanh hon
            if (DiNgoais.Count == 0)
                return;
            string listMaHieu = "";
            string addressDefault = "https://bccp.vnpost.vn/BCCP.aspx?act=MultiTrace&id=";
            foreach (DiNgoaiItemModel diNgoaiItem in DiNgoais)
            {
                listMaHieu += diNgoaiItem.Code + ",";
            }
            addressDefault += listMaHieu;
            WeakReferenceMessenger.Default.Send(new ContentModel { Key = "ListAddressDiNgoai", Content = addressDefault });
        }

        private void AddMaHieu(string MaHieu)
        {
            MaHieu = MaHieu.Trim().ToUpper();
            if (MaHieu.Length != 13)
            {
                return;
            }                //    //kiem tra trung khong
            if (DiNgoais.Count == 0)
            {
                DiNgoais.Add(new DiNgoaiItemModel(DiNgoais.Count + 1, MaHieu));
                SoundManager.playSound(@"Number\1.wav");
            }
            else
            {
                foreach (DiNgoaiItemModel item in DiNgoais)
                {
                    if (item.Code == MaHieu)
                    {
                        return;
                    }
                }
                DiNgoais.Add(new DiNgoaiItemModel(DiNgoais.Count + 1, MaHieu));
                if (IsSayNumber)
                {
                    SoundManager.playSound(@"Number\" + DiNgoais.Count.ToString() + ".wav");
                }
            }
        }

        private void AddRange()
        {
            foreach (MaHieuDiNgoaiInfo item in LocTextTho(TextsRange))
            {
                if (item.Code.Length != 13)
                {
                    continue;
                }                //    //kiem tra trung khong
                if (DiNgoais.Count == 0)
                {
                    if (string.IsNullOrEmpty(item.TinhGocGui))
                    {
                        DiNgoais.Add(new DiNgoaiItemModel(DiNgoais.Count + 1, item.Code));
                    }
                    else
                    {
                        DiNgoais.Add(new DiNgoaiItemModel(DiNgoais.Count + 1, item.Code, item.BuuCucGui, item.BuuCucNhanTemp, item.TinhGocGui));
                    }
                }
                else
                {
                    bool isTrundle = false;
                    foreach (DiNgoaiItemModel diNgoai in DiNgoais)
                    {
                        if (diNgoai.Code.ToUpper() == item.Code)
                        {
                            isTrundle = true;
                            break;
                        }
                    }
                    if (isTrundle)
                        continue;

                    if (string.IsNullOrEmpty(item.TinhGocGui))
                    {
                        DiNgoais.Add(new DiNgoaiItemModel(DiNgoais.Count + 1, item.Code));
                    }
                    else
                    {
                        DiNgoais.Add(new DiNgoaiItemModel(DiNgoais.Count + 1, item.Code, item.BuuCucGui, item.BuuCucNhanTemp, item.TinhGocGui));
                    }
                }
            }
        }

        public ICommand CreateAddressCommand { get; }

        private void CreateAddress()
        {
            //thuc hien lay du lieu tu file
            List<MaQuanHuyenInfo> quanHuyens = FileManager.GetDanhSachQuanHuyen();
            if (DiNgoais.Count == 0)
            {
                return;
            }
            SetTinhs();
            foreach (var item in DiNgoais)
            {
                if (string.IsNullOrEmpty(item.BuuCucNhanTemp))
                    continue;
                int maHuyenItem = int.Parse(item.BuuCucNhanTemp.Substring(0, 4));
                var firstQuanHuyen = quanHuyens.FirstOrDefault(m => m.MaQuanHuyen > maHuyenItem);
                if (firstQuanHuyen == null)
                    continue;
                int iQuanHuyens = quanHuyens.IndexOf(firstQuanHuyen);

                MaQuanHuyenInfo quanHuyenDung = quanHuyens[iQuanHuyens - 1];
                if (quanHuyenDung.MaQuanHuyen.ToString().Substring(0, 2) != item.MaTinh)
                {
                    quanHuyenDung = quanHuyens[iQuanHuyens];
                }

                item.Address = "chưa biết -xã chưa biết-" + quanHuyenDung.TenQuanHuyen + "-" + quanHuyenDung.TenTinh;
                AutoSetBuuCuc(item);
            }
        }

        private void AutoSetBuuCuc(DiNgoaiItemModel diNgoai)
        {
            if (diNgoai.MaTinh == null)
                return;

            //thuc hien lay loai buu gui
            string loai = diNgoai.Code.Substring(0, 1).ToUpper();
            if (diNgoai.MaTinh == "59")
            {
                List<string> fillAddress = diNgoai.Address.Split('-').Select(s => s.Trim()).ToList();
                if (fillAddress == null)
                    return;
                if (fillAddress.Count < 3)
                    return;
                string addressExactly = fillAddress[fillAddress.Count - 2];
                if (BoDauAndToLower(addressExactly).IndexOf("phu my") != -1)
                {
                    diNgoai.TenBuuCuc = "592810 - KT Phù Mỹ";
                    diNgoai.MaBuuCuc = "592810";
                }
                else if (BoDauAndToLower(addressExactly).IndexOf("phu cat") != -1)
                {
                    diNgoai.TenBuuCuc = "592460 - BCP Phù Cát";
                    diNgoai.MaBuuCuc = "592460";
                }
                else if (BoDauAndToLower(addressExactly).IndexOf("an nhon") != -1)
                {
                    diNgoai.TenBuuCuc = "592020 - KT An Nhơn";
                    diNgoai.MaBuuCuc = "592020";
                }
                else if (BoDauAndToLower(addressExactly).IndexOf("tay son") != -1)
                {
                    diNgoai.TenBuuCuc = "594210 - KT Tây Sơn";
                    diNgoai.MaBuuCuc = "594210";
                }
                else if (BoDauAndToLower(addressExactly).IndexOf("van canh") != -1)
                {
                    diNgoai.TenBuuCuc = "594560 - KT Vân Canh";
                    diNgoai.MaBuuCuc = "594560";
                }
                else if (BoDauAndToLower(addressExactly).IndexOf("vinh thanh") != -1)
                {
                    diNgoai.TenBuuCuc = "594080 - KT Vĩnh Thạnh";
                    diNgoai.MaBuuCuc = "594080";
                }
                else if (BoDauAndToLower(addressExactly).IndexOf("tuy phuoc") != -1)
                {
                    diNgoai.TenBuuCuc = "591720 - KT Tuy Phước";
                    diNgoai.MaBuuCuc = "591720";
                }
                else if (BoDauAndToLower(addressExactly).IndexOf("quy nhon") != -1)
                {
                    string xa = BoDauAndToLower(fillAddress[fillAddress.Count - 3]);
                    if (xa.Contains("bui thi xuan") || xa.Contains("nhon binh") || xa.Contains("nhon phu") || xa.Contains("phuoc my") || xa.Contains("tran quang dieu"))
                    {
                        diNgoai.TenBuuCuc = "591218 - BCP Quy Nhơn 2";
                        diNgoai.MaBuuCuc = "591218";
                    }
                    else
                    {
                        diNgoai.TenBuuCuc = "591520 - BCP Quy Nhơn";
                        diNgoai.MaBuuCuc = "591520";
                    }
                }
                else if (BoDauAndToLower(addressExactly).IndexOf("hoai an") != -1)
                {
                    string xa = BoDauAndToLower(fillAddress[fillAddress.Count - 3]);
                    if (xa.Contains("an my") || xa.Contains("an tin") || xa.IndexOf("an hao") != -1)
                    {
                        diNgoai.TenBuuCuc = "593630 - Ân Mỹ";
                        diNgoai.MaBuuCuc = "593630";
                    }
                    else
                    {
                        diNgoai.TenBuuCuc = "593740 - KT Hoài Ân";
                        diNgoai.MaBuuCuc = "593740";
                    }
                }
            }
            else if (diNgoai.MaTinh == "70")
            {
                if (loai == "C")
                {
                    diNgoai.TenBuuCuc = "700920 - KTNT TP.HCM";
                    diNgoai.MaBuuCuc = "700920";
                }
                else if (loai == "E")
                {
                    diNgoai.TenBuuCuc = "701000 - HCM EMS NT";
                    diNgoai.MaBuuCuc = "701000";
                }
            }
            else if (diNgoai.MaTinh == "10")
            {
                if (loai == "C")
                {
                    diNgoai.TenBuuCuc = "100920 - KTNT Hà Nội";
                    diNgoai.MaBuuCuc = "100920";
                }
                else if (loai == "E")
                {
                    diNgoai.TenBuuCuc = "101000 - KT EMS Hà Nội nội tỉnh";
                    diNgoai.MaBuuCuc = "101000";
                }
            }
            else if (diNgoai.MaTinh == "55")
            {
                if (loai == "C")
                {
                    diNgoai.TenBuuCuc = "550920 - Đà Nẵng NT";
                    diNgoai.MaBuuCuc = "550920";
                }
                else if (loai == "E")
                {
                    diNgoai.TenBuuCuc = "550100 - Đà Nẵng EMS NT";
                    diNgoai.MaBuuCuc = "550100";
                }
            }
            else if (diNgoai.MaTinh == "85")
            {
                if (loai == "C")
                {
                    diNgoai.TenBuuCuc = "850100 - KTC1 Long An";
                    diNgoai.MaBuuCuc = "850100";
                }
                else if (loai == "E")
                {
                    diNgoai.TenBuuCuc = "850100 - KTC1 Long An";
                    diNgoai.MaBuuCuc = "850100";
                }
            }
            else
            {
                //thuc hien lay dia chi
                List<string> fillAddress = diNgoai.Address.Split('-').Select(s => s.Trim()).ToList();
                if (fillAddress == null)
                    return;
                if (fillAddress.Count < 3)
                    return;
                string addressExactly = fillAddress[fillAddress.Count - 2];
                //thuc hien lay danh sach buu cuc
                List<string> listBuuCuc = GetListBuuCucFromTinh(diNgoai.MaTinh);
                if (listBuuCuc.Count == 0)
                    return;

                string data = listBuuCuc.FirstOrDefault(m => BoDauAndToLower(m).IndexOf(BoDauAndToLower(addressExactly)) != -1);
                if (!string.IsNullOrEmpty(data))
                {
                    diNgoai.TenBuuCuc = data;
                    diNgoai.MaBuuCuc = data.Substring(0, 6);
                }
                else
                {
                    diNgoai.TenBuuCuc = listBuuCuc[0];
                    diNgoai.MaBuuCuc = listBuuCuc[0].Substring(0, 6);
                }

                if (diNgoai.MaBuuCuc == "561090")
                {
                    diNgoai.TenBuuCuc = "561150 - BCP Tam Kỳ";
                    diNgoai.MaBuuCuc = "561150";
                }
                else if (diNgoai.MaBuuCuc == "824727")
                {
                    diNgoai.TenBuuCuc = listBuuCuc[0];
                    diNgoai.MaBuuCuc = listBuuCuc[0].Substring(0, 6);
                }
                else if (diNgoai.MaBuuCuc == "221830")
                {
                    diNgoai.TenBuuCuc = listBuuCuc[0];
                    diNgoai.MaBuuCuc = listBuuCuc[0].Substring(0, 6);
                }
                else if (diNgoai.MaBuuCuc == "816420")
                {
                    diNgoai.TenBuuCuc = listBuuCuc[0];
                    diNgoai.MaBuuCuc = listBuuCuc[0].Substring(0, 6);
                }
                //foreach (string item in listBuuCuc)
                //{
                //    if (boDauAndToLower(addressExactly).IndexOf(boDauAndToLower(item)) != -1)
                //    {
                //        diNgoai.TenBuuCuc = item;
                //        diNgoai.MaBuuCuc = item.Substring(0, 6);
                //        break;
                //    }
                //}
            }
        }

        private string AutoSetTinh(string address)
        {
            if (tinhs.Count == 0)
                return "";
            List<string> fillAddress = address.Split('-').Select(s => s.Trim()).ToList();
            if (fillAddress == null)
                return "";
            if (fillAddress.Count < 3)
                return "";
            string addressBoDau = APIManager.BoDauAndToLower(fillAddress[fillAddress.Count - 1].Trim());
            foreach (var item in tinhs)
            {
                if (APIManager.BoDauAndToLower(item.Ten) == addressBoDau)
                {
                    return item.Ma;
                }
            }
            return "";
        }

        private string BoDauAndToLower(string text)
        {
            return APIManager.ConvertToUnSign3(text).ToLower();
        }

        private void BwKhoiTao_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (!IsTuDongDong)
                {
                    WindowInfo currentWindow = APIManager.WaitingFindedWindow("khoi tao chuyen thu");
                    if (currentWindow == null)
                    {
                        APIManager.ShowSnackbar("Không tìm thấy window khởi tạo chuyến thư");
                        return;
                    }
                    System.Collections.Generic.List<TestAPIModel> childControls = APIManager.GetListControlText(currentWindow.hwnd);
                    //thuc hien lay vi tri nao do

                    APIManager.SendMessage(childControls[14].Handle, 0x0007, 0, 0);
                    APIManager.SendMessage(childControls[14].Handle, 0x0007, 0, 0);

                    //Thuc hien trong nay
                    if (!string.IsNullOrEmpty(SelectedSimple.MaBuuCuc))
                    {
                        APIManager.setTextControl(childControls[15].Handle, SelectedSimple.TenBuuCuc);
                        //Thread.Sleep(300);
                        //SendKeys.SendWait("{DOWN}");
                        //Thread.Sleep(100);
                        SendKeys.SendWait("{TAB}");
                        Thread.Sleep(200);

                        //Nhan F1 ngang cho nay
                        if (IsAutoF1)
                        {
                            bwPrintDiNgoai.RunWorkerAsync();
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(SelectedSimple.MaTinh))
                        {
                            APIManager.ShowSnackbar("Không có mã tỉnh");
                            return;
                        }
                        SendKeys.SendWait(SelectedSimple.MaTinh);
                    }
                }
                else
                {
                    WindowInfo currentWindow = APIManager.WaitingFindedWindow("khai thac buu cuc phat");
                    if (currentWindow == null)
                    {
                        APIManager.ShowSnackbar("Chưa tìm thấy window");
                        return;
                    }
                    List<TestAPIModel> childControls = APIManager.GetListControlText(currentWindow.hwnd);
                    //thuc hien lay vi tri nao do

                    //Thuc hien trong nay
                    //APIManager.setTextControl(childControls[16].Handle, SelectedSimple.Code);
                    //Thread.Sleep(300);
                    //SendKeys.SendWait("{DOWN}");
                    APIManager.SendMessage(childControls[16].Handle, 0x0007, 0, 0);
                    APIManager.SendMessage(childControls[16].Handle, 0x0007, 0, 0);
                    SendKeys.SendWait("^(a)");
                    SendKeys.SendWait(SelectedSimple.Code);

                    Thread.Sleep(100);
                    SendKeys.SendWait("{ENTER}");
                    Thread.Sleep(700);
                    string testText = "";
                    var windows = APIManager.WaitingFindedWindow("khai thac buu cuc phat", "xac nhan");
                    testText = "Title: " + windows.text + @"\n";

                    if (windows.text == "khai thac buu cuc phat")
                    {
                        LocalPrintServer localPrintServer = new LocalPrintServer();

                        PrinterSettings settings = new PrinterSettings();
                        PrintQueueStatus statusPrint = PrintQueueStatus.None;
                        for (int i = 0; i < 8; i++)
                        {
                            foreach (PrintQueue printQueue in localPrintServer.GetPrintQueues())
                            {
                                if (settings.PrinterName == printQueue.FullName)
                                {
                                    statusPrint = printQueue.QueueStatus;
                                    break;
                                }
                            }
                            if (statusPrint != PrintQueueStatus.None)
                            {
                                break;
                            }
                            Thread.Sleep(100);
                        }
                        if (statusPrint == PrintQueueStatus.None)
                        {
                            APIManager.ShowSnackbar("Khong In");
                        }
                        else
                        {
                            APIManager.ShowSnackbar("KT In Duoc");
                        }
                    }
                    else if (windows.text == "xac nhan")
                    {
                        testText += "Run In Xac Nhan \n";
                        List<IntPtr> controls = APIManager.GetAllChildHandles(windows.hwnd);
                        //APIManager.setTextControl(controls[2], "593330");
                        SendKeys.SendWait("593330{ENTER}");
                    }

                    APIManager.OpenNotePad(testText);

                    //Set text
                    //APIManager.setTextControl(childControls[2].Handle, temp);
                }
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "loi Line DiNgoaiViewModel" + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
                throw;
            }
        }

        private void BwPrintDiNgoai_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                WindowInfo currentWindow = APIManager.WaitingFindedWindow("khoi tao chuyen thu");
                if (currentWindow == null)
                {
                    APIManager.ShowSnackbar("Không tìm thấy window khởi tạo chuyến thư");
                    return;
                }

                List<TestAPIModel> childControls = APIManager.GetListControlText(currentWindow.hwnd);
                //thuc hien lay vi tri nao do

                APIManager.SendMessage(childControls[11].Handle, 0x0007, 0, 0);
                APIManager.SendMessage(childControls[11].Handle, 0x0007, 0, 0);
                string temp = "";
                string charCodeFirst = SelectedSimple.Code[0].ToString().ToLower();

                if (charCodeFirst == "c")
                {
                    temp = "Bưu kiện - Parcel";
                }
                else if (charCodeFirst == "e")
                {
                    temp = "EMS - Chuyển phát nhanh - Express Mail Service";
                }
                else if (charCodeFirst == "p")
                {
                    temp = "Logistic";
                }
                APIManager.setTextControl(childControls[12].Handle, temp);
                SendKeys.SendWait("{TAB}");

                //SendKeys.SendWait("{BS}" + temp + "{TAB}");
                Thread.Sleep(100);
                APIManager.ClickButton(currentWindow.hwnd, "chap nhan", isExactly: false);

                currentWindow = APIManager.WaitingFindedWindow("tao tui");
                if (currentWindow == null)
                {
                    APIManager.ShowSnackbar("Không tìm thấy window tao tui");
                    return;
                }

                var controls = APIManager.GetListControlText(currentWindow.hwnd);
                var handleTaoTui = controls[8].Handle;

                APIManager.SendMessage(handleTaoTui, 0x0007, 0, 0);
                APIManager.SendMessage(handleTaoTui, 0x0007, 0, 0);
                string tuiText = "";
                if (charCodeFirst == "c")
                {
                    tuiText = "Ði ngoài(BK)";
                }
                else if (charCodeFirst == "e")
                {
                    if (IsTMDT)
                    {
                        tuiText = "Đi ngoài EMS-TMĐT Đồng Giá";
                    }
                    else
                    {
                        tuiText = "Ði ngoài(EMS)";
                    }
                }
                else if (charCodeFirst == "p")
                {
                    tuiText = "Ði ngoài(UT)";
                }
                APIManager.setTextControl(controls[9].Handle, tuiText);
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(100);
                //SendKeys.SendWait("{F10}");

                APIManager.ClickButton(currentWindow.hwnd, "f10", isExactly: false);

                //Thread.Sleep(100);
                //SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}");
                //for (int i = 0; i < downTaoTui; i++)
                //{
                //    SendKeys.SendWait("{DOWN}");
                //}
                //SendKeys.SendWait("A{BS}{BS}");

                currentWindow = APIManager.WaitingFindedWindow("dong chuyen thu");

                if (currentWindow == null)
                {
                    APIManager.ShowSnackbar("Không tìm thấy window đóng chuyến thư");
                    return;
                }

                childControls = APIManager.GetListControlText(currentWindow.hwnd);
                TestAPIModel tinhControl = childControls.Where(m => m.ClassName.IndexOf("WindowsForms10.EDIT") != -1).ToList()[2];

                if (!string.IsNullOrEmpty(tinhControl.Text))
                {
                    if (SelectedSimple.MaBuuCuc.ToUpper() != tinhControl.Text.Substring(0, 6).ToUpper())
                    {
                        APIManager.ShowSnackbar("Không đúng tỉnh rồi");
                        return;
                    }
                }
                List<DiNgoaiItemModel> listDiNgoaiCungMaBC = new List<DiNgoaiItemModel>();

                //chia lam 2 phan he trong nay
                if (_IsGroupCT)
                {
                    //thuc hien trong nay
                    int iSelected = DiNgoais.IndexOf(SelectedSimple);

                    if (iSelected != -1 && DiNgoais.Count - 1 != iSelected)
                    {
                        for (int i = iSelected; i < DiNgoais.Count; i++)
                        {
                            if (DiNgoais[i].MaBuuCuc == SelectedSimple.MaBuuCuc && charCodeFirst == DiNgoais[i].Code[0].ToString().ToLower())
                            {
                                if (charCodeFirst == "p")
                                {
                                    if (DiNgoais[i].Address.Trim().ToLower() == SelectedSimple.Address.Trim().ToLower())
                                    {
                                        listDiNgoaiCungMaBC.Add(DiNgoais[i]);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                listDiNgoaiCungMaBC.Add(DiNgoais[i]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                bool IsRunGroup = false;
                if (listDiNgoaiCungMaBC.Count > 1)
                    IsRunGroup = true;
                else
                    IsRunGroup = false;

                if (IsRunGroup)
                {
                    for (int i = 0; i < listDiNgoaiCungMaBC.Count - 1; i++)
                    {
                        SendKeys.SendWait(listDiNgoaiCungMaBC[i].Code);
                        SendKeys.SendWait("{ENTER}");
                        Thread.Sleep(500);
                    }

                    //vao check f cai cuoi cung

                    SendKeys.SendWait("{F5}");
                    SendKeys.SendWait("{F5}");
                    SendKeys.SendWait("^{DOWN}");
                    SendKeys.SendWait("^{RIGHT}");
                    SendKeys.SendWait("{LEFT}");
                    Thread.Sleep(100);
                    SendKeys.SendWait(" ");
                    Thread.Sleep(500);
                    //CheckF Ngang cho nay
                    ///////////////////////////////////////
                    string clipboard1 = "";
                    bool isCheckTui = false;
                    for (int i = 0; i < 3; i++)
                    {
                        //Kiem tra Da dong tui chua

                        clipboard1 = APIManager.GetCopyData();

                        if (string.IsNullOrEmpty(clipboard1))
                        {
                            APIManager.ShowSnackbar("Khong copy duoc");
                            return;
                        }

                        if (clipboard1.IndexOf("True") != -1)
                        {
                            isCheckTui = true;
                            break;
                        }
                        SendKeys.SendWait(" ");
                        Thread.Sleep(500);
                    }
                    if (!isCheckTui)
                    {
                        APIManager.ShowSnackbar("Chưa đóng túi được");
                        return;
                    }
                    /////////////////////////////////////////////

                    SendKeys.SendWait("{F6}");
                    SendKeys.SendWait(listDiNgoaiCungMaBC.Last().Code);
                    SendKeys.SendWait("{ENTER}");
                    Thread.Sleep(500);

                    //sau do chon tat ca
                    SendKeys.SendWait("+{TAB}");
                    Thread.Sleep(100);
                    SendKeys.SendWait(" ");
                    Thread.Sleep(500);

                    //

                    SendKeys.SendWait("{F6}");
                    Thread.Sleep(200);
                }
                else
                {
                    SendKeys.SendWait("{F5}");

                    SendKeys.SendWait("^{RIGHT}");
                    Thread.Sleep(50);
                    SendKeys.SendWait("{LEFT}");
                    Thread.Sleep(50);
                    SendKeys.SendWait(" ");
                    Thread.Sleep(50);

                    string copedText = APIManager.GetCopyData();
                    string clipboard1 = "";
                    bool isCheckTui = false;
                    for (int i = 0; i < 3; i++)
                    {
                        //Kiem tra Da dong tui chua

                        clipboard1 = APIManager.GetCopyData();

                        if (string.IsNullOrEmpty(clipboard1))
                        {
                            APIManager.ShowSnackbar("Khong copy duoc");
                            return;
                        }

                        if (clipboard1.IndexOf("True") != -1)
                        {
                            isCheckTui = true;
                            break;
                        }
                        SendKeys.SendWait(" ");
                        Thread.Sleep(500);
                    }
                    SendKeys.SendWait("{F6}");
                    SendKeys.SendWait("{F6}");
                    Thread.Sleep(200);
                    SendKeys.SendWait(SelectedSimple.Code);
                    SendKeys.SendWait("{ENTER}");
                    Thread.Sleep(200);
                    clipboard1 = "";
                    isCheckTui = false;
                    for (int i = 0; i < 3; i++)
                    {
                        SendKeys.SendWait("{F5}");
                        SendKeys.SendWait("{F5}");
                        SendKeys.SendWait("^{LEFT}");
                        Thread.Sleep(100);
                        SendKeys.SendWait(" ");
                        Thread.Sleep(500);
                        //Kiem tra Da dong tui chua

                        clipboard1 = APIManager.GetCopyData();

                        if (string.IsNullOrEmpty(clipboard1))
                        {
                            APIManager.ShowSnackbar("Khong copy duoc");
                            return;
                        }

                        if (clipboard1.IndexOf("Selected") != -1)
                        {
                            isCheckTui = true;
                            break;
                        }
                    }
                    if (!isCheckTui)
                    {
                        APIManager.ShowSnackbar("Chưa đóng túi được");
                        return;
                    }

                    SendKeys.SendWait("{F6}");
                    Thread.Sleep(200);
                }

                ///////////////////////////////////////////////////////////////

                SendKeys.SendWait("{F7}");

                currentWindow = APIManager.WaitingFindedWindow("in an pham");

                if (currentWindow == null)
                {
                    APIManager.ShowSnackbar("Không tìm thấy window in ấn phẩm");
                    return;
                }

                APIManager.SetPrintBD8();

                SendKeys.SendWait("{TAB}");
                Thread.Sleep(100);
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(100);

                SendKeys.SendWait("^(a)");
                string clipboard = APIManager.GetCopyData();

                if (string.IsNullOrEmpty(clipboard))
                {
                    APIManager.ShowSnackbar("Khong copy duoc");
                    return;
                }
                if (clipboard.IndexOf("BĐ8") == -1)
                {
                    APIManager.ShowSnackbar("Không tìm thấy BĐ8");
                    return;
                }

                foreach (string item in clipboard.Split('\n'))
                {
                    string[] datas1 = item.Split('\t');
                    if (datas1[1].IndexOf("BĐ8") != -1)
                    {
                        SendKeys.SendWait(" ");
                        break;
                    }
                    if (datas1[4].IndexOf("BĐ8") != -1)
                    {
                        SendKeys.SendWait("{RIGHT}");
                        Thread.Sleep(50);
                        SendKeys.SendWait("{RIGHT}");
                        Thread.Sleep(50);
                        SendKeys.SendWait("{RIGHT}");
                        Thread.Sleep(50);
                        SendKeys.SendWait(" ");
                        Thread.Sleep(100);

                        break;
                    }
                    SendKeys.SendWait("{DOWN}");
                    Thread.Sleep(50);
                }
                if (IsRunGroup)
                {
                    APIManager.ShowSnackbar("Stop is Printer");
                    return;
                }
                childControls = APIManager.GetListControlText(currentWindow.hwnd);
                TestAPIModel thoatControl = childControls.FirstOrDefault(m => m.ClassName.IndexOf("WindowsForms10.BUTTON.app") != -1);
                if (thoatControl == null)
                {
                    APIManager.ShowSnackbar("Không tìm thấy button thoát");
                    return;
                }

                SendKeys.SendWait("{F10}");
                APIManager.SendMessage(thoatControl.Handle, 0x00F5, 0, 0);

                currentWindow = APIManager.WaitingFindedWindow("dong chuyen thu");

                if (currentWindow == null)
                {
                    APIManager.ShowSnackbar("Không tìm thấy window đóng chuyến thư");
                    return;
                }

                Thread.Sleep(200);
                APIManager.ClickButton(currentWindow.hwnd, "dong chuyen thu", isExactly: false);
                Thread.Sleep(200);
                currentWindow = APIManager.WaitingFindedWindow("xac nhan");
                if (currentWindow == null)
                    return;
                APIManager.ClickButton(currentWindow.hwnd, "yes", isExactly: false);

                // coi chung loi cho nay
                currentWindow = APIManager.WaitingFindedWindow("khoi tao chuyen thu");
                if (currentWindow == null)
                {
                    APIManager.ShowSnackbar("Không tìm thấy window khởi tạo chuyến thư");
                    return;
                }

                if (DiNgoais.Count == 0)
                {
                    APIManager.ShowSnackbar("Không có dữ liệu");
                    return;
                }
                //lay vi tri tiep theo
                //get index
                int index = DiNgoais.IndexOf(SelectedSimple);
                if (index == -1)
                {
                    APIManager.ShowSnackbar("Chưa chọn mã hiệu");
                    return;
                }
                index++;
                if (index > DiNgoais.Count - 1)
                {
                    APIManager.ShowSnackbar("Đã tới vị trí cuối cùng");
                    //txtInfo.Text = "Đã tới vị trí cuối cùng";
                    return;
                }

                //////xem thu no co chay cai gi khong

                SelectedSimple = DiNgoais[index];
                Selection(SelectedSimple);
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                APIManager.OpenNotePad(ex.Message + '\n' + "loi Line DiNgoaiViewModel" + line + " Number Line " + APIManager.GetLineNumber(ex), "loi ");
                throw;
            }
        }

        private void Clear()
        {
        }

        private void ClearDiNgoai()
        {
            DiNgoais.Clear();
        }

        private void CheckEnterKey()
        {
            if (TextCode.IndexOf('\n') != -1)
            {
                TextCode = TextCode.Trim().ToUpper();
                if (TextCode.Length != 13)
                {
                    TextCode = "";
                    return;
                }                //    //kiem tra trung khong
                if (DiNgoais.Count == 0)
                {
                    DiNgoais.Add(new DiNgoaiItemModel(DiNgoais.Count + 1, TextCode));
                    SoundManager.playSound(@"Number\1.wav");
                    TextCode = "";
                }
                else
                {
                    foreach (DiNgoaiItemModel item in DiNgoais)
                    {
                        if (item.Code == TextCode)
                        {
                            TextCode = "";
                            return;
                        }
                    }
                    DiNgoais.Add(new DiNgoaiItemModel(DiNgoais.Count + 1, TextCode));
                    if (IsSayNumber)
                    {
                        SoundManager.playSound(@"Number\" + DiNgoais.Count.ToString() + ".wav");
                    }
                    TextCode = "";
                }
            }
        }

        private void ChuyenDataDiNgoaiToPhone()
        {
            //thuc hien send du lieu qua phone
            string json = JsonConvert.SerializeObject(DiNgoais);
            //MqttManager.Pulish(FileManager.MQTTKEY + "_dingoai", json);
            FileManager.SendMessage(new MessageToPhoneModel("senddingoaitophone", json));
        }

        private void DiNgoaiTuDongNext()
        {
            if (DiNgoais.Count == 0)
            {
                APIManager.ShowSnackbar("Không có dữ liệu");
                return;
            }
            //lay vi tri tiep theo
            //get index
            int index = DiNgoais.IndexOf(SelectedSimple);
            if (index == -1)
            {
                APIManager.ShowSnackbar("Chưa chọn mã hiệu");
                return;
            }
            index++;
            if (index > DiNgoais.Count - 1)
            {
                APIManager.ShowSnackbar("Đã tới vị trí cuối cùng");
                //txtInfo.Text = "Đã tới vị trí cuối cùng";
                return;
            }

            //////xem thu no co chay cai gi khong

            SelectedSimple = DiNgoais[index];
            Selection(SelectedSimple);
        }

        private List<string> GetListBuuCucFromTinh(string maTinh)
        {
            List<string> buucucs = new List<string>();
            if (!IsTuDongDong)
            {
                for (int i = 0; i < listBuuCuc.Count; i++)
                {
                    if (maTinh == listBuuCuc[i].Substring(0, 2))
                    {
                        buucucs.Add(listBuuCuc[i].Trim());
                    }
                }
            }
            else
            {
                for (int i = 0; i < listBuuCucTuDong.Count; i++)
                {
                    if (maTinh == listBuuCucTuDong[i].Substring(0, 2))
                    {
                        buucucs.Add(listBuuCucTuDong[i].Trim());
                    }
                }
            }
            return buucucs;
        }

        private string GetTinhFromBuuCuc(string buucuc)
        {
            if (string.IsNullOrEmpty(buucuc))
            {
                return "";
            }
            string maTinh = buucuc.Substring(0, 2);
            string maTinhFilled;
            switch (maTinh)
            {
                case "11":
                case "12":
                case "13":
                case "15":
                    maTinhFilled = "10";
                    break;

                case "45":
                    maTinhFilled = "44";
                    break;

                case "73":
                case "75":
                case "76":
                case "71":
                case "74":
                case "72":
                    maTinhFilled = "70";
                    break;

                default:
                    maTinhFilled = maTinh;
                    break;
            }
            return maTinhFilled;
        }

        private List<MaHieuDiNgoaiInfo> LocTextTho(string textsRange)
        {
            //thuc hien loc ma hieu dia chi gui va dia chi nhan
            List<MaHieuDiNgoaiInfo> list = new List<MaHieuDiNgoaiInfo>();
            var datas = textsRange.Split('\n');
            foreach (string data in datas)
            {
                //thuc hien loc danh sach trong nay
                if (data.Count() < 13)
                    continue;
                string[] splitedData = data.Split(' ');

                if (splitedData.Length >= 10 && data.ToUpper().IndexOf("VN") != -1)
                {
                    list.Add(new MaHieuDiNgoaiInfo(splitedData[1].ToUpper(), splitedData[3], splitedData[2], splitedData[4]));
                }
                else
                if (splitedData.Length == 7 && data.ToUpper().IndexOf("VN") != -1)
                {
                    list.Add(new MaHieuDiNgoaiInfo(splitedData[1].ToUpper(), splitedData[2], "", splitedData[3]));
                }
                else
                {
                    int indexVN = data.ToUpper().IndexOf("VN");
                    if (indexVN - 11 < 0)
                        continue;

                    list.Add(new MaHieuDiNgoaiInfo(data.Substring(indexVN - 11, 13).Trim().ToUpper()));
                }
                //if(splitedData.Length )
            }
            return list;
        }

        [RelayCommand]
        private void MoRong()
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "Center" });
        }

        private void OnSelectedBuuCuc()
        {
            if (SelectedBuuCuc == null)
                return;
            if (SelectedDiNgoai == null)
                return;
            //DiNgoaiItemModel dingoai = DiNgoais.FirstOrDefault(d => d.Code == SelectedDiNgoai.Code);
            //if (dingoai == null)
            //    return;
            //dingoai.MaBuuCuc = SelectedBuuCuc;
            if (string.IsNullOrEmpty(SelectedDiNgoai.MaTinh))
                return;
            SelectedDiNgoai.TenBuuCuc = SelectedBuuCuc;
            SelectedDiNgoai.MaBuuCuc = SelectedBuuCuc.Substring(0, 6);

            //thuc hien qua cai tiep theo
            foreach (DiNgoaiItemModel diNgoai in DiNgoais)
            {
                if (string.IsNullOrEmpty(diNgoai.MaBuuCuc))
                {
                    BuuCucs.Clear();
                    SelectedDiNgoai = diNgoai;
                    break;
                }
            }
        }

        private void OnSelectedDiNgoai()
        {
            if (SelectedDiNgoai == null)
                return;
            //chuyen vo cbx
            BuuCucs.Clear();
            List<string> listBuuCuc = GetListBuuCucFromTinh(SelectedDiNgoai.MaTinh);
            if (listBuuCuc.Count != 0)
            {
                foreach (string item in listBuuCuc)
                {
                    BuuCucs.Add(item);
                }
            }
        }

        private void OnSelectedSimple()
        {
            if (!bwKhoiTao.IsBusy)
            {
                bwKhoiTao.CancelAsync();
                bwKhoiTao.RunWorkerAsync();
            }
            //thuc hien
        }

        private void Selection(DiNgoaiItemModel selected)
        {
            if (selected == null)
                return;

            //thuc hien doc index
            SoundManager.playSound(@"Number\" + selected.Index + ".wav");
            OnSelectedSimple();
        }

        private void SelectionChiTiet()
        {
            OnSelectedDiNgoai();
        }

        private void SetMaTinhGui()
        {
            if (SelectedDiNgoai == null)
                return;
            if (DiNgoais.Count == 0)
                return;
            if (string.IsNullOrEmpty(SelectedDiNgoai.BuuCucGui))
                return;
            SelectedDiNgoai.MaTinh = GetTinhFromBuuCuc(SelectedDiNgoai.BuuCucGui);
        }

        private void SetTinhFromMaTinh()
        {
            foreach (var diNgoai in DiNgoais)
            {
                switch (diNgoai.MaTinh)
                {
                    case "88":
                        diNgoai.TenTinh = "An Giang";
                        break;

                    case "79":
                        diNgoai.TenTinh = "Ba Ria Vung Tau";
                        break;

                    case "26":
                        diNgoai.TenTinh = "Bac Can";
                        break;

                    case "23":
                        diNgoai.TenTinh = "Bac Giang";
                        break;

                    case "96":
                        diNgoai.TenTinh = "Bac Lieu";
                        break;

                    case "22":
                        diNgoai.TenTinh = "Bac Ninh";
                        break;

                    case "93":
                        diNgoai.TenTinh = "Ben Tre";
                        break;

                    case "82":
                        diNgoai.TenTinh = "Binh Duong";
                        break;

                    case "83":
                        diNgoai.TenTinh = "Binh Phuoc";
                        break;

                    case "80":
                        diNgoai.TenTinh = "Binh Thuan";
                        break;

                    case "97":
                        diNgoai.TenTinh = "Ca Mau";
                        break;

                    case "90":
                        diNgoai.TenTinh = "Can Tho";
                        break;

                    case "27":
                        diNgoai.TenTinh = "Cao Bang";
                        break;

                    case "55":
                        diNgoai.TenTinh = "Da Nang";
                        break;

                    case "63":
                        diNgoai.TenTinh = "Dak Lak";
                        break;

                    case "64":
                        diNgoai.TenTinh = "Dak Nong";
                        break;

                    case "38":
                        diNgoai.TenTinh = "Dien Bien";
                        break;

                    case "81":
                        diNgoai.TenTinh = "Dong Nai";
                        break;

                    case "87":
                        diNgoai.TenTinh = "Dong Thap";
                        break;

                    case "60":
                        diNgoai.TenTinh = "Gia Lai";
                        break;

                    case "40":
                        diNgoai.TenTinh = "Ha Nam";
                        break;

                    case "10":
                        diNgoai.TenTinh = "Ha Noi";
                        break;

                    case "48":
                        diNgoai.TenTinh = "Ha Tinh";
                        break;

                    case "17":
                        diNgoai.TenTinh = "Hai Duong";
                        break;

                    case "18":
                        diNgoai.TenTinh = "Hai Phong";
                        break;

                    case "91":
                        diNgoai.TenTinh = "Hau Giang";
                        break;

                    case "70":
                        diNgoai.TenTinh = "Ho Chi Minh";
                        break;

                    case "16":
                        diNgoai.TenTinh = "Hung Yen";
                        break;

                    case "65":
                        diNgoai.TenTinh = "Khanh Hoa";
                        break;

                    case "92":
                        diNgoai.TenTinh = "Kien Giang";
                        break;

                    case "31":
                        diNgoai.TenTinh = "Ha Giang";
                        break;

                    case "58":
                        diNgoai.TenTinh = "Kon Tum";
                        break;

                    case "39":
                        diNgoai.TenTinh = "Lai Chau";
                        break;

                    case "67":
                        diNgoai.TenTinh = "Lam Dong";
                        break;

                    case "24":
                        diNgoai.TenTinh = "Lang Son";
                        break;

                    case "33":
                        diNgoai.TenTinh = "Lao Cai";
                        break;

                    case "85":
                        diNgoai.TenTinh = "Long An";
                        break;

                    case "42":
                        diNgoai.TenTinh = "Nam Dinh";
                        break;

                    case "46":
                        diNgoai.TenTinh = "Nghe An";
                        break;

                    case "43":
                        diNgoai.TenTinh = "Ninh Binh";
                        break;

                    case "66":
                        diNgoai.TenTinh = "Ninh Thuan";
                        break;

                    case "62":
                        diNgoai.TenTinh = "Phu Yen";
                        break;

                    case "51":
                        diNgoai.TenTinh = "Quang Binh";
                        break;

                    case "56":
                        diNgoai.TenTinh = "Quang Nam";
                        break;

                    case "57":
                        diNgoai.TenTinh = "Quang Ngai";
                        break;

                    case "20":
                        diNgoai.TenTinh = "Quang Ninh";
                        break;

                    case "52":
                        diNgoai.TenTinh = "Quang Tri";
                        break;

                    case "95":
                        diNgoai.TenTinh = "Soc Trang";
                        break;

                    case "36":
                        diNgoai.TenTinh = "Son La";
                        break;

                    case "84":
                        diNgoai.TenTinh = "Tay Ninh";
                        break;

                    case "41":
                        diNgoai.TenTinh = "Thai Binh";
                        break;

                    case "25":
                        diNgoai.TenTinh = "Thai Nguyen";
                        break;

                    case "44":
                        diNgoai.TenTinh = "Thanh Hoa";
                        break;

                    case "53":
                        diNgoai.TenTinh = "Thua Thien - Hue";
                        break;

                    case "86":
                        diNgoai.TenTinh = "Tien Giang";
                        break;

                    case "94":
                        diNgoai.TenTinh = "Tra Vinh";
                        break;

                    case "30":
                        diNgoai.TenTinh = "Tuyen Quang";
                        break;

                    case "89":
                        diNgoai.TenTinh = "Vinh Long";
                        break;

                    case "32":
                        diNgoai.TenTinh = "Yen Bai";
                        break;

                    case "29":
                        diNgoai.TenTinh = "Phu Tho";
                        break;

                    case "28":
                        diNgoai.TenTinh = "Vinh Phuc";
                        break;

                    case "35":
                        diNgoai.TenTinh = "Hoa Binh";
                        break;

                    case "59":
                        //Thuc Hien Loc Trong Nay
                        switch (diNgoai.MaBuuCuc)
                        {
                            case "591218":
                                diNgoai.TenTinh = "Quy Nhon 2";
                                break;

                            case "591520":
                                diNgoai.TenTinh = "Quy Nhon";
                                break;

                            case "591720":
                            case "591760":
                                diNgoai.TenTinh = "Tuy Phuoc";
                                break;

                            case "592020":
                                diNgoai.TenTinh = "An Nhon";
                                break;

                            case "592440":
                                diNgoai.TenTinh = "Phu Cat";
                                break;

                            case "592810":
                                diNgoai.TenTinh = "Phu My";
                                break;

                            case "593330":
                                diNgoai.TenTinh = "Tam Quan";
                                break;

                            case "593630":
                                diNgoai.TenTinh = "An My";
                                break;

                            case "593740":
                                diNgoai.TenTinh = "Hoai An";
                                break;

                            case "593850":
                                diNgoai.TenTinh = "An Lao";
                                break;

                            case "593880":
                                diNgoai.TenTinh = "An Hoa";
                                break;

                            case "594080":
                                diNgoai.TenTinh = "Vinh Thanh";
                                break;

                            case "594210":
                                diNgoai.TenTinh = "Tay Son";
                                break;

                            case "594560":
                                diNgoai.TenTinh = "Van Canh";
                                break;

                            default:
                                diNgoai.TenTinh = "AAA";
                                break;
                        }
                        break;

                    default:
                        diNgoai.TenTinh = "AAA";
                        break;
                }
            }
        }

        private void SetTinhs()
        {
            foreach (var item in DiNgoais)
            {
                item.MaTinh = GetTinhFromBuuCuc(item.BuuCucNhanTemp);
            }
        }

        private void ShowData()
        {
            if (DiNgoais.Count != 0)
            {
                string text = "";
                foreach (DiNgoaiItemModel item in DiNgoais)
                {
                    text += item.Code + "\n";
                }
                APIManager.OpenNotePad(text, "noi dung");
            }
        }

        private void Sort()
        {
            if (DiNgoais.Count == 0)
                return;
            SetTinhFromMaTinh();
            //Thuc hien soft Tinh
            var dingoaisTemp = DiNgoais.OrderByDescending(x => x.TenTinh).ToList();
            DiNgoais.Clear();
            int index = 0;
            foreach (var diNgoai in dingoaisTemp)
            {
                index++;
                diNgoai.Index = index;
                DiNgoais.Add(diNgoai);
            }
        }

        private void SortTinh()
        {
            if (DiNgoais.Count == 0)
                return;
            SetTinhFromMaTinh();
            List<DiNgoaiItemModel> listDiNgoai = new List<DiNgoaiItemModel>();
            //Thuc hien soft Tinh
            IOrderedEnumerable<DiNgoaiItemModel> dingoaisRa = DiNgoais.Where(m => int.Parse(m.MaTinh) < 59 && int.Parse(m.MaTinh) != 58).OrderByDescending(x => x.TenTinh).ThenByDescending(x => int.Parse(x.Code[10].ToString()));
            IOrderedEnumerable<DiNgoaiItemModel> dingoaisVo = DiNgoais.Where(m => int.Parse(m.MaTinh) >= 58).OrderByDescending(x => x.TenTinh).ThenByDescending(x => int.Parse(x.Code[10].ToString()));
            listDiNgoai.AddRange(dingoaisRa);
            listDiNgoai.AddRange(dingoaisVo);
            DiNgoais.Clear();
            int index = 0;
            foreach (DiNgoaiItemModel diNgoai in listDiNgoai)
            {
                index++;
                diNgoai.Index = index;
                DiNgoais.Add(diNgoai);
            }
        }

        private void StopDiNgoai()
        {
            bwPrintDiNgoai.CancelAsync();
        }

        private void TimerPrint_Tick(object sender, EventArgs e)
        {
            if (isWaitingPrint) return;
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
            {
                return;
            }

            switch (stateDiNgoai)
            {
                case StateDiNgoai.KhoiTao:
                    if (currentWindow.text.IndexOf("khoi tao chuyen thu") != -1)
                    {
                        isWaitingPrint = true;
                        var childHandles3 = APIManager.GetAllChildHandles(currentWindow.hwnd);
                        int countCombobox = 0;
                        IntPtr loadDiNgoai = IntPtr.Zero;
                        foreach (var item in childHandles3)
                        {
                            string className = APIManager.GetWindowClass(item);
                            string classDefault = "WindowsForms10.COMBOBOX.app.0.1e6fa8e";
                            //string classDefault = "WindowsForms10.COMBOBOX.app.0.141b42a_r8_ad1";
                            if (className == classDefault)
                            {
                                if (countCombobox == 1)
                                {
                                    loadDiNgoai = item;
                                    break;
                                }
                            }
                        }
                        APIManager.SendMessage(loadDiNgoai, 0x0007, 0, 0);
                        APIManager.SendMessage(loadDiNgoai, 0x0007, 0, 0);
                        string temp = "";
                        string charCodeFirst = SelectedSimple.Code[0].ToString().ToLower();

                        if (charCodeFirst == "c")
                        {
                            temp = "bưu k";
                            downTaoTui = 2;
                        }
                        else if (charCodeFirst == "e")
                        {
                            temp = "em";
                            downTaoTui = 6;
                        }
                        else if (charCodeFirst == "p")
                        {
                            temp = "lo";
                            downTaoTui = 4;
                        }

                        SendKeys.SendWait("{BS}" + temp + "{TAB}");
                        Thread.Sleep(100);
                        SendKeys.SendWait("{F10}");
                        stateDiNgoai = StateDiNgoai.TaoTui;
                        isRunFirst = false;
                        isWaitingPrint = false;
                    }
                    break;

                case StateDiNgoai.TaoTui:
                    if (currentWindow.text.IndexOf("tao tui") != -1)
                    {
                        if (!isRunFirst)
                        {
                            isRunFirst = true;
                            return;
                        }

                        isWaitingPrint = true;
                        SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}");
                        for (int i = 0; i < downTaoTui; i++)
                        {
                            SendKeys.SendWait("{DOWN}");
                        }
                        SendKeys.SendWait("{F10}");
                        SendKeys.SendWait("A{BS}{BS}");
                        stateDiNgoai = StateDiNgoai.DongChuyen;

                        //stateDiNgoai = StateDiNgoai.MoLaiTiep;
                        isWaitingPrint = false;
                        isRunFirst = false;
                    }
                    break;

                case StateDiNgoai.DongChuyen:
                    if (currentWindow.text.IndexOf("dong chuyen thu") != -1)
                    {
                        if (!isRunFirst)
                        {
                            isRunFirst = true;
                            return;
                        }

                        //thuc hien kiem tra thu co dung khong
                        List<IntPtr> datas = APIManager.GetAllChildHandles(currentWindow.hwnd);
                        int countIndexWindowForm = 0;
                        foreach (var item in datas)
                        {
                            //thuc hien lay text cua handle item
                            String text = APIManager.GetControlText(item);

                            string className = APIManager.GetWindowClass(item);
                            if (className.IndexOf("WindowsForms10.EDIT") != -1)
                            {
                                countIndexWindowForm++;
                                if (countIndexWindowForm == 3)
                                {
                                    if (!string.IsNullOrEmpty(text))
                                    {
                                        if (SelectedSimple.MaBuuCuc.ToUpper() != text.Substring(0, 6).ToUpper())
                                        {
                                            APIManager.ShowSnackbar("Không đúng tỉnh rồi");

                                            timerPrint.Stop();
                                            isWaitingPrint = false;
                                        }
                                    }
                                }
                            }
                        }

                        isWaitingPrint = true;
                        for (int i = 0; i < 20; i++)
                        {
                            SendKeys.SendWait("+{TAB}");
                            SendKeys.SendWait("^C");
                            string textClip = Clipboard.GetText();
                            if (textClip.IndexOf("Túi số") != -1)
                            {
                                SendKeys.SendWait("{DOWN}");
                                SendKeys.SendWait("{LEFT}");
                                SendKeys.SendWait("{LEFT}");
                                SendKeys.SendWait("{LEFT}");
                                SendKeys.SendWait("{LEFT}");
                                SendKeys.SendWait("{LEFT}");
                                SendKeys.SendWait("{RIGHT}");
                                SendKeys.SendWait("{RIGHT}");
                                SendKeys.SendWait("{RIGHT}");
                                SendKeys.SendWait("{RIGHT}");
                                SendKeys.SendWait(" ");
                                SendKeys.SendWait("{RIGHT}");
                                SendKeys.SendWait("{RIGHT}");
                                SendKeys.SendWait("{F6}");
                                SendKeys.SendWait("{F6}");
                                Thread.Sleep(100);
                                SendKeys.SendWait(SelectedSimple.Code);
                                SendKeys.SendWait("{ENTER}");
                                Thread.Sleep(200);
                                SendKeys.SendWait("+{TAB}");
                                SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}");
                                SendKeys.SendWait("{LEFT}");
                                SendKeys.SendWait("{LEFT}");
                                SendKeys.SendWait("{LEFT}");
                                SendKeys.SendWait("{LEFT}");
                                SendKeys.SendWait("{LEFT}");
                                SendKeys.SendWait("{LEFT}");
                                SendKeys.SendWait("{LEFT}");
                                SendKeys.SendWait(" ");
                                Thread.Sleep(500);
                                APIManager.ShowTest("11");
                                //Kiem tra Da dong tui chua
                                SendKeys.SendWait("^C");
                                Thread.Sleep(200);
                                string textClip1 = Clipboard.GetText();
                                if (textClip1.IndexOf("Selected") == -1)
                                {
                                    isWaitingPrint = false;
                                    timerPrint.Stop();
                                    WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = "Lỗi chưa chọn" });
                                    APIManager.ShowTest("8");
                                    return;
                                }

                                SendKeys.SendWait("{F6}");
                                Thread.Sleep(200);
                                SendKeys.SendWait("{F7}");
                                break;
                            }
                        }
                        stateDiNgoai = StateDiNgoai.In;
                        isRunFirst = false;
                        isWaitingPrint = false;
                    }

                    break;

                case StateDiNgoai.In:
                    if (!isRunFirst)
                    {
                        isRunFirst = true;
                        return;
                    }

                    isWaitingPrint = true;
                    APIManager.SetPrintBD8();

                    Thread.Sleep(200);
                    SendKeys.SendWait("{TAB}");
                    Thread.Sleep(100);
                    SendKeys.SendWait("{TAB}");
                    Thread.Sleep(100);

                    SendKeys.SendWait("^(a)");
                    SendKeys.SendWait("^(c)");
                    Thread.Sleep(200);
                    string clipboard = Clipboard.GetText();
                    if (string.IsNullOrEmpty(clipboard))
                    {
                        isWaitingPrint = false;

                        timerPrint.Stop();
                        return;
                    }
                    if (clipboard.IndexOf("BĐ8") == -1)
                    {
                        isWaitingPrint = false;
                        timerPrint.Stop();
                        return;
                    }
                    foreach (string item in clipboard.Split('\n'))
                    {
                        var datas = item.Split('\t');
                        if (datas[1].IndexOf("BĐ8") != -1)
                        {
                            SendKeys.SendWait(" ");
                            break;
                        }
                        if (datas[4].IndexOf("BĐ8") != -1)
                        {
                            SendKeys.SendWait("{RIGHT}");
                            Thread.Sleep(50);
                            SendKeys.SendWait("{RIGHT}");
                            Thread.Sleep(50);
                            SendKeys.SendWait("{RIGHT}");
                            Thread.Sleep(50);
                            SendKeys.SendWait(" ");
                            Thread.Sleep(100);

                            break;
                        }
                        SendKeys.SendWait("{DOWN}");
                    }

                    var childHandlesIn = APIManager.GetAllChildHandles(currentWindow.hwnd);
                    IntPtr buttonThoat = IntPtr.Zero;
                    foreach (var item in childHandlesIn)
                    {
                        string className = APIManager.GetWindowClass(item);
                        string classDefault = "WindowsForms10.BUTTON.app.0.1e6fa8e";
                        //string classDefault = "WindowsForms10.COMBOBOX.app.0.141b42a_r8_ad1";
                        if (className == classDefault)
                        {
                            buttonThoat = item;
                            break;
                        }
                    }

                    SendKeys.SendWait("{F10}");
                    if (buttonThoat != IntPtr.Zero)
                    {
                        APIManager.SendMessage(buttonThoat, 0x00F5, 0, 0);
                    }
                    isWaitingPrint = false;
                    isRunFirst = false;
                    stateDiNgoai = StateDiNgoai.Thoat;
                    break;

                case StateDiNgoai.Thoat:
                    if (currentWindow.text.IndexOf("dong chuyen thu") != -1)
                    {
                        if (!isRunFirst)
                        {
                            isRunFirst = true;
                            return;
                        }

                        isWaitingPrint = true;

                        stateDiNgoai = StateDiNgoai.MoLaiTiep;
                        SendKeys.SendWait("{F10}");
                        Thread.Sleep(200);
                        SendKeys.SendWait("{F10}");
                        Thread.Sleep(200);
                        SendKeys.SendWait("{ENTER}");
                    }
                    else if (currentWindow.text.IndexOf("khoi tao chuyen thu") != -1)
                    {
                        isWaitingPrint = false;
                        MessageBox.Show("Vui lòng đóng chuyến thư hiện tại.");
                        timerPrint.Stop();
                    }

                    isRunFirst = false;
                    isWaitingPrint = false;
                    break;

                case StateDiNgoai.MoLaiTiep:

                    if (currentWindow.text.IndexOf("khoi tao chuyen thu") != -1)
                    {
                        if (!isRunFirst)
                        {
                            isRunFirst = true;
                            return;
                        }
                        isWaitingPrint = true;
                        //lay du lieu tiep theo
                        if (DiNgoais.Count == 0)
                        {
                            timerPrint.Stop();
                            isWaitingPrint = false;
                            return;
                        }
                        //lay vi tri tiep theo
                        //get index
                        int index = DiNgoais.IndexOf(SelectedSimple);
                        if (index == -1)
                        {
                            timerPrint.Stop();
                            isWaitingPrint = false;
                            return;
                        }

                        if (index > DiNgoais.Count - 1)
                        {
                            timerPrint.Stop();
                            isWaitingPrint = false;
                            //txtInfo.Text = "Đã tới vị trí cuối cùng";
                            return;
                        }

                        //////xem thu no co chay cai gi khong
                        index++;
                        SelectedSimple = DiNgoais[index];
                        Selection(SelectedSimple);
                        timerPrint.Stop();
                        isWaitingPrint = false;
                    }
                    break;

                default:
                    break;
            }
        }

        private void ThuHep()
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "SmallRight" });
        }

        private void Xoa()
        {
            //xu ly them phan trung index
        }

        private void XoaDiNgoai()
        {
            if (SelectedDiNgoai == null)
                return;
            if (DiNgoais.Count == 0)
                return;

            DiNgoais.Remove(SelectedDiNgoai);
        }

        public ICommand AddAddressCommand { get; }
        public ICommand AddFastCommand { get; }
        public ICommand AddRangeCommand { get; }

        public ObservableCollection<string> BuuCucs
        {
            get { return _BuuCucs; }
            set { SetProperty(ref _BuuCucs, value); }
        }

        public ICommand ClearCommand { get; }
        public ICommand ClearDiNgoaiCommand { get; }

        public ObservableCollection<DiNgoaiItemModel> DiNgoais
        {
            get { return _DiNgoais; }
            set { SetProperty(ref _DiNgoais, value); }
        }

        public bool IsAutoF1
        {
            get { return _IsAutoF1; }
            set { SetProperty(ref _IsAutoF1, value); }
        }

        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                SetProperty(ref _IsExpanded, value);

                if (_IsExpanded == false)
                {
                    ThuHep();
                }
                else
                {
                    MoRong();
                }
            }
        }

        public bool IsGroupCT
        {
            get { return _IsGroupCT; }
            set { SetProperty(ref _IsGroupCT, value); }
        }

        public bool IsSayNumber
        {
            get { return _isSayNumber; }
            set { SetProperty(ref _isSayNumber, value); }
        }

        public bool IsTMDT
        {
            get { return _IsTMDT; }
            set { SetProperty(ref _IsTMDT, value); }
        }

        public bool IsTuDongDong
        {
            get { return _IsTuDongDong; }
            set { SetProperty(ref _IsTuDongDong, value); }
        }

        public ICommand MoRongCommand { get; }

        public string SelectedBuuCuc
        {
            get { return _SelectedBuuCuc; }
            set
            {
                SetProperty(ref _SelectedBuuCuc, value);
                OnSelectedBuuCuc();
            }
        }

        public DiNgoaiItemModel SelectedDiNgoai
        {
            get { return _SelectedDiNgoai; }
            set
            {
                SetProperty(ref _SelectedDiNgoai, value);
            }
        }

        public DiNgoaiItemModel SelectedSimple
        {
            get { return _SelectedSimple; }
            set
            {
                SetProperty(ref _SelectedSimple, value);
            }
        }

        public IRelayCommand<DiNgoaiItemModel> SelectionCommand { get; }
        public ICommand SelectionChiTietCommand { get; }
        public ICommand SetMaTinhGuiCommand { get; }
        public ICommand SetTinhCommand { get; }
        public ICommand ShowDataCommand { get; }
        public ICommand SortCommand { get; }
        public ICommand SortTinhCommand { get; }
        public ICommand StopDiNgoaiCommand { get; }

        public string TextCode
        {
            get { return _TextCode; }
            set
            {
                SetProperty(ref _TextCode, value);
                CheckEnterKey();
            }
        }

        public string TextsRange
        {
            get { return _TextsRange; }
            set { SetProperty(ref _TextsRange, value); }
        }

        public ICommand XoaCommand { get; }
        public ICommand XoaDiNgoaiCommand { get; }
        private readonly BackgroundWorker bwKhoiTao;
        private readonly BackgroundWorker bwPrintDiNgoai;
        private readonly DispatcherTimer timerPrint;
        private ObservableCollection<string> _BuuCucs;
        private ObservableCollection<DiNgoaiItemModel> _DiNgoais;
        private bool _IsAutoF1 = true;
        private bool _IsExpanded = false;
        private bool _IsGroupCT = false;
        private bool _isSayNumber = true;
        private bool _IsTMDT;
        private bool _IsTuDongDong;
        private string _SelectedBuuCuc;
        private DiNgoaiItemModel _SelectedDiNgoai;
        private DiNgoaiItemModel _SelectedSimple;
        private string _TextCode;
        private string _TextsRange;
        private int downTaoTui = 0;
        private bool isAutoRunDiNgoai = false;
        private bool IsPhoneRunning = false;
        private bool isRunFirst = false;
        private bool isWaitingPrint = false;
        private List<string> listBuuCuc;
        private List<string> listBuuCucTuDong;
        private StateDiNgoai stateDiNgoai = StateDiNgoai.KhoiTao;
        private List<TinhHuyenModel> tinhs;
    }
}