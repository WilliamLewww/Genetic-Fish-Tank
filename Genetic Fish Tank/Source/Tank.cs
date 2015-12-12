using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Genetic_Fish_Tank.Source
{
    class Tank
    {
        static int FOODCOUNT = 40, FISHCOUNT = 20, EXTERMINATIONPERCENT = 50, MUTATIONPERCENT = 100, NEURONMUTATIONPERCENT = 50, NEGATIVEHIDDENPERCENT = 50;

        static GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm();
        NeuralNetwork[] tempNeuralNetwork;

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
                tempNeuralNetwork = geneticAlgorithm.CrossMutate(geneticAlgorithm.ExterminatePopulation(fishList, EXTERMINATIONPERCENT), FISHCOUNT);
                tempNeuralNetwork = geneticAlgorithm.Mutate(tempNeuralNetwork, MUTATIONPERCENT, NEURONMUTATIONPERCENT);
                tempNeuralNetwork = geneticAlgorithm.AddHidden(tempNeuralNetwork, 50);

                for (int x = 0; x < tempNeuralNetwork.Length; x++)
                {
                    fishList[x].ResetFish(tempNeuralNetwork[x].hidden.Length, tempNeuralNetwork[x]);
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
