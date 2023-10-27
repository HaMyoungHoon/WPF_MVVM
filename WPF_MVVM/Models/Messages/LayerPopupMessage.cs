using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_MVVM.Models.Messages
{
    internal class LayerPopupMessage : ValueChangedMessage<bool>
    {
        public string? ControlName { get; set; }
        public string? Parameter { get; set; }

        public LayerPopupMessage(bool value) : base(value)
        {

        }
    }
}
