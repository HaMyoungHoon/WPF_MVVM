using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_MVVM.Models.Messages
{
    internal class LoadingMessage : ValueChangedMessage<bool>
    {
        public string? LoadingId { get; set; }
        public string? LoadingText { get; set; }

        public LoadingMessage(bool value) : base(value)
        {

        }
    }
}
