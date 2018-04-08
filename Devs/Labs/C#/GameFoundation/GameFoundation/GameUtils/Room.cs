using System;
using System.Collections.Generic;
using System.Text;

namespace GameFoundation.GameUtils
{    
    class Room
    {
        public string RoomName;
        public string RoomId;
        private List<Player> playerLists = new List<Player>();

        public Room(Player pl)
        {
            this.RoomId = pl.playerRoomId;
            this.RoomName = pl.playerRoomName;
            this.playerLists.Add(pl);
        }

        public void addPlayer(Player pl){
            playerLists.Add(pl);
            // SendEvent JOIN_ROOM to all players in Room
        }

        public void removePlayer(Player pl) {
            playerLists.Remove(pl);
            // SendEvent LEFT_ROOM to all players in Room
        }

    }
}
