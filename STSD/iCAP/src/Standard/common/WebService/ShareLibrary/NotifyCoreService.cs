using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ShareLibrary
{
    public class NotifyCoreService
    {
        private string _header = null;
        private string _command = null;
        private string _payload = null;
        private string _ip = null;
        const int _restartCount = 3;
        const int _delayTime = 200;
        SocketDispatcher _socketDispatcher = new SocketDispatcher(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp));
        public string[] _currentRecipient { get; set; }
        public NotifyCoreService(string ip, string command, string payload)
        {
            _ip = ip;
            _command = command;
            _payload = payload;
        }

        public NotifyCoreService(){}
        public void Send()
        {
            for (int i = 0; i < _restartCount; i++)
            {
                if (!_socketDispatcher.Connect(_ip, 8787))
                {
                    if (i != _restartCount - 1)
                    {
                        Thread.Sleep(_delayTime);
                        continue;
                    }
                    else
                    {
                        _socketDispatcher.Close();
                        return;
                    }
                }
                break;
            }

            for (int i = 0; i < _restartCount; i++)
            {
                if (!_socketDispatcher.SendMessage("INNO", _command, _payload))
                {
                    if (i != _restartCount - 1)
                    {
                        Thread.Sleep(_delayTime);
                        continue;
                    }
                    else
                    {
                        _socketDispatcher.Close();
                        return;
                    }
                }
                break;
            }

            for (int i = 0; i < _restartCount; i++)
            {
                string msg = _socketDispatcher.ReceiveMessage();

                if (msg == null || msg == "TIMEOUT" || msg == "PACKAGE ERROR" || msg == "0")
                {
                    Thread.Sleep(_delayTime);
                    _socketDispatcher.SendMessage("INNO", _command, _payload);
                }
                else if (msg == "1")
                {
                    break;
                }
            }
            _socketDispatcher.Close();
            return;
        }

        public bool Connect(string ip)
        {
            for (int i = 0; i < _restartCount; i++)
            {
                if (!_socketDispatcher.Connect(ip, 8787))
                {
                    if (i != _restartCount - 1)
                    {
                        Thread.Sleep(_delayTime);
                        continue;
                    }
                    else
                    {
                        _socketDispatcher.Close();
                        return false; 
                    }
                }
                break;
            }
            return true;
        }

        public bool Send(string header, string command, string payload)
        {
            _header = header;
            _command = command;
            _payload = payload;

            for (int i = 0; i < _restartCount; i++)
            {
                if (!_socketDispatcher.SendMessage(header, command, payload))
                {
                    if (i != _restartCount - 1)
                    {
                        Thread.Sleep(_delayTime);
                        continue;
                    }
                    else
                    {
                        _socketDispatcher.Close();
                        return false;
                    }
                }
                break;
            }
            return true;
        }
        public bool Send(string header, string payload)
        {
            _header = header;
            _payload = payload;

            for (int i = 0; i < _restartCount; i++)
            {
                if (!_socketDispatcher.SendMessage(header, payload))
                {
                    if (i != _restartCount - 1)
                    {
                        Thread.Sleep(_delayTime);
                        continue;
                    }
                    else
                    {
                        _socketDispatcher.Close();
                        return false;
                    }
                }
                break;
            }
            return true;
        }
        public string ReceiveMessage()
        {
            byte[] buff = _socketDispatcher.ReceiveBytes();

            if (buff == null || buff.Length == 0)
            {
                return null;
            }
            else
            {
                return _socketDispatcher.GetPayloadforNewPkg(buff);
            }
        }

        public string ReceivePackage(SocketDispatcher.PACKAGE pkg)
        {
            SocketUtility state = _socketDispatcher.ReceiveMessage(pkg);

            if (state == SocketUtility.Success)
            {
                return pkg.payload[0].ToString();
            }
            else if (state == SocketUtility.TimedOut)
            {
                int i = 0;
                while (i++<2)
                {
                    state = _socketDispatcher.ReceiveMessage(pkg);
                    if (state == SocketUtility.Success)
                    {
                        return pkg.payload[0].ToString();
                    }
                }
            }
            return null;
        }

        public void Close()
        {
            _socketDispatcher.Close();
        }
    }
}
