using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Genetic_Fish_Tank.Source
{
    class Fish
    {
        NeuralNetwork brain;
        Sensor inputSensor;

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

            inputSensor = new Sensor((int)(position.X), (int)(position.Y), rotation);
        }

        public void Update(GameTime gameTime)
        {
            int[] input = new int[2];

            brain.SendInput(input);

            rectangle = new Rectangle((int)(position.X), (int)(position.Y), texture.Width, texture.Height);
            inputSensor.Update(gameTime, (int)(position.X), (int)(position.Y), rotation);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, MathHelper.ToRadians(rotation), origin, 1f, SpriteEffects.None, 0);
            inputSensor.Draw(spriteBatch);
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
}