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
            XoaCommand = new RelayCommand(Xoa);
            ClearCommand = new RelayCommand(Clear);
            MoRongCommand = new RelayCommand(MoRong);
            AddAddressCommand = new RelayCommand(AddAddress);
        }
        public ICommand AddAddressCommand { get; }
        public ICommand ClearCommand { get; }
        public ObservableCollection<DiNgoaiItemModel> DiNgoais
        {
            get { return _DiNgoais; }
            set { SetProperty(ref _DiNgoais, value); }
        }

        private bool _isSayNumber = true;

        public bool isSayNumber
        {
            get { return _isSayNumber; }
            set { SetProperty(ref _isSayNumber, value); }
        }


        private bool _IsAutoF1 = true;

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

        public ICommand MoRongCommand { get; }
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

        void AddAddress()
        {
            if (DiNgoais.Count == 0)
                return;

            foreach (DiNgoaiItemModel diNgoaiItem in DiNgoais)
            {
                if (string.IsNullOrEmpty(diNgoaiItem.MaTinh))
                {
                    WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "LoadAddressWeb", Content = diNgoaiItem.Code });
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

        void MoRong()
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "Center" });
        }

        void ThuHep()
        {
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "SmallRight" });
        }

        void Xoa()
        {
            //xu ly them phan trung index
        }

        private ObservableCollection<DiNgoaiItemModel> _DiNgoais;
        private bool _IsExpanded = false;
        private string _TextCode;
    }
}
