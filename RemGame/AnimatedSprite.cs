using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RemGame
{
    public class AnimatedSprite
    {
        private int currentFrame;
        private int totalFrames;
        private double timer;
        private double speed;

        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int CurrentFrame { get => currentFrame; set => currentFrame = value; }

        public AnimatedSprite(Texture2D texture, int rows, int columns)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            CurrentFrame = 0;
            totalFrames = Rows * Columns;
            speed = 0.15D;
            timer = speed;
            
        }

        public void Update(GameTime gameTime)
        {
            timer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (timer <= 0)
            {
                CurrentFrame++;
                timer = speed;
            }
            if (CurrentFrame == totalFrames)
                CurrentFrame = 0;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location,Vector2 size)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)CurrentFrame / (float)Columns);
            int column = CurrentFrame % Columns;
            Console.WriteLine(width);
            Console.WriteLine(height);
            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X-(int)size.X, (int)location.Y-(int)size.Y, width, height);


            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);

        }
    }
}