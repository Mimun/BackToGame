using System;
using Fleck;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Converters;
using GameFoundation.GameUtils;
using Newtonsoft.Json;

namespace GameFoundation
{
    class StartPoint
    {

        public static List<Room> roomList = new List<Room>();

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
                        Console.WriteLine(message);

                        Player pl = null;
                        try
                        {
                        // https://weblog.west-wind.com/posts/2012/Aug/30/Using-JSONNET-for-dynamic-JSON-parsing
                        pl = JsonConvert.DeserializeObject<Player>(message);
                        }
                        catch
                        {
                            Console.WriteLine("invalid message Type");
                        }
                        if (pl != null && pl.GetType()== typeof(Player))
                        {
                            // assign websocket for each player to reusage in next steps
                            pl.playerWebsocket = socket;
                            // Do something here
                            for (int i = 0;i<= StartPoint.roomList.Count; i++)
                            {                                
                                if (  StartPoint.roomList.Count >0 && StartPoint.roomList[i].RoomId == pl.playerRoomId)
                                {                               
                                        StartPoint.roomList[i].addPlayer(pl);
                                        break;                                   
                                }
                                else
                                {                                    
                                    StartPoint.roomList.Add(new Room(pl));
                                    break;
                                }
                            }
                        }

                        Console.WriteLine(roomList.Count);
                        socketList.ToList().ForEach(s => s.Send("Echo: " + message));
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
