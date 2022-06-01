using ShareLibrary;
using System;
using System.Threading;

namespace ShareLibraryTester
{
    class Program
    {
        static void Main(string[] args)
        {
            KeyChecker ky = new KeyChecker();
            ThreadPool.QueueUserWorkItem(new WaitCallback(ky.KeyStatusChecker));
            int count = 0;
            for (int i = 0; i < 5; i++)
            {
                count = ky.GetAvailableDeviceCount();
            }
            Console.WriteLine("Get count={0}", count);
            Console.ReadLine();
        }
    }
}
