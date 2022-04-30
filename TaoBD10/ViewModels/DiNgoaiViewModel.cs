using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TaoBD10.ViewModels
{
    public class DiNgoaiViewModel : ObservableObject
    {
        public DiNgoaiViewModel()
        {

            XoaCommand = new RelayCommand(Xoa);
            ClearCommand = new RelayCommand(Clear);
            MoRongCommand = new RelayCommand(MoRong);
        }
        public ICommand ClearCommand { get; }
        public ICommand MoRongCommand { get; }

        public ICommand XoaCommand { get; }


        void Clear()
        {

        }

        void MoRong()
        {

        }

        void Xoa()
        {

        }
    }
}
