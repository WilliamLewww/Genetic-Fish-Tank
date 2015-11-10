using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Genetic_Fish_Tank.Source
{
    class Fish
    {
        NeuralNetwork brain;
        ProximitySensor proximitySensor;

        Texture2D texture;
        Vector2 position;
        
        Vector2 origin;

        Food closestFood;

        static Random random = new Random();

        private static ContentManager content;
        public static ContentManager Content
        {
            get { return content; }
            set { content = value; }
        }

        private int rotation;
        public int Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        private Rectangle rectangle;
        public Rectangle FishRectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }

        public Fish()
        {
            texture = content.Load<Texture2D>("Sprites/fish.png");
            brain = new NeuralNetwork(2, 1, 3);

            position = new Vector2(random.Next(Game1.screenWidth - texture.Width), random.Next(Game1.screenHeight - texture.Height));
            rectangle = new Rectangle((int)(position.X), (int)(position.Y), texture.Width, texture.Height);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);

            rotation = 90;

            proximitySensor = new ProximitySensor();
        }

        public void Update(GameTime gameTime)
        {
            closestFood = proximitySensor.GetClosestFood(FishRectangle, Tank.foodList);
            int[] input = new int[2];
            if (closestFood.FoodRectangle.X + closestFood.FoodRectangle.Width < rectangle.X) input[0] = 1;
            else input[0] = 0;
            if (closestFood.FoodRectangle.X > rectangle.X + rectangle.Width) input[1] = 1;
            else input[1] = 0;
            brain.SendInput(input);

            rectangle = new Rectangle((int)(position.X), (int)(position.Y), texture.Width, texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, MathHelper.ToRadians(rotation), origin, 1f, SpriteEffects.None, 0);
        }
    }

    class Sensor
    {
        Texture2D texture;
        Vector2 position;

        int rotation;
        Vector2 origin;

        public Sensor(int x, int y, int z)
        {
            texture = Fish.Content.Load<Texture2D>("Sprites/fishsensor.png");
            position = new Vector2(x, y);
            origin = new Vector2(texture.Width / 2, texture.Height / 2 + 49);
            rotation = z;
        }

        public void Update(GameTime gameTime, int x, int y, int z)
        {
            position = new Vector2(x, y);
            rotation = z;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, MathHelper.ToRadians(rotation), origin, 1f, SpriteEffects.None, 0);
        }
    }

    class ProximitySensor
    {
        public Food GetClosestFood(Rectangle rectangle, Food[] foodList)
        {
            int closestID = 0;
            float closestValue = Math.Abs(rectangle.X - foodList[0].FoodRectangle.X) + Math.Abs(rectangle.Y - foodList[0].FoodRectangle.Y);

            for (int x = 0; x < foodList.Length; x++)
            {
                if (Math.Abs(rectangle.X - foodList[x].FoodRectangle.X) + Math.Abs(rectangle.Y - foodList[x].FoodRectangle.Y) < closestValue)
                {
                    closestID = x;
                    closestValue = Math.Abs(rectangle.X - foodList[x].FoodRectangle.X) + Math.Abs(rectangle.Y - foodList[x].FoodRectangle.Y);
                }
            }

            return foodList[closestID];
        }
    }
}