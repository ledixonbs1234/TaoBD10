﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    class ChiTietViewModel:ObservableObject
    {
        private string[] fillDaNang = new string[] { "26", "27", "23", "22", "55", "38", "40", "10", "48", "17", "18", "16", "31", "39", "24", "33", "42", "46", "43", "51", "20", "52", "36", "41", "25", "44", "31", "53", "30", "32", "29", "28", "35", "13" };
        private string[] fillNoiTinh = new string[] { "591218", "591520", "591720", "591760", "591900", "592100", "592120", "592220", "594080", "594090", "594210", "594220", "594300", "594350", "594560", "594610", "590100" };
        private string[] fillNTB = new string[] { "88", "79", "96", "93", "82", "83", "80", "97", "90", "63", "64", "81", "87", "60", "91", "70", "65", "92", "58", "67", "85", "66", "62", "95", "84", "86", "94", "89", "74" };

        private string _NTamQuan;

        public string NTamQuan
        {
            get { return _NTamQuan; }
            set { SetProperty(ref _NTamQuan,value); }
        }
        private string _NKienDaNang;

        public string NKienDaNang
        {
            get { return _NKienDaNang; }
            set { SetProperty(ref _NKienDaNang, value); }
        }
        private string _NEMSDaNang;

        public string NEMSDaNang
        {
            get { return _NEMSDaNang; }
            set { SetProperty(ref _NEMSDaNang, value); }
        }
        private string _NQuangNam;

        public string NQuangNam
        {
            get { return _NQuangNam; }
            set { SetProperty(ref _NQuangNam, value); }

        }
        private string _NQuangNgai;

        public string NQuangNgai
        {
            get { return _NQuangNgai; }
            set { SetProperty(ref _NQuangNgai, value); }
        }
        private string _NKNTB;

        public string NKNTB
        {
            get { return _NKNTB; }
            set { SetProperty(ref _NKNTB, value); }
        }
        private string _NTNTB;

        public string NTNTB
        {
            get { return _NTNTB; }
            set { SetProperty(ref _NTNTB, value); }
        }
        private string _NPhuMy;

        public string NPhuMy
        {
            get { return _NPhuMy; }
            set { SetProperty(ref _NPhuMy, value); }
        }
        private string _NPhuCat;

        public string NPhuCat
        {
            get { return _NPhuCat; }
            set { SetProperty(ref _NPhuCat, value); }
        }
        private string _NAnNhon;

        public string NAnNhon
        {
            get { return _NAnNhon; }
            set { SetProperty(ref _NAnNhon, value); }
        }
        private string _NKT1;

        public string NKT1
        {
            get { return _NKT1; }
            set { SetProperty(ref _NKT1, value); }
        }



        List<HangHoaDetailModel> currentListHangHoa;
        public ChiTietViewModel()
        {
            WeakReferenceMessenger.Default.Register<BD10Message>(this, (r, m) =>
            {
                //Thuc Hien Trong ngay
                if(m.Value!= null)
                {
                    currentListHangHoa = new List<HangHoaDetailModel>();
                    foreach (var BD10 in m.Value)
                    {
                        foreach (TuiHangHoa tuiHangHoa in BD10.TuiHangHoas)
                        {
                            currentListHangHoa.Add(new HangHoaDetailModel(tuiHangHoa,EnumAll.PhanLoaiTinh.None));
                        }
                    }
                    FillData();
                    string b = "dfd";
                }
            });

        }
        void ResetAndCount()
        {
            NTamQuan = "0";
            NKienDaNang = "0";
            NEMSDaNang = "0";
            NQuangNam = "0";
            NQuangNgai = "0";
            NKNTB = "0";
            NTNTB = "0";
            NPhuMy = "0";
            NPhuCat = "0";
            NAnNhon = "0";
            NKT1 = "0";
             
            NTamQuan = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.TamQuan).Count.ToString(); 
            NKienDaNang = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.KienDaNang).Count.ToString();
            NEMSDaNang = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.EMSDaNang).Count.ToString();
            NQuangNam = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.QuangNam).Count.ToString();
            NQuangNgai = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.QuangNgai).Count.ToString();
            NKNTB = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.DiNgoaiNamTrungBo).Count.ToString();
            NTNTB = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.TuiNTB).Count.ToString();
            NPhuMy = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.PhuMy).Count.ToString();
            NPhuCat = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.PhuCat).Count.ToString();
            NAnNhon = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.AnNhon).Count.ToString();
            NKT1 = currentListHangHoa.FindAll(m => m.PhanLoai == EnumAll.PhanLoaiTinh.KT1).Count.ToString();
        }


        void FillData()
        {
            if (currentListHangHoa.Count == 0)
                return;
            //Thuc hien loc tung cai 1
            int countForeach = 0;
            
            foreach (var hangHoa in currentListHangHoa.ToList())
            {

                string maSoTinh = hangHoa.TuiHangHoa.ToBC.Substring(0, 2);
                if (hangHoa.TuiHangHoa.ToBC.IndexOf("593740") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("593630") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("593850") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("593880") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("593760") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("593870") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.HA_AL;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("593330") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.TamQuan;
                }
                else
               if (maSoTinh == "56")
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.QuangNam;
                }
                else if (maSoTinh == "57")
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.QuangNgai;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("592810") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("592850") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.PhuMy;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("592440") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("592460") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.PhuCat;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("592020") != -1 || hangHoa.TuiHangHoa.ToBC.IndexOf("592040") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.AnNhon;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("590900") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.KT1;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("593230") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.KTHN;
                }
                else if (hangHoa.TuiHangHoa.ToBC.IndexOf("593280") != -1)
                {
                    currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.BCPHN;
                }

                else if (maSoTinh == "59")
                {
                     string temp = fillNoiTinh.FirstOrDefault(m=>m.IndexOf(hangHoa.TuiHangHoa.ToBC) != -1);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (hangHoa.TuiHangHoa.PhanLoai.IndexOf("Túi") != -1)
                        {
                            currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.TuiNTB;
                        }
                        else if (APIManager.convertToUnSign3(hangHoa.TuiHangHoa.PhanLoai).ToLower().IndexOf("ngoai") != -1)
                        {
                            currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.DiNgoaiNamTrungBo;
                        }
                    }
                }
                else
                {
                    string temp = fillNTB.FirstOrDefault(m => m == maSoTinh);
                    if (!string.IsNullOrEmpty(temp))
                    {
                        if (APIManager.convertToUnSign3(hangHoa.TuiHangHoa.PhanLoai).ToLower().IndexOf("ngoai") != -1)
                        {
                            currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.DiNgoaiNamTrungBo;
                        }
                    }
                    else
                    {
                        string temp1 = fillDaNang.FirstOrDefault(m => m == maSoTinh);
                        if (!string.IsNullOrEmpty(temp1))
                        {
                            if (hangHoa.TuiHangHoa.DichVu.IndexOf("Bưu") != -1 || hangHoa.TuiHangHoa.DichVu.IndexOf("Logi") != -1)
                            {
                                currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.KienDaNang;
                            }
                            else if (hangHoa.TuiHangHoa.DichVu.IndexOf("EMS") != -1)
                            {
                                currentListHangHoa[countForeach].PhanLoai = Manager.EnumAll.PhanLoaiTinh.EMSDaNang;
                            }
                        }
                    }
                }
                countForeach++;
            }
            ResetAndCount();
        }
    }
}
