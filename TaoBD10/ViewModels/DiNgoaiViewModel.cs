using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

            timerPrint = new DispatcherTimer();
            timerPrint.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timerPrint.Tick += TimerPrint_Tick;
            timerDiNgoai = new DispatcherTimer();
            timerDiNgoai.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timerDiNgoai.Tick += TimerDiNgoai_Tick; ;
            SelectionCommand = new RelayCommand<DiNgoaiItemModel>(Selection);

            XoaCommand = new RelayCommand(Xoa);
            ClearCommand = new RelayCommand(Clear);
            MoRongCommand = new RelayCommand(MoRong);
            ClearDiNgoaiCommand = new RelayCommand(ClearDiNgoai);
            AddRangeCommand = new RelayCommand(AddRange);
            XoaDiNgoaiCommand = new RelayCommand(XoaDiNgoai);

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
                AddAddress();
            });

            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "RunPrintDiNgoai")
                {
                    stateDiNgoai = StateDiNgoai.KhoiTao;
                    timerPrint.Start();
                }

            });

            FileManager.GetCode();
        }

        bool isWaitDiNgoai = false;

        public IRelayCommand<DiNgoaiItemModel> SelectionCommand { get; }
        void Selection(DiNgoaiItemModel selected)
        {
            if (selected == null)
                return;
            OnSelectedSimple();
            
        }
        private void TimerDiNgoai_Tick(object sender, EventArgs e)
        {
            if (isWaitDiNgoai)
            {
                return;
            }
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
            {
                return;
            }

            if (currentWindow.text.IndexOf("khoi tao chuyen thu") != -1)
            {
                isWaitingTimer = false;
                isWaitDiNgoai = true;
                //Form1.instance.infoShare.Text = "Đang chọn bưu cục";
                Thread.Sleep(100);
                IntPtr loaiDiNgoai = IntPtr.Zero;
                //Thread.Sleep(400);
                var childHandles3 = APIManager.GetAllChildHandles(currentWindow.hwnd);
                int countCombobox = 0;
                IntPtr tinh = IntPtr.Zero;
                foreach (var item in childHandles3)
                {
                    string className = APIManager.GetWindowClass(item);
                    string classDefault = "WindowsForms10.COMBOBOX.app.0.1e6fa8e";
                    //string classDefault = "WindowsForms10.COMBOBOX.app.0.141b42a_r8_ad1";
                    if (className == classDefault)
                    {
                        if (countCombobox == 2)
                        {
                            tinh = item;
                            break;
                        }
                        else
                        if (countCombobox == 1)
                        {
                            loaiDiNgoai = item;
                            //StringBuilder sbBuffer = new StringBuilder();

                            //const int WM_GETTEXT = 0x000D;

                            //APIManager.SendMessageA(item, WM_GETTEXT, IntPtr.Zero, sbBuffer);
                            //MessageBox.Show(sbBuffer.ToString()) ;
                        }
                        countCombobox++;
                    }
                }

                APIManager.SendMessage(tinh, 0x0007, 0, 0);
                Thread.Sleep(50);
                APIManager.SendMessage(tinh, 0x0007, 0, 0);
                Thread.Sleep(50);
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
                        SendKeys.SendWait("{F1}");
                    }
                   
                }
                else
                {
                    if (string.IsNullOrEmpty(SelectedSimple.MaTinh))
                    {
                        //Form1.instance.infoShare.Text = "Chưa có mã Tỉnh";

                        timerDiNgoai.Stop();
                        isWaitDiNgoai = false;
                        return;
                    }
                    SendKeys.SendWait(SelectedSimple.MaTinh);
                }

                //////////////////////////////////////////////////////

                timerDiNgoai.Stop();
                isWaitDiNgoai = false;
                return;
            }else
            {
                WaitingCloseTimer(timerDiNgoai);
            }
        }

        bool isWaitingTimer = false;
        int countTimer = 0;
        private void WaitingCloseTimer(DispatcherTimer timerDiNgoai)
        {
            if (!isWaitingTimer)
            {
                isWaitingTimer = true;
                countTimer = 0;
            }else
            {
                countTimer++;
                if (countTimer >= 200)
                {
                    countTimer = 0;
                    isWaitingTimer = false;
                    isWaitDiNgoai = false;
                    APIManager.showSnackbar("Close");
                    timerDiNgoai.Stop();
                }
            }
        }

        bool isWaitingDiNgoai = false;
        StateDiNgoai stateDiNgoai = StateDiNgoai.KhoiTao;
        bool isRunFirst = false;
        int downTaoTui = 0;

        private void TimerPrint_Tick(object sender, EventArgs e)
        {
            if (isWaitingDiNgoai) return;
            APIManager.showTest("1");
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
            {
                return;
            }

            switch (stateDiNgoai)
            {
                case StateDiNgoai.KhoiTao:
                    APIManager.showTest("2");
                    if (currentWindow.text.IndexOf("khoi tao chuyen thu") != -1)
                    {
                        isWaitingDiNgoai = true;
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
                       APIManager.showTest("3");
                        APIManager.SendMessage(loadDiNgoai, 0x0007, 0, 0);
                        APIManager.SendMessage(loadDiNgoai, 0x0007, 0, 0);
                        string temp = "";
                        string charCodeFirst = SelectedSimple.Code[0].ToString().ToLower();

                        if (charCodeFirst == "c")
                        {
                            temp = "bưu k";
                            downTaoTui = 1;
                        }
                        else if (charCodeFirst == "e")
                        {
                            temp = "em";
                            downTaoTui = 2;
                        }
                        else if (charCodeFirst == "p")
                        {
                            temp = "lo";
                            downTaoTui = 3;
                        }

                        SendKeys.SendWait("{BS}" + temp + "{TAB}");
                        Thread.Sleep(100);
                        SendKeys.SendWait("{F10}");
                        stateDiNgoai = StateDiNgoai.TaoTui;
                        isRunFirst = false;
                        isWaitingDiNgoai = false;
                        APIManager.showTest("4");
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
                        APIManager.showTest("5");
                        isWaitingDiNgoai = true;
                        SendKeys.SendWait("{UP}{UP}{UP}{UP}{UP}");
                        for (int i = 0; i < downTaoTui; i++)
                        {
                            SendKeys.SendWait("{DOWN}");
                        }
                        SendKeys.SendWait("{F10}");
                        SendKeys.SendWait("A{BS}{BS}");
                        stateDiNgoai = StateDiNgoai.DongChuyen;

                        //stateDiNgoai = StateDiNgoai.MoLaiTiep;
                        isWaitingDiNgoai = false;
                        isRunFirst = false;
                        APIManager.showTest("6");
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
                        APIManager.showTest("7");
                        isWaitingDiNgoai = true;
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
                                APIManager.showTest("11");
                                //Kiem tra Da dong tui chua
                                SendKeys.SendWait("^C");
                                Thread.Sleep(200);
                                string textClip1 = Clipboard.GetText();
                                if (textClip1.IndexOf("Selected") == -1)
                                {
                                    isWaitingDiNgoai = false;
                                    timerPrint.Stop();
                                    WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = "Lỗi chưa chọn" });
                                    APIManager.showTest("8");
                                    return;
                                }

                                SendKeys.SendWait("{F6}");
                                Thread.Sleep(200);
                                SendKeys.SendWait("{F7}");
                                APIManager.showTest("10");
                                break;
                            }
                        }
                        stateDiNgoai = StateDiNgoai.In;
                        isRunFirst = false;
                        isWaitingDiNgoai = false;
                        APIManager.showTest("9");
                    }

                    break;

                case StateDiNgoai.In:
                    if (!isRunFirst)
                    {
                        isRunFirst = true;
                        return;
                    }

                    isWaitingDiNgoai = true;
                    APIManager.SetZ420Print();

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
                        isWaitingDiNgoai = false;

                        timerPrint.Stop();
                        return;
                    }
                    if (clipboard.IndexOf("BĐ8") == -1)
                    {
                        isWaitingDiNgoai = false;
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
                    isWaitingDiNgoai = false;
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

                        isWaitingDiNgoai = true;

                        stateDiNgoai = StateDiNgoai.MoLaiTiep;
                        SendKeys.SendWait("{F10}");
                        Thread.Sleep(200);
                        SendKeys.SendWait("{F10}");
                        Thread.Sleep(200);
                        SendKeys.SendWait("{ENTER}");
                    }
                    else if (currentWindow.text.IndexOf("khoi tao chuyen thu") != -1)
                    {
                        isWaitingDiNgoai = false;
                        MessageBox.Show("Vui lòng đóng chuyến thư hiện tại.");
                        timerPrint.Stop();
                    }

                    isRunFirst = false;
                    isWaitingDiNgoai = false;
                    break;

                case StateDiNgoai.MoLaiTiep:

                    if (currentWindow.text.IndexOf("khoi tao chuyen thu") != -1)
                    {
                        if (!isRunFirst)
                        {
                            isRunFirst = true;
                            return;
                        }
                        isWaitingDiNgoai = true;

                        //lay du lieu tiep theo
                        if (DiNgoais.Count == 0)
                        {
                            timerPrint.Stop();
                            isWaitingDiNgoai = false;
                            return;
                        }
                        //lay vi tri tiep theo
                        //var currentCell = dgvDiNgoai.SelectedRows[0];
                        //if (currentCell == null)
                        //{
                        //    timerPrintDiNgoai.Stop();
                        //    isWaitingDiNgoai = false;
                        //    txtInfo.Text = "Chưa chọn dữ liệu";
                        //    return;
                        //}
                        //int indexCurrentRow = currentCell.Index;

                        //if (indexCurrentRow > dgvDiNgoai.RowCount - 1)
                        //{
                        //    timerPrintDiNgoai.Stop();
                        //    isWaitingDiNgoai = false;
                        //    txtInfo.Text = "Đã tới vị trí cuối cùng";
                        //    return;
                        //}

                        //////xem thu no co chay cai gi khong
                        ////dgvDiNgoai.Rows[indexCurrentRow].Selected = false;
                        ////dgvDiNgoai.Rows[++indexCurrentRow].Selected = true;
                        //////dgvDiNgoai.CurrentCell = dgvDiNgoai[0, ++indexCurrentRow];
                        ////dgvDiNgoai_CellClick(this.dgvDiNgoai, new DataGridViewCellEventArgs(0, indexCurrentRow ));

                        ////txtInfo.Text = indexCurrentRow.ToString();
                        ////hien vi tri dau tien
                        //dgvDiNgoai.FirstDisplayedScrollingRowIndex = indexCurrentRow;
                        ////isAutoRun = true;

                        //// cho vao cong doan nhap,
                        //// sau do thi neu thanh cong thi hien so
                        ////va ghi du lieu vao cai dang khoi tao nay
                        ////neu ma khong duoc thi thoat ra khong dung nua
                        //Thread.Sleep(200);
                        //SendKeys.SendWait("{F3}");
                        //timerPrintDiNgoai.Stop();
                        //isWaitingDiNgoai = false;
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

        DispatcherTimer timerDiNgoai;

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
            timerDiNgoai.Start();
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = "Dang chon tinh" });
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

        public bool isSayNumber
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
                OnSelectedDiNgoai();
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

        DispatcherTimer timerPrint;

        void AddAddress()
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
            //            if (chrWeb.IsBrowserInitialized)
            //            {

            //                chrWeb.Stop();
            //            }
            //            else
            //            {
            //                NavigateToWebControl();
            //                return;
            //            }
            //            for (int i = 0; i < diNgoaiViewModel.diNgoais.Count; i++)
            //            {
            //                if (string.IsNullOrEmpty(diNgoaiViewModel.diNgoais[i].MaTinh))
            //                {
            //                    iCurrentItemDiNgoai = i;

            //                    string script = @"
            //                document.getElementById('MainContent_ctl00_txtID').value='" + diNgoaiViewModel.diNgoais[i].Code + @"';
            //				document.getElementById('MainContent_ctl00_btnView').click();
            //";
            //                    txtInfo.Text = "Web Loadding " + iCurrentItemDiNgoai.ToString();
            //                    chrWeb.ExecuteScriptAsync(script);
            //                    break;
            //                }
            //            }
        }

        void AddRange()
        {
            foreach (string item in LocTextTho(TextsRange))
            {
                if (string.IsNullOrEmpty(item))
                    continue;
                string textChanged = item.Trim().ToUpper();
                if (textChanged.Length != 13)
                {
                    continue;
                }                //    //kiem tra trung khong
                if (DiNgoais.Count == 0)
                {
                    DiNgoais.Add(new DiNgoaiItemModel(DiNgoais.Count + 1, textChanged));
                }
                else
                {
                    bool isTrundle = false;
                    foreach (DiNgoaiItemModel diNgoai in DiNgoais)
                    {
                        if (diNgoai.Code == textChanged)
                        {
                            isTrundle = true;
                            break;
                        }
                    }
                    if (isTrundle)
                        continue;

                    DiNgoais.Add(new DiNgoaiItemModel(DiNgoais.Count + 1, textChanged));
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
                if (boDauAndToLower(addressExactly).IndexOf("phu my") != -1)
                {
                    diNgoai.TenBuuCuc = "592810 - KT Phù Mỹ";
                    diNgoai.MaBuuCuc = "592810";
                }
                else if (boDauAndToLower(addressExactly).IndexOf("phu cat") != -1)
                {
                    diNgoai.TenBuuCuc = "592460 - BCP Phù Cát";
                    diNgoai.MaBuuCuc = "592460";
                }
                else if (boDauAndToLower(addressExactly).IndexOf("an nhon") != -1)
                {
                    diNgoai.TenBuuCuc = "592020 - KT An Nhơn";
                    diNgoai.MaBuuCuc = "592020";
                }
                else if (boDauAndToLower(addressExactly).IndexOf("tay son") != -1)
                {
                    diNgoai.TenBuuCuc = "594210 - KT Tây Sơn";
                    diNgoai.MaBuuCuc = "594210";
                }
                else if (boDauAndToLower(addressExactly).IndexOf("van canh") != -1)
                {
                    diNgoai.TenBuuCuc = "594560 - KT Vân Canh";
                    diNgoai.MaBuuCuc = "594560";
                }
                else if (boDauAndToLower(addressExactly).IndexOf("vinh thanh") != -1)
                {
                    diNgoai.TenBuuCuc = "594080 - KT Vĩnh Thạnh";
                    diNgoai.MaBuuCuc = "594080";
                }
                else if (boDauAndToLower(addressExactly).IndexOf("tuy phuoc") != -1)
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
                List<string> listBuuCuc = getListBuuCucFromTinh(diNgoai.MaTinh);
                if (listBuuCuc.Count == 0)
                    return;

                string data = listBuuCuc.FirstOrDefault(m => boDauAndToLower(m).IndexOf(boDauAndToLower(addressExactly)) != -1);
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

        string boDauAndToLower(string text)
        {
            return APIManager.convertToUnSign3(text).ToLower();

        }

        void CheckEnterKey()
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
                    if (isSayNumber)
                    {
                        SoundManager.playSound(@"Number\" + DiNgoais.Count.ToString() + ".wav");
                    }
                    TextCode = "";
                }
            }
        }

        void Clear()
        {

        }

        void ClearDiNgoai()
        {
            DiNgoais.Clear();
        }

        List<string> getListBuuCucFromTinh(string maTinh)
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

        void MoRong()
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

        void OnSelectedDiNgoai()
        {

            if (SelectedDiNgoai == null)
                return;
            //chuyen vo cbx
            BuuCucs.Clear();
            List<string> listBuuCuc = getListBuuCucFromTinh(SelectedDiNgoai.MaTinh);
            if (listBuuCuc.Count != 0)
            {
                foreach (string item in listBuuCuc)
                {
                    BuuCucs.Add(item);
                }
            }
        }

        void ThuHep()
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "SmallRight" });
        }

        void Xoa()
        {
            //xu ly them phan trung index
        }

        void XoaDiNgoai()
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
        int count = 0;
    }
}
