using System;

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
            //108	1627	Bưu kiện - Parcel	2	590100	593760	15/11/2021	TB	16,0	590100 593760 ACB1 1627 002 01 0160	True
            this.FromBC = bd10.Substring(0, 6);
            this.ToBC = bd10.Substring(6, 6);
            this.Date = "15/11/2021";
            this.SCT = bd10.Substring(16, 4);
            this.TuiSo = bd10.Substring(20, 3);
            this.PhanLoai = "Đi ngoài";
            this.KhoiLuong = "1";
            this.Tinh = FillTinh(this.ToBC);
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
            string nameTinh = "";
            string maTinh = maBuuCuc.Substring(0, 2);
            switch (maTinh)
            {
                case "88":
                    nameTinh = "An Giang";
                    break;

                case "79":
                    nameTinh = "Bà Rịa Vũng Tàu";
                    break;

                case "26":
                    nameTinh = "Bắc Cạn";
                    break;

                case "23":
                    nameTinh = "Bắc Giang";
                    break;

                case "96":
                    nameTinh = "Bạc Liêu";
                    break;

                case "22":
                    nameTinh = "Bắc Ninh";
                    break;

                case "93":
                    nameTinh = "Bến Tre";
                    break;

                case "59":
                    nameTinh = "Bình Định";

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

                    break;

                case "82":
                    nameTinh = "Bình Dương";
                    break;

                case "83":
                    nameTinh = "Bình Phước";
                    break;

                case "80":
                    nameTinh = "Bình Thuận";
                    break;

                case "97":
                    nameTinh = "Cà Mau";
                    break;

                case "90":
                    nameTinh = "Cần Thơ";
                    break;

                case "27":
                    nameTinh = "Cao Bằng";
                    break;

                case "55":
                    nameTinh = " Đà Nẵng";
                    break;

                case "63":
                    nameTinh = " Đãk Lăk";
                    break;

                case "64":
                    nameTinh = " Đăk Nông";
                    break;

                case "38":
                    nameTinh = " Điện Biên";
                    break;

                case "81":
                    nameTinh = " Đồng Nai";
                    break;

                case "87":
                    nameTinh = " Đồng Tháp";
                    break;

                case "60":
                    nameTinh = " Gia Lai";
                    break;

                case "40":
                    nameTinh = " Hà Nam";
                    break;

                case "10":
                    nameTinh = " Hà Nội";
                    break;

                case "48":
                    nameTinh = " Hà Tĩnh";
                    break;

                case "17":
                    nameTinh = " Hãi Dương";
                    break;

                case "18":
                    nameTinh = " Hải Phỏng";
                    break;

                case "91":
                    nameTinh = " Hậu Giang";
                    break;

                case "70":
                    nameTinh = " Hồ Chĩ Minh";
                    break;

                case "16":
                    nameTinh = " Hưng Yên";
                    break;

                case "65":
                    nameTinh = " Khánh Hòa";
                    break;

                case "92":
                    nameTinh = " Kiên Giang";
                    break;

                case "31":
                    nameTinh = " Hà Giang";
                    break;

                case "58":
                    nameTinh = " KonTum";
                    break;

                case "39":
                    nameTinh = "Lai Châu";
                    break;

                case "67":
                    nameTinh = " Lâm Đồng";
                    break;

                case "24":
                    nameTinh = " Lạng Sơn";
                    break;

                case "33":
                    nameTinh = " Lào Cai";
                    break;

                case "85":
                    nameTinh = " Long An";
                    break;

                case "42":
                    nameTinh = " Nam Định";
                    break;

                case "46":
                    nameTinh = " Nghệ An";
                    break;

                case "43":
                    nameTinh = " Ninh Bình";
                    break;

                case "66":
                    nameTinh = " Ninh Thuận";
                    break;

                case "62":
                    nameTinh = " Phú Yên";
                    break;

                case "51":
                    nameTinh = " Quảng Bình";
                    break;

                case "56":
                    nameTinh = " Quảng Nam";
                    break;

                case "57":
                    nameTinh = " Quảng Ngãi";
                    break;

                case "20":
                    nameTinh = " Quảng Ninh";
                    break;

                case "52":
                    nameTinh = " Quảng Trị";
                    break;

                case "95":
                    nameTinh = " Sóc Trăng";
                    break;

                case "36":
                    nameTinh = " Sơn La";
                    break;

                case "84":
                    nameTinh = " Tây Ninh";
                    break;

                case "41":
                    nameTinh = " Thái Bình";
                    break;

                case "25":
                    nameTinh = " Thái Nguyên";
                    break;

                case "44":
                    nameTinh = " Thanh Hóa";
                    break;

                case "53":
                    nameTinh = " Thừa Thiên Huế";
                    break;

                case "86":
                    nameTinh = " Tiền Giang";
                    break;

                case "94":
                    nameTinh = " Trà Vinh";
                    break;

                case "30":
                    nameTinh = " Tuyên Quang";
                    break;

                case "89":
                    nameTinh = " Vĩnh Long";
                    break;

                case "32":
                    nameTinh = " Yên Bái";
                    break;

                case "29":
                    nameTinh = "Phú Thọ";
                    break;

                case "28":
                    nameTinh = "Vĩnh Phúc";
                    break;

                case "35":
                    nameTinh = "Hòa Bình";
                    break;

                default:
                    break;
            }
            if (nameTinh == "")
            {
                nameTinh = "Chưa Biết";
            }

            return nameTinh;
        }
    }
}