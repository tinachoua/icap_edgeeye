using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ShareLibrary
{
    public class MqttDispatcher
    {
        public MqttClient _client;
        string _serverIPAddr = "172.30.0.4";
        int _serverPort = 1883;
        string _account = "admin";
        string _password = "AH0MBwnqi3O-9Dxlt7ZxGHBGsZC5TnEA";

        //MqttDispatcher(MqttClient client)
        //{
        //    _client = client;
        //}

        //async Task Connect(string serverIPAddr, int serverPort, string account, string password)
        //{
        //    MqttDispatcher md = new MqttDispatcher(new MqttClient(IPAddress.Parse(serverIPAddr), serverPort, false, null, null, new MqttSslProtocols()));
        //    md._client.Connect(Guid.NewGuid().ToString(), account, password);
        //}
        public delegate void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e);
        async public Task Connect()
        {
            //MqttDispatcher md = new MqttDispatcher(new MqttClient(IPAddress.Parse(_serverIPAddr), _serverPort, false, null, null, new MqttSslProtocols()));
            //md._client.Connect(Guid.NewGuid().ToString(), _account, _password);
            _client = new MqttClient(IPAddress.Parse(_serverIPAddr), _serverPort, false, null, null, new MqttSslProtocols());
            _client.Connect(Guid.NewGuid().ToString(), _account, _password);
        }

        public void Send(string channel, string action, string devName)
        {
            var cmd = new
            {
                Cmd = action,
                ID = devName
            };

            _client.Publish(channel, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cmd)));
        }

        async public Task Subscribe(string channel, client_MqttMsgPublishReceived onReceived)
        {
            _client.Subscribe(new string[] { channel}, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        //public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        //{
        //    string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
        //}

        //void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        //{
        //    string ReceivedMessage = Encoding.UTF8.GetString(e.Message);

        //    SetText(ReceivedMessage);
        //}

    }
}
