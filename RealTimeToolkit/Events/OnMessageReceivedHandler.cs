namespace RealTimeToolkit.Events
{
    using System;

    ///<summary>
    ///Събитие, когато е получено съобщение
    ///</summary>
    public class OnMessageReceivedHandler : EventArgs
    {
        ///<summary>
        ///Клиентът, който изпраща съобщението към сървъра
        ///</summary>
        private Client client;

        ///<summary>
        ///Извличане на клиента, който е изпратил полученото съобщение към сървъра
        ///</summary>
        ///<returns>
        ///Клиентът, изпраща съобщението
        ///</returns>
        public Client Client => client;

        ///<summary>
        ///Съобщението, изпратено от клиента към сървъра
        ///</summary>
        private string message;

        ///<summary>
        ///Извличане на съобщението, изпратено от клиента към сървъра
        ///</summary>
        ///<returns>
        ///Съобщението, изпратено от клиента
        ///</returns>
        public string Message => message;

        ///<summary>
        ///Създаване на ново обект за обработка на събития, когато клиент е изпратено въобщение към сървъра
        ///</summary>
        ///<param name="client">
        ///Клиентът, който е изпратил съобщението
        ///</param>
        ///<param name="message">
        ///Съобщението, изпратено от клиента
        ///</param>
        public OnMessageReceivedHandler(Client client, string message)
        {
            this.client = client;
            this.message = message;
        }
    }
}
