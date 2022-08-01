using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class OptionModel : ObservableObject
    {

        public string MaBuuCuc { get; set; }

        public string GoFastKTCTKT { get; set; }
        public string GoFastKTCTBCP { get; set; }
        public string GoFastBD10Di { get; set; }
        public string GoFastBD10Den { get; set; }

        public string CodeBCMPSKT { get; set; }
        public string CodeBCMPSBCP { get; set; }
        public string StateBCMPSKT { get; set; }
        public string StateBCMPSBCP { get; set; }
        public string MaKhaiThac { get; internal set; }
        public string MaBuuCucPhat { get; internal set; }


        public string AccountDinhVi { get; set; }
        public string PWDinhVi { get; set; }
        public string AccountMPS { get; set; }
        public string PWMPS { get; set; }
        public string AccountPNS { get; set; }
        public string PWPNS { get; set; }
        public string GoFastQLCTCDBCP { get; internal set; }
        public string GoFastQLCTCDKT { get; internal set; }

        public OptionModel()
        {

        }

    }
}
