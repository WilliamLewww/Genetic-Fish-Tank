﻿using System;
using System.Collections.Generic;

namespace Genetic_Fish_Tank.Source
{
    class GeneticAlgorithm
    {
        static Random random = new Random();

        public int generation = 0;
        public int generationScore = 0;
        public int peakScore = 0;
        public double generationTimer = 0;
        public double trendState = 0;

        public List<Fish> fittest = new List<Fish>();

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

        public void CleanFittestList(int count)
        {
            if (fittest.Count > count)
            {
                List<Fish> tempFittest = new List<Fish>();
                Fish[] rankedFittest = RankFittest(fittest.ToArray());

                for (int x = 0; x < count; x++)
                    tempFittest.Add(rankedFittest[x]);

                fittest.Clear();

                foreach (Fish fish in tempFittest)
                    fittest.Add(new Fish(fish.brain));
            }
        }

        public Fish[] ReturnFittestStream(int count)
        {
            if (fittest.Count < count)
                count = fittest.Count;

            Fish[] fishList = new Fish[count];
            for (int x = 0; x < count; x++)
            {
                fishList[x] = new Fish(RankFittest(fittest.ToArray())[x].brain);
            }

            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!! SET TO FITTEST !!!!!!!!!!!!!!!!!!!!!!!!!!!");

            return fishList;
        }

        public bool ReturnZeroScore(Fish[] fishList)
        {
            bool containsZero = true;

            foreach (Fish fish in fishList)
                if (fish.collisionCircle.Score > 0) containsZero = false;

            return containsZero;
        }

        public bool GetTrendState(double trend, double score, bool zero)
        {
            if (trend == .5f || (score < 0 && trend == .3f) || zero || score < -3)
            {
                generationScore = 0;
                return true;
            }

            return false;
        }

        public Fish GetFittest(Fish[] fishList) { return RankFittest(fishList)[0]; }

        public double CheckTrend(Fish[] fishList)
        {
            double cumulative = 0;
            int counter = generation;
            int greater = 0;

            if (fishList.Length > 1)
            {
                if (fishList[generation].collisionCircle.Score > fishList[generation - 1].collisionCircle.Score)
                {
                    generationScore += 1;
                    greater = 1;

                    if (generationScore > peakScore) peakScore = generationScore;
                }
                if (fishList[generation].collisionCircle.Score < fishList[generation - 1].collisionCircle.Score)
                {
                    generationScore -= 1;
                    greater = -1;
                }
                if (fishList[generation].collisionCircle.Score == fishList[generation - 1].collisionCircle.Score)
                    greater = 2;

                while (counter > 1)
                {
                    if (greater == 1)
                    {
                        if (fishList[counter].collisionCircle.Score > fishList[counter - 1].collisionCircle.Score)
                        {
                            cumulative += 1;
                            counter -= 1;
                        }
                        else
                        {
                            trendState = cumulative;
                            return cumulative;
                        }
                    }
                    if (greater == -1)
                    {
                        if (fishList[counter].collisionCircle.Score < fishList[counter - 1].collisionCircle.Score)
                        {
                            cumulative -= 1;
                            counter -= 1;
                        }
                        else
                        {
                            trendState = cumulative;
                            return cumulative;
                        }
                    }

                    if (greater == 2)
                    {
                        if (fishList[counter].collisionCircle.Score == fishList[counter - 1].collisionCircle.Score)
                        {
                            cumulative += .01;
                            counter -= 1;
                        }
                        else
                        {
                            trendState = cumulative;
                            return cumulative;
                        }
                    }

                    if (greater == 0)
                    {
                        trendState = cumulative;
                        return cumulative;
                    }
                }
            }

            trendState = cumulative;
            return cumulative;
        }

        public NeuralNetwork[] AddHidden(NeuralNetwork[] neuralNetworkArgs, int percentNegative, int percentHidden)
        {
            List<NeuralNetwork> neuralNetwork = new List<NeuralNetwork>();
            NeuralNetwork neuralNetworkMethods = new NeuralNetwork(0, 0, 0);
            int randomInt = random.Next(101);

            for (int x = 0; x < neuralNetworkArgs.Length; x++)
            {
                if (randomInt < percentHidden)
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
                }
                else
                {
                    neuralNetwork.Add(neuralNetworkArgs[x]);
                }

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

        public NeuralNetwork[] CrossMutate(NeuralNetwork[] neuralNetworkArgs, int total)
        {
            List<NeuralNetwork> neuralNetwork = new List<NeuralNetwork>();
            NeuralNetwork neuralNetworkMethods = new NeuralNetwork(0, 0, 0);
            int randomInt = random.Next(neuralNetworkArgs.Length);

            for (int x = 0; x < total; x++)
            {
                List<Neuron> input = new List<Neuron>(), hidden = new List<Neuron>(), output = new List<Neuron>();
                List<int> additionalConnections = new List<int>();

                for (int y = 0; y < neuralNetworkArgs[0].output.Length; y++)
                {
                    output.Add(neuralNetworkArgs[randomInt].output[y]);
                    randomInt = random.Next(neuralNetworkArgs.Length);
                }

                for (int y = 0; y < neuralNetworkArgs[0].hidden.Length; y++)
                {
                    if (neuralNetworkArgs[0].hidden.Length > neuralNetworkArgs[randomInt].hidden.Length && y > neuralNetworkArgs[randomInt].hidden.Length - 1)
                    {
                        hidden.Add(neuralNetworkMethods.CloneConnections(neuralNetworkArgs[0].hidden[y], output.ToArray(), neuralNetworkArgs[0].hidden[y].negative));
                        additionalConnections.Add(randomInt);
                    }
                    else
                        hidden.Add(neuralNetworkMethods.CloneConnections(neuralNetworkArgs[randomInt].hidden[y], output.ToArray(), neuralNetworkArgs[randomInt].hidden[y].negative));

                    randomInt = random.Next(neuralNetworkArgs.Length);
                }

                for (int y = 0; y < neuralNetworkArgs[0].input.Length; y++)
                {
                    bool passable = true;

                    if (additionalConnections.Count > 0)
                    {
                        while (passable == false)
                        {
                            passable = true;

                            foreach (int z in additionalConnections)
                            {
                                if (randomInt == z)
                                    passable = false;
                            }

                            randomInt = random.Next(neuralNetworkArgs.Length);
                        }

                        input.Add(neuralNetworkMethods.CloneConnections(neuralNetworkArgs[randomInt].input[y], hidden.ToArray(), neuralNetworkArgs[randomInt].input[y].negative));
                    }
                    else
                        input.Add(neuralNetworkMethods.CloneConnections(neuralNetworkArgs[randomInt].input[y], hidden.ToArray(), neuralNetworkArgs[randomInt].input[y].negative));

                    randomInt = random.Next(neuralNetworkArgs.Length);
                }

                neuralNetwork.Add(new NeuralNetwork(input.Count, hidden.Count, output.Count));
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
