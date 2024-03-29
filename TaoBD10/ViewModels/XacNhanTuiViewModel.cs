﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class XacNhanTuiViewModel : ObservableObject
    {
        private ObservableCollection<XacNhanTuiModel> _XacNhanTuis;

        public ObservableCollection<XacNhanTuiModel> XacNhanTuis
        {
            get { return _XacNhanTuis; }
            set { SetProperty(ref _XacNhanTuis, value); }
        }

        private ObservableCollection<MaHieuTuiModel> _MaHieuTuis;

        public ObservableCollection<MaHieuTuiModel> MaHieuTuis
        {
            get { return _MaHieuTuis; }
            set { SetProperty(ref _MaHieuTuis, value); }
        }

        private XacNhanTuiModel _SelectedXacNhan;

        public XacNhanTuiModel SelectedXacNhan
        {
            get { return _SelectedXacNhan; }
            set
            {
                SetProperty(ref _SelectedXacNhan, value);
                OnSelectedTui();
            }
        }

        public ICommand GetAddressCommand { get; }

        private void GetAddress()
        {
            string listMaHieu = "";
            string addressDefault = "https://bccp.vnpost.vn/BCCP.aspx?act=MultiTrace&id=";
            foreach (XacNhanTuiModel tui in XacNhanTuis)
            {
                foreach (MaHieuTuiModel maHieu in tui.MaHieuTuis)
                {
                    listMaHieu += maHieu.MaHieu + ",";
                }
            }
            addressDefault += listMaHieu;
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "ListAddress", Content = addressDefault });
        }

        private string _TextSHTui = "";

        public string TextSHTui
        {
            get
            { return _TextSHTui; }
            set
            {
                SetProperty(ref _TextSHTui, value);
                OnEnterKey();
            }
        }

        private void OnEnterKey()
        {
            if (TextSHTui.IndexOf('\n') != -1)
            {
                if (XacNhanTuis.Count != 0)
                {
                    XacNhanTuiModel haveData = XacNhanTuis.Where(m => m.SHTui.ToLower() == TextSHTui.ToLower()).FirstOrDefault();
                    if (haveData == null)
                    {
                        XacNhanTuis.Add(new XacNhanTuiModel() { Index = XacNhanTuis.Count, SHTui = TextSHTui.Trim().ToUpper(), MaHieuTuis = new ObservableCollection<MaHieuTuiModel>() });
                    }
                }
                else
                {
                    XacNhanTuis.Add(new XacNhanTuiModel() { Index = 1, SHTui = TextSHTui.Trim().ToUpper(), MaHieuTuis = new ObservableCollection<MaHieuTuiModel>() });
                }
                TextSHTui = "";
            }
        }

        public ICommand LayTuiCommand { get; }

        private void LayTui()
        {
            ChonTatCaTrongTui();

            APIManager.WaitingFindedWindow("xac nhan chi tiet tui thu");
            RunGetData();
            //thuc hien ma
            SendKeys.SendWait("{ESC}");

            //Thuc hien kiem tra thu Cai nay co phai xac nhan tui chi tiet khong
        }

        public ICommand MoTuiCommand { get; }

        private WindowInfo ChonTatCaTrongTui()
        {
            if (!APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            Thread.Sleep(200);
            SendKeys.SendWait("{F3}");
            SendKeys.SendWait(SelectedXacNhan.SHTui);
            SendKeys.SendWait("{ENTER}");
            WindowInfo window = APIManager.WaitingFindedWindow("xac nhan chi tiet tui thu");

            return window;
        }

        private void MoTui()
        {
            WindowInfo window = ChonTatCaTrongTui();
            if (window != null)
            {
                if (SelectedXacNhan.TuiHave == SelectedXacNhan.MaHieuTuis.Count)
                {
                    SendKeys.SendWait("{TAB}");
                    Thread.Sleep(50);
                    SendKeys.SendWait("{TAB}");
                    Thread.Sleep(50);
                    SendKeys.SendWait("^(a)");
                    Thread.Sleep(200);
                    APIManager.ClickButton(window.hwnd, "Đối kiểm");
                    SendKeys.SendWait("{ESC}");
                }
            }
        }

        private void OnSelectedTui()
        {
            if (SelectedXacNhan != null)
            {
                MaHieuTuis = SelectedXacNhan.MaHieuTuis;
            }
        }

        private ObservableCollection<string> _KhongTonTais;

        public ObservableCollection<string> KhongTonTais
        {
            get { return _KhongTonTais; }
            set { SetProperty(ref _KhongTonTais, value); }
        }

        private string _MaHieu;

        public string MaHieu
        {
            get { return _MaHieu; }
            set
            {
                SetProperty(ref _MaHieu, value);
                OnCheckEnter();
            }
        }

        private string currentSHTui = "";

        private void RunGetData()
        {
            var currentWindow = APIManager.GetActiveWindowTitle();
            if (currentWindow == null)
            {
                return;
            }
            System.Windows.Clipboard.Clear();

            //thuc hien lay du lieu con
            var childHandlesIn = APIManager.GetAllChildHandles(currentWindow.hwnd);
            int countDefault = 0;
            foreach (var item in childHandlesIn)
            {
                string className = APIManager.GetWindowClass(item);
                string classDefault = "WindowsForms10.EDIT.app.0.1e6fa8e";

                //string classDefault = "WindowsForms10.COMBOBOX.app.0.141b42a_r8_ad1";
                if (className == classDefault)
                {
                    countDefault++;
                    if (countDefault == 5)
                    {
                        //thuc hien lay text cua handle item
                        string text = APIManager.GetControlText(item);

                        if (string.IsNullOrEmpty(text))
                        {
                            APIManager.ShowSnackbar("Không có SH Túi");
                            return;
                        }
                        currentSHTui = text;
                        break;
                    }
                }
            }

            SendKeys.SendWait("{TAB}");
            Thread.Sleep(50);
            SendKeys.SendWait("{TAB}");
            Thread.Sleep(50);
            SendKeys.SendWait("^(a)");
            Thread.Sleep(500);
            string clipboard = APIManager.GetCopyData();

            if (string.IsNullOrEmpty(clipboard))
            {
                APIManager.ShowSnackbar("Không copy được");
                return;
            }
            //thuc hien them tui
            AddSHTui(currentSHTui, clipboard);

            //thuc hien nhan nut thoat trong nay
        }

        private bool _IsExpanded = false;

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

        private void ThuHep()
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "SmallRight" });
        }

        private void MoRong()
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "Center" });
        }

        private void AddSHTui(string name, string content)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(name))
                return;

            XacNhanTuiModel xacNhan = new XacNhanTuiModel
            {
                Index = XacNhanTuis.Count + 1,
                SHTui = name
            };

            List<string> texts = content.Split('\n').ToList();
            if (texts[0].IndexOf("STT") != -1)
            {
                texts.RemoveAt(0);
            }
            List<MaHieuTuiModel> items = new List<MaHieuTuiModel>();
            foreach (string text in texts)
            {
                string[] textTemp = text.Split('\t');
                if (textTemp.Length >= 6)
                {
                    if (textTemp[1].Length == 13)
                    {
                        items.Add(new MaHieuTuiModel() { MaHieu = textTemp[1].ToUpper() });
                    }
                }
                else
                {
                    APIManager.ShowSnackbar("Loi Update SH Tui");
                    return;
                }
            }
            if (items.Count == 0)
            {
                APIManager.ShowSnackbar("Khong co ma hieu");
                return;
            }
            xacNhan.MaHieuTuis = new ObservableCollection<MaHieuTuiModel>();
            foreach (var item in items)
            {
                xacNhan.MaHieuTuis.Add(item);
            }

            bool isAddedSHTuiHaved = false;

            if (XacNhanTuis.Count != 0)
            {
                foreach (XacNhanTuiModel sHTuiTemp in XacNhanTuis)
                {
                    if (sHTuiTemp.SHTui.ToLower() == xacNhan.SHTui.ToLower())
                    {
                        if (sHTuiTemp.MaHieuTuis.Count == 0)
                        {
                            sHTuiTemp.MaHieuTuis = xacNhan.MaHieuTuis;
                            isAddedSHTuiHaved = true;
                            break;
                        }
                        else
                        {
                            APIManager.ShowSnackbar("Đã có túi này rồi");
                            return;
                        }
                    }
                }
            }
            if (!isAddedSHTuiHaved)
                XacNhanTuis.Add(xacNhan);

            int temp = 0;
            foreach (var item in XacNhanTuis)
            {
                temp += item.MaHieuTuis.Count;
            }
            TongCong = temp;
        }

        private int _TongCong = 0;

        public int TongCong
        {
            get { return _TongCong; }
            set { SetProperty(ref _TongCong, value); }
        }

        private readonly string[] tamquansAddress = { "tam quan", "hoai thanh", "hoai hao", "hoai phu", "hoai son", "hoai chau" };

        private void OnCheckEnter()
        {
            if (MaHieu.IndexOf('\n') != -1)
            {
                MaHieu = MaHieu.Trim().ToUpper();
                if (MaHieu.Length != 13)
                {
                    MaHieu = "";
                    return;
                }                //    //kiem tra trung khong
                bool isFinded = false;
                foreach (XacNhanTuiModel item in XacNhanTuis)
                {
                    MaHieuTuiModel have = item.MaHieuTuis.FirstOrDefault(m => m.MaHieu.ToUpper() == MaHieu);
                    if (have != null)
                    {
                        isFinded = true;

                        if (!have.IsChecked)
                        {
                            if (!string.IsNullOrEmpty(have.Address))
                            {
                                string addressLower = APIManager.ConvertToUnSign3(have.Address).ToLower();
                                string isHaveTamQuan = tamquansAddress.FirstOrDefault(m => addressLower.IndexOf(m) != -1);
                                if (!string.IsNullOrEmpty(isHaveTamQuan))
                                {
                                    SoundManager.playSound2(@"Number\tamquan.wav");
                                }
                            }

                            have.IsChecked = true;

                            item.TuiHave++;
                        }
                        else
                        {
                            SoundManager.playSound2(@"Number\buuguidaduocxacnhan.wav");
                        }
                        break;
                    }
                }
                int temp = 0;
                foreach (XacNhanTuiModel item in XacNhanTuis)
                {
                    temp += item.TuiHave;
                }
                Current = temp;
                if (isFinded)
                    SoundManager.playSound(@"Number\" + Current.ToString() + ".wav");
                else
                    SoundManager.playSound(@"Number\chuaxacdinh.wav");
                if (Current == TongCong)
                {
                    SoundManager.playSound3(@"Number\dusoluong.wav");
                }

                //thuc hien viec dem so trong nay
                MaHieu = "";
            }
        }

        private int _Current;

        public int Current
        {
            get { return _Current; }
            set { SetProperty(ref _Current, value); }
        }

        public XacNhanTuiViewModel()
        {
            XacNhanTuis = new ObservableCollection<XacNhanTuiModel>();
            MoTuiCommand = new RelayCommand(MoTui);
            LayTuiCommand = new RelayCommand(LayTui);
            GetAddressCommand = new RelayCommand(GetAddress);
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key != "XacNhan")
                    return;
                if (m.Content == "GetData")
                    RunGetData();
            });
            WeakReferenceMessenger.Default.Register<ChiTietTuiMessage>(this, (r, m) =>
            {
                if (m.Value != null)
                {
                    if (m.Value.Key == "XacNhanTui")
                    {
                        List<ChiTietTuiModel> chiTietTuis = m.Value.ChiTietTuis;

                        foreach (ChiTietTuiModel chiTietTui in chiTietTuis)
                        {
                            foreach (XacNhanTuiModel item in XacNhanTuis)
                            {
                                MaHieuTuiModel have = item.MaHieuTuis.FirstOrDefault(s => s.MaHieu.ToUpper() == chiTietTui.MaHieu.ToUpper());
                                if (have != null)
                                {
                                    have.Address = chiTietTui.Address;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            );
        }
    }
}