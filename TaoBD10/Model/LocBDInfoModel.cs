using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace TaoBD10.Model
{
    [Serializable]
    public class LocBDInfoModel : ObservableObject
    {
        public string TenBD { get; set; }
        public bool IsTinh { get; set; } = true;
        public string PhanLoais { get; set; }
        public string DichVus { get; set; }
        private ObservableCollection<TinhHuyenModel> _DanhSachTinh;

        public ObservableCollection<TinhHuyenModel> DanhSachTinh
        {
            get { return _DanhSachTinh; }
            set { SetProperty(ref _DanhSachTinh, value); }
        }

        private bool _IsEnabledButton = false;

        public bool IsEnabledButton
        {
            get { return _IsEnabledButton; }
            set { SetProperty(ref _IsEnabledButton, value); }
        }

        public string DanhSachHuyen { get; set; }
        private ObservableCollection<HangHoaDetailModel> _HangHoas;

        public ObservableCollection<HangHoaDetailModel> HangHoas
        {
            get { return _HangHoas; }
            set { SetProperty(ref _HangHoas, value); }
        }

        private ObservableCollection<TaoBdInfoModel> _TaoBDs;

        public ObservableCollection<TaoBdInfoModel> TaoBDs
        {
            get { return _TaoBDs; }
            set { SetProperty(ref _TaoBDs, value); }
        }

        public LocBDInfoModel()
        {
            DanhSachTinh = new ObservableCollection<TinhHuyenModel>();
            HangHoas = new ObservableCollection<HangHoaDetailModel>();
            TaoBDs = new ObservableCollection<TaoBdInfoModel>();
        }

        public LocBDInfoModel(string maBC)
        {
            TenBD = maBC;
            DanhSachHuyen = maBC;
            DanhSachTinh = new ObservableCollection<TinhHuyenModel>();
            HangHoas = new ObservableCollection<HangHoaDetailModel>();
            TaoBDs = new ObservableCollection<TaoBdInfoModel>();
        }
    }
}