using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace RemGame
{
    public class Obstacle : Component
    {

        private Body body;
        private Texture2D texture;
        private Vector2 size;
        private bool kinesisOn = false;
        private bool inAir = false;

        public Obstacle(World world, Texture2D texture, Vector2 size)
        {
            body = BodyFactory.CreateRectangle(world, size.X * CoordinateHelper.pixelToUnit, size.Y * CoordinateHelper.pixelToUnit, 1);
            this.size = size;
            this.texture = texture;
        }

        public Body Body { get => body; set => body = value; }
        public Texture2D Texture { get => texture; set => texture = value; }
        public Vector2 Size { get => size; set => size = value; }
        public Vector2 Position { get => body.Position * CoordinateHelper.unitToPixel; set => body.Position = value * CoordinateHelper.pixelToUnit; }
        public bool KinesisOn { get => kinesisOn; set => kinesisOn = value; }
        public bool InAir { get => inAir; set => inAir = value; }

        public Rectangle physicsObjRecToDraw()
        {
            Rectangle destination = new Rectangle
            (
                (int)Position.X ,
                (int)Position.Y,
                (int)Size.X,
                (int)Size.Y
            );
            return destination;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Position.Y < 0)
            {
                //Console.WriteLine(this.Position.Y);
                //this.Body.BodyType = BodyType.Static;
                this.Body.Enabled = false;

                //this.Body.GravityScale = 1;

            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            Rectangle destination = new Rectangle
           (
               (int)Position.X,
               (int)Position.Y + (int)Size.Y / 2,
               (int)Size.X,
               (int)Size.Y
           );
            //spriteBatch.Draw(texture, destination, null, Color.White);
            spriteBatch.Draw(texture, destination, null, Color.White, body.Rotation, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);

        }
    }
}


