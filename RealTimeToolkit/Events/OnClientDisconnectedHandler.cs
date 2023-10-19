namespace RealTimeToolkit.Events
{
    using System;

    ///<summary>
    ///Събитие, когато клиент напусне
    ///</summary>
    public class OnClientDisconnectedHandler : EventArgs
    {
        ///<summary>
        ///Клиентът, който е прекъснал връзката със сървъра
        ///</summary>
        private Client client;

        ///<summary>
        ///Извличане на клиента, който е бил прекъснал връзката със сървъра
        ///</summary>
        ///<returns>
        ///Клиентът прекъснал връзката
        ///</returns>
        public Client Client => client;

        ///<summary>
        ///Създаване на ново обект за обработка на събития, когато клиент е прекъснал връзката със съвъра
        ///</summary>
        ///<param name="Client">
        ///Клиентът прекъснал връзката
        ///</param>
        public OnClientDisconnectedHandler(Client client)
        {
            this.client = client;
        }
    }
}
