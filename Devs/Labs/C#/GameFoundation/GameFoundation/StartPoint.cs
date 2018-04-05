using System;
using Fleck;
using System.Collections.Generic;
using System.Linq;


namespace GameFoundation
{
    class StartPoint
    {
        public static List<IWebSocketConnection> socketList = new List<IWebSocketConnection>();
        public static IWebSocketServer gameServer = new WebSocketServer("ws://0.0.0.0:8765");
        static void Main(string[] args)
        {
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
