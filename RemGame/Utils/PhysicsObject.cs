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
    public class PhysicsObject:Component
    {

        private Body body;
        private Texture2D texture;
        private Vector2 size;
        private float diameter;
        private float radius;
        private World world;
        private bool kinesisOn = false;

        public PhysicsObject(World world, Texture2D texture, float diameter, float mass)
        {
            radius = diameter / 2.0f;
            size = new Vector2(diameter,diameter);
            body = BodyFactory.CreateCircle(world, (radius) * CoordinateHelper.pixelToUnit, 1);
            body.BodyType = BodyType.Dynamic;
            
            
            this.texture = texture;
            this.diameter = diameter;
            this.world = world;

        }

        public Body Body { get => body; set => body = value; }
        public Texture2D Texture { get => texture; set => texture = value; }
        public Vector2 Size { get => size; set => size = value; }
        public Vector2 Position { get => body.Position * CoordinateHelper.unitToPixel; set => body.Position = value * CoordinateHelper.pixelToUnit; }
        public bool KinesisOn { get => kinesisOn; set => kinesisOn = value; }

        public Rectangle physicsObjRecToDraw()
        {
            Rectangle destination = new Rectangle
            (
                (int)Position.X - (int)radius,
                (int)Position.Y,
                (int)Size.X,
                (int)Size.Y
            );
            return destination;
        }

        public override void Update(GameTime gameTime)
        {
           if(this.Position.Y < 0)
            {
                //Console.WriteLine(this.Position.Y);
                //this.Body.BodyType = BodyType.Static;
                this.Body.Enabled = false; 
                
               //this.Body.GravityScale = 1;

            }
        }

        public override void Draw(GameTime gameTime,SpriteBatch spriteBatch)
        {
            

            Rectangle destination = new Rectangle
            (
                (int)Position.X,
                (int)Position.Y+85,
                (int)Size.X*3,
                (int)Size.Y*4
            );
            //spriteBatch.Draw(texture, destination, null, Color.White);
            spriteBatch.Draw(texture, destination, null, Color.White, body.Rotation, new Vector2(texture.Width/2, texture.Height/2), SpriteEffects.None, 0);
            
        }
    }
}


