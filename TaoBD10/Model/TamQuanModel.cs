﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace TaoBD10.Model
{
    public class TamQuanModel : ObservableObject
    {
        private int _Index;

        public int Index
        {
            get { return _Index; }
            set { SetProperty(ref _Index, value); }
        }

        private double _TrongLuong;

        public double TrongLuong
        {
            get { return _TrongLuong; }
            set { SetProperty(ref _TrongLuong, value); }
        }

        private string _MaHieu;

        public string MaHieu
        {
            get { return _MaHieu; }
            set { SetProperty(ref _MaHieu, value); }
        }

        public TamQuanModel(int index, string maHieu)
        {
            this.Index = index;
            this.MaHieu = maHieu;
        }

        public TamQuanModel(int index, string maHieu, double KL)
        {
            this.Index = index;
            this.MaHieu = maHieu;
            TrongLuong = KL;
        }
    }
}