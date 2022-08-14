using System;
using System.Collections.Generic;
using System.Linq;
using TaoBD10.Manager;

namespace TaoBD10.Model
{
    [Serializable]
    public class TuiHangHoa
    {
        public string STT { get; set; }
        public string SCT { get; set; }
        public string TuiSo { get; set; }
        public string PhanLoai { get; set; }
        public string DichVu { get; set; }
        public string FromBC { get; set; }
        public string ToBC { get; set; }
        public string Tinh { get; set; }
        public string Date { get; set; }
        public string KhoiLuong { get; set; }
        public string SHTui { get; set; }

        public TuiHangHoa()
        {
        }

        public TuiHangHoa(string stt, string sct, string tuiso, string dichvu, string frombc, string tobc, string date, string khoiluong, string shTui)
        {
            this.STT = stt;
            this.SCT = sct;
            this.TuiSo = tuiso;
            this.PhanLoai = "Chưa Biết";
            this.DichVu = dichvu;
            this.FromBC = frombc;
            this.ToBC = tobc;
            this.Tinh = FillTinh(tobc);
            //loc tinh
            this.Date = date;
            this.KhoiLuong = khoiluong;
            this.SHTui = shTui;
        }

        public TuiHangHoa(string stt, string bd10)
        {
            this.STT = stt;
            this.SHTui = bd10;
            phanGiaiBD10(bd10);
        }

        public void phanGiaiBD10(string bd10)
        {
            bd10 = bd10.ToUpper();
            if (bd10.Length != 29)
                return;
            //904600593230ACB20006001010053
            //108	1627	Bưu kiện - Parcel	2	590100	593760	15/11/2021	TB	16,0	590100 593760 ACB1 1627 002 01 0160	True
            this.FromBC = bd10.Substring(0, 6);
            this.ToBC = bd10.Substring(6, 6);
            this.Date = "15/11/2021";
            this.SCT = bd10.Substring(16, 4);
            this.TuiSo = bd10.Substring(20, 3);
            this.PhanLoai = "Đi ngoài";
            this.KhoiLuong = "1";
            this.Tinh = FillTinh(this.ToBC);
            string tempMa = bd10.Substring(13, 2);

            foreach (TuiThuModel tuiThu in FileManager.TuiThus)
            {
                if (tempMa == tuiThu.Ma)
                {
                    this.PhanLoai = tuiThu.Content;
                    break;
                }
            }
            if (string.IsNullOrEmpty(this.PhanLoai))
                this.PhanLoai = "Đi ngoài";

            switch (bd10.Substring(13, 1))
            {
                case "C":
                    this.DichVu = "Bưu kiện - Parcel";
                    break;

                case "E":
                    this.DichVu = "EMS - Chuyển phát nhanh";
                    break;

                case "U":
                    this.DichVu = "Bưu phẩm bảo đảm - Registed Mail";
                    break;

                case "P":
                    if (bd10.Substring(14, 1) == "H")
                    {
                        this.DichVu = "Phát hành báo chí";
                    }
                    else
                    {
                        this.DichVu = "Logistic";
                    }
                    break;

                default:
                    this.DichVu = "Chả biết";
                    break;
            }
        }

