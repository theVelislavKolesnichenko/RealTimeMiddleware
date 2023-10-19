using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.NeuralNetwork
{
    public class Network
    {
        public int Era { get; set; }
        public double[] Error { get; set; }
        public double[] Input { get; set; }
        public double[] Weigth { get; set; }
        public double[] Output { get; set; }
    }
}
