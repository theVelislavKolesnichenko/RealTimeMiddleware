namespace RealTimeToolkit.Events
{
    using System;

    /// <summary>
    /// Събитие, когато е свързан клиент
    /// </summary>
    public class OnClientConnectedHandler : EventArgs
    {
        ///<summary>
        ///Клиентът, свързан със сървъра
        ///</summary>
        private Client client;

        ///<summary>
        ///Извличане на клиента, който е свързан със сървъра
        ///</summary>
        ///<returns>
        ///Клиентът, който е свързан със сършъра
        ///</returns>
        public Client Client => client;

        ///<summary>
        ///Създаване на ново обект за обработка на събития, когато клиент е свързан със съвъра
        ///</summary>
        ///<param name="client">
        ///Клиентът, който е свързан със сършъра
        ///</param>
        public OnClientConnectedHandler(Client client)
        {
            this.client = client;
        }
    }
}
