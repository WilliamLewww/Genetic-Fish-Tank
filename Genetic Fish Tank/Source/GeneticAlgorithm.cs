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

        public NeuralNetwork[] AddHidden(NeuralNetwork[] neuralNetworkArgs, int percentNegative)
        {
            List<NeuralNetwork> neuralNetwork = new List<NeuralNetwork>();
            NeuralNetwork neuralNetworkMethods = new NeuralNetwork(0, 0, 0);
            int randomInt = random.Next(101);

            for (int x = 0; x < neuralNetworkArgs.Length; x++)
            {
                List<Neuron> input = new List<Neuron>(), hidden = new List<Neuron>(), output = new List<Neuron>();

                foreach (Neuron neuronArgs in neuralNetworkArgs[x].input)
                    input.Add(neuronArgs);

                foreach (Neuron neuronArgs in neuralNetworkArgs[x].output)
                    output.Add(neuronArgs);

                for (int y = 0; y < neuralNetworkArgs[x].hidden.Length; y++)
                {
                    hidden.Add(neuralNetworkArgs[x].hidden[y]);

                    if (y == neuralNetworkArgs[x].hidden.Length - 1)
                    {
                        if (randomInt < percentNegative)
                            hidden.Add(neuralNetworkMethods.CreateHidden(neuralNetworkArgs[x].hidden.Length, true));
                        else
                            hidden.Add(neuralNetworkMethods.CreateHidden(neuralNetworkArgs[x].hidden.Length, false));

                        neuralNetworkMethods.MutateNeuron(hidden[y + 1], output.ToArray());
                        neuralNetworkMethods.ConnectInput(input.ToArray(), hidden[y + 1]);
                    }
                }

                neuralNetwork.Add(new NeuralNetwork(input.Count, hidden.Count, output.Count));
                neuralNetwork[x].EstablishExistingNetwork(input.ToArray(), hidden.ToArray(), output.ToArray());

                randomInt = random.Next(101);
            }

            return neuralNetwork.ToArray();
        }

        public NeuralNetwork[] Mutate(NeuralNetwork[] neuralNetworkArgs, int percentAmountMutate, int percentMutate)
        {
            List<NeuralNetwork> neuralNetwork = new List<NeuralNetwork>();
            NeuralNetwork neuralNetworkMethods = new NeuralNetwork(0, 0, 0);
            int randomInt = random.Next(101);

            for (int x = 0; x < neuralNetworkArgs.Length; x++)
            {
                if (randomInt < percentAmountMutate)
                {
                    List<Neuron> input = new List<Neuron>(), hidden = new List<Neuron>(), output = new List<Neuron>();

                    randomInt = random.Next(101);

                    for (int y = 0; y < neuralNetworkArgs[x].input.Length; y++)
                    {
                        if (randomInt < percentMutate)
                            input.Add(neuralNetworkMethods.MutateNeuron(neuralNetworkArgs[x].input[y], neuralNetworkArgs[x].hidden));
                        else
                            input.Add(neuralNetworkArgs[x].input[y]);

                        randomInt = random.Next(101);
                    }

                    for (int y = 0; y < neuralNetworkArgs[x].hidden.Length; y++)
                    {
                        if (randomInt < percentMutate)
                            hidden.Add(neuralNetworkMethods.MutateNeuron(neuralNetworkArgs[x].hidden[y], neuralNetworkArgs[x].output));
                        else
                            hidden.Add(neuralNetworkArgs[x].hidden[y]);

                        randomInt = random.Next(101);
                    }

                    foreach (Neuron neuron in neuralNetworkArgs[x].output)
                        output.Add(neuron);

                    neuralNetwork.Add(new NeuralNetwork(input.Count, hidden.Count, output.Count));
                    neuralNetwork[x].EstablishExistingNetwork(input.ToArray(), hidden.ToArray(), output.ToArray());
                }
                else
                {
                    neuralNetwork.Add(neuralNetworkArgs[x]);
                }

                randomInt = random.Next(101);
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
