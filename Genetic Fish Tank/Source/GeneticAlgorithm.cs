using System;
using System.Collections.Generic;

namespace Genetic_Fish_Tank.Source
{
    class GeneticAlgorithm
    {
        static Random random = new Random();

        public int generation = 0;
        public double generationTimer = 0;

        public bool GetGenerationState(Fish[] fishList)
        {
            if (generationTimer > 120f)
                return true;

            foreach (Fish fish in fishList)
                if (fish.collisionCircle.dead == false)
                    return false;

            return true;
        }

        public NeuralNetwork[] Offset(Fish[] fishList, int percent)
        {
            NeuralNetwork neuralNetworkBackground = new NeuralNetwork(0, 0, 0);
            List<NeuralNetwork> neuralNetwork = new List<NeuralNetwork>();

            int randomInt = random.Next(101);

            for (int x = 0; x < fishList.Length; x++)
            {
                List<Neuron> input = new List<Neuron>(), hidden = new List<Neuron>();

                foreach (Neuron neuron in fishList[x].brain.input)
                {
                    if (randomInt < percent)
                    {
                        input.Add(neuralNetworkBackground.OffsetNeuron(neuron, fishList[x].brain.hidden));
                    }
                    else
                        input.Add(neuron);

                    randomInt = random.Next(101);
                }

                foreach (Neuron neuron in fishList[x].brain.hidden)
                {
                    if (randomInt < percent)
                    {
                        hidden.Add(neuralNetworkBackground.OffsetNeuron(neuron, fishList[x].brain.output));
                    }
                    else
                        hidden.Add(neuron);

                    randomInt = random.Next(101);
                }

                neuralNetwork.Add(new NeuralNetwork(input.Count, hidden.Count, fishList[x].brain.output.Length));
                neuralNetwork[x].EstablishExistingNetwork(input.ToArray(), hidden.ToArray(), fishList[x].brain.output);
            }

            return neuralNetwork.ToArray();
        }

        public NeuralNetwork[] CrossMutate(Fish[] fishList, int total)
        {
            List<NeuralNetwork> neuralNetwork = new List<NeuralNetwork>();
            int randomInt = random.Next(fishList.Length);

            for (int x = 0; x < total; x++)
            {
                List<Neuron> input = new List<Neuron>(), hidden = new List<Neuron>(), output = new List<Neuron>();

                for (int y = 0; y < fishList[0].brain.input.Length; y++)
                {
                    input.Add(fishList[randomInt].brain.input[y]);
                    randomInt = random.Next(fishList.Length);
                }
                for (int y = 0; y < fishList[0].brain.hidden.Length; y++)
                {
                    hidden.Add(fishList[randomInt].brain.hidden[y]);
                    randomInt = random.Next(fishList.Length);
                }
                for (int y = 0; y < fishList[0].brain.output.Length; y++)
                {
                    output.Add(fishList[randomInt].brain.output[y]);
                    randomInt = random.Next(fishList.Length);
                }

                neuralNetwork.Add(new NeuralNetwork(fishList[0].brain.input.Length, fishList[0].brain.hidden.Length, fishList[0].brain.output.Length));
                neuralNetwork[x].EstablishExistingNetwork(input.ToArray(), hidden.ToArray(), output.ToArray());
            }

            return neuralNetwork.ToArray();
        }

        public Fish[] ExterminatePopulation(Fish[] fishList, int percent)
        {
            List<Fish> newFishList = new List<Fish>();
            foreach (Fish fish in fishList)
                newFishList.Add(fish);

            int amountRemoved = 0;
            int randomInt = random.Next(101);

            while ((amountRemoved / fishList.Length) * 100 < percent)
            {
                for (int x = 0; x < fishList.Length; x++)
                {
                    if ((x / fishList.Length) * 100 >= randomInt)
                        newFishList.Remove(newFishList[x]);

                    amountRemoved += 1;

                    if ((amountRemoved / fishList.Length) * 100 >= percent) return newFishList.ToArray();

                    randomInt = random.Next(101);
                }
            }

            return newFishList.ToArray();
        }

        public Fish[] RankFittest(Fish[] fishList)
        {
            Fish[] newFishList = fishList;
            Fish temp;

            for (int x = 0; x < fishList.Length; x++)
            {
                for (int y = 1; y < fishList.Length - x; y++)
                {
                    if (newFishList[y - 1].collisionCircle.Score < newFishList[y].collisionCircle.Score)
                    {
                        temp = newFishList[y - 1];
                        newFishList[y - 1] = newFishList[y];
                        newFishList[y] = temp;
                    }
                }
            }

            return newFishList;
        }
    }
}
