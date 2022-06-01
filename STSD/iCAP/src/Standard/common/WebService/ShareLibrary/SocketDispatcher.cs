using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ShareLibrary.Interface;
using System.Runtime.InteropServices;

namespace ShareLibrary
{
    public class SocketDispatcher:ISocket
    {
        private Socket _socket;
        private const int restartCount = 3;
        private const int _reveiveTimeout = 3000;
        private string _ip;
        private int _port;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct PACKAGE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public string header;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
            public string command;
            [MarshalAs(UnmanagedType.I2, SizeConst = 3)]
            public short length;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public string checksum;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string payload;

            public byte[] GetBytes()
            {
                return Encoding.ASCII.GetBytes(
                        string.Concat(header, command, payload)
                    );
            }

            public short GetPackageLength()
            {
                return (short)(header.Length + command.Length + payload.Length + checksum.Length + sizeof(short));
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct NEWPACKAGE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public string header;
            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint length;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public string checksum;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
            public string payload;

            public byte[] GetBytes()
            {
                return Encoding.ASCII.GetBytes(
                        string.Concat(header, payload)
                    );
            }

            public uint GetPackageLength()
            {
                return (114);
            }
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct RECEIVEPACKAGE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public string header;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
            public string command;
            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint length;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
            public string checksum;

            public byte[] GetBytes()
            {
                return Encoding.ASCII.GetBytes(
                        string.Concat(header, command)
                    );
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct NEWRECEIVEPACKAGE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public string header;
            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint length;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public string checksum;

            public byte[] GetBytes()
            {
                return Encoding.ASCII.GetBytes(
                        string.Concat(header)
                    );
            }
        }

        private byte[] SerializePackage(PACKAGE obj)
        {
            Type objectType;
            int objectSize;
            IntPtr buffer;
            byte[] array;

            objectType = obj.GetType();
            objectSize = Marshal.SizeOf(obj);
            buffer = Marshal.AllocHGlobal(objectSize);

            array = new byte[objectSize];

            Marshal.StructureToPtr(obj, buffer, false);
            Marshal.Copy(buffer, array, 0, objectSize);
            Marshal.FreeHGlobal(buffer);

            return array;
        }

        private byte[] SerializePackage(NEWPACKAGE obj)
        {
            Type objectType;
            int objectSize;
            IntPtr buffer;
            byte[] array;

            objectType = obj.GetType();
            objectSize = Marshal.SizeOf(obj);
            buffer = Marshal.AllocHGlobal(objectSize);

            array = new byte[objectSize];

            Marshal.StructureToPtr(obj, buffer, false);
            Marshal.Copy(buffer, array, 0, objectSize);
            Marshal.FreeHGlobal(buffer);

            return array;
        }

        public SocketDispatcher(Socket socket)
        {
            _socket = socket;
        }

        private uint GetCheckSum(PACKAGE pkg)
        {
            uint ret = 0;

            byte[] buff = pkg.GetBytes();

            foreach (byte b in buff)
            {
                ret += b;
            }

            return ret;
        }

        private uint GetCheckSum(NEWPACKAGE pkg)
        {
            uint ret = 0;

            byte[] buff = pkg.GetBytes();

            foreach (byte b in buff)
            {
                ret += b;
            }

            return ret;
        }

        public bool Connect(string ip, int port)
        {
            _ip = ip;
            _port = port;
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), port);

