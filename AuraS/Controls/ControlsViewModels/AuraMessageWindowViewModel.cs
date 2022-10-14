using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraS.Controls.ControlsViewModels
{
    public class AuraMessageWindowViewModel
    {
        public AuraMessageWindowViewModel(string title, string message, string buttonText, MessageTypes messageType)
        {
            Title = title;
            Message = message;
            ButtonText = buttonText;
            MessageType = messageType;
        }
        public AuraMessageWindowViewModel(string message)
        {
            MessageType = MessageTypes.Info;
            Title = "Информация";
            Message = message;
            ButtonText = "Ok";
        }
        public string Title { get; set; }
        public string Message { get; set; }
        public string ButtonText { get; set; }
        public MessageTypes MessageType { get; set; }

        public enum MessageTypes
        {
            Info,
            Warning,
            Error
        }
    }
}
