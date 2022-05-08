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
using System.Windows;
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
            set { SetProperty(ref _SelectedXacNhan, value);
                OnSelectedTui();
            }
        }

        public ICommand MoTuiCommand { get; }

        
        void MoTui()
        {
            if (! APIManager.ThoatToDefault("593230", "quan ly chuyen thu chieu den"))
            {
                SendKeys.SendWait("1");
                Thread.Sleep(200);
                SendKeys.SendWait("3");
            }
            Thread.Sleep(200);
            SendKeys.SendWait("{F3}");
            SendKeys.SendWait(SelectedXacNhan.SHTui);
            SendKeys.SendWait("{ENTER}");
        }


        private void OnSelectedTui()
        {
            if(SelectedXacNhan != null)
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
        string currentSHTui = "";
        string currentData = "";

        void RunGetData()
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
                            APIManager.showSnackbar("Không có SH Túi");
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
            string clipboard = "";
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    SendKeys.SendWait("^(c)");
                    Thread.Sleep(100);
                    clipboard = System.Windows.Clipboard.GetText();
                    if (!string.IsNullOrEmpty(clipboard))
                        break;
                }
                catch (Exception edd)
                {
                    APIManager.showSnackbar("Không copy được");
                }
            }

            if (string.IsNullOrEmpty(clipboard))
            {
                APIManager.showSnackbar("Không copy được");
                return;
            }
            //thuc hien them tui
            AddSHTui(currentSHTui, clipboard);
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

        void ThuHep()
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "SmallRight" });
        }

        void MoRong()
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "Center" });
        }

        void AddSHTui(string name, string content)
        {

            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(name))
                return;
            XacNhanTuiModel xacNhan = new XacNhanTuiModel();
            xacNhan.Index = XacNhanTuis.Count + 1;
            xacNhan.SHTui = name;
            if (XacNhanTuis.Count != 0)
            {
                foreach (XacNhanTuiModel sHTuiTemp in XacNhanTuis)
                {
                    if (sHTuiTemp.SHTui.ToLower() == xacNhan.SHTui.ToLower())
                    {
                        APIManager.showSnackbar("Đã có túi này rồi");
                        return;
                    }
                }
            }


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
                    APIManager.showSnackbar("Loi Update SH Tui");
                    return;
                }
            }
            if (items.Count == 0)
            {
                APIManager.showSnackbar("Khong co ma hieu");
                return;
            }
            xacNhan.MaHieuTuis = new ObservableCollection<MaHieuTuiModel>();
            foreach (var item in items)
            {
                xacNhan.MaHieuTuis.Add(item);
            }
            XacNhanTuis.Add(xacNhan);

        }

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

                foreach (XacNhanTuiModel item in XacNhanTuis)
                {
                    MaHieuTuiModel have = item.MaHieuTuis.Where(m => m.MaHieu.ToUpper() == MaHieu).FirstOrDefault();
                    if (have != null)
                    {
                        have.IsChecked = true;
                        break;
                    }
                }
                MaHieu = "";
            }
        }

        public XacNhanTuiViewModel()
        {
            XacNhanTuis = new ObservableCollection<XacNhanTuiModel>();
            MoTuiCommand = new RelayCommand(MoTui);

            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key != "XacNhan")
                    return;
                if (m.Content == "GetData")
                    RunGetData();

            });


        }
    }
}
