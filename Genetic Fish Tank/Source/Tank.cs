using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Genetic_Fish_Tank.Source
{
    class Tank
    {
        GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm();
        List<FontSeparation.Character> characterList = new List<FontSeparation.Character>();

        public static Fish[] fishList = new Fish[20];
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
            for (int x = 0; x < 40; x++)
                foodList.Add(new Food());
        }

        public void Update(GameTime gameTime)
        {
            geneticAlgorithm.generationTimer += gameTime.ElapsedGameTime.TotalSeconds;

            for (int x = 0; x < fishList.Length; x++)
            {
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

            characterList[characterList.Count - 2].Update(geneticAlgorithm.generation.ToString() + ":" + ((int)geneticAlgorithm.generationTimer).ToString());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Food food in foodList)
                food.Draw(spriteBatch);

            foreach (Fish fish in fishList)
                fish.Draw(spriteBatch);

            foreach (FontSeparation.Character character in characterList)
                character.Draw(spriteBatch);
        }
    }
}
