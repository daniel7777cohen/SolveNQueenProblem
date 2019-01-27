using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

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
            speed = 0.25D;
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

        public void Draw(SpriteBatch spriteBatch, Rectangle destination,Body body)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)CurrentFrame / (float)Columns);
            int column = CurrentFrame % Columns;
            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);


            ///// adjust measures is neccesery!/////////////////////////////////////////
            Rectangle destinationRectangle = new Rectangle (destination.X-30,destination.Y-30,destination.Width+72,destination.Height+96);

            //spriteBatch.Draw(texture, destination, null, Color.White, body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), SpriteEffects.None, 0);
            //spriteBatch.Draw(Texture, destination, sourceRectangle, Color.White, body.Rotation, new Vector2(width / 2.0f, height / 3.0f), SpriteEffects.None, 0);

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);

        }
    }
}