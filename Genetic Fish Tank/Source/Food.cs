using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Genetic_Fish_Tank.Source
{
    class Food
    {
        Texture2D texture;
        Vector2 position;

        static Random random = new Random();

        private static ContentManager content;
        public static ContentManager Content
        {
            get { return content; }
            set { content = value; }
        }

        private Rectangle rectangle;
        public Rectangle FoodRectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }

        public Food()
        {
            texture = content.Load<Texture2D>("Sprites/food.png");

        Search:
            int randomX = random.Next(Game1.screenWidth - texture.Width);
            int randomY = random.Next(Game1.screenHeight - texture.Height);
            rectangle = new Rectangle(randomX, randomY, texture.Width, texture.Height);

            foreach (Fish fish in Tank.fishList)
                if (rectangle.Intersects(fish.FishRectangle)) goto Search;

            position = new Vector2(randomX, randomY);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
