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

		public static string TAKE_START_BUTTON_SERVER_to_CLIENT = "TAKE_START_BUTTON_SERVER_to_CLIENT";
		public static string START_NEW_GAME_CLIENT_to_SERVER = "START_NEW_GAME_CLIENT_to_SERVER";
		public static string START_NEW_GAME_SERVER_to_CLIENT = "START_NEW_GAME_SERVER_to_CLIENT";

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

		public static List<int> CardShuffle()
		{
			// using https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
			Random _random = new Random();

			int[] array = new int[52];
			for (var i = 0; i < 52; i++)
			{
				array[i] = i;
			}

			int n = array.Length;
			for (int i = 0; i < n; i++)
			{
				// Use Next on random instance with an argument.
				// ... The argument is an exclusive bound.
				//     So we will not go past the end of the array.
				int r = i + _random.Next(n - i);
				int t = array[r];
				array[r] = array[i];
				array[i] = t;
			}

			return array.ToList();
		}

		public static Player FindLastWinner(List<Player> players)
		{
			
			Player lastWinner = null;
			if (players.Count == 0)
			{
				return lastWinner ;
			}

			lastWinner = (from p in players where p.isWinner == true select p).FirstOrDefault();
			if (lastWinner != null)
			{				
				return lastWinner;
			}
			// other case, find min score of player in room - for the case the winner left room
			//...
			// other case, take a first player join in room
			if (lastWinner == null)
			{
				int minPost = players.Min(p => p.pos_in_room);
				lastWinner = players.Find(p => { return p.pos_in_room == minPost; });
				lastWinner.isWinner = true;
			}
			return lastWinner;
		}
	}
}
