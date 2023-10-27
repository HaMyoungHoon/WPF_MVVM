using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WPF_MVVM.Models.Messages
{
    internal class NavigationMessage : ValueChangedMessage<string>
    {
        public object? Param { get; set; }
        public object? Sender { get; set; }
        public bool Close { get; set; }
        public NavigationMessage(string value) : base(value)
        {

        }
    }
}
