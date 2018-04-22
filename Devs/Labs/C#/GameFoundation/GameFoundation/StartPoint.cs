﻿using System;
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
						//pl.playerWebsocketList.Remove(socket);
						pl.AllConnections.Remove(socket);
						if (pl.AllConnections.Count < 1)
						{
							pl.playerRoom.removePlayer(pl);
						}
						Console.WriteLine("WebsocketManager count: {0}", WebSocketClientManager.Count);
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
						//JOIN_OR_CREATE_ROOM_CLIENT_to_SERVER
						if (expando.msgEvent == StaticEvent.JOIN_OR_CREATE_ROOM_CLIENT_to_SERVER)
						{
							if (pl != null)
							{
								Player player = StaticEvent.CreateOrJoin_Handler(pl, socket);
								WebSocketClientManager.TryAdd(socket, player);
								Console.WriteLine("WebsocketManager count: {0} Room {1}", WebSocketClientManager.Count , player.playerRoom.ID);
							}
						}
						if (expando.msgEvent == StaticEvent.START_NEW_GAME_CLIENT_to_SERVER)
						{
							Console.WriteLine("Iam here..");
							// Get player, room from socket
							Player player;
							WebSocketClientManager.TryGetValue(socket, out player);
							Room room = player.playerRoom;
							Console.WriteLine("In room {0} and player {1}", room.Name, player.playerName);
							room.StartGame();

						}
						//						
					}
					catch (Exception ext)
					{
						Console.WriteLine(ext.ToString());
					}
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
