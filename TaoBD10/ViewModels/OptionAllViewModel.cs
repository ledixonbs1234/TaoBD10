using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TaoBD10.Manager;
using TaoBD10.Model;

namespace TaoBD10.ViewModels
{
    public class OptionAllViewModel : ObservableObject
    {
        private OptionModel _Option;

        public OptionModel Option
        {
            get { return _Option; }
            set { SetProperty(ref _Option, value); }
        }


        public ICommand SaveCommand { get; }

        void Save()
        {
            FileManager.SaveOptionAll(Option);
        }




        public OptionAllViewModel()
        {
            Option = new OptionModel();
            GetOptionData();
            SaveCommand = new RelayCommand(Save);

        }

        private void GetOptionData()
        {
            Option = FileManager.GetOptionOffline();

        }
    }
}
