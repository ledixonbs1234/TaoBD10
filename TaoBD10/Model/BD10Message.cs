using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class BD10Message : ValueChangedMessage<List<BD10InfoModel>>
    {
        public BD10Message( List<BD10InfoModel> list):base(list)
        {
        }

    }
}
