using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class DanhSachViewModel : ObservableObject
    {
        private ObservableCollection<BD10InfoModel> _BD10List;

        public ObservableCollection<BD10InfoModel> BD10List
        {
            get => _BD10List
;  set => SetProperty(ref _BD10List, value);
        }

        public ICommand LayDuLieuCommand { get; }
        public ICommand XoaCommand { get; }

        public DanhSachViewModel()
        {
            LoadBD10();
            LayDuLieuCommand = new RelayCommand(LayDuLieu);
            XoaCommand = new RelayCommand(Xoa);
            WeakReferenceMessenger.Default.Register<string>(this, (r, m) =>
            {
                if (m == "LoadBD10")
                {
                    LoadBD10();
                }
            });
        }

        void LoadBD10()
        {
            BD10List = new ObservableCollection<BD10InfoModel>();
            //thuc hien lay du lieu
            foreach (var item in FileManager.LoadData())
            {
                BD10List.Add(item);
            }
        }

        private void Xoa()
        {
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
            WeakReferenceMessenger.Default.Send("GoChiTiet");
        }

        //thuc hien viec get Data
    }
}