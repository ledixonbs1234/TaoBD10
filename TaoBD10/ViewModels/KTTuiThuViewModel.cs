using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class KTTuiThuViewModel : ObservableObject
    {
        private ObservableCollection<FindItemModel> _KTTuiThus;

        public ObservableCollection<FindItemModel> KTTuiThus
        {
            get { return _KTTuiThus; }
            set { SetProperty(ref _KTTuiThus, value); }
        }

        private ObservableCollection<FindItemModel> _TuiThusHave;

        public ObservableCollection<FindItemModel> TuiThusHave
        {
            get { return _TuiThusHave; }
            set { SetProperty(ref _TuiThusHave, value); }
        }

        private string _MaHieu;

        public string MaHieu
        {
            get { return _MaHieu; }
            set { SetProperty(ref _MaHieu, value); }
        }

        public ICommand SearchCommand { get; }

        private void Search()
        {
            if (string.IsNullOrEmpty(_MaHieu))
            {
                return;
            }
            TuiThusHave = new ObservableCollection<FindItemModel>();
            foreach (var item in KTTuiThus)
            {
                if (item.SHTui.ToUpper().IndexOf(_MaHieu.Trim().ToUpper()) != -1)
                {
                    TuiThusHave.Add(item);
                }
            }
        }

        public ICommand LayDuLieuCommand { get; }

        private void LayDuLieu()
        {
            List<FindItemModel> tuiThus = FileManager.LoadFindItemOnFirebase();
            Show(tuiThus);
        }

        private void Show(List<FindItemModel> SHTuis)
        {
            KTTuiThus = new ObservableCollection<FindItemModel>();
            foreach (FindItemModel item in SHTuis)
                KTTuiThus.Add(item);
        }

        public KTTuiThuViewModel()
        {
            //List<FindItemModel> tuiThus = FileManager.LoadFindItemOffline();
            SearchCommand = new RelayCommand(Search);
            LayDuLieuCommand = new RelayCommand(LayDuLieu);
            //Show(tuiThus);
        }

        //thuc hien
    }
}