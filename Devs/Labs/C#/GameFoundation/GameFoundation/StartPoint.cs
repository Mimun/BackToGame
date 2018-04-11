using System;
using Fleck;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
			var WebSocketClientManager = new Dictionary<IWebSocketConnection, Player>();

			IWebSocketServer gameServer = new WebSocketServer("ws://0.0.0.0:8765");

			gameServer.Start(socket =>
			{
				socket.OnOpen = () =>
				{
					Console.WriteLine("Open!");
					Console.WriteLine("detail: " + socket.ConnectionInfo.Id);
				};
				socket.OnClose = () =>
				{

					if (WebSocketClientManager.Remove(socket, out Player pl))
					{
						pl.playerWebsocketList.Remove(socket);
						if (pl.playerWebsocketList.Count < 1)
						{
							pl.playerRoom.removePlayer(pl);
						}
						Console.WriteLine("WebsocketManager count: " + WebSocketClientManager.Count);
					}
					

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
							if (pl != null)
							{
								Player player = StaticEvent.CreateOrJoin_Handler(pl, socket);
								WebSocketClientManager.TryAdd(socket, player);
								Console.WriteLine("WebsocketManager count: " + WebSocketClientManager.Count + " Room: " + player.playerRoom.ID);
							}

						}
					}
					catch (Exception ext)
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
				input = Console.ReadLine();
			}

		}
	}
}
