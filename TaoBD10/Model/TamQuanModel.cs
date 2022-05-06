using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class TamQuanModel :ObservableObject
    {
        private int _Index;

        public int Index
        {
            get { return _Index; }
            set { SetProperty(ref _Index, value); }
        }
        private string _MaHieu;


        public string MaHieu
        {
            get { return _MaHieu; }
            set { SetProperty(ref _MaHieu, value); }
        }
        public TamQuanModel(int index,string maHieu)
        {
            this.Index = index;
            this.MaHieu = maHieu;
        }

    }
}
