using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GameFoundation.GameUtils
{
	public class Room
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public List<Player> Players { get; set; } = new List<Player>();

		enum Stage {
			Ready,Playing, Counting
		}

		private Stage status;

		public Room(Player pl)
		{
			this.ID = pl.playerRoomId;
			this.Name = pl.playerRoomName;
			this.addPlayer(pl);
			this.status = Stage.Ready;
		}

		public void addPlayer(Player pl)
		{			
			Players.Add(pl);
			//(position in game to calculate respective postion in display of each player)
			// check and prepair again
			#region Assign position for player 
			int post = 0;			
			Players.ForEach(p => {
				if (p.pos_in_room == -1 || p.pos_in_room != post) {					
					p.pos_in_room = post;					
				}				
				post++;
			});
			Players.ForEach(p => {
				this.Broadcast(p, StaticEvent.JOIN_OR_CREATE_ROOM_SERVER_to_CLIENT);
			});
			#endregion
			// Send Start Button to the lastWinner or the player has min Score or the player minPostion in Game
			Player lastWinner = StaticEvent.FindLastWinner(this.Players);
			lastWinner.Send(lastWinner, StaticEvent.TAKE_START_BUTTON_SERVER_to_CLIENT);
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
			// Send Start Button to the lastWinner or the player has min Score or the player minPostion in Game
			Player lastWinner = StaticEvent.FindLastWinner(this.Players);
			if (lastWinner != null) {
				lastWinner.Send(lastWinner, StaticEvent.TAKE_START_BUTTON_SERVER_to_CLIENT);
			}
			
		}
		// Send start game signal to all of players
		public void StartGame() {
			List<int> shuffleCards = StaticEvent.CardShuffle();

			

			//1. Distribute cards to each player in room. Only one have winner flag
			Player winner = (from Player p in Players where p.isWinner == true select p).FirstOrDefault();
			winner.Cards.Add(shuffleCards[0]);
			shuffleCards.Remove(shuffleCards[0]);

			for (var i = 0; i< 8; i++)
			{
				Players.ForEach(p => {
					p.Cards.Add(shuffleCards[0]);
					shuffleCards.Remove(shuffleCards[0]);
				});
			}

			#region check
			// using for check during development
			string json = JsonConvert.SerializeObject(shuffleCards);
			Console.WriteLine("Remain Cards {0} and total count {1}",json, shuffleCards.Count);
			json = string.Join(",", winner.Cards.ToArray());
			Console.WriteLine("Card on Winner: {0}", json);
			#endregion

			Players.ForEach(p =>
			{
				p.Send(p, StaticEvent.START_NEW_GAME_SERVER_to_CLIENT, json = string.Join(",", p.Cards.ToArray()));
			});
		}
			


	}
}
