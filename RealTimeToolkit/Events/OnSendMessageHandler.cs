namespace RealTimeToolkit.Events
{
    using System;

    ///<summary>
    ///Събитие, когато съобщението е било изпратено на клиент
    ///</summary>
    public class OnSendMessageHandler : EventArgs
    {
        ///<summary>
        ///Клиентът, на когото е изпратено съобщението от сървъра
        ///</summary>
        private Client client;

        ///<summary>
        ///Извличане на клиента на когото е изпратено съобщението от сървъра
        ///</summary>
        ///<returns>
        ///Клиентът, на когото е изпратено съобщението
        ///</returns>
        public Client Client => client;

        ///<summary>
        ///Съобщението, изпратено до клиента
        ///</summary>
        private string message;

        ///<summary>
        ///Извича съобщението, изпратено до клиента
        ///</summary>
        ///<returns>
        ///Изпратеното съобщение
        ///</returns>
        public string Message => message;

        ///<summary>
        ///Създаване на ново обект за обработка на събития, когато е изпратено съобщение
        ///</summary>
        ///<param name="client">
        ///Клиентът, на когото е изпратено съобщението
        ///</param>
        ///<param name="message">
        ///Съобщението, изпратено до клиента
        ///</param>
        public OnSendMessageHandler(Client client, string message)
        {
            this.client = client;
            this.message = message;
        }
    }
}
