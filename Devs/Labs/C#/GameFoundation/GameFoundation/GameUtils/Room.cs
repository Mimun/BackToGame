using System;
using System.Collections.Generic;
using System.Text;

namespace GameFoundation.GameUtils
{    
    public class Room
    {
        public string RoomName;
        public string RoomId;
        public List<Player> playerLists = new List<Player>();

        public Room(Player pl)
        {
            this.RoomId = pl.playerRoomId;
            this.RoomName = pl.playerRoomName;
            this.addPlayer(pl);
        }

        public void addPlayer(Player pl){
            playerLists.Add(pl);
            // SendEvent JOIN_ROOM to all players in Room
            // Actually, we need update all of players in Room about other
            playerLists.ForEach(q => {
                playerLists.ForEach(p =>                {
                    Console.WriteLine("from player: " + p.playerName + " about " + q.playerName);
                    p.Send(q, StaticEvent.JOIN_OR_CREATE_ROOM_SERVER_to_CLIENT);
                });
            });

            
        }

        public void removePlayer(Player pl) {
            playerLists.Remove(pl);
            // SendEvent LEFT_ROOM to all players in Room
        }

    }
}
