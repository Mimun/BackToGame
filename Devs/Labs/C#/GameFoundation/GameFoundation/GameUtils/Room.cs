using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace GameFoundation.GameUtils
{
	public class Room
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public List<Player> Players { get; set; } = new List<Player>();

		public Room(Player pl)
		{
			this.ID = pl.playerRoomId;
			this.Name = pl.playerRoomName;
			this.addPlayer(pl);
		}

		public void addPlayer(Player pl)
		{
			Players.Add(pl);
			//(position in game to calculate respective postion in display of each player)
			#region Assign position for player 
			int post = 0;			
			Players.ForEach(p => {
				if (p.pos_in_room == -1) {
					p.pos_in_room = post;					
				}
				post++;
			});
			#endregion
			this.Broadcast(pl, StaticEvent.JOIN_OR_CREATE_ROOM_SERVER_to_CLIENT);
		}

		public void Broadcast(Player pl, string msgEvent)
		{
			Players.ForEach(p =>
			{
				p.Send(pl, msgEvent);
				pl.Send(p, msgEvent);
			});
		}

		public void removePlayer(Player pl)
		{
			Players.Remove(pl);
			// SendEvent LEFT_ROOM to all players in Room
			Players.ForEach(p =>
			{
				// 1. Send information of new player to everyone in room
				p.Send(pl, StaticEvent.PLAYER_LEFT_ROOM_SERVER_to_CLIENT);
			});
		}

	}
}
