using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
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
            FileManager.SaveBD10Offline(null);
        }

        public ObservableCollection<BD10InfoModel> BD10List
        {
            get => _BD10List
; set => SetProperty(ref _BD10List, value);
        }

        public ICommand LayDuLieuCommand { get; }
        private bool IsSendToPhone = false;

        public DanhSachViewModel()
        {
            LoadBD10();
            DeleteCommand = new RelayCommand(Delete);
            LayDuLieuCommand = new RelayCommand(LayDuLieu);
            SelectedBuoiCommand = new RelayCommand(SelectedBuoiVoid);
            WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
            {
                if (m == "LoadBD10")
                {
                    LoadBD10();
                }
            });
            WeakReferenceMessenger.Default.Register<ContentModel>(this, (r, m) =>
            {
                if (m.Key == "ToDanhSach_CheckBD")
                {
                    //thuch ien lay danh sach voi
                    IsSendToPhone = true;
                    _BuoiArray = new bool[] { false, false, false, false };
                    _BuoiArray[int.Parse(m.Content)] = true;
                    LoadBD10();
                    APIManager.ShowSnackbar("Chay Danh Sach");
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
            List<BD10InfoModel> tempBDs = new List<BD10InfoModel>();
            foreach (var item in FileManager.LoadBD10Offline())
            {
                //thuc hien load theo ngay
                if (item.DateCreateBD10.DayOfYear == time.DayOfYear)
                {
                    if (item.TimeTrongNgay == buoi)
                    {
                        item.isChecked = false;
                        tempBDs.Add(item);
                    }
                }
            }
            if (tempBDs.Count == 0)
            {
                IsSendToPhone = false;
                return;
            }
            if (IsSendToPhone)
            {
                IsSendToPhone = false;
                string jsonText = JsonConvert.SerializeObject(tempBDs, Formatting.Indented);
                APIManager.ShowSnackbar("Da Send");
                MqttManager.Pulish(FileManager.FirebaseKey + "_checkbd", jsonText);
            }
            else
            {
                foreach (var item in tempBDs)
                {
                    BD10List.Add(item);
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