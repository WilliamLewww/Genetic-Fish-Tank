using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Genetic_Fish_Tank.Source
{
    class Fish
    {
        NeuralNetwork brain;

        public CollisionCircle collisionCircle;

        ProximitySensor[] proximitySensor = new ProximitySensor[3];
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

            position = new Vector2(random.Next((Game1.screenWidth - Game1.theoreticalScreenWidth) / 2, ((Game1.screenWidth - Game1.theoreticalScreenWidth) / 2) + Game1.theoreticalScreenWidth - texture.Width), random.Next((Game1.screenHeight - Game1.theoreticalScreenHeight) / 2, ((Game1.screenHeight - Game1.theoreticalScreenHeight) / 2) + Game1.theoreticalScreenHeight - texture.Height));
            rectangle = new Rectangle((int)(position.X), (int)(position.Y), texture.Width, texture.Height);
            origin = new Vector2(texture.Width / 2, texture.Height / 2);

            collisionCircle = new CollisionCircle((int)(position.X - origin.X), (int)(position.Y - origin.Y));

            rotation = 90;
            proximitySensor[0] = new ProximitySensor((int)(position.X), (int)(position.Y));
            proximitySensor[1] = new ProximitySensor((int)(position.X), (int)(position.Y));
            proximitySensor[2] = new ProximitySensor((int)(position.X), (int)(position.Y));
        }

        public void Update(GameTime gameTime)
        {
            collisionCircle.Update(gameTime, (int)(position.X - origin.X), (int)(position.Y - origin.Y));

            proximitySensor[0].Update(gameTime, (int)(position.X), (int)(position.Y), rotation, 8, 16);
            proximitySensor[1].Update(gameTime, (int)(position.X), (int)(position.Y), rotation, -8, 16);
            proximitySensor[2].Update(gameTime, (int)(position.X), (int)(position.Y), rotation, 0, -20);

            float x = Math.Abs(proximitySensor[0].rotatedPosition.X - proximitySensor[1].rotatedPosition.X) + Math.Min(proximitySensor[0].rotatedPosition.X, proximitySensor[1].rotatedPosition.X);
            float y = Math.Abs(proximitySensor[0].rotatedPosition.Y - proximitySensor[1].rotatedPosition.Y) + Math.Min(proximitySensor[0].rotatedPosition.Y, proximitySensor[1].rotatedPosition.Y);

            closestLeft = sensorFunctions.GetClosestFood(proximitySensor[1].rotatedPosition, Tank.foodList);
            closestRight = sensorFunctions.GetClosestFood(proximitySensor[0].rotatedPosition, Tank.foodList);
            closest = sensorFunctions.CompareClosest(new Vector2(x, y), closestLeft, closestRight);

            int[] input = new int[2];

            if (sensorFunctions.GetClosestSensor(closest, proximitySensor[1], proximitySensor[0])) input[1] = 1;
            else if (sensorFunctions.GetClosestSensor(closest, proximitySensor[2], proximitySensor[1]) || sensorFunctions.GetClosestSensor(closest, proximitySensor[2], proximitySensor[0])) input[1] = 1;
            else input[0] = 1;

            brain.SendInput(input);
            brain.EstablishRandomNetwork();
            if (brain.GetOutput()[0] == 1) rotation -= 1;
            if (brain.GetOutput()[1] == 1) rotation += 1;

            rectangle = new Rectangle((int)(position.X), (int)(position.Y), texture.Width, texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            collisionCircle.Draw(spriteBatch);

            spriteBatch.Draw(texture, position, null, Color.White, MathHelper.ToRadians(rotation), origin, 1f, SpriteEffects.None, 0);

            foreach (ProximitySensor sensor in proximitySensor)
                sensor.Draw(spriteBatch);
        }
    }

    class CollisionCircle
    {
        static Texture2D texture;
        static Color[] textureData;

        public Vector2 position;
        public Rectangle rectangle;

        public int score = 0;

        public CollisionCircle(int x, int y)
        {
            texture = Fish.Content.Load<Texture2D>("Sprites/fishcollision.png");
            textureData = new Color[texture.Width * texture.Height];
            texture.GetData(textureData);

            rectangle = new Rectangle(x, y, texture.Width, texture.Height);
        }

        public void Update(GameTime gameTime, int x, int y)
        {
            position = new Vector2(x, y);
            rectangle = new Rectangle(x, y, texture.Width, texture.Height);

            foreach (Food food in Tank.foodList)
            {
                if (PixelCollision(rectangle, textureData, food.FoodRectangle, Food.textureData))
                {
                    score += 1;
                    food.eaten = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, new Color(255, 255, 255, 100));
        }

        bool PixelCollision(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color colorA = dataA[(x - rectangleA.Left) + (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) + (y - rectangleB.Top) * rectangleB.Width];

                    if (colorA.A != 0 && colorB.A != 0)
                        return true;
                }
            }

            return false;
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
        Vector2 theoreticalPosition;
        Vector2 theoreticalCenter;
        Vector2 theorecticalCornerPoint;
        Vector2 temp;

        public Vector2 rotatedPosition
        {
            get { return theoreticalPosition + theorecticalCornerPoint; }
        }

        public ProximitySensor(int x, int y)
        {
            texture = Fish.Content.Load<Texture2D>("Sprites/proximitysensor.png");
        }

        public void Update(GameTime gameTime, int x, int y, int z, int offsetX, int offsetY)
        {
            theoreticalCenter = new Vector2(x + offsetX, y + offsetY);
            theorecticalCornerPoint = new Vector2(x, y);
            temp = theorecticalCornerPoint - theoreticalCenter;

            theoreticalPosition.X = (float)(temp.X * Math.Cos(MathHelper.ToRadians(z)) - temp.Y * Math.Sin(MathHelper.ToRadians(z))) - (texture.Width / 2);
            theoreticalPosition.Y = (float)(temp.X * Math.Sin(MathHelper.ToRadians(z)) + temp.Y * Math.Cos(MathHelper.ToRadians(z))) - (texture.Height / 2);
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

        public Food GetClosestFood(Vector2 position, List<Food> foodList)
        {
            int closestID = 0;
            float closestValue = Math.Abs(position.X - foodList[0].FoodRectangle.X) + Math.Abs(position.Y - foodList[0].FoodRectangle.Y);

            for (int x = 0; x < foodList.Count; x++)
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