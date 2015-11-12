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
        UniversalSensorFunctions sensorFunctions = new UniversalSensorFunctions();

        Texture2D texture;
        Vector2 position;
        
        Vector2 origin;

        Food closest, closestLeft, closestRight;

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

        public void Swim(float speed)
        {
            Vector2 direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(rotation - 90)), (float)Math.Sin(MathHelper.ToRadians(rotation - 90)));
            direction.Normalize();
            position += direction * speed;
        }

        public Fish()
        {
            texture = content.Load<Texture2D>("Sprites/fish.png");
            brain = new NeuralNetwork(2, 1, 3);

            position = new Vector2(random.Next(Game1.screenWidth - texture.Width), random.Next(Game1.screenHeight - texture.Height));
            rectangle = new Rectangle((int)(position.X), (int)(position.Y), texture.Width, texture.Height);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);

            rotation = 90;
            proximitySensor[0] = new ProximitySensor((int)(position.X), (int)(position.Y), rotation, -16);
            proximitySensor[1] = new ProximitySensor((int)(position.X), (int)(position.Y), rotation, 16);
        }

        public void Update(GameTime gameTime)
        {
            proximitySensor[0].Update(gameTime, (int)(position.X), (int)(position.Y), rotation, 8);
            proximitySensor[1].Update(gameTime, (int)(position.X), (int)(position.Y), rotation, -8);

            closestLeft = sensorFunctions.GetClosestFood(proximitySensor[1].rotatedPosition, Tank.foodList);
            closestRight = sensorFunctions.GetClosestFood(proximitySensor[0].rotatedPosition, Tank.foodList);
            closest = sensorFunctions.CompareClosest(new Vector2(position.X + (texture.Width / 2), position.Y + (texture.Height / 2)), closestLeft, closestRight);

            int[] input = new int[2];

            if (sensorFunctions.GetClosestSensor(closest, proximitySensor[1], proximitySensor[0])) rotation += 1;
            else rotation -= 1;

            brain.SendInput(input);
            Swim(.5f);

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
        Texture2D texture;
        Vector2 position;

        Vector2 theoreticalPosition;
        Vector2 theoreticalCenter;
        Vector2 theorecticalCornerPoint;
        Vector2 temp;

        public Vector2 rotatedPosition
        {
            get { return theoreticalPosition + theorecticalCornerPoint; }
        }

        public ProximitySensor(int x, int y, int z, int offset)
        {
            texture = Fish.Content.Load<Texture2D>("Sprites/proximitysensor.png");
            position = new Vector2(x, y);
        }

        public void Update(GameTime gameTime, int x, int y, int z, int offset)
        {
            position = new Vector2(x, y);

            theoreticalCenter = new Vector2(x + offset, y + 16);
            theorecticalCornerPoint = new Vector2(x, y);
            temp = theorecticalCornerPoint - theoreticalCenter;

            theoreticalPosition.X = (float)(temp.X * Math.Cos(MathHelper.ToRadians(z)) - temp.Y * Math.Sin(MathHelper.ToRadians(z)));
            theoreticalPosition.Y = (float)(temp.X * Math.Sin(MathHelper.ToRadians(z)) + temp.Y * Math.Cos(MathHelper.ToRadians(z)));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, theoreticalPosition + theorecticalCornerPoint, Color.White);
        }
    }

    class UniversalSensorFunctions
    {
        public bool GetClosestSensor(Food food, ProximitySensor sensorA, ProximitySensor sensorB)
        {
            if (Math.Abs(food.FoodRectangle.X - sensorA.rotatedPosition.X) + Math.Abs(food.FoodRectangle.Y - sensorA.rotatedPosition.Y) < Math.Abs(food.FoodRectangle.X - sensorB.rotatedPosition.X) + Math.Abs(food.FoodRectangle.Y - sensorB.rotatedPosition.Y))
                return true;

            return false;
        }

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

        public Food CompareClosest(Vector2 position, Food foodLeft, Food foodRight)
        {
            if (Math.Abs(position.X - foodLeft.FoodRectangle.X) + Math.Abs(position.Y - foodLeft.FoodRectangle.Y) < Math.Abs(position.X - foodRight.FoodRectangle.X) + Math.Abs(position.Y - foodRight.FoodRectangle.Y))
                return foodLeft;

            return foodRight;
        }
    }
}