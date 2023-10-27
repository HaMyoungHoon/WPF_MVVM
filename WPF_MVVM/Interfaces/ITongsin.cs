using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_MVVM.Interfaces
{
    internal interface ITongsin
    {
        public enum MODE
        {
            BYTE,
            STRING
        }

        public void SetMode(MODE mode);
        public bool IsOpen();
        public bool IsConnected();
        public void Connect();
        public Task ConnectAsync();
        public void Disconnect();
        public Task DisconnectAsync();
        public void SendMsg(string data);
        public Task SendMsgAsync(string data);
        public void SendMsg(byte[] data);
        public Task SendMsgAsync(byte[] data);
        public Task RecvMsgAsync(string data);
        public void RecvMsg(string data);
        public Task RecvMsgAsync(byte[] data);
        public void RecvMsg(byte[] data);
        public void OnAccept();
        public Task OnConnectAsync();
        public void OnConnect();
        public Task OnDisconnectAsync();
        public void OnDisconnect();
        public Task NotifyErrorAsync(string error);
        public void NotifyError(string error);
    }
}
