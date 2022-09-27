using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using static TaoBD10.Manager.EnumAll;

namespace TaoBD10.Model
{
    public class HangHoaDetailModel : ObservableObject
    {
        public TuiHangHoa TuiHangHoa { get; set; }

        //public TrangThaiBD TrangThaiBD { get; set; } = TrangThaiBD.ChuaChon;
        private TrangThaiBD _TrangThaiBD = Manager.EnumAll.TrangThaiBD.ChuaChon;

        private string _Address;

        public string Address
        {
            get { return _Address; }
            set { SetProperty(ref _Address, value); }
        }

        private string _AddressSend;

        public string AddressSend
        {
            get { return _AddressSend; }
            set { SetProperty(ref _AddressSend, value); }
        }

        private string _IsTamQuan = "None";

        public string IsTamQuan
        {
            get { return _IsTamQuan; }
            set
            {
                SetProperty(ref _IsTamQuan, value);
                WeakReferenceMessenger.Default.Send(new ContentModel { Key = "IsTamQuan", Content = "Update" });
            }
        }

        private string _Code;

        public string Code
        {
            get { return _Code; }
            set { SetProperty(ref _Code, value); }
        }

        private string _PhanLoai = "";

        public string PhanLoai
        {
            get { return _PhanLoai; }
            set { SetProperty(ref _PhanLoai, value); }
        }

        public int Index { get; set; }

        public TrangThaiBD TrangThaiBD
        {
            get { return _TrangThaiBD; }
            set { SetProperty(ref _TrangThaiBD, value); }
        }

        public string Key { get; set; }

        public HangHoaDetailModel(TuiHangHoa tuiHangHoa)
        {
            TuiHangHoa = tuiHangHoa;
        }
    }
}