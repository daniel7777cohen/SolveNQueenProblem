using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using XELibrary;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using Microsoft.Xna.Framework.Content;

namespace RemGame
{
    class HealthBar
    {
        private Texture2D texture;
        private Rectangle rectangle;
        public Rectangle getRectangle { get => rectangle; set => rectangle = value; }
        public HealthBar(ContentManager Content)
        {
            texture = Content.Load<Texture2D>("misc/HealthBar");
            rectangle = new Rectangle(0, 0, texture.Width*3, texture.Height);
        }

        public void decrease(int damage)
        {
            this.rectangle.Width -= damage;
        }
        public void Draw(SpriteBatch spriteBatch,Camera2D cam)
        {
 
            spriteBatch.Draw(texture, new Vector2(cam.Position.X, cam.Position.Y),rectangle,Color.White); 
           
        }

        public void Update()
        {

        }
    }

}
