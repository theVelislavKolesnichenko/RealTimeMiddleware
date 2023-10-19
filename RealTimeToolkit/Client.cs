namespace RealTimeToolkit
{
    using Common;
    using System;
    using System.Net.Sockets;

    ///<summary>
    /// Обект на свързаните клиенти
    /// </summary>
    public partial class Client
    {

        #region Fields

        ///<summary>Сокет на свързания клиент</summary>
        private Socket socket;

        ///<summary>Извлича сокета на свързания клиент</summary>
        ///<returns>Сокета на клиента</return>
        public Socket Socket => socket;

        ///<summary>Уникален идентификатор на клиента</summary>
        private string guid;

        /// <summary>Извлича уникалния идентификатор на клиента</summary>
        /// <returns>Уникален идентификатор на клиента</returns>
        public string GuId => guid;

        /// <summary>Сървър към които е свързан клиента</summary>
        private Server server;

        /// <summary>Извлича сървъра към който е свързан клиента</summary>
        /// <returns>Сървъра към които е свързън клиента</returns>
        public Server Server => server;


        /// <summary>If the server has sent a ping to the client and is waiting for a pong</summary>
        private bool bIsWaitingForPong;
        public bool IsWaitingForPong
        {
            /// <summary>Gets if the server is waiting for a pong response</summary>
            /// <returns>If the server is waiting for a pong response</returns>
            get { return bIsWaitingForPong; }

            /// <summary>Sets if the server is waiting for a pong response</summary>
            /// <param name="bIsWaitingForPong">If the server is waiting for a pong response</param>
            set { this.bIsWaitingForPong = value; }
        }

        #endregion

        /// <summary>Създава обект за свързан клиент</summary>
        /// <param name="server">Сървър към които е свързан клиента</param>
        /// <param name="socket">Сокета на клиента</param>
        public Client(Server server, Socket socket)
        {
            this.server = server;
            this.socket = socket;
            this.guid = Helpers.CreateGuid("client");

            socket.BeginReceive(new byte[] { 0 }, 0, 0, SocketFlags.None, messageCallback, null);
        }

        #region Methods

        /// <summary>Метод за обратно извикване при изпращане на съобщение</summary>
        private void messageCallback(IAsyncResult AsyncResult)
        {
            try
            {
                Socket.EndReceive(AsyncResult);

                // Read the incomming message 
                byte[] messageBuffer = new byte[255];
                int bytesReceived = Socket.Receive(messageBuffer);

                // Resize the byte array to remove whitespaces 
                //if (bytesReceived < messageBuffer.Length) Array.Resize<byte>(ref messageBuffer, bytesReceived);

                // Get the opcode of the frame
                OpcodeType opcode = Helpers.GetFrameOpcode(messageBuffer);

                // If the connection was closed
                if (opcode == OpcodeType.ClosedConnection)
                {
                    Server.ClientDisconnect(this);
                    return;
                }

                // Pass the message to the server event to handle the logic
                //string result = System.Text.Encoding.UTF8.GetString(messageBuffer);

                Server.ReceiveMessage(this, Helpers.GetDataFromFrame(messageBuffer)); //result);//

                // Start to receive messages again
                Socket.BeginReceive(new byte[] { 0 }, 0, 0, SocketFlags.None, messageCallback, null);

            }
            catch (Exception)
            {
                Socket.Close();
                Socket.Dispose();
                Server.ClientDisconnect(this);
            }
        }

        #endregion
    }
}
