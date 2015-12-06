using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Genetic_Fish_Tank.Source
{
    class Food
    {
        static Texture2D texture;
        public static Color[] textureData;

        Vector2 position;

        static Random random = new Random();

        public bool eaten = false;

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
            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);

        Search:
            int randomX = random.Next((Game1.screenWidth - Game1.theoreticalScreenWidth) / 2, ((Game1.screenWidth - Game1.theoreticalScreenWidth) / 2) + Game1.theoreticalScreenWidth - texture.Width);
            int randomY = random.Next((Game1.screenHeight - Game1.theoreticalScreenHeight) / 2, ((Game1.screenHeight - Game1.theoreticalScreenHeight) / 2) + Game1.theoreticalScreenHeight - texture.Height);
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
