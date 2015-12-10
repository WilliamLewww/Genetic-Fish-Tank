using System;
using System.Collections.Generic;

namespace Genetic_Fish_Tank.Source
{
    class NeuralNetwork
    {
        static Random random = new Random();
        public Neuron[] input, hidden, output;

        public NeuralNetwork(int inputCount, int hiddenCount, int outputCount)
        {
            input = new Neuron[inputCount];
            for (int x = 0; x < input.Length; x++)
                input[x] = new Neuron();

            hidden = new Neuron[hiddenCount];
            for (int x = 0; x < hidden.Length; x++)
                hidden[x] = new Neuron();

            output = new Neuron[outputCount];
            for (int x = 0; x < output.Length; x++)
                output[x] = new Neuron();
        }

        public void SendInput(int[] inputNew)
        {
            for (int x = 0; x < inputNew.Length; x++)
                input[x].value = inputNew[x];
        }

        public Neuron MutateNeuron(Neuron neuron, Neuron[] neuronList)
        {
            Neuron newNeuron = new Neuron();
            int randomInt = random.Next(2);

            for (int x = 0; x < neuronList.Length; x++)
            {
                if (randomInt == 1)
                    newNeuron.connections.Add(neuronList[x]);

                randomInt = random.Next(2);

                if (newNeuron.connections.Count == 0) x -= 1;
            }

            return newNeuron;
        }

        public void EstablishExistingNetwork(Neuron[] inputArgs, Neuron[] hiddenArgs, Neuron[] outputArgs)
        {
            input = inputArgs;
            hidden = hiddenArgs;
            output = outputArgs;
        }

        public void EstablishRandomNetwork()
        {
            int randomInt = random.Next(2);

            for (int x = 0; x < input.Length; x++)
            {
                for (int y = 0; y < hidden.Length; y++)
                {
                    if (randomInt == 1)
                        input[x].connections.Add(hidden[y]);

                    randomInt = random.Next(2);
                }

                if (input[x].connections.Count == 0) x -= 1;
            }

            for (int x = 0; x < hidden.Length; x++)
            {
                for (int y = 0; y < output.Length; y++)
                {
                    if (randomInt == 1)
                        hidden[x].connections.Add(output[y]);

                    randomInt = random.Next(2);
                }

                if (hidden[x].connections.Count == 0) x -= 1;
            }
        }

        public void UpdateNetwork()
        {
            foreach (Neuron neuron in input)
                if (neuron.value == 1)
                    foreach (Neuron neuronB in neuron.connections)
                        neuronB.value = 1;

            foreach (Neuron neuron in hidden)
                if (neuron.value == 1)
                    foreach (Neuron neuronB in neuron.connections)
                        neuronB.value = 1;
        }

        public Neuron[] GetHidden()
        {
            return hidden;
        }

        public Neuron[] GetOutput()
        {
            return output;
        }
    }

    class Neuron
    {
        public int value = 0;
        public List<Neuron> connections = new List<Neuron>();
    }
}
