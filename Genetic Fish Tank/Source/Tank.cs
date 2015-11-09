using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic_Fish_Tank.Source
{
    class Tank
    {
        public static Fish[] fishList = new Fish[10];
        public Food[] foodList = new Food[40];

        public void LoadContent(ContentManager content, GraphicsDevice graphics)
        {
            Fish.Content = content;
            for (int x = 0; x < fishList.Length; x++)
                fishList[x] = new Fish();

            Food.Content = content;
            for (int x = 0; x < foodList.Length; x++)
                foodList[x] = new Food();
        }

        public void Update(GameTime gameTime)
        {
            foreach (Fish fish in fishList)
                fish.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Fish fish in fishList)
                fish.Draw(spriteBatch);

            foreach (Food food in foodList)
                food.Draw(spriteBatch);
        }
    }
}
