﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Genetic_Fish_Tank.Source
{
    class Tank
    {
        static int FOODCOUNT = 40, FISHCOUNT = 20, EXTERMINATIONPERCENT = 50, MUTATIONPERCENT = 25, 
            NEURONMUTATIONPERCENT = 50, NEGATIVEHIDDENPERCENT = 50, ADDITIONALHIDDENPERCENT = 25;

        static GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm();

        NeuralNetwork[] tempNeuralNetwork = new NeuralNetwork[FISHCOUNT];
        Fish[] tempFishList = new Fish[FISHCOUNT];

        List<FontSeparation.Character> characterList = new List<FontSeparation.Character>();

        public static Fish[] fishList = new Fish[FISHCOUNT];
        public static List<Food> foodList = new List<Food>();
        static List<Food> tempFoodList = new List<Food>();

        public void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            FontSeparation.Character.Content = content;

            Fish.Content = content;
            for (int x = 0; x < fishList.Length; x++)
            {
                fishList[x] = new Fish();
                fishList[x].fishIndex = x;

                characterList.Add(new FontSeparation.Character("font.png", x.ToString() + ":" + fishList[x].collisionCircle.life, new Vector2(0, x * 25)));
            }

            characterList.Add(new FontSeparation.Character("font.png", "", new Vector2(10, Game1.screenHeight - 64)));
            characterList.Add(new FontSeparation.Character("font.png", "", new Vector2(10, Game1.screenHeight - 32)));

            Food.Content = content;
            for (int x = 0; x < FOODCOUNT; x++)
                foodList.Add(new Food());
        }

        public void Update(GameTime gameTime)
        {
            geneticAlgorithm.generationTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (geneticAlgorithm.GetGenerationState(fishList))
            {
                geneticAlgorithm.fittest.Add(new Fish(geneticAlgorithm.GetFittest(fishList).brain, geneticAlgorithm.GetFittest(fishList).collisionCircle));
                geneticAlgorithm.CleanFittestList(100);
                Console.WriteLine("Generation: " + geneticAlgorithm.generation);
                Console.WriteLine("Trend: " + geneticAlgorithm.CheckTrend(geneticAlgorithm.fittest.ToArray()) + " TrendState: " + geneticAlgorithm.trendState);
                Console.WriteLine("Generation Score: " + geneticAlgorithm.generationScore + " Peak Score: " + geneticAlgorithm.peakScore);                

                if (geneticAlgorithm.GetTrendState(geneticAlgorithm.trendState, geneticAlgorithm.generationScore, geneticAlgorithm.ReturnZeroScore(fishList)))
                {
                    if (geneticAlgorithm.generation < 20)
                    {
                        foreach (Fish fish in fishList)
                            fish.ResetFish();
                    }
                    else
                        fishList = geneticAlgorithm.ReturnFittestStream(fishList.Length);
                }
                else
                {
                    tempFishList = geneticAlgorithm.ExterminatePopulation(fishList, EXTERMINATIONPERCENT);
                    for (int x = 0; x < tempNeuralNetwork.Length; x++)
                    {
                        tempNeuralNetwork[x] = tempFishList[x].brain;
                    }

                    tempNeuralNetwork = geneticAlgorithm.CrossMutate(tempNeuralNetwork, FISHCOUNT);
                    tempNeuralNetwork = geneticAlgorithm.Mutate(tempNeuralNetwork, MUTATIONPERCENT, NEURONMUTATIONPERCENT);
                    tempNeuralNetwork = geneticAlgorithm.AddHidden(tempNeuralNetwork, NEGATIVEHIDDENPERCENT, ADDITIONALHIDDENPERCENT);

                    for (int x = 0; x < tempNeuralNetwork.Length; x++)
                    {
                        fishList[x].ResetFish(tempNeuralNetwork[x].hidden.Length, tempNeuralNetwork[x]);
                    }
                }  

                foodList.Clear();
                for (int x = 0; x < FOODCOUNT; x++)
                    foodList.Add(new Food());

                geneticAlgorithm.generation += 1;
            }

            for (int x = 0; x < fishList.Length; x++)
            {
                if (fishList[x].collisionCircle.dead == false)
                    fishList[x].Update(gameTime);

                characterList[x].Update(x.ToString() + ":" + fishList[x].collisionCircle.life);
            }

            foreach (Food food in foodList)
                if (food.eaten == true)
                    tempFoodList.Add(food);

            foreach (Food food in tempFoodList)
                foodList.Remove(food);
            tempFoodList.Clear();

            string rankingString = "";

            foreach (Fish fish in geneticAlgorithm.RankFittest(fishList))
                rankingString += fish.fishIndex + ":" + fish.collisionCircle.Score + ",,";
            characterList[characterList.Count - 1].Update(rankingString);

            characterList[characterList.Count - 2].Update(geneticAlgorithm.generation.ToString() + ":" + (30 - (int)geneticAlgorithm.generationTimer).ToString());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Food food in foodList)
                food.Draw(spriteBatch);

            foreach (Fish fish in fishList)
                if (fish.collisionCircle.dead == false)
                    fish.Draw(spriteBatch);

            foreach (FontSeparation.Character character in characterList)
                character.Draw(spriteBatch);
        }
    }
}
