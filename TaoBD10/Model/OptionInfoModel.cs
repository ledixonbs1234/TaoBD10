using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class OptionInfoModel : ObservableObject
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

        public OptionInfoModel(string name, string content1)
        {
            Name = name;
            Content1 = content1;
        }
        public OptionInfoModel(string name, string content1,string content2)
        {
            Name = name;
            Content1 = content1;
            Content2 = content2;
        }

        public string Content1 { get; set; }
        public string Content2 { get; set; }
        public OptionInfoModel()
        {

        }


    }
}
