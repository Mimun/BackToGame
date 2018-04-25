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

		public List<int> RemainCards;

		enum Stage {
			Ready,Playing, Finallizing
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
			if (this.status != Stage.Ready) {
				return;
			}
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
				p.Status = Player.Stage.Idle;
			});
			#endregion
			// Send Start Button to the lastWinner or the player has min Score or the player minPostion in Game
			Player lastWinner = StaticEvent.FindLastWinner(this.Players);
			//lastWinner.Send(lastWinner, StaticEvent.TAKE_START_BUTTON_SERVER_to_CLIENT);
			if (lastWinner != null && this.Players.Count > 1)
			{
				lastWinner.Send(lastWinner, StaticEvent.TAKE_START_BUTTON_SERVER_to_CLIENT);
			}
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
			if (this.status == Stage.Playing) {
				// End game, left player pay a penanty to other in Game
				// the declare a winner in State.Finallizing
				return;
			}
			Players.Remove(pl);
			// SendEvent LEFT_ROOM to all players in Room
			Players.ForEach(p =>
			{
				// 1. Send information of new player to everyone in room
				p.Send(pl, StaticEvent.PLAYER_LEFT_ROOM_SERVER_to_CLIENT);
			});
			// Send Start Button to the lastWinner or the player has min Score or the player minPostion in Game
			Player lastWinner = StaticEvent.FindLastWinner(this.Players);
			if (lastWinner != null && this.Players.Count > 1) {
				lastWinner.Send(lastWinner, StaticEvent.TAKE_START_BUTTON_SERVER_to_CLIENT);
			}
			
		}
		// Send start game signal to all of players
		public void StartGame() {
			List<int> shuffleCards = StaticEvent.CardShuffle();

			// clear all cards remain in each player
			Players.ForEach(p => p.Cards = new List<int>());
			//1. Distribute cards to each player in room. Only one have winner flag
			Player winner = Players.FirstOrDefault(p => p.isWinner == true);
			winner.Cards.Add(shuffleCards[0]);
			shuffleCards.Remove(shuffleCards[0]);
			winner.Status = Player.Stage.Placing;

			
			// Distribute card to each other
			for (var i = 0; i< 8; i++)
			{
				Players.ForEach(p => {
					p.Cards.Add(shuffleCards[0]);
					shuffleCards.Remove(shuffleCards[0]);
				});
			}
			this.RemainCards = shuffleCards;

			#region check
			// using for check during development
			string json = JsonConvert.SerializeObject(RemainCards);
			Console.WriteLine("Remain Cards {0} and total count {1}",json, shuffleCards.Count);
			json = string.Join(",", winner.Cards.ToArray());
			Console.WriteLine("Card on Winner: {0}", json);
			#endregion

			Players.ForEach(p =>
			{
				p.Send(p, StaticEvent.START_NEW_GAME_SERVER_to_CLIENT, json = string.Join(",", p.Cards.ToArray()));
			});
		}
			
		public void PlacingCard(Player pl, int cardVal)
		{
			// 1. Check the card is valid then remove this card from player.Cards <List>
			if (!pl.Cards.Contains(cardVal))
			{
				return;
			}
			pl.Cards.Remove(cardVal);
			// 2. Broadcasd the card was placed to all players
			Players.ForEach(p => {
				p.Send(pl, StaticEvent.PLACING_CARD_SERVER_to_CLIENT, cardVal.ToString());
			});
			// 3. Change Status of next Player
			Player nextPlayer = StaticEvent.FindNextPlayer(pl, Players);
			pl.Status = Player.Stage.Idle;
			nextPlayer.Status = Player.Stage.Considering;
		}

	}
}
