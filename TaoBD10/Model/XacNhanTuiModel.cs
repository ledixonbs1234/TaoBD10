using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace TaoBD10.Model
{
    public class XacNhanTuiModel : ObservableObject
    {
        private int _Index;

        public int Index
        {
            get { return _Index; }
            set { SetProperty(ref _Index, value); }
        }

        private string _SHTui;

        public string SHTui
        {
            get { return _SHTui; }
            set { SetProperty(ref _SHTui, value); }
        }

        private ObservableCollection<MaHieuTuiModel> _MaHieuTuis;

        public ObservableCollection<MaHieuTuiModel> MaHieuTuis
        {
            get { return _MaHieuTuis; }
            set { SetProperty(ref _MaHieuTuis, value); }
        }

        private int _TuiHave = 0;

        public int TuiHave
        {
            get { return _TuiHave; }
            set { SetProperty(ref _TuiHave, value); }
        }

        public XacNhanTuiModel()
        {
        }
    }
}