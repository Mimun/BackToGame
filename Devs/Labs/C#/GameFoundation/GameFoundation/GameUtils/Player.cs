using System;
using System.Collections.Generic;
using System.Text;
using Fleck;

namespace GameFoundation.GameUtils
{
    class Player : IDisposable
    {

        public string playerName;
        public string playerJwt;
        public string playerRoomName;
        public string playerRoomId;
        public string avatarUrl;
        public IWebSocketConnection playerWebsocket;


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
        public Player( string playerName,
            string playerJwt,
            string playerRoomName,
            string playerRoomId,
            string avatarUrl,
            IWebSocketConnection playerWebsocket )
        {
            this.playerName = playerName;
            this.playerJwt = playerJwt;
            this.playerRoomName = playerRoomName;
            this.playerRoomId = playerRoomId;
            this.avatarUrl = avatarUrl;
            this.playerWebsocket = playerWebsocket;
        }
        #endregion

    }
}
