using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Genetic_Fish_Tank.Source
{
    class Fish
    {
        NeuralNetwork brain;
        ProximitySensor[] proximitySensor = new ProximitySensor[2];

        Texture2D texture;
        Vector2 position;
        
        Vector2 origin;

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
            proximitySensor[0] = new ProximitySensor((int)(position.X), (int)(position.Y), rotation, -5);
            proximitySensor[1] = new ProximitySensor((int)(position.X), (int)(position.Y), rotation, 5);
        }

        public void Update(GameTime gameTime)
        {
            int[] input = new int[2];

            foreach (ProximitySensor sensor in proximitySensor)
                sensor.Update(gameTime, (int)(position.X), (int)(position.Y), rotation);

            brain.SendInput(input);

            rectangle = new Rectangle((int)(position.X), (int)(position.Y), texture.Width, texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, MathHelper.ToRadians(rotation), origin, 1f, SpriteEffects.None, 0);

            foreach (ProximitySensor sensor in proximitySensor)
                sensor.Draw(spriteBatch);
        }
    }

    class Sensor
    {
        Texture2D texture;
        Vector2 position;

        int rotation;
        Vector2 origin;

        public Vector2 theoreticalPosition;

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

            theoreticalPosition.X = (float)(position.X * Math.Cos(rotation)) - (float)(position.Y * Math.Sin(rotation));
            theoreticalPosition.Y = (float)(position.X * Math.Sin(rotation)) + (float)(position.Y * Math.Cos(rotation));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, MathHelper.ToRadians(rotation), origin, 1f, SpriteEffects.None, 0);
        }
    }

    class ProximitySensor
    {
        Texture2D texture;
        Vector2 position;

        int rotation;
        Vector2 origin;

        public ProximitySensor(int x, int y, int z, int offset)
        {
            texture = Fish.Content.Load<Texture2D>("Sprites/proximitysensor.png");
            position = new Vector2(x, y);
            origin = new Vector2(texture.Width / 2 + offset, texture.Height / 2 + 20);
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

    class UniversalSensorFunctions
    {
        public Food GetClosestFood(Vector2 position, Food[] foodList)
        {
            int closestID = 0;
            float closestValue = Math.Abs(position.X - foodList[0].FoodRectangle.X) + Math.Abs(position.Y - foodList[0].FoodRectangle.Y);

            for (int x = 0; x < foodList.Length; x++)
            {
                if (Math.Abs(position.X - foodList[x].FoodRectangle.X) + Math.Abs(position.Y - foodList[x].FoodRectangle.Y) < closestValue)
                {
                    closestID = x;
                    closestValue = Math.Abs(position.X - foodList[x].FoodRectangle.X) + Math.Abs(position.Y - foodList[x].FoodRectangle.Y);
                }
            }

            return foodList[closestID];
        }

        public bool CompareClosest(Vector2 position, Food foodLeft, Food foodRight)
        {
            if (Math.Abs(position.X - foodLeft.FoodRectangle.X) + Math.Abs(position.Y - foodLeft.FoodRectangle.Y) < Math.Abs(position.X - foodRight.FoodRectangle.X) + Math.Abs(position.Y - foodRight.FoodRectangle.Y))
                return true;

            return false;
        }
    }
}