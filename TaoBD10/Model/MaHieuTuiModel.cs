﻿using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace TaoBD10.Model
{
    public class MaHieuTuiModel : ObservableObject
    {
        private bool _IsChecked;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { SetProperty(ref _IsChecked, value); }
        }

        private string _MaHieu;

        public string MaHieu
        {
            get { return _MaHieu; }
            set { SetProperty(ref _MaHieu, value); }
        }

        public MaHieuTuiModel()
        {
        }
    }
}