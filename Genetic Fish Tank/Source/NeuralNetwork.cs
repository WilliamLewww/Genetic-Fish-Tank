using System;
using System.Collections.Generic;

namespace Genetic_Fish_Tank.Source
{
    class NeuralNetwork
    {
        static Random random = new Random();
        Neuron[] input, hidden, output;

        bool established = false;

        public NeuralNetwork(int inputCount, int hiddenCount, int outputCount)
        {
            input = new Neuron[inputCount];
            for (int x = 0; x < input.Length; x++)
            {
                input[x] = new Neuron();
                input[x].layer = 0;
            }

            hidden = new Neuron[hiddenCount];
            for (int x = 0; x < hidden.Length; x++)
            {
                hidden[x] = new Neuron();
                hidden[x].layer = 1;
            }

            output = new Neuron[outputCount];
            for (int x = 0; x < output.Length; x++)
            {
                output[x] = new Neuron();
                output[x].layer = 2;
            }
        }

        public void SendInput(int[] inputNew)
        {
            for (int x = 0; x < inputNew.Length; x++)
                input[x].value = inputNew[x];
        }

        public void EstablishRandomNetwork()
        {
            if (established == false)
            {
                int randomInt = random.Next(2);

                for (int x = 0; x < input.Length; x++)
                {
                    for (int y = 0; y < hidden.Length; y++)
                    {
                        if (randomInt == 1)
                            input[x].connections.Add(y);

                        randomInt = random.Next(2);
                    }

                    if (input[x].connections.Count == 0) x -= 1;
                }

                for (int x = 0; x < hidden.Length; x++)
                {
                    for (int y = 0; y < output.Length; y++)
                    {
                        if (randomInt == 1)
                            hidden[x].connections.Add(y);

                        randomInt = random.Next(2);
                    }

                    if (hidden[x].connections.Count == 0) x -= 1;
                }
            }

            established = true;
        }

        public int[] GetOutput()
        {
            int[] outputValue = new int[output.Length];

            for (int x = 0; x < output.Length; x++)
                outputValue[x] = output[x].value;

            return outputValue;
        }
    }

    class Neuron
    {
        public bool dominant = false;
        public int value = 0;
        public int layer = -1;
        public List<int> connections = new List<int>();
    }
}
