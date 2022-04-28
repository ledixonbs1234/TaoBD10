﻿namespace TaoBD10.Manager
{
    public class EnumAll
    {
        public enum OptionXacNhan
        {
            None,
            TimKiem,
            SHTui
        }

        public enum BuuCuc
        {
            None,
            KT,BCP
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
            LocTui
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
    }
}