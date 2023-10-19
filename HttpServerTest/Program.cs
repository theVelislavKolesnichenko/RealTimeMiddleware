using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace HttpServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServiceHost _serviceHost = new WebServiceHost(typeof(RestDemoServices), new Uri("http://localhost:9999/HTTPTest"));
            _serviceHost.Open();
            Console.ReadKey();
            _serviceHost.Close();

        }
    }

    [ServiceContract(Name = "HTTPTest")]
    public class RestDemoServices
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Client/{id}")]
        public string Set(string id)
        {
            int Idnum = Convert.ToInt32(id);
            string ReturnString = new string('x', Idnum);
            return ReturnString;
        }
    }
}
