using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using Fleck;
using System.Dynamic;
using Newtonsoft.Json;
using System.Linq;

namespace GameFoundation.GameUtils
{
	public class Player : IDisposable
	{
		public string playerName;
		public string playerUID;
		public string playerJwt;
		public float playerBalance;
		public string playerRoomName;
		public string playerRoomId;
		public string avatarUrl;
		public Room playerRoom;

		public List<IWebSocketConnection> playerWebsocketList = new List<IWebSocketConnection>();

		public ConcurrentDictionary<string, List<IWebSocketConnection>> Connections { get; set; } = new ConcurrentDictionary<string, List<IWebSocketConnection>>();

		public List<IWebSocketConnection> AllConnections
		{
			get
			{
				var connections = new List<IWebSocketConnection>();
				foreach (var kvp in this.Connections)
					connections.AddRange(kvp.Value);
				return connections;
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~Player() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion


		#region GameRule
		//public Player( string playerName,
		//    string playerJwt,
		//    string playerRoomName,
		//    string playerRoomId,
		//    string avatarUrl,
		//    IWebSocketConnection playerWebsocket )
		//{
		//    this.playerName = playerName;
		//    this.playerJwt = playerJwt;
		//    this.playerRoomName = playerRoomName;
		//    this.playerRoomId = playerRoomId;
		//    this.avatarUrl = avatarUrl;
		//    this.playerWebsocketList.Add(playerWebsocket);
		//}
		#endregion

		public void Send(Player pl, String msgEvent)
		{
			dynamic expando = new ExpandoObject();
			expando.avatarUrl = pl.avatarUrl;
			expando.playerName = pl.playerName;
			expando.playerRoomId = pl.playerRoomId;
			expando.playerRoomName = pl.playerRoomName;
			expando.playerUID = pl.playerUID;
			expando.playerBalance = pl.playerBalance;
			expando.msgEvent = msgEvent;

			this.playerWebsocketList.ForEach(s =>
			{
				//Console.WriteLine("s: " + s.ToString() + " : "+JsonConvert.SerializeObject(expando));
				s.Send(JsonConvert.SerializeObject(expando));
			});
		}



	}
}
