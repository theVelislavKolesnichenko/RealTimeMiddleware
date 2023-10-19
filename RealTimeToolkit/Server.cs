namespace RealTimeToolkit
{
    using Events;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    ///<summary>
    /// Обект за създаване на сървър
    ///</summary>
    public partial class Server
    {
        /// <summary>Сокет на сървъра</summary>
        private Socket socket;

        /// <summary>Извлича сокета на сървъра</summary>
        /// <returns>сокет на сървъра</returns>
        public Socket Socket => socket;

        /// <summary>Крайната точка на сървъра</summary>
        private IPEndPoint endPoint;

        /// <summary>Извлича крайната точка на сървъра</summary>
        /// <returns>Крайната точка на сървъра</returns>
        public IPEndPoint EndPoint => endPoint;

        /// <summary>Списък със свързаните клиенти към сървъра</summary>
        private List<Client> clients;

        #region Clients Getters

        /// <summary>Извлича свързан към сървъра клиент по номер</summary>
        /// <param name="index">Номер на свързания клиент</param>
        /// <returns>Свързан клиент с този номер или нищо ако не съществува</returns>
        public Client GetConnectedClient(int index)
        {
            if (index < 0 || index >= clients.Count) return null;
            return clients[index];
        }

        /// <summary>Извлича свързан към сървъра клиент по идентификатор</summary>
        /// <param name="guid">дентификатор на свързания клиент</param>
        /// <returns>Свързан клиент с този идентификатор или нищо ако не съществува</returns>
        public Client GetConnectedClient(string guid)
        {
            foreach (Client client in clients)
            {
                if (client.GuId == guid) return client;
            }
            return null;
        }

        /// <summary>Извлича свързан клиент по дадения сокет</summary>
        /// <param name="Socket">Сокет на клиента </param>
        /// <returns>Свързан клиент с този сокет или нищо ако не съществува</returns>
        public Client GetConnectedClient(Socket Socket)
        {
            foreach (Client client in clients)
            {
                if (client.Socket == Socket) return client;
            }
            return null;
        }

        /// <summary>Извлича броя на свързаните клиенти със сървъра</summary>
        /// <returns>Броят на свързаните клиенти със сървъра</returns>
        public int ConnectedClientCount => clients.Count;

        #endregion

        /// <summary>Създаване на обект и стартиране на нов сървър в крайна точка</summary>
        /// <param name="еndPoint">райната точка на сървъра</param>
        public Server(IPEndPoint еndPoint)
        {
            //Присвоява краина точка при валидн входен параметър
            if (еndPoint == null) return;
            this.endPoint = еndPoint;

            //Създаване на нов сокет за клиента
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            clients = new List<Client>();

            Console.WriteLine("WebSocket Server Started\nListening on {0}:{1}\n", EndPoint.Address.ToString(), EndPoint.Port);

            //Стартиране на сървъра
            start();
        }

        #region Methods

        ///<summary>
        ///Стартира сървъра, след като е създаден сървърен обект
        ///</summary>
        private void start()
        {
            //Свързване на краината точка
            Socket.Bind(EndPoint);
            Socket.Listen(0);

            //Задаване на метод за обратно извиване при свързване на клиент
            Socket.BeginAccept(connectionCallback, null);
        }

        /// <summary>
        /// Спиране на сървъра
        /// </summary>
        public void Stop()
        {
            Socket.Close();
            Socket.Dispose();
        }

        /// <summary>Метод за обратно извикване когато се свързва клиент към сървъра</summary>
        /// <param name="аsyncResult">Състояние на асинхронна операция</param>
        private void connectionCallback(IAsyncResult аsyncResult)
        {
            try
            {
                // Приема сокета на клиента които иска да се свържи към сървъра
                Socket clientSocket = Socket.EndAccept(аsyncResult);

                // Прочита заявката на клиента които иска да се свържи
                byte[] handshakeBuffer = new byte[1024];
                int handshakeReceived = clientSocket.Receive(handshakeBuffer);

                //Извличане на ключа на заявката за свързване към сървъра и образуване на одговора
                string requestKey = Helpers.GetHandshakeRequestKey(Encoding.Default.GetString(handshakeBuffer));
                string hanshakeResponse = Helpers.GetHandshakeResponse(Helpers.HashKey(requestKey));

                // Изпращане на одговор за свързване на клиента който иска да се свържи
                clientSocket.Send(Encoding.Default.GetBytes(hanshakeResponse));

                // Създаване на нов обект за клиент
                Client client = new Client(this, clientSocket);
                // Добавяне на новия клиент в списъка с клиенти
                clients.Add(client);

                // Извикване на събитието за свързване при успешно свързване на клиент към сървъра 
                if (OnClientConnected == null) throw new Exception("Server error: event OnClientConnected is not bound!");
                OnClientConnected(this, new OnClientConnectedHandler(client));

                // Задаване на метод за обратно извиване при свързване на клиент
                Socket.BeginAccept(connectionCallback, null);

            }
            catch (Exception Exception)
            {
                string error = string.Format("An error has occured while trying to accept a connecting client.\n\n{0}", Exception.Message);
                Console.WriteLine(error);
                //throw new Exception(error);
            }
        }

        /// <summary>Когато се получи съобщение от клиент се извиква събитието за обработка</summary>
        /// <param name="client">Клиента изпратил съобщението</param>
        /// <param name="message">Съобщението изпратено от клиента</param>
        public void ReceiveMessage(Client client, string message)
        {
            if (OnMessageReceived == null) throw new Exception("Server error: event OnMessageReceived is not bound!");
            OnMessageReceived(this, new OnMessageReceivedHandler(client, message));
        }

        /// <summary>Когато слиента прекъсне връзката се изпраща съобщение за напускане</summary>
        /// <param name="client">Клиента които напуска</param>
        public void ClientDisconnect(Client client)
        {
            clients.Remove(client);
            
            if (OnClientDisconnected == null) throw new Exception("Server error: OnClientDisconnected is not bound!");
            OnClientDisconnected(this, new OnClientDisconnectedHandler(client));
        }

        #endregion

        #region Server Events

        /// <summary>Изпращане на съобщение до клиент</summary>
        /// <param name="client">Клиент до които се изпращат данни</param>
        /// <param name="data">Данните които се изпращат</param>
        public void SendMessage(Client client, string data)
        {
            // Създаване на пакет и изпращане на съобщението
            byte[] frameMessage = Helpers.GetFrameFromString(data);

            client.Socket.Send(frameMessage);

            if (OnSendMessage == null) throw new Exception("Server error: event OnSendMessage is not bound!");
            OnSendMessage(this, new OnSendMessageHandler(client, data));
        }

        /// <summary>Събитие за изпращане на съобщение</summary>
        public event EventHandler<OnSendMessageHandler> OnSendMessage;

        /// <summary>Събитие за свързване на клиент</summary>
        public event EventHandler<OnClientConnectedHandler> OnClientConnected;

        /// <summary>Събитие за получаване на съобщение</summary>
        public event EventHandler<OnMessageReceivedHandler> OnMessageReceived;

        /// <summary>Събитие за прекратяване на връзка</summary>
        public event EventHandler<OnClientDisconnectedHandler> OnClientDisconnected;

        #endregion
    }
}
