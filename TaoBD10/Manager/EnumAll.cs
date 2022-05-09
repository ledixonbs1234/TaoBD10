namespace TaoBD10.Manager
{
    public class EnumAll
    {
        public enum OptionXacNhan
        {
            None,
            TimKiem,
            SHTui
        }

        public enum ChuaPhat
        {
            C593230,
            C593280
        }

        public enum BuuCuc
        {
            None,
            KT,BCP
        }
        public enum PrintState
        {
            CheckF,
            DongTui,
            CheckTaoTui,
            CheckButton,
            ChuanBiInBD8,
            PrintBD8,
            DongChuyenThu,
            DaThoat
        }
        public enum StateTaoBd10
        {
            DanhSachBD10,
            LapBD10

        }

        public enum StateDiNgoai
        {
            //go to chuyen thu
            KhoiTao,

            TaoTui,
            DongChuyen,
            In,
            Thoat,
            MoLaiTiep
        }

        public enum LoadWebChoose
        {
            ReadWord,
            XacNhan,
            DiNgoai,
            DiNgoaiAddress,
            None,
            CodeFromBD,
            TamQuan,
            AddressTamQuan,
            KiemTraWeb,
            AddressChuaPhat
        }

        public enum PhanLoaiTinh
        {
            None,
            HA_AL,
            TamQuan,
            KienDaNang,
            EMSDaNang,
            QuangNam,
            QuangNgai,
            DiNgoaiNamTrungBo,
            TuiNTB,
            PhuMy,
            PhuCat,
            AnNhon, KT1,
            KTHN,
            BCPHN
        }

        public enum CurrentTab
        {
            GetBD10,
            DanhSach,
            ChiTiet,
            ThuGon,
            LayChuyenThu,
            LocTui,
            Address
        }

        public enum TrangThaiBD
        {
            ChuaChon,DaChon,
            DaIn,TamQuan
        }

        public enum VungLamViec
        {
            TruyenBD10,
            XacNhanTuiThu
        }

        public enum StatePrintDiNgoai
        {
            ThongTinChuyenThu,
            PrintDocument,
            Print,
            AfterPrintDocument,
            AfterThongTinBuuGui
        }

        public enum ChuyenThu
        {
            KhaiThac,
            BuuCucPhat,
            None
        }

        public enum StateWeb
        {
            None,
            WebBd10,
        }

        public enum TimeSet
        {
            Sang,
            Trua,
            Chieu,
            Toi
        }

        public enum StateChuyenThuChieuDen
        {
            GetData,
            ShowInfo,
            Find
        }
    }
}