using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class WebContentModel : ObservableObject
    {
        public string Key { get; set; }
        public string Code { get; set; }
        public string AddressSend { get; set; }
        public string AddressReiceive { get; set; }
        public string BuuCucPhat { get; set; }
        public string BuuCucGui { get; set; }
    }
}
