using System;
using Fleck;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Converters;
using GameFoundation.GameUtils;
using Newtonsoft.Json;
using System.Dynamic;

namespace GameFoundation
{
    class StartPoint
    {       

        static void Main(string[] args)
        {
            List<IWebSocketConnection> socketList = new List<IWebSocketConnection>();
            IWebSocketServer gameServer = new WebSocketServer("ws://0.0.0.0:8765");

            gameServer.Start(socket =>
                {

                    socket.OnOpen = () =>
                    {
                        Console.WriteLine("Open!");
                        Console.WriteLine("detail: " + socket.ToString());
                        socketList.Add(socket);
                    };
                    socket.OnClose = () =>
                    {
                        Console.WriteLine("Close!");
                        socketList.Remove(socket);
                        
                    };
                    socket.OnMessage = message =>
                    {
                        //Console.WriteLine(message);                        
                        
                        try
                        {
                            // https://weblog.west-wind.com/posts/2012/Aug/30/Using-JSONNET-for-dynamic-JSON-parsing
                        Player pl = JsonConvert.DeserializeObject<Player>(message);
                        dynamic expando = JsonConvert.DeserializeObject<ExpandoObject>(message);
                            if (expando.msgEvent == StaticEvent.JOIN_OR_CREATE_ROOM_CLIENT_to_SERVER)
                            {
                                if (pl != null && pl.GetType() == typeof(Player))
                                {
                                    StaticEvent.CreateOrJoin_Handler(pl, socket);
                                }
                                
                            }
                        }
                        catch(Exception ext)
                        {
                            Console.WriteLine(ext.ToString());
                        }                      



                        //Console.WriteLine(StaticEvent.roomList.Count);
                        /*socketList.ToList().ForEach(s => s.Send("Echo: " + message))*/;

                    };
                });

            var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (var socket in socketList.ToList())
                {
                    socket.Send(input);
                }
                input = Console.ReadLine();
            }

        }


    }
}
