using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Genetic_Fish_Tank.Source
{
    class Fish
    {
        NeuralNetwork brain;

        Texture2D texture;
        Vector2 position;

        private ContentManager content;
        public ContentManager Content
        {
            get { return content; }
            set { content = value; }
        }

        public Fish()
        {
            brain = new NeuralNetwork(2, 1, 3);
        }

        public void Update(GameTime gameTime)
        {
            int[] input = new int[2];

            brain.SendInput(input);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}