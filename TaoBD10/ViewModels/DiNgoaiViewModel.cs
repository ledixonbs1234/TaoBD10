using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        private readonly BackgroundWorker bwPrintDiNgoai;

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

            bwPrintDiNgoai.DoWork += BwPrintDiNgoai_DoWork;
            bwPrintDiNgoai.WorkerSupportsCancellation = true;
            SortTinhCommand = new RelayCommand(SortTinh);

            XoaCommand = new RelayCommand(Xoa);
            ClearCommand = new RelayCommand(Clear);
            MoRongCommand = new RelayCommand(MoRong);
            ClearDiNgoaiCommand = new RelayCommand(ClearDiNgoai);
            AddRangeCommand = new RelayCommand(AddRange);
            XoaDiNgoaiCommand = new RelayCommand(XoaDiNgoai);
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
                                have.MaTinh = ToolManager.AutoSetTinh(have.Address);
                                AutoSetBuuCuc(have);
                            }
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
            });

            FileManager.GetCode();
            ToolManager.SetUpTinh();
        }

        public ICommand StopDiNgoaiCommand { get; }

        private void StopDiNgoai()
        {
            bwPrintDiNgoai.CancelAsync();
        }
        public ICommand AddFastCommand { get; }


        void AddFast()
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

        public ICommand SetMaTinhGuiCommand { get; }

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

        public ICommand SortTinhCommand { get; }

        private void SortTinh()
        {
            if (DiNgoais.Count == 0)
                return;
            SetTinhFromMaTinh();
            List<DiNgoaiItemModel> listDiNgoai = new List<DiNgoaiItemModel>();
            //Thuc hien soft Tinh
            IOrderedEnumerable<DiNgoaiItemModel> dingoaisRa = DiNgoais.Where(m => int.Parse(m.MaTinh) < 59).OrderByDescending(x => x.TenTinh);
            IOrderedEnumerable<DiNgoaiItemModel> dingoaisVo = DiNgoais.Where(m => int.Parse(m.MaTinh) >= 59).OrderByDescending(x => x.TenTinh);
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

        private bool _IsChooseLan = false;

        public bool IsChooseLan
        {
            get { return _IsChooseLan; }
            set { SetProperty(ref _IsChooseLan, value); }
        }
        private const uint WM_SETTEXT = 0x000C;
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
                    downTaoTui = 2;
                }
                else if (charCodeFirst == "e")
                {
                    temp = "EMS - Chuyển phát nhanh - Express Mail Service";
                    downTaoTui = 6;
                }
                else if (charCodeFirst == "p")
                {
                    temp = "Logistic";
                    downTaoTui = 4;
                }

                SendKeys.SendWait("{BS}" + temp + "{TAB}");
                Thread.Sleep(100);
                SendKeys.SendWait("{F10}");

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
                    tuiText = "Ði ngoài(EMS)";
                }
                else if (charCodeFirst == "p")
                {
                    tuiText = "Ði ngoài(UT)";
                }
                APIManager.SendMessage(controls[9].Handle, WM_SETTEXT, IntPtr.Zero, new StringBuilder(tuiText));
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(100);
                SendKeys.SendWait("{F10}");


                //Thread.Sleep(100);
                //SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}");
                //for (int i = 0; i < downTaoTui; i++)
                //{
                //    SendKeys.SendWait("{DOWN}");
                //}
                SendKeys.SendWait("A{BS}{BS}");

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
                SendKeys.SendWait("{F5}");

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
                Thread.Sleep(50);
                SendKeys.SendWait("{F6}");
                SendKeys.SendWait("{F6}");
                Thread.Sleep(200);
                SendKeys.SendWait(SelectedSimple.Code);
                SendKeys.SendWait("{ENTER}");
                Thread.Sleep(200);
                SendKeys.SendWait("{F5}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                SendKeys.SendWait("{LEFT}");
                string clipboard = "";
                bool isCheckTui = false;
                for (int i = 0; i < 3; i++)
                {
                    currentWindow = APIManager.WaitingFindedWindow("dong chuyen thu");
                    SendKeys.SendWait("{F5}");
                    SendKeys.SendWait("{F5}");
                    SendKeys.SendWait("{LEFT}");
                    SendKeys.SendWait("{LEFT}");
                    Thread.Sleep(100);
                    SendKeys.SendWait(" ");
                    Thread.Sleep(500);
                    //Kiem tra Da dong tui chua

                    clipboard = APIManager.GetCopyData();

                    if (string.IsNullOrEmpty(clipboard))
                    {
                        APIManager.ShowSnackbar("Khong copy duoc");
                        return;
                    }

                    if (clipboard.IndexOf("Selected") != -1)
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
                clipboard = APIManager.GetCopyData();

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

                SendKeys.SendWait("{F10}");
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

        private void BwKhoiTao_DoWork(object sender, DoWorkEventArgs e)
        {
            try
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
                SendKeys.SendWait("{BS}{BS}{BS}{BS}");

                //Thuc hien trong nay
                if (!string.IsNullOrEmpty(SelectedSimple.MaBuuCuc))
                {
                    SendKeys.SendWait(SelectedSimple.MaBuuCuc);
                    Thread.Sleep(300);
                    SendKeys.SendWait("{DOWN}");
                    Thread.Sleep(100);
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

        private readonly BackgroundWorker bwKhoiTao;

        public IRelayCommand<DiNgoaiItemModel> SelectionCommand { get; }

        private void Selection(DiNgoaiItemModel selected)
        {
            if (selected == null)
                return;

            //thuc hien doc index
            SoundManager.playSound(@"Number\" + selected.Index + ".wav");
            OnSelectedSimple();
        }

        public ICommand SelectionChiTietCommand { get; }

        private void SelectionChiTiet()
        {
            OnSelectedDiNgoai();
        }

        public ICommand SortCommand { get; }

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

        private bool isWaitingPrint = false;
        private StateDiNgoai stateDiNgoai = StateDiNgoai.KhoiTao;
        private bool isRunFirst = false;
        private int downTaoTui = 0;

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

        public ICommand AddAddressCommand { get; }
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

        private DiNgoaiItemModel _SelectedSimple;

        public DiNgoaiItemModel SelectedSimple
        {
            get { return _SelectedSimple; }
            set
            {
                SetProperty(ref _SelectedSimple, value);
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

        public bool IsSayNumber
        {
            get { return _isSayNumber; }
            set { SetProperty(ref _isSayNumber, value); }
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

        private string _TextsRange;

        public string TextsRange
        {
            get { return _TextsRange; }
            set { SetProperty(ref _TextsRange, value); }
        }

        public DiNgoaiItemModel SelectedDiNgoai
        {
            get { return _SelectedDiNgoai; }
            set
            {
                SetProperty(ref _SelectedDiNgoai, value);
            }
        }

        public string TextCode
        {
            get { return _TextCode; }
            set
            {
                SetProperty(ref _TextCode, value);
                CheckEnterKey();
            }
        }

        public ICommand XoaCommand { get; }

        public ICommand XoaDiNgoaiCommand { get; }

        private readonly DispatcherTimer timerPrint;

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

        public ICommand SetTinhCommand { get; }

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

        private void SetTinhs()
        {
            foreach (var item in DiNgoais)
            {
                item.MaTinh = GetTinhFromBuuCuc(item.BuuCucNhanTemp);
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

        private string BoDauAndToLower(string text)
        {
            return APIManager.ConvertToUnSign3(text).ToLower();
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

        private void Clear()
        {
        }

        private void ClearDiNgoai()
        {
            DiNgoais.Clear();
        }

        private List<string> GetListBuuCucFromTinh(string maTinh)
        {
            List<string> buucucs = new List<string>();
            for (int i = 0; i < FileManager.listBuuCuc.Count; i++)
            {
                if (maTinh == FileManager.listBuuCuc[i].Substring(0, 2))
                {
                    buucucs.Add(FileManager.listBuuCuc[i].Trim());
                }
            }

            return buucucs;
        }

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

        private ObservableCollection<string> _BuuCucs;
        private ObservableCollection<DiNgoaiItemModel> _DiNgoais;
        private bool _IsAutoF1 = true;
        private bool _IsExpanded = false;
        private bool _isSayNumber = true;
        private string _SelectedBuuCuc;
        private DiNgoaiItemModel _SelectedDiNgoai;
        private string _TextCode;
    }
}