            try
            {
                _socket.Connect(ipep);
                _socket.ReceiveTimeout = _reveiveTimeout;
                return true;
            }
            catch (ArgumentNullException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Connect",
                    Ip = ip,
                    Port = port,
                    Msg = null,
                    Remark = "[ArgumentNullException] " + exc.Message
                });
                return false;
            }
            catch (ArgumentOutOfRangeException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Connect",
                    Ip = ip,
                    Port = port,
                    Msg = null,
                    Remark = "[ArgumentOutOfRangeException] " + exc.Message
                });
                return false;
            }
            catch (SocketException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Connect",
                    Ip = ip,
                    Port = port,
                    Msg = null,
                    Remark = "[SocketException] " + exc.Message
                });
                return false;
            }
            catch (ObjectDisposedException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Connect",
                    Ip = ip,
                    Port = port,
                    Msg = null,
                    Remark = "[ObjectDisposedException] " + exc.Message
                });
                return false;
            }
            catch (NotSupportedException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Connect",
                    Ip = ip,
                    Port = port,
                    Msg = null,
                    Remark = "[NotSupportedException] " + exc.Message
                });
                return false;
            }
            catch (InvalidOperationException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Connect",
                    Ip = ip,
                    Port = port,
                    Msg = null,
                    Remark = "[InvalidOperationException] " + exc.Message
                });
                return false;
            }
        }

        public bool SendMessage(string header, string command, string payload)
        {
            PACKAGE package = new PACKAGE();
            package.header = header;
            package.command = command;
            package.payload = payload;
            package.checksum = Convert.ToString(GetCheckSum(package), 16).ToUpper();
            package.length = package.GetPackageLength();

            byte[] send_buf = SerializePackage(package);

            int _len = 0;
            try
            {
                _len = _socket.Send(send_buf, send_buf.Length, SocketFlags.None);
                if (_len > 0)
                {
                    return true;
                }
                return false;
            }
            catch (SocketException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Send",
                    Ip = _ip,
                    Port = _port,
                    Msg = payload,
                    Remark = "[SocketException] " + exc.Message
                });
                return false;
            }
            catch (ObjectDisposedException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Send",
                    Ip = _ip,
                    Port = _port,
                    Msg = payload,
                    Remark = "[ObjectDisposedException] " + exc.Message
                });
                return false;
            }
        }
        public bool SendMessage(string header, string payload)
        {
            NEWPACKAGE package = new NEWPACKAGE();
            package.header = header;
            package.payload = payload;
            package.checksum = Convert.ToString(GetCheckSum(package), 16).ToUpper();
            package.length = package.GetPackageLength();

            byte[] send_buf = SerializePackage(package);

            int _len = 0;
            try
            {
                _len = _socket.Send(send_buf, send_buf.Length, SocketFlags.None);
                if (_len > 0)
                {
                    //Console.WriteLine($"Send Message length {_len}");
                    return true;
                }
                return false;
            }
            catch (SocketException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Send",
                    Ip = _ip,
                    Port = _port,
                    Msg = payload,
                    Remark = "[SocketException] " + exc.Message
                });
                return false;
            }
            catch (ObjectDisposedException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Send",
                    Ip = _ip,
                    Port = _port,
                    Msg = payload,
                    Remark = "[ObjectDisposedException] " + exc.Message
                });
                return false;
            }
        }
        private PACKAGE GetPackage(byte[] arr)
        {
            PACKAGE pkg = new PACKAGE();

            int size = Marshal.SizeOf(pkg);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            pkg = Marshal.PtrToStructure<PACKAGE>(ptr);

            Marshal.FreeHGlobal(ptr);

            return pkg;
        }

        private void GetPackage(byte[] arr, PACKAGE pkg)
        {
            int size = Marshal.SizeOf(pkg);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(arr, 0, ptr, size);

            pkg = Marshal.PtrToStructure<PACKAGE>(ptr);

            Marshal.FreeHGlobal(ptr);
        }


        private bool CheckPackage(PACKAGE pkg)
        {
            string checkall;

            checkall = Convert.ToString(GetCheckSum(pkg), 16).ToUpper();

            if (String.Equals(checkall, pkg.checksum))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public byte[] ReceiveBytes()
        {
            int _len = 0;
            List<byte> receiveList = new List<byte>();
            while (true)
            {
                try
                {
                    byte[] _recv_buf = new byte[4096];
                    _len = _socket.Receive(_recv_buf);
                    if (_len <= 0)
                    {
                        break;
                    }

                    for (var i = 0; i < _len; i++)
                    {
                        receiveList.Add(_recv_buf[i]);
                    }
                }
                catch (System.IO.IOException exc)
                {
                    LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                    {
                        Type = "Receive",
                        Ip = _ip,
                        Port = _port,
                        Msg = "Received Length: " + _len,
                        Remark = "[IOException] " + exc.Message
                    });
                    throw exc;
                }
                catch (SocketException exc)
                {
                    if (exc.SocketErrorCode == SocketError.TimedOut)
                    {
                        return null;
                    }

                    LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                    {
                        Type = "Receive",
                        Ip = _ip,
                        Port = _port,
                        Msg = "Received Length: " + _len,
                        Remark = "[SocketException] " + exc.Message
                    });
                    throw exc;
                }
                catch (Exception exc)
                {
                    LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                    {
                        Type = "Receive",
                        Ip = _ip,
                        Port = _port,
                        Msg = "Received Length: " + _len,
                        Remark = "[Exception] " + exc.Message
                    });
                    throw exc;
                }
            }
            return receiveList.ToArray();
        }

        public string GetPayload(byte[] dataBytes)
        {
            RECEIVEPACKAGE headerPkg = new RECEIVEPACKAGE();
            int size = Marshal.SizeOf(headerPkg);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(dataBytes, 0, ptr, size);
            headerPkg = Marshal.PtrToStructure<RECEIVEPACKAGE>(ptr);
            Marshal.FreeHGlobal(ptr);
            //check checksum
            uint ret = 0;
            byte[] buff = headerPkg.GetBytes();

            foreach (byte b in buff)
            {
                ret += b;
            }

            for (var i = size; i < dataBytes.Length; i++)
            {
                ret += dataBytes[i];
            }

            string checkall = Convert.ToString(ret, 16).ToUpper();

            if (String.Equals(checkall, headerPkg.checksum))
            {
                System.Text.Encoding encoding = System.Text.Encoding.ASCII;

                return encoding.GetString(dataBytes, size, dataBytes.Length - size);
            }
            else
            {
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "GetPayload",
                    URL = "",
                    ResponseCode = 0,
                    Remark = $"[Checksum error]Header: {headerPkg.header}, command: {headerPkg.command}, checksum:{headerPkg.checksum}, checkall: {checkall}, dataBytes length:{dataBytes.Length}"
                });

                return null;
            }
        }

        public string GetPayloadforNewPkg(byte[] dataBytes)
        {
            NEWRECEIVEPACKAGE headerPkg = new NEWRECEIVEPACKAGE();
            int size = Marshal.SizeOf(headerPkg);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(dataBytes, 0, ptr, size);
            headerPkg = Marshal.PtrToStructure<NEWRECEIVEPACKAGE>(ptr);
            Marshal.FreeHGlobal(ptr);
            //check checksum
            uint ret = 0;
            byte[] buff = headerPkg.GetBytes();
            foreach (byte b in buff)
            {
                ret += b; //header
            }
            for (var i = size; i < dataBytes.Length; i++)
            {
                ret += dataBytes[i]; //payload
            }

            string checkall = Convert.ToString(ret & 0xFFFF, 16).ToUpper();
            //todo
            if (true || String.Equals(checkall, headerPkg.checksum))
            {
                System.Text.Encoding encoding = System.Text.Encoding.ASCII;

                return encoding.GetString(dataBytes, size, dataBytes.Length - size);
            }
            else
            {
#if Debug
                /Console.WriteLine($"Header: {headerPkg.header}");
                Console.WriteLine($"length: {headerPkg.length}");
                Console.WriteLine($"checksum:{headerPkg.checksum}");
                Console.WriteLine($"checkall: {checkall}");
                Console.WriteLine($"dataBytes length:{dataBytes.Length}");
                Console.WriteLine("Checksum error");
#endif
                return null;
            }
        }

        public SocketUtility ReceiveMessage(PACKAGE recv_pkg)
        {
            byte[] _recv_buf = new byte[128];
            int _len = 0;
            try
            {
                _len = _socket.Receive(_recv_buf);
                if (_len == 0)
                {
                    LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                    {
                        Type = "Receive",
                        Ip = _ip,
                        Port = _port,
                        Msg = "Received Length: " + _len,
                        Remark = ""
                    });
                    return SocketUtility.ReceiveNull;
                }

                GetPackage(_recv_buf, recv_pkg);
                if (!CheckPackage(recv_pkg))
                {
                    LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                    {
                        Type = "Receive",
                        Ip = _ip,
                        Port = _port,
                        Msg = "Received Length: " + _len,
                        Remark = "PACKAGE ERROR"

                    });
                    return SocketUtility.PackageError;
                }
                return SocketUtility.Success;
            }
            catch (System.IO.IOException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Receive",
                    Ip = _ip,
                    Port = _port,
                    Msg = "Received Length: " + _len,
                    Remark = "[IOException] " + exc.Message
                });
                return SocketUtility.IOException;
            }
            catch (SocketException exc)
            {
                if (exc.SocketErrorCode == SocketError.TimedOut)
                {
                    return SocketUtility.TimedOut;
                }

                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Receive",
                    Ip = _ip,
                    Port = _port,
                    Msg = "Received Length: " + _len,
                    Remark = "[SocketException] " + exc.Message
                });
                return SocketUtility.SocketError;
            }
            catch (Exception exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Receive",
                    Ip = _ip,
                    Port = _port,
                    Msg = "Received Length: " + _len,
                    Remark = "[Exception] " + exc.Message
                });
                return SocketUtility.OtherException;
            }
        }

        public string ReceiveMessage()
        {
            byte[] _recv_buf = new byte[128];
            int _len = 0;
            PACKAGE recv_pkg;
            try
            {
                _len = _socket.Receive(_recv_buf);
                if (_len == 0)
                {
                    LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                    {
                        Type = "Receive",
                        Ip = _ip,
                        Port = _port,
                        Msg = "Received Length: " + _len,
                        Remark = ""
                    });
                    return null;
                }

                recv_pkg = GetPackage(_recv_buf);
                if (!CheckPackage(recv_pkg))
                {
                    LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                    {
                        Type = "Receive",
                        Ip = _ip,
                        Port = _port,
                        Msg = "Received Length: " + _len,
                        Remark = "PACKAGE ERROR"

                    });
                    return "PACKAGE ERROR";
                }
                return recv_pkg.payload[0].ToString();
            }
            catch (System.IO.IOException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Receive",
                    Ip = _ip,
                    Port = _port,
                    Msg = "Received Length: " + _len,
                    Remark = "[IOException] " + exc.Message
                });
                return "EXCEPTION";
            }
            catch (SocketException exc)
            {
                if (exc.SocketErrorCode == SocketError.TimedOut)
                {
                    return "TIMEOUT";
                }
                        
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Receive",
                    Ip = _ip,
                    Port = _port,
                    Msg = "Received Length: " + _len,
                    Remark = "[SocketException] " + exc.Message
                });
                return "EXCEPTION";
            }
            catch (Exception exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Receive",
                    Ip = _ip,
                    Port = _port,
                    Msg = "Received Length: " + _len,
                    Remark = "[Exception] " + exc.Message
                });
                return "EXCEPTION";
            }
        }
        public void Close()
        {
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Dispose();
            }
            catch(SocketException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Close",
                    Ip = _ip,
                    Port = _port,
                    Msg = null,
                    Remark = "[SocketException] " + exc.Message
                });
            }
            catch (ObjectDisposedException exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Close",
                    Ip = _ip,
                    Port = _port,
                    Msg = null,
                    Remark = "[ObjectDisposedException] " + exc.Message
                });
            }
            catch (Exception exc)
            {
                LogAgent.WriteToLog(new LogAgent.socketLogFormat()
                {
                    Type = "Close",
                    Ip = _ip,
                    Port = _port,
                    Msg = null,
                    Remark = "[Exception] " + exc.Message
                });
            }
        }
    }
}
