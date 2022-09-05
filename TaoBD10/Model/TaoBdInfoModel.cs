using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class TaoBdInfoModel:ObservableObject
    {
        private string _DuongThu;

        public string DuongThu
        {
            get { return _DuongThu; }
            set { SetProperty(ref _DuongThu, value); }
        }

        private string _Chuyen1;

        public string Chuyen1
        {
            get { return _Chuyen1; }
            set { SetProperty(ref _Chuyen1, value); }
        }
        private string _Chuyen2;

        public string Chuyen2
        {
            get { return _Chuyen2; }
            set { SetProperty(ref _Chuyen2, value); }
        }


        private string _BCNhan;

        public string BCNhan
        {
            get { return _BCNhan; }
            set { SetProperty(ref _BCNhan, value); }
        }
        private bool _IsSangChieu;

        public bool IsSangChieu
        {
            get { return _IsSangChieu; }
            set { SetProperty(ref _IsSangChieu, value); }
        }
        
        private int _ThoiGianChia2LanDT;

        public int ThoiGianChia2LanDT
        {
            get { return _ThoiGianChia2LanDT; }
            set { SetProperty(ref _ThoiGianChia2LanDT, value); }
        }

        private bool _IsNextDay;

        public bool IsNextDay
        {
            get { return _IsNextDay; }
            set { SetProperty(ref _IsNextDay, value); }
        }






        public TaoBdInfoModel()
        {

        }
    }
}