        private string FillTinh(string maBuuCuc)
        {
            List<TinhHuyenModel> tinhThanhs = FileManager.TinhThanhs;
            string nameTinh = "";
            string maTinh = maBuuCuc.Substring(0, 2);
            var tinh = tinhThanhs.FirstOrDefault(m => m.Ma == maTinh);
            if (tinh == null)
                return "Chưa Biết";
            if (tinh.Ma == "59")
            {
                //thuc hien xu ly trong nay
                if (maBuuCuc.IndexOf("590100") != -1)
                {
                    nameTinh = "Nam Trung Bộ";
                }
                else if (maBuuCuc.IndexOf("591218") != -1)
                {
                    nameTinh = "Quy Nhơn 2";
                }
                else if (maBuuCuc.IndexOf("591520") != -1)
                {
                    nameTinh = "Quy Nhơn";
                }
                else if (maBuuCuc.IndexOf("591720") != -1)
                {
                    nameTinh = "KT Tuy Phước";
                }
                else if (maBuuCuc.IndexOf("591760") != -1)
                {
                    nameTinh = "BCP Tuy Phước";
                }
                else if (maBuuCuc.IndexOf("591900") != -1)
                {
                    nameTinh = "Diêu Trì";
                }
                else if (maBuuCuc.IndexOf("592020") != -1)
                {
                    nameTinh = "KT An Nhơn";
                }
                else if (maBuuCuc.IndexOf("592040") != -1)
                {
                    nameTinh = "BCP An Nhơn";
                }
                else if (maBuuCuc.IndexOf("592100") != -1)
                {
                    nameTinh = "Đập Đá";
                }
                else if (maBuuCuc.IndexOf("592120") != -1)
                {
                    nameTinh = "Nhơn Thành";
                }
                else if (maBuuCuc.IndexOf("592220") != -1)
                {
                    nameTinh = "Nhơn Hòa";
                }
                else if (maBuuCuc.IndexOf("592440") != -1)
                {
                    nameTinh = "KT Phù Cát";
                }
                else if (maBuuCuc.IndexOf("592460") != -1)
                {
                    nameTinh = "BCP Phù Cát";
                }
                else if (maBuuCuc.IndexOf("592810") != -1)
                {
                    nameTinh = "KT Phù Mỹ";
                }
                else if (maBuuCuc.IndexOf("592850") != -1)
                {
                    nameTinh = "BCP Phù Mỹ";
                }
                else if (maBuuCuc.IndexOf("593100") != -1)
                {
                    nameTinh = "Bình Dương";
                }
                else if (maBuuCuc.IndexOf("593230") != -1)
                {
                    nameTinh = "KT Hoài Nhơn";
                }
                else if (maBuuCuc.IndexOf("593270") != -1)
                {
                    nameTinh = "Hoài Hương";
                }
                else if (maBuuCuc.IndexOf("593280") != -1)
                {
                    nameTinh = "BCP Hoài Nhơn";
                }
                else if (maBuuCuc.IndexOf("593330") != -1)
                {
                    nameTinh = "Tam Quan";
                }
                else if (maBuuCuc.IndexOf("593522") != -1)
                {
                    nameTinh = "Bồng Sơn";
                }
                else if (maBuuCuc.IndexOf("593630") != -1)
                {
                    nameTinh = "Ân Mỹ";
                }
                else if (maBuuCuc.IndexOf("593740") != -1)
                {
                    nameTinh = "KT Hoài Ân";
                }
                else if (maBuuCuc.IndexOf("593760") != -1)
                {
                    nameTinh = "BCP Hoài Ân";
                }
                else if (maBuuCuc.IndexOf("593850") != -1)
                {
                    nameTinh = "KT An Lão";
                }
                else if (maBuuCuc.IndexOf("593870") != -1)
                {
                    nameTinh = "BCP An Lão";
                }
                else if (maBuuCuc.IndexOf("593880") != -1)
                {
                    nameTinh = "An Hòa";
                }
                else if (maBuuCuc.IndexOf("594080") != -1)
                {
                    nameTinh = "KT Vĩnh Thạnh";
                }
                else if (maBuuCuc.IndexOf("594090") != -1)
                {
                    nameTinh = "BCP Vĩnh Thạnh";
                }
                else if (maBuuCuc.IndexOf("594210") != -1)
                {
                    nameTinh = "KT Tây Sơn";
                }
                else if (maBuuCuc.IndexOf("594220") != -1)
                {
                    nameTinh = "Bình Hòa";
                }
                else if (maBuuCuc.IndexOf("594300") != -1)
                {
                    nameTinh = "Tây Giang";
                }
                else if (maBuuCuc.IndexOf("594350") != -1)
                {
                    nameTinh = "BCP Tây Sơn";
                }
                else if (maBuuCuc.IndexOf("594560") != -1)
                {
                    nameTinh = "KT Vân Canh";
                }
                else if (maBuuCuc.IndexOf("594610") != -1)
                {
                    nameTinh = "BCP Vân Canh";
                }
                else if (maBuuCuc.IndexOf("594900") != -1)
                {
                    nameTinh = "KTNT Bình Định";
                }

            }
            else
            {
                nameTinh = tinh.Ten;
            }

            return nameTinh;
        }
    }
}