using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class XacNhanTuiViewModel:ObservableObject
    {

        private ObservableCollection<XacNhanTuiModel> _XacNhanTuis;

        public ObservableCollection<XacNhanTuiModel> XacNhanTuis
        {
            get { return _XacNhanTuis; }
            set { SetProperty(ref _XacNhanTuis, value); }
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
            set { SetProperty(ref _MaHieu, value);
                OnCheckEnter();
            }
        }
        string currentSHTui = "";
        string currentData = "";

        void RunGetData(IntPtr hwndMain)
        {
            System.Windows.Clipboard.Clear();

            //thuc hien lay du lieu con
            var childHandlesIn = APIManager.GetAllChildHandles(hwndMain);
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
            SendKeys.SendWait("^(c)");
            Thread.Sleep(100);
            string clipboard = "";
            try
            {
                clipboard = System.Windows.Clipboard.GetText();
            }
            catch (Exception edd)
            {
                APIManager.showSnackbar("Không copy được");
                return;
            }
            if (string.IsNullOrEmpty(clipboard))
            {
                APIManager.showSnackbar("Không copy được");
                return;
            }
            //thuc hien them tui
            AddSHTui(currentSHTui, clipboard);
        }

        void AddSHTui(string name,string content)
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
                        items.Add(new MaHieuTuiModel);
                    }
                }
                else
                {
                    MessageBox.Show("Loi Update SH Tui");
                    return;
                }
            }
            if (items.Count == 0)
            {
                txtInfo.Text = "Khong Co Ma Vat Pham";
                return;
            }
            shTui.addMaItems(shTuiModels.Count + 1, items);
            shTuiModels.Add(shTui);
            dgvDanhSachSHTui.DataSource = null;
            dgvDanhSachSHTui.DataSource = shTuiModels;
            dgvDanhSachSHTui.Refresh();

            dgvLocTui.DataSource = null;
            dgvLocTui.DataSource = shTuiModels;
            dgvLocTui.Refresh();

            txtSHTuiAdd.Text = "";
            txtSHTuiChild.Text = "";

            XacNhanTuis.Add(new XacNhanTuiModel() { Index = , })

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
                if (XacNhanTuis.Count == 0)
                {
                    XacNhanTuis.Add(new XacNhanTuiModel());
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

        public XacNhanTuiViewModel()
        {
            XacNhanTuis = new ObservableCollection<XacNhanTuiModel>();


        }
    }
}
