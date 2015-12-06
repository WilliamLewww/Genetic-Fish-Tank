using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Genetic_Fish_Tank.Source
{
    class Tank
    {
        public static Fish[] fishList = new Fish[20];
        public static List<Food> foodList = new List<Food>();
        static List<Food> tempFoodList = new List<Food>();

        public void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            Fish.Content = content;
            for (int x = 0; x < fishList.Length; x++)
                fishList[x] = new Fish();

            Food.Content = content;
            for (int x = 0; x < 40; x++)
                foodList.Add(new Food());
        }

        public void Update(GameTime gameTime)
        {
            foreach (Fish fish in fishList)
                fish.Update(gameTime);

            foreach (Food food in foodList)
                if (food.eaten == true)
                    tempFoodList.Add(food);

            foreach (Food food in tempFoodList)
                foodList.Remove(food);
            tempFoodList.Clear();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Food food in foodList)
                food.Draw(spriteBatch);

            foreach (Fish fish in fishList)
                fish.Draw(spriteBatch);
        }
    }
}
