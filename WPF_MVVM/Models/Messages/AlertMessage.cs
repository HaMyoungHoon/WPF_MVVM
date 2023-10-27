using CommunityToolkit.Mvvm.Messaging.Messages;

namespace WPF_MVVM.Models.Messages
{
    internal class AlertMessage : ValueChangedMessage<bool>
    {
        public string? Header { get; set; }
        public string? Message { get; set; }
        public object? Sender { get; set; }
        public int? During { get; set; } // sec
        public bool Close { get; set; }
        public AlertMessage(bool value) : base(value)
        {

        }
    }
}
