using Microsoft.Toolkit.Mvvm.Messaging.Messages;

namespace TaoBD10.Manager
{
    public class MessageManager : ValueChangedMessage<string>
    {
        public MessageManager(string user) : base(user)
        {
        }
    }
}