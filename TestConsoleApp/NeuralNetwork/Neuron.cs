using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp.NeuralNetwork
{
    public class Neuron
    {

        public Neuron() { }
        
        public Neuron(Dictionary<double, Neuron> leyer)
        {
            weights = new List<Weight>();
            foreach (var input in leyer)
            {
                weights.Add(new Weight { Value = input.Key, Input = input.Value });
            }
        }

        public double error;                      // Сума от грешките на предходните неврони
        private double input;                      // Входна сума
        private double learnRate = 0.5;           // Убочаваща скорост
        private double output = double.MinValue;   // Изхудна стоиност от неврона
        public List<Weight> weights;              // Колекция от входните тегла

        /// <summary>
        /// Изход от неврона
        /// </summary>
        public double Output
        {
            get
            {
                if (output != double.MinValue)
                {
                    return output;
                }
                return 1.0 / (1.0 + Math.Exp(-input));
            }
            set
            {
                output = value;
            }
        }

        /// <summary>
        /// Първа производна на актвационата функция
        /// </summary>
        private double Derivative
        {
            get
            {
                double activation = Output;
                return activation * (1 - activation);
            }
        }

        /// <summary>
        /// Активиране на неврона
        /// </summary>
        public void Activate()
        {
            error = 0;
            input = 0;
            foreach (Weight w in weights)
            {
                input += w.Value * w.Input.Output;
            }
        }

        /// <summary>
        /// Предаване на грешка във вътрешност
        /// </summary>
        /// <param name="delta">Грешката от предходния неврон</param>
        public void CollectError(double delta)
        {
            if (weights != null)
            {
                error += delta;
                foreach (Weight w in weights)
                {
                    w.Input.CollectError(error * Derivative * w.Value);
                }
            }
        }

        /// <summary>
        /// Коригиране на теглата 
        /// </summary>
        public void AdjustWeights()
        {
            for (int i = 0; i < weights.Count; i++)
            {
                weights[i].Value += learnRate * error * Derivative * weights[i].Input.Output;
            }
        }
    }
}
