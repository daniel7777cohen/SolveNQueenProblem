using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemGame
{

    class Backgrounds
    {
        protected Texture2D texture;
        protected Rectangle rectangle;
        protected int scrollingSpeed;



        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
    }

    class Scrollingbackground : Backgrounds
    {
        public Scrollingbackground(Texture2D newTexture, Rectangle newRectangle, int newScrollingSpeed)
        {
            texture = newTexture;
            rectangle = newRectangle;
            scrollingSpeed = newScrollingSpeed;

        }

        public void Update(Camera2D cam, int direction, float playerSpeed, GameTime gameTime)
        {
            rectangle.X += scrollingSpeed * direction * (int)playerSpeed / 7;
            if (rectangle.X + texture.Width <= cam.Position.X)
            {
                rectangle.X = (int)cam.Position.X + texture.Width;
            }

        }
    }


}