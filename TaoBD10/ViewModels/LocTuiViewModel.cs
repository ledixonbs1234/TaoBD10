using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class LocTuiViewModel : ObservableObject
    {
        private ObservableCollection<TuiHangHoa> _ListHangHoa;

        public ObservableCollection<TuiHangHoa> ListHangHoa
        {
            get { return _ListHangHoa; }
            set
            {
                SetProperty(ref _ListHangHoa, value);
                ;
            }
        }

        private string _NameBD;

        public string NameBD
        {
            get { return _NameBD; }
            set
            {
                SetProperty(ref _NameBD, value);
                CapNhatCommand.NotifyCanExecuteChanged();
                ClearCommand.NotifyCanExecuteChanged();
            }
        }

        public IRelayCommand CapNhatCommand { get; }

        private void CapNhat()
        {
            //cap nhat ma khong co ten thi se chuyen qua chi tiet de kiem tra lai
            BD10InfoModel bd10 = new BD10InfoModel();
            bd10.Name = "Tam Thoi";
            bd10.DateCreateBD10 = DateTime.Now;
            bd10.LanLap = "1";
            bd10.TimeTrongNgay = EnumAll.TimeSet.Sang;
            bd10.TuiHangHoas = ListHangHoa.ToList();

            if (string.IsNullOrEmpty(NameBD))
            {
                List<BD10InfoModel> listBD10 = new List<BD10InfoModel>();
                listBD10.Add(bd10);
                WeakReferenceMessenger.Default.Send<BD10Message>(new BD10Message(listBD10));
                //thuc hien navigate to chi tiet
                WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Navigation", Content = "GoChiTiet" });
            }
            else
            {
                FileManager.SaveData(new BD10InfoModel(NameBD, ListHangHoa.ToList(), DateTime.Now, EnumAll.TimeSet.Sang, "1"));
                WeakReferenceMessenger.Default.Send<string>("LoadBD10");
                WeakReferenceMessenger.Default.Send<ContentModel>(new ContentModel { Key = "Snackbar", Content = "Đã Tạo BĐ 10 với tên : " + NameBD });
            }

            //MessageShow("Đã Tạo BĐ 10 với tên : " + NameBD);
        }

        public IRelayCommand ClearCommand { get; }

        private void ClearData()
        {
        }

        private string _TextBD;

        public string TextBD
        {
            get { return _TextBD; }
            set
            {
                if (value != _TextBD)
                {
                    SetProperty(ref _TextBD, value);
                    //Kiem tra thu Text trong nay
                    CheckEnterKey();
                }
            }
        }

        private void CheckEnterKey()
        {
            if (TextBD.IndexOf('\n') != -1)
            {
                TextBD = TextBD.Trim();
                if (TextBD.Length != 29)
                {
                    TextBD = "";
                    return;
                }
                //kiem tra trung khong
                if (ListHangHoa.Count == 0)
                {
                    ListHangHoa.Add(new TuiHangHoa((ListHangHoa.Count + 1).ToString(), TextBD));
                    TextBD = "";
                }
                else
                {
                    foreach (TuiHangHoa item in ListHangHoa)
                    {
                        if (item.SHTui == TextBD)
                        {
                            TextBD = "";
                            return;
                        }
                    }
                    ListHangHoa.Add(new TuiHangHoa((ListHangHoa.Count + 1).ToString(), TextBD));
                    TextBD = "";
                }
            }
        }

        public LocTuiViewModel()
        {
            ListHangHoa = new ObservableCollection<TuiHangHoa>();
            ClearCommand = new RelayCommand(ClearData, () =>
            {
                if (ListHangHoa.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            CapNhatCommand = new RelayCommand(CapNhat, () =>
            {
                if (ListHangHoa.Count != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }
    }
}