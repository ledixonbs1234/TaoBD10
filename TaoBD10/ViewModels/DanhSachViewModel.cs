using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.ViewModels
{
    public class DanhSachViewModel : ObservableObject
    {
        private ObservableCollection<BD10InfoModel> _BD10List;
        private DateTime _CurrentTime = DateTime.Now;
        private bool[] _BuoiArray = new bool[] { true, false, false, false };

        public bool[] BuoiArray
        {
            get
            {
                return _BuoiArray;
            }
        }

        public int SelectedBuoi
        {
            get
            {
                return Array.IndexOf(_BuoiArray, true);
            }
        }

        public DateTime CurrentTime
        {
            get { return _CurrentTime; }

            set
            {
                if (value != _CurrentTime)
                {
                    SetProperty(ref _CurrentTime, value);
                    //Thuc hien thay doi torng ngay
                    LoadBD10();
                }
            }
        }

        public ICommand DeleteCommand { get; }

        private void Delete()
        {
            var tempList = new List<BD10InfoModel>(BD10List);
            foreach (var item in tempList)
            {
                if (item.isChecked)
                {
                    FileManager.list.Remove(item);
                    BD10List.Remove(item);
                }
            }
            //thuc hien save
            FileManager.SaveData(null);
        }

        public ObservableCollection<BD10InfoModel> BD10List
        {
            get => _BD10List
; set => SetProperty(ref _BD10List, value);
        }

        public ICommand LayDuLieuCommand { get; }

        public DanhSachViewModel()
        {
            //LoadBD10();
            DeleteCommand = new RelayCommand(Delete);
            LayDuLieuCommand = new RelayCommand(LayDuLieu);
            SelectedBuoiCommand = new RelayCommand(SelectedBuoiVoid);
            WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
            {
                if (m == "LoadBD10")
                {
                    //LoadBD10();
                }
            });
        }

        private void LoadBD10()
        {
            BD10List = new ObservableCollection<BD10InfoModel>();
            //thuc hien lay du lieu
            LoadForDate(CurrentTime, (TimeSet)SelectedBuoi);
        }

        private void SelectedBuoiVoid()
        {
            LoadBD10();
        }

        public ICommand SelectedBuoiCommand { get; }

        private void LoadForDate(DateTime time, TimeSet buoi)
        {
            foreach (var item in FileManager.LoadData())
            {
                //thuc hien load theo ngay
                if (item.DateCreateBD10.DayOfYear == time.DayOfYear)
                {
                    if (item.TimeTrongNgay == buoi)
                    {
                        item.isChecked = false;
                        BD10List.Add(item);
                    }
                }
            }
        }

        private void LayDuLieu()
        {
            List<BD10InfoModel> listBD10 = new List<BD10InfoModel>();
            //thuc hien viec lay du lieu
            foreach (var item in _BD10List)
            {
                if (item.isChecked)
                {
                    //thuc hien chuyen du lieu qua tab tiep theo
                    listBD10.Add(item);
                }
            }
            WeakReferenceMessenger.Default.Send(new BD10Message(listBD10));
            //thuc hien vao chi tiet
            WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Content = "GoChiTiet", Key = "Navigation" });
        }

        //thuc hien viec get Data
    }
}