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
        private World world;

        public PhysicsObject(World world, Texture2D texture, float diameter, float mass)
        {
            size = new Vector2(diameter, diameter);
            body = BodyFactory.CreateCircle(world, (diameter / 2.0f) * CoordinateHelper.pixelToUnit, 1);
            body.BodyType = BodyType.Dynamic;

            //this.size = size;
            this.texture = texture;
            this.diameter = diameter;
            this.world = world;

        }

        public Body Body { get => body; set => body = value; }
        public Texture2D Texture { get => texture; set => texture = value; }
        public Vector2 Size { get => size; set => size = value; }
        public Vector2 Position { get => body.Position * CoordinateHelper.unitToPixel; set => body.Position = value * CoordinateHelper.pixelToUnit; }

        public override void Draw(GameTime gameTime,SpriteBatch spriteBatch)
        {

             Rectangle destination = new Rectangle
            (
                (int)Position.X,
                (int)Position.Y,
                (int)Size.X,
                (int)Size.Y
            );

            spriteBatch.Draw(texture, destination, null, Color.White, body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), SpriteEffects.None, 0);
            
        }

        public override void Update(GameTime gameTime)
        {
            return;
        }
    }
}


