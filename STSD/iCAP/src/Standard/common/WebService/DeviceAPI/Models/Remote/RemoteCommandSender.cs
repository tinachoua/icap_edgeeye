using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using ShareLibrary.Interface;

namespace DeviceAPI.Models.Remote
{
    /// <summary>
    /// 
    /// </summary>
    public class RemoteCommandSender : IRemoteCommandSender
    {
        string ServerIPAddr = "172.30.0.4";
        int ServerPort = 1883;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="RemoteTarget"></param>
        /// <param name="RemoteCmd"></param>
        public void SendRemoteCommand(string deviceName, string RemoteTarget, string RemoteCmd)
        {
            MqttClient client = new MqttClient(IPAddress.Parse(ServerIPAddr), ServerPort, false, null, null, new MqttSslProtocols());
            client.Connect(Guid.NewGuid().ToString(), "admin", "AH0MBwnqi3O-9Dxlt7ZxGHBGsZC5TnEA");

            var cmd = new
            {
                Cmd = "Remote",
                ID = deviceName,
                Remote = new
                {
                    Name = RemoteTarget,
                    Value = RemoteCmd
                }
            };

            string jsonstring = JsonConvert.SerializeObject(cmd);

            client.Publish("Remote", Encoding.UTF8.GetBytes(jsonstring));
        }
    }
}
