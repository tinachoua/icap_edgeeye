using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShareLibrary
{
    public class KeyChecker
    {
        private int CheckInterval = 60000;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct PACKAGE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public string header;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
            public string command;
            [MarshalAs(UnmanagedType.I2 ,SizeConst = 3)]
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

        private uint GetCheckSum(PACKAGE pkg)
        {
            uint ret = 0;

            byte[] buff = pkg.GetBytes();
            
            foreach(byte b in buff)
            {
                ret += b;
            }

            return ret;
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

        private PACKAGE GetLicenasePackage()
        {
            PACKAGE package = new PACKAGE();
            package.header = "INNO";
            package.command = "L";
            package.payload = CommonFunctions.GetCurrentEpoch().ToString();
            package.checksum = Convert.ToString(GetCheckSum(package), 16).ToUpper();
            package.length = package.GetPackageLength();

            return package;
        }

        private PACKAGE GetStatusPackage()
        {
            PACKAGE package = new PACKAGE();
            package.header = "INNO";
            package.command = "S";
            package.payload = CommonFunctions.GetCurrentEpoch().ToString();
            package.checksum = Convert.ToString(GetCheckSum(package), 16).ToUpper();
            package.length = package.GetPackageLength();

            return package;
        }

        public void KeyStatusChecker(object obj)
        {
            byte[] recv_buf = new byte[128], send_buf;
            int len;
            Socket client;
            PACKAGE recv_pkg;

            send_buf = SerializePackage(GetStatusPackage());

            Array.Clear(recv_buf, 0, recv_buf.Length);

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.Connect(IPAddress.Parse("172.30.0.15"), 8787);
                len = client.Send(send_buf, send_buf.Length, SocketFlags.None);
                len = client.Receive(recv_buf);
            }
            catch (Exception)
            {
                return;
            }

            recv_pkg = GetPackage(recv_buf);

            if (!CheckPackage(recv_pkg))
            {
                return;
            }
            
            if(recv_pkg.payload[0] == '1')
            {
                //Console.WriteLine("Keypro connected.");
            }
            else
            {
               // Console.WriteLine("Keypro disconnected.");
            }
        }

        public int GetAvailableDeviceCount()
        {
            byte[] recv_buf = new byte[128], send_buf;
            string[] tok_str;
            int len;
            Socket client;
            PACKAGE recv_pkg;
            int numberofdevice = 5;

            send_buf = SerializePackage(GetLicenasePackage());
            
            for(int i = 0; i < 128; i++)
            {
                recv_buf[i] = 0;
            }

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.Connect(IPAddress.Parse("172.30.0.15"), 8787);
                len = client.Send(send_buf, send_buf.Length, SocketFlags.None);
                len = client.Receive(recv_buf);
            }
            catch (Exception)
            {
                return 5;
            }
            
            recv_pkg = GetPackage(recv_buf);

            if (!CheckPackage(recv_pkg)) 
            {
                return 5;
            }
            
            tok_str = recv_pkg.payload.Split('#');
            if(!int.TryParse(tok_str[2], out numberofdevice))
            {
                return 5;
            }

            return numberofdevice;
        }
        
    }
}
