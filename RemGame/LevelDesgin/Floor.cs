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
    class Floor
    {
        private Body body;
        private Texture2D texture;
        private Vector2 size;

        public Floor(World world, Texture2D texture, Vector2 size)
        {
            body = BodyFactory.CreateRectangle(world, size.X * CoordinateHelper.pixelToUnit, size.Y * CoordinateHelper.pixelToUnit, 1);
            this.size = size;
            this.texture = texture;
        }

        public Vector2 Position
        {
            get { return body.Position * CoordinateHelper.unitToPixel; }
            set { body.Position = value * CoordinateHelper.pixelToUnit; }
        }


        public Vector2 Size
        {
            get { return size; }
            set { size = value * CoordinateHelper.pixelToUnit; }
        }

        public Body Body { get => body; set => body = value; }

        public void Draw(SpriteBatch spriteBatch)
        {
            
            Rectangle destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            spriteBatch.Draw(texture, destinationRectangle, null, Color.White);
        }
    }
}
