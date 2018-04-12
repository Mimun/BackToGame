using System;
using System.Collections.Generic;
using System.Text;
using Fleck;
using System.Linq;

namespace GameFoundation.GameUtils
{
	public static class StaticEvent
	{
		public static string JOIN_OR_CREATE_ROOM_SERVER_to_CLIENT = "JOIN_OR_CREATE_ROOM_SERVER_to_CLIENT";
		public static string JOIN_OR_CREATE_ROOM_CLIENT_to_SERVER = "JOIN_OR_CREATE_ROOM_CLIENT_to_SERVER";
		public static string PLAYER_LEFT_ROOM_SERVER_to_CLIENT = "PLAYER_LEFT_ROOM_SERVER_to_CLIENT";


		public static List<Room> roomList = new List<Room>();

		public static Player CreateOrJoin_Handler(Player pl, IWebSocketConnection socket)
		{
			// Join And Create new Room
			#region Join And Create new Room

			var room = StaticEvent.roomList.FirstOrDefault(r => r.Name == pl.playerRoomName);

			// not in any room
			if (room == null)
			{
				pl.AllConnections.Add(socket);
				room = new Room(pl);
				pl.playerRoom = room;
				StaticEvent.roomList.Add(room);
			}
			else
			{
				if (room.Players.FirstOrDefault(p => p.playerName == pl.playerName) != null)
				{
					pl = room.Players.FirstOrDefault(p => p.playerName == pl.playerName);
					pl.AllConnections.Add(socket);
					room.Broadcast(pl, StaticEvent.JOIN_OR_CREATE_ROOM_SERVER_to_CLIENT);
				}
				else
				{
					pl.AllConnections.Add(socket);
					room.addPlayer(pl);
				}
				pl.playerRoom = room;
			}
			#endregion
			
			return pl;
		}
	}
}
