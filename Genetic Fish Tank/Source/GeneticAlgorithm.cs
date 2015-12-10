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
            if (generationTimer > 30f)
            {
                generationTimer = 0f;
                return true;
            }

            foreach (Fish fish in fishList)
                if (fish.collisionCircle.dead == false)
                    return false;

            generationTimer = 0f;
            return true;
        }

        public NeuralNetwork[] Mutate(NeuralNetwork[] neuralNetworkArgs, int percentAmountMutate, int percentMutate, int percentHidden)
        {
            NeuralNetwork neuralNetworkBackground = new NeuralNetwork(0, 0, 0);
            List<NeuralNetwork> neuralNetwork = new List<NeuralNetwork>();

            int randomInt = random.Next(101);
            int counter = 0;

            for (int x = 0; x < neuralNetworkArgs.Length; x++)
            {
                List<Neuron> input = new List<Neuron>(), hidden = new List<Neuron>();

                if (randomInt < percentAmountMutate)
                {
                    foreach (Neuron neuron in neuralNetworkArgs[x].hidden)
                    {
                        if (randomInt < percentHidden)
                        {
                            hidden.Add(neuralNetworkBackground.MutateNeuron(neuron, neuralNetworkArgs[x].output, counter, 1));
                            counter += 1;
                        }

                        if (randomInt < percentMutate)
                            hidden.Add(neuralNetworkBackground.MutateNeuron(neuron, neuralNetworkArgs[x].output, counter, 1));
                        else
                            hidden.Add(neuron);

                        randomInt = random.Next(101);

                        counter += 1;
                    }

                    counter = 0;

                    foreach (Neuron neuron in neuralNetworkArgs[x].input)
                    {
                        if (randomInt < percentMutate)
                            input.Add(neuralNetworkBackground.MutateNeuron(neuron, neuralNetworkArgs[x].hidden, counter, 0));
                        else
                            input.Add(neuron);

                        randomInt = random.Next(101);

                        counter += 1;
                    }

                    //FIX THIS RIGHT NOW!!!!!//FIX THIS RIGHT NOW!!!!!//FIX THIS RIGHT NOW!!!!!//FIX THIS RIGHT NOW!!!!!//FIX THIS RIGHT NOW!!!!!
                    //FIX THIS RIGHT NOW!!!!!//FIX THIS RIGHT NOW!!!!!//FIX THIS RIGHT NOW!!!!!//FIX THIS RIGHT NOW!!!!!//FIX THIS RIGHT NOW!!!!!
                    //FIX THIS RIGHT NOW!!!!!//FIX THIS RIGHT NOW!!!!!//FIX THIS RIGHT NOW!!!!!//FIX THIS RIGHT NOW!!!!!//FIX THIS RIGHT NOW!!!!!
                    for (int y = 0; y < hidden.Count; y++)
                    {
                        if (hidden[y].parentConnectors == 0)
                        {
                            for (int z = 0; z < input.Count; z++)
                            {
                                randomInt = random.Next(2);

                                if (randomInt == 1)
                                {
                                    if (!input[z].connections.Contains(hidden[y]))
                                    {
                                        input[z].connections.Add(hidden[y]);
                                        hidden[y].parentConnectors += 1;
                                    }
                                }
                            }

                            if (hidden[y].parentConnectors == 0) y -= 1;
                        }
                    }
                }
                else
                {
                    foreach (Neuron neuron in neuralNetworkArgs[x].input)
                        input.Add(neuron);
                    foreach (Neuron neuron in neuralNetworkArgs[x].hidden)
                        hidden.Add(neuron);
                }

                randomInt = random.Next(101);

                neuralNetwork.Add(new NeuralNetwork(input.Count, hidden.Count, neuralNetworkArgs[x].output.Length));
                neuralNetwork[x].EstablishExistingNetwork(input.ToArray(), hidden.ToArray(), neuralNetworkArgs[x].output);
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
                    if (fishList[randomInt].brain.hidden.Length < fishList[0].brain.hidden.Length && y > fishList[randomInt].brain.hidden.Length - 1)
                        hidden.Add(fishList[0].brain.hidden[y]);
                    else
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
            List<Fish> tempNewFishList = new List<Fish>();
            foreach (Fish fish in fishList)
                tempNewFishList.Add(fish);

            int amountRemoved = 0;
            int randomInt = random.Next(101);

            while ((amountRemoved / fishList.Length) * 100 < percent)
            {
                for (int x = 0; x < fishList.Length; x++)
                {
                    if ((x / fishList.Length) * 100 >= randomInt)
                        tempNewFishList.Remove(newFishList[x]);

                    amountRemoved += 1;

                    if ((amountRemoved / fishList.Length) * 100 >= percent) return newFishList.ToArray();

                    randomInt = random.Next(101);
                }
            }

            return tempNewFishList.ToArray();
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
