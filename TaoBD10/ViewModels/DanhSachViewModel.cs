using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
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
; private set => SetProperty(ref _BD10List, value);
        }

        public ICommand LayDuLieuCommand { get; }
        public ICommand XoaCommand { get; }

        public DanhSachViewModel()
        {
            _BD10List = new ObservableCollection<BD10InfoModel>();
            //thuc hien lay du lieu
            foreach (var item in FileManager.LoadData())
            {
                BD10List.Add(item);
            }
            LayDuLieuCommand = new RelayCommand(LayDuLieu);
            XoaCommand = new RelayCommand(Xoa);
        }

        void Xoa()
        {

        }

        void LayDuLieu()
        {
            //thuc hien viec lay du lieu

        }

        //thuc hien viec get Data
    }
}