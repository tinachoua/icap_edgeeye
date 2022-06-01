using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShareLibrary.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShareLibrary.DataTemplate;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net.Sockets;
using ShareLibrary;

namespace ShareLibrary
{
    public class WebSocketDispatcher
    {
        public const int BufferSize = 4096;
        WebSocket _socket;
        private IRedisCacheDispatcher _rcd = new RedisCacheDispatcher();
        WebSocketDispatcher(WebSocket socket)
        {
            _socket = socket;
        }

        async Task Send()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);

            while (_socket.State == WebSocketState.Open)
            {
                IDevice add = new AdminDBDispatcher._device();
                var incoming = await _socket.ReceiveAsync(seg, CancellationToken.None);

                ArraySegment<byte> recvBuf = new ArraySegment<byte>(buffer, 0, incoming.Count);
                JObject recvObj;
                using (var ms = new MemoryStream())
                {
                    ms.Write(recvBuf.Array, recvBuf.Offset, incoming.Count);
                    var str = Encoding.UTF8.GetString(ms.ToArray());
                    recvObj = JObject.Parse(str);
                }

                string token = recvObj.GetValue("Token").ToString();
                int cmd = Convert.ToInt32(recvObj.GetValue("Cmd").ToString());
                string devName = recvObj.GetValue("DevName").ToString();  
                string user = _rcd.GetCache(0, token);
                var alias = add.GetAlias("DevName");
                if (user != null)
                {
                    if (cmd == (int)Action.Screenshot)
                    {
                        NotifyCoreService notifyCoreService = new NotifyCoreService();

                        notifyCoreService.Connect("172.30.0.11");
                        notifyCoreService.Send("INNO", "D", JsonConvert.SerializeObject(new {
                            Cmd = cmd,
                            DevName = devName
                        }));
                        string jsonStr = notifyCoreService.ReceiveMessage();
                        notifyCoreService.Close();
                        JObject jsonObj = JObject.Parse(jsonStr);
                        jsonObj.Add("Alias", alias);
                        byte[] retBuffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jsonObj));

                        await _socket.SendAsync(new ArraySegment<byte>(retBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                else
                {
                    LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                    {
                        Direction = true,
                        Name = user,
                        Method = "Websocket",
                        URL = "/ws",
                        ResponseCode = 403,
                        Remark = $"Token Error. {cmd}, {devName}"
                    });
                }
            }
        }

        static async Task Acceptor(HttpContext hc, Func<Task> n)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;

            var socket = await hc.WebSockets.AcceptWebSocketAsync();
            var h = new WebSocketDispatcher(socket);
            await h.Send();
        }

        /// <summary>
        /// branches the request pipeline for this SocketHandler usage
        /// </summary>
        /// <param name="app"></param>
        public static void Map(IApplicationBuilder app)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);
            app.Use(WebSocketDispatcher.Acceptor);
        }
    }
}