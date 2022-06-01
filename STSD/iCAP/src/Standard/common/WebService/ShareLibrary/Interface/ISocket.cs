using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary;

namespace ShareLibrary.Interface
{
    public interface ISocket
    {
        bool Connect(string ip, int port);
        bool SendMessage(string header, string command, string payload);
        string ReceiveMessage();
        void Close();
        byte[] ReceiveBytes();
        string GetPayload(byte[] dataBytes);
    }
}
