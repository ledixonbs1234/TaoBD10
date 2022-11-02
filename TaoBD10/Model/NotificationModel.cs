using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaoBD10.Model
{
    public class NotificationModel
    {
        public string Message { get; set; }
        public string TimeStamp { get; set; }

        public NotificationModel()
        {
        }

        public NotificationModel(string message, string timeStamp)
        {
            Message = message;
            TimeStamp = timeStamp;
        }
    }
}