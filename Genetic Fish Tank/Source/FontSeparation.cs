using System.Drawing;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FontSeparation
{
    class FontSeparation
    {
        static Bitmap font;
        public List<int[]> coordinateList = new List<int[]>();

        public void LoadBitmap(Bitmap bitmap)
        {
            font = bitmap;

            bool a = false;
            int minX = -1, minY = bitmap.Height, maxX = 0, maxY = 0;

            for (int x = 0; x < bitmap.Width; x++)
            {
                a = false;

                for (int y = 0; y < bitmap.Height; y++)
                {
                    if (bitmap.GetPixel(x, y).A != 0)
                    {
                        a = true;

                        if (minX == -1)
                            minX = x;

                        if (y < minY)
                            minY = y;

                        if (x > maxX)
                            maxX = x;

                        if (y > maxY)
                            maxY = y;
                    }
                }

                if (a == false)
                {
                    if (minX != -1)
                        coordinateList.Add(new int[4] { minX, minY, (maxX + 1) - minX, (maxY + 1) - minY });

                    minX = -1;
                    minY = bitmap.Height;
                    maxX = 0;
                    maxY = 0;
                }
            }
        }
    }

    class Character
    {
        static Texture2D texture;
        static FontSeparation fontSeparation;

        Vector2 position;

        int OFFSET = 5;

        public static List<Microsoft.Xna.Framework.Rectangle> characterList = new List<Microsoft.Xna.Framework.Rectangle>();
        List<Microsoft.Xna.Framework.Rectangle> newCharacterList = new List<Microsoft.Xna.Framework.Rectangle>();

        private static ContentManager content;
        public static ContentManager Content
        {
            set { content = value; }
        }

        public Character(string image, string text, Vector2 positionParameter)
        {
            texture = content.Load<Texture2D>("Sprites/" + image);

            fontSeparation = new FontSeparation();
            fontSeparation.LoadBitmap(new Bitmap(image));

            foreach (int[] coordinate in fontSeparation.coordinateList)
                characterList.Add(new Microsoft.Xna.Framework.Rectangle(coordinate[0], coordinate[1], coordinate[2], coordinate[3]));

            newCharacterList = DrawString(text);

            position = positionParameter;
        }

        public void Update(string text)
        {
            newCharacterList = DrawString(text);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int a = 0, b = 0;

            for (int x = 0; x < characterList.Count; x++)
                if (x != 26 && x != 42 && x != 45 && x != 51 && x != 52 && x != 60)
                    if (characterList[x].Height > b)
                        b = characterList[x].Height;

            for (int x = 0; x < newCharacterList.Count; x++)
            {
                //if (newCharacterList[x] == characterList[GetIndexFromChar('g')] || newCharacterList[x] == characterList[GetIndexFromChar('j')] || newCharacterList[x] == characterList[GetIndexFromChar('p')] || newCharacterList[x] == characterList[GetIndexFromChar('q')] || newCharacterList[x] == characterList[GetIndexFromChar('y')])
                    //spriteBatch.Draw(texture, new Vector2(a + (OFFSET * x), b - newCharacterList[x].Height) + position, newCharacterList[x], new Microsoft.Xna.Framework.Color(0, 0, 0, 255));
                //else 
                spriteBatch.Draw(texture, new Vector2(a + (OFFSET * x), b - newCharacterList[x].Height) + position, newCharacterList[x], new Microsoft.Xna.Framework.Color(0, 0, 0, 255));

                a += newCharacterList[x].Width;
            }
        }

        public List<Microsoft.Xna.Framework.Rectangle> DrawString(string text)
        {
            List<Microsoft.Xna.Framework.Rectangle> newCharacterList = new List<Microsoft.Xna.Framework.Rectangle>();

            foreach (char c in text)
            {
                newCharacterList.Add(characterList[GetIndexFromChar(c)]);
            }

            return newCharacterList;
        }

        public int GetIndexFromChar(char c)
        {
            switch (c)
            {
                case ',':
                    return 0;
                case '.':
                    return 1;
                case '0':
                    return 2;
                case '1':
                    return 3;
                case '2':
                    return 4;
                case '3':
                    return 5;
                case '4':
                    return 6;
                case '5':
                    return 7;
                case '6':
                    return 8;
                case '7':
                    return 9;
                case '8':
                    return 10;
                case '9':
                    return 11;
                case ':':
                    return 12;
            }

            return -1;
        }
    }
}
