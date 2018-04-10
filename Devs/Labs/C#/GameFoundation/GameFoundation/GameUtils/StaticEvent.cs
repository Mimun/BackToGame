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
        //public static string CREATE_NEW_ROOM_SERVER_to_CLIENT = "CREATE_NEW_ROOM_SERVER_to_CLIENT";
        //public static string CREATE_NEW_ROOM_CLIENT_to_SERVER = "CREATE_NEW_ROOM_CLIENT_to_SERVER";


        public static List<Room> roomList = new List<Room>();

        public static void CreateOrJoin_Handler(Player pl, IWebSocketConnection socket)
        {
            // Join And Create new Room
            #region Join And Create new Room

            // assign websocket for each player to reusage in next steps
            //pl.playerWebsocket = socket;
            // Do something here
            for (int i = 0; i <= StaticEvent.roomList.Count; i++)
            {
                if (StaticEvent.roomList.Count > 0 && StaticEvent.roomList[i].RoomId == pl.playerRoomId)
                {
                    // Check if player is existed in this room (change playerName to uniqueID in next step of development)
                    Player exPl = null;
                    bool pl_existed = roomList[i].playerLists.Any(x => {
                        exPl = x;
                        return x.playerName == pl.playerName;
                    });
                    if (!pl_existed) {
                        pl.playerWebsocketList.Add(socket);
                        StaticEvent.roomList[i].addPlayer(pl);
                    }
                    else
                    {
                        exPl.playerWebsocketList.Add(socket);
                    }
                    
                    break;
                }
                else
                {
                    pl.playerWebsocketList.Add(socket);
                    StaticEvent.roomList.Add(new Room(pl));
                    break;
                }                
                
            }
            //roomList.ForEach(x => {
            //    Console.WriteLine("List of Room " + x.RoomName);
            //});
            #endregion

        }
    }
}
