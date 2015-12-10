﻿using System;
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
            {
                input[x] = new Neuron(x, 0);
            }

            hidden = new Neuron[hiddenCount];
            for (int x = 0; x < hidden.Length; x++)
            {
                hidden[x] = new Neuron(x, 1);
                hidden[x].index = x;
                hidden[x].layer = 1;
            }

            output = new Neuron[outputCount];
            for (int x = 0; x < output.Length; x++)
            {
                output[x] = new Neuron(x, 2);
                output[x].index = x;
                output[x].layer = 2;
            }
        }

        public void SendInput(int[] inputNew)
        {
            for (int x = 0; x < inputNew.Length; x++)
                input[x].value = inputNew[x];
        }

        public Neuron MutateNeuron(Neuron neuron, Neuron[] neuronList, int index, int layer)
        {
            Neuron newNeuron = new Neuron(index, layer);
            int randomInt = random.Next(2);

            while (newNeuron.connections.Count == 0)
            {
                for (int x = 0; x < neuronList.Length; x++)
                {
                    if (randomInt == 1)
                    {
                        newNeuron.connections.Add(neuronList[x]);
                        neuronList[x].parentConnectors += 1;
                    }

                    randomInt = random.Next(2);
                }
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
                    {
                        input[x].connections.Add(hidden[y]);
                        hidden[y].parentConnectors += 1;
                    }

                    randomInt = random.Next(2);
                }

                if (input[x].connections.Count == 0) x -= 1;
            }

            for (int x = 0; x < hidden.Length; x++)
            {
                for (int y = 0; y < output.Length; y++)
                {
                    if (randomInt == 1)
                    {
                        hidden[x].connections.Add(output[y]);
                        output[y].parentConnectors += 1;
                    }

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
        public int index;
        public int layer;

        public Neuron(int indexArg, int layerArg)
        {
            index = indexArg;
            layer = layerArg;
        }

        public int value = 0;
        public int parentConnectors = 0;
        public List<Neuron> connections = new List<Neuron>();
    }
}
