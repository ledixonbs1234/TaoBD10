﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class DanhSachViewModel : ObservableObject
    {
        private ObservableCollection<BD10InfoModel> _BD10List;

        public ObservableCollection<BD10InfoModel> BD10List
        {
            get => _BD10List
; private set => SetProperty(ref _BD10List, value);
        }

        public DanhSachViewModel()
        {
            _BD10List = new ObservableCollection<BD10InfoModel>();
            //thuc hien lay du lieu
            foreach (var item in FileManager.LoadData())
            {
                BD10List.Add(item);
            }
        }

        //thuc hien viec get Data
    }
}