using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class DiNgoaiViewModel : ObservableObject
    {
        public DiNgoaiViewModel()
        {
            DiNgoais = new ObservableCollection<DiNgoaiItemModel>();
            BuuCucs = new ObservableCollection<string>();

            XoaCommand = new RelayCommand(Xoa);
            ClearCommand = new RelayCommand(Clear);
            MoRongCommand = new RelayCommand(MoRong);
            ClearDiNgoaiCommand = new RelayCommand(ClearDiNgoai);
            AddRangeCommand = new RelayCommand(AddRange);
            XoaDiNgoaiCommand = new RelayCommand(XoaDiNgoai);

            AddAddressCommand = new RelayCommand(AddAddress);
            WeakReferenceMessenger.Default.Register<WebContentModel>(this, (r, m) =>
            {
                DiNgoaiItemModel diNgoai = DiNgoais.FirstOrDefault(c => m.Code.IndexOf(c.Code.ToUpper()) != -1);
                if (diNgoai != null)
                {
                    diNgoai.Address = m.AddressReiceive;
                    diNgoai.MaTinh = m.BuuCucPhat;
                    diNgoai.AddressSend = m.AddressSend;
                    diNgoai.BuuCucGui = m.BuuCucGui;

                    AutoSetBuuCuc(diNgoai);
                }
                AddAddress();
            });

            FileManager.GetCode();
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

        void AddAddress()
        {
            if (DiNgoais.Count == 0)
                return;

            foreach (DiNgoaiItemModel diNgoaiItem in DiNgoais)
            {
                if (string.IsNullOrEmpty(diNgoaiItem.MaTinh))
                {
                    WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "LoadAddressWeb", Content = diNgoaiItem.Code });
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
                            break ;
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
            var datas =textsRange.Split('\n');
            foreach (string data in datas)
            {
                if (data.Count()<13)
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
            string loai = diNgoai.Code.Substring(0, 1);
            if (diNgoai.MaTinh == "59")
            {

            }
            else if (diNgoai.MaTinh == "70")
            {

            }
            else if (diNgoai.MaTinh == "10")
            {

            }
            else if (diNgoai.MaTinh == "55")
            {

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
                    buucucs.Add(FileManager.listBuuCuc[i]);
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
