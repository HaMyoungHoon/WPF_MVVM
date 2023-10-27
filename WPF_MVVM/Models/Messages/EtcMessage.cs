using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_MVVM.Models.Messages
{
    internal class EtcMessage : ValueChangedMessage<KeyValuePair<string, object?>>
    {

        public EtcMessage(KeyValuePair<string, object?> keyValuePair) : base(keyValuePair)
        {
            
        }
    }
}
