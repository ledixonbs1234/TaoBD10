using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
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
    public class OptionTinhThanhViewModel : ObservableObject
    {
        public OptionTinhThanhViewModel()
        {

            PublishTinhThanhCommand = new RelayCommand(PublishTinhThanh);
            LayDuLieuTinhThanhCommand = new RelayCommand(LayDuLieuTinhThanh);
            SaveTinhThanhCommand = new RelayCommand(SaveTinhThanh);
            LayDuLieuBuuCucCommand = new RelayCommand(LayDuLieuBuuCuc);
            SaveBuuCucCommand = new RelayCommand(SaveBuuCuc);
            MoFileBuuCucCommand = new RelayCommand(MoFileBuuCuc);
            PublishBuuCucCommand = new RelayCommand(PublishBuuCuc);
            MoFileTinhThanhCommand = new RelayCommand(MoFileTinhThanh);
            List<TinhHuyenModel> tinhThanhs = FileManager.LoadTinhThanhOffline();
            List<TuiThuModel> tuiThus = FileManager.LoadTuiThuOffline();
            MoFileTuiThuCommand = new RelayCommand(MoFileTuiThu);
            SaveTuiThuCommand = new RelayCommand(SaveTuiThu);
            PublishTuiThuCommand = new RelayCommand(PublishTuiThu);
            LayDuLieuTuiThuCommand = new RelayCommand(LayDuLieuTuiThu);
            ShowTinhThanh(tinhThanhs);
            List<string> buuCucs = FileManager.LoadBuuCucsOffline();
            ShowBuuCucs(buuCucs);
            ShowTuiThu(tuiThus);


        }

        private ObservableCollection<TuiThuModel> _TuiThus;

        public ObservableCollection<TuiThuModel> TuiThus
        {
            get { return _TuiThus; }
            set { SetProperty(ref _TuiThus, value); }
        }





        public ICommand MoFileTuiThuCommand { get; }

        void MoFileTuiThu()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = "matuithu";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";
            // Show open file dialog box
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                var tempList = FileManager.LoadTuiThuFromFile(filename);
                ShowTuiThu(tempList);
            }
        }

       
        public ICommand SaveTuiThuCommand { get; }

        void SaveTuiThu()
        {
            if (TuiThus.Count != 0)
                FileManager.SaveTuiThuOffline(TuiThus.ToList());
        }

       
        public ICommand PublishTuiThuCommand { get; }

        void PublishTuiThu()
        {
            if (TuiThus.Count != 0)
            {
                FileManager.SaveTuiThuFirebase(TuiThus.ToList());
            }
        }

       
        public ICommand LayDuLieuTuiThuCommand { get; }

        void LayDuLieuTuiThu()
        {
            List<TuiThuModel> tempList = FileManager.LoadTuiThuOnFirebase();
            ShowTuiThu(tempList);
        }

        private void ShowTuiThu(List<TuiThuModel> tempList)
        {
            if (tempList == null)
                return;
            TuiThus = new ObservableCollection<TuiThuModel>();
            foreach (var item in tempList)
            {
                TuiThus.Add(item);
            }
        }

        public ICommand MoFileTinhThanhCommand { get; }

        void MoFileTinhThanh()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = "TinhThanh";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";
            // Show open file dialog box
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                var tempList = FileManager.LoadTinhThanhFromFile(filename);
                ShowTinhThanh(tempList);
            }
        }


        public ICommand MoFileBuuCucCommand { get; }

        void MoFileBuuCuc()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = "BuuCuc";
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";
            // Show open file dialog box
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                List<string> tempList = FileManager.LoadBuuCucsFromFile(filename);
                ShowBuuCucs(tempList);
            }
        }
        private ObservableCollection<string> _BuuCucs;

        public ObservableCollection<string> BuuCucs
        {
            get { return _BuuCucs; }
            set { SetProperty(ref _BuuCucs, value); }
        }

        void ShowBuuCucs(List<string> buuCucs)
        {

            BuuCucs = new ObservableCollection<string>();
            foreach (var item in buuCucs)
            {
                BuuCucs.Add(item);
            }
        }







        void ShowTinhThanh(List<TinhHuyenModel> tinhThanhs)
        {

            TinhHuyens = new ObservableCollection<TinhHuyenModel>();
            foreach (var item in tinhThanhs)
            {
                TinhHuyens.Add(item);
            }
        }


        private ObservableCollection<TinhHuyenModel> _TinhHuyens;

        public ObservableCollection<TinhHuyenModel> TinhHuyens
        {
            get { return _TinhHuyens; }
            set { SetProperty(ref _TinhHuyens, value); }
        }


        public ICommand LayDuLieuTinhThanhCommand { get; }

        void LayDuLieuTinhThanh()
        {
            List<TinhHuyenModel> tempList = FileManager.LoadTinhThanhOnFirebase();
            ShowTinhThanh(tempList);
        }

        public ICommand SaveTinhThanhCommand { get; }

        void SaveTinhThanh()
        {
            if (TinhHuyens.Count != 0)
                FileManager.SaveTinhThanhOffline(TinhHuyens.ToList());
        }



        public ICommand PublishTinhThanhCommand { get; }

        void PublishTinhThanh()
        {

            if (TinhHuyens.Count != 0)
            {
                FileManager.SaveLayTinhThanhFirebase(TinhHuyens.ToList());
            }

        }


        public ICommand LayDuLieuBuuCucCommand { get; }

        void LayDuLieuBuuCuc()
        {
            List<string> buuCucs = FileManager.LoadBuuCucsOnFirebase();
            ShowBuuCucs(buuCucs);
        }

        public ICommand SaveBuuCucCommand { get; }

        void SaveBuuCuc()
        {
            if (BuuCucs.Count != 0)
                FileManager.SaveBuuCucsOffline(BuuCucs.ToList());
        }

        public ICommand PublishBuuCucCommand { get; }

        void PublishBuuCuc()
        {
            if (BuuCucs.Count != 0)
            {
                FileManager.SaveBuuCucsFirebase(BuuCucs.ToList());
            }
        }




    }
}
