using System;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using RealTimeToolkit;
using RealTimeToolkit.Events;
using StudentSystem;
using System.Web.Script.Serialization;
using TestConsoleApp.NeuralNetwork;
using System.Collections.Generic;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //NewMethod();

            Server();

            ServerStudent();

            Test();

            ServerFactory();

            NeuronNetwork();

            // Close the application only when the close button is clicked
            Process.GetCurrentProcess().WaitForExit();
        }

        private static void Server()
        {

            Server server = new Server(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8181));

            server.OnClientConnected += (object sender, OnClientConnectedHandler e) =>
            {
                Console.WriteLine("Client with GUID: {0} Connected!", e.Client.GuId);
            };

            server.OnClientDisconnected += (object sender, OnClientDisconnectedHandler e) =>
            {
                Console.WriteLine("Client {0} Disconnected", e.Client.GuId);
            };

            server.OnMessageReceived += (object sender, OnMessageReceivedHandler e) =>
            {
                Console.WriteLine("Received Message: '{1}' from client: {0}", e.Client.GuId, e.Message);

                while (true)
                {
                    Thread.Sleep(1000);
                    for (int c = 0; c < e.Client.Server.ConnectedClientCount; c++)
                    {
                        server.SendMessage(e.Client.Server.GetConnectedClient(c), DateTime.Now.ToString());
                    }
                }
            };

            server.OnSendMessage += (object sender, OnSendMessageHandler e) =>
            {
                Console.WriteLine("Sent message: '{0}' to client {1}", e.Message, e.Client.GuId);
            };
        }

        private static void ServerStudent()
        {
            IStudentManager manager = new StudentManager();

            Server server = new Server(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8182));

            server.OnClientConnected += (object sender, OnClientConnectedHandler e) =>
            {
                Console.WriteLine("Client with GUID: {0} Connected!", e.Client.GuId);
                
                server.SendMessage(e.Client, new JavaScriptSerializer().Serialize(manager.GetAllStudents()));
            };

            server.OnClientDisconnected += (object sender, OnClientDisconnectedHandler e) =>
            {
                Console.WriteLine("Client {0} Disconnected", e.Client.GuId);
            };

            server.OnMessageReceived += (object sender, OnMessageReceivedHandler e) =>
            {
                Student student = new JavaScriptSerializer().Deserialize<Student>(e.Message);

                if (student.Id == 0)
                {
                    bool state = manager.Add(student);
                }
                else
                {
                    bool state = manager.Save(student);
                }

                Console.WriteLine("Received Message: '{1}' from client: {0}", e.Client.GuId, e.Message);

                for (int c = 0; c < e.Client.Server.ConnectedClientCount; c++)
                {
                    server.SendMessage(e.Client.Server.GetConnectedClient(c), new JavaScriptSerializer().Serialize(manager.GetAllStudents()));
                }
            };

            server.OnSendMessage += (object sender, OnSendMessageHandler e) =>
            {
                Console.WriteLine("Sent message: '{0}' to client {1}", e.Message, e.Client.GuId);
            };
        }
        
        private static void Test()
        {

            Server server = new Server(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8183));

            server.OnClientConnected += (object sender, OnClientConnectedHandler e) =>
            {
                Console.WriteLine("Client with GUID: {0} Connected!", e.Client.GuId);
            };

            server.OnClientDisconnected += (object sender, OnClientDisconnectedHandler e) =>
            {
                Console.WriteLine("Client {0} Disconnected", e.Client.GuId);
            };

            server.OnMessageReceived += (object sender, OnMessageReceivedHandler e) =>
            {
                //Console.WriteLine("Received Message: '{1}' from client: {0}", e.Client.GuId, e.Message);
                for (int j = 0; j < 1000000; j++)
                {
                    string ReturnString = new string('x', 1000);
                    server.SendMessage(e.Client, ReturnString);
                }
            };

            server.OnSendMessage += (object sender, OnSendMessageHandler e) =>
            {
                //Console.WriteLine("Sent message: '{0}' to client {1}", e.Message, e.Client.GuId);
            };
        }

        public static Factory Factory = new Factory();

        private static void ServerFactory()
        {
            Server serverManagment = new Server(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8185));

            serverManagment.OnClientConnected += (object sender, OnClientConnectedHandler e) =>
            {
                serverManagment.SendMessage(e.Client, Factory.Sum.ToString());
                Console.WriteLine("Client with GUID: {0} Connected!", e.Client.GuId);
            };

            serverManagment.OnClientDisconnected += (object sender, OnClientDisconnectedHandler e) =>
            {
                Console.WriteLine("Client {0} Disconnected", e.Client.GuId);
            };

            serverManagment.OnMessageReceived += (object sender, OnMessageReceivedHandler e) =>
            {
                Console.WriteLine("Received Message: '{1}' from client: {0}", e.Client.GuId, e.Message);
            };

            serverManagment.OnSendMessage += (object sender, OnSendMessageHandler e) =>
            {
                Console.WriteLine("Sent message: '{0}' to client {1}", e.Message, e.Client.GuId);
            };

            Server serverFactory = new Server(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8184));

            serverFactory.OnClientConnected += (object sender, OnClientConnectedHandler e) =>
            {
                Console.WriteLine("Client with GUID: {0} Connected!", e.Client.GuId);
            };

            serverFactory.OnClientDisconnected += (object sender, OnClientDisconnectedHandler e) =>
            {
                Console.WriteLine("Client {0} Disconnected", e.Client.GuId);
            };

            serverFactory.OnMessageReceived += (object sender, OnMessageReceivedHandler e) =>
            {
                if (Factory.HasWorkshop)
                {
                    Workshop Workshop = Factory.GetByClient(e.Client.GuId);
                    Workshop.Value = Convert.ToInt32(e.Message);
                }
                else
                {
                    Factory.Add(e.Client.GuId, Convert.ToInt32(e.Message));
                }

                for (int c = 0; c < serverManagment.ConnectedClientCount; c++)
                {
                    serverManagment.SendMessage(serverManagment.GetConnectedClient(c), Factory.Sum.ToString());
                }

                Console.WriteLine("Received Message: '{1}' from client: {0}", e.Client.GuId, e.Message);
            };

            serverFactory.OnSendMessage += (object sender, OnSendMessageHandler e) =>
            {
                Console.WriteLine("Sent message: '{0}' to client {1}", e.Message, e.Client.GuId);
            };           
        }
        
        private static void NeuronNetwork()
        {
            Server server = new Server(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8186));
            //double speed = 10.1;
            Network network = new Network();

            network.Era = 0;
            network.Error = new double[] { 0, 0, 0, 0 };
            network.Input = new double[] { 0.05, 0.1 };
            network.Weigth = new double[] { 0.15, 0.2, 0.25, 0.3, 0.4, 0.45, 0.5, 0.55 };
            network.Output = new double[] { 0.1, 0.99 };
            
            server.OnClientConnected += (object sender, OnClientConnectedHandler e) =>
            {
                Console.WriteLine("Client with GUID: {0} Connected!", e.Client.GuId);
                server.SendMessage(e.Client, new JavaScriptSerializer().Serialize(network));
            };

            server.OnClientDisconnected += (object sender, OnClientDisconnectedHandler e) =>
            {
                Console.WriteLine("Client {0} Disconnected", e.Client.GuId);
            };

            server.OnMessageReceived += (object sender, OnMessageReceivedHandler e) =>
            {
                Network wetwork = new JavaScriptSerializer().Deserialize<Network>(e.Message);

                Dictionary<double, Neuron> leyer = new Dictionary<double, Neuron>();

                Neuron neuron0 = new Neuron();
                neuron0.Output = wetwork.Input[0];

                Neuron neuron1 = new Neuron();
                neuron1.Output = wetwork.Input[1];
                
                leyer.Add(wetwork.Weigth[0], neuron0);
                leyer.Add(wetwork.Weigth[1], neuron1);
                Neuron neuron2 = new Neuron(leyer);

                leyer.Clear();
                leyer.Add(wetwork.Weigth[2], neuron0);
                leyer.Add(wetwork.Weigth[3], neuron1);
                Neuron neuron3 = new Neuron(leyer);

                neuron2.Activate();
                neuron3.Activate();

                Console.WriteLine("Received Message: '{1}' from client: {0}", e.Client.GuId, e.Message);
                
                network.Input = new double[] { neuron2.Output, neuron3.Output };
                //network.Weigth = new double[] { 0.15, 0.2, 0.25, 0.3, 0.4, 0.45, 0.5, 0.55 };
                //network.Output = new double[] { 0.1, 0.99 };

                if (network.Era > 0)
                {
                    neuron2.CollectError(network.Error[2]);
                    neuron3.CollectError(network.Error[3]);

                    neuron2.AdjustWeights();
                    neuron3.AdjustWeights();

                    neuron2.Activate();
                    neuron3.Activate();

                    network.Input = new double[] { neuron2.Output, neuron3.Output };
                    //network.Error = new double[] { 0, 0, neuron2.Output - 0.1, neuron3.Output - 0.99 };
                    network.Error[0] = neuron2.error;
                    network.Error[1] = neuron3.error;
                    //network.Weigth = new double[] { 0.15, 0.2, 0.25, 0.3, neuron2.weights[0].Value, neuron2.weights[1].Value, neuron2.weights[2].Value, neuron2.weights[3].Value };
                    network.Weigth[0] = neuron2.weights[0].Value;
                    network.Weigth[1] = neuron2.weights[1].Value;
                    network.Weigth[2] = neuron3.weights[0].Value;
                    network.Weigth[3] = neuron3.weights[1].Value;
                }

                server.SendMessage(e.Client, new JavaScriptSerializer().Serialize(network));

            };

            server.OnSendMessage += (object sender, OnSendMessageHandler e) =>
            {
                Console.WriteLine("Sent message: '{0}' to client {1}", e.Message, e.Client.GuId);
            };

            Server server1 = new Server(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8187));

            server1.OnClientConnected += (object sender, OnClientConnectedHandler e) =>
            {
                Console.WriteLine("Client with GUID: {0} Connected!", e.Client.GuId);
            };

            server1.OnClientDisconnected += (object sender, OnClientDisconnectedHandler e) =>
            {
                Console.WriteLine("Client {0} Disconnected", e.Client.GuId);
            };

            server1.OnMessageReceived += (object sender, OnMessageReceivedHandler e) =>
            {
                network.Era++;
                Network wetwork = new JavaScriptSerializer().Deserialize<Network>(e.Message);

                Dictionary<double, Neuron> leyer = new Dictionary<double, Neuron>();

                Neuron neuron0 = new Neuron();
                neuron0.Output = wetwork.Input[0];

                Neuron neuron1 = new Neuron();
                neuron1.Output = wetwork.Input[1];

                leyer.Add(wetwork.Weigth[4], neuron0);
                leyer.Add(wetwork.Weigth[5], neuron1);
                Neuron neuron2 = new Neuron(leyer);

                leyer.Clear();
                leyer.Add(wetwork.Weigth[6], neuron0);
                leyer.Add(wetwork.Weigth[7], neuron1);
                Neuron neuron3 = new Neuron(leyer);

                neuron2.Activate();
                neuron3.Activate();

                neuron2.CollectError(neuron2.Output - 0.1);
                neuron3.CollectError(neuron3.Output - 0.99);

                neuron2.AdjustWeights();
                neuron3.AdjustWeights();

                Console.WriteLine("Received Message: '{1}' from client: {0}", e.Client.GuId, e.Message);

                network.Input = new double[] { 0.05, 0.1 };
                network.Error[2] = neuron2.error;
                network.Error[3] = neuron3.error;// = new double[] { network.Error[0], network.Error[1], neuron2.error, neuron3.error };
                network.Weigth[4] = neuron2.weights[0].Value;
                network.Weigth[5] = neuron2.weights[1].Value;
                network.Weigth[6] = neuron3.weights[0].Value;
                network.Weigth[7] = neuron3.weights[1].Value;//new double[] { 0.15, 0.2, 0.25, 0.3, neuron2.weights[0].Value, neuron2.weights[1].Value, neuron2.weights[2].Value, neuron2.weights[3].Value };
                //network.Output = new double[] { 0.1, 0.99 };
                server.SendMessage(e.Client, new JavaScriptSerializer().Serialize(network));
            };

            server1.OnSendMessage += (object sender, OnSendMessageHandler e) =>
            {
                Console.WriteLine("Sent message: '{0}' to client {1}", e.Message, e.Client.GuId);
            };
        }

        private static void NewMethod()
        {
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8181);

            server.Start();
            Console.WriteLine("Server has started on 127.0.0.1:80.{0}Waiting for a connection...", Environment.NewLine);

            TcpClient client = server.AcceptTcpClient();

            Console.WriteLine("A client connected.");

            NetworkStream stream = client.GetStream();

            //enter to an infinite cycle to be able to handle every change in stream
            while (true)
            {
                while (!stream.DataAvailable) ;

                Byte[] bytes = new Byte[client.Available];

                stream.Read(bytes, 0, bytes.Length);

                String data = Encoding.UTF8.GetString(bytes);

                if (new Regex("^GET").IsMatch(data))
                {
                    Byte[] response = Encoding.UTF8.GetBytes(
                        string.Format("HTTP/1.1 101 Switching Protocols\r\nConnection: Upgrade\r\nUpgrade: websocket\r\nSec-WebSocket-Accept: {0}\r\n\r\n",
                        Convert.ToBase64String(
                        SHA1.Create().ComputeHash(
                            Encoding.UTF8.GetBytes(
                                new Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                            )
                        )
                    )));

                    stream.Write(response, 0, response.Length);

                }
                else
                {
                    byte[] msg = System.Text.Encoding.UTF8.GetBytes(data);
                    stream.Write(msg, 0, msg.Length);
                }
            }
        }
    }
}
