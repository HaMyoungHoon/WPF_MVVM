using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_MVVM.Models.Messages
{
    internal class NewWindowMessage : ValueChangedMessage<object?>
    {
        public bool Close { get; set; }
        public bool Hide { get; set; }
        public object? Sender { get; set; }
        public object? NavigatedEventArgs { get; set; }
        public NewWindowMessage(object? value) : base(value)
        {

        }
    }
}
