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

            this.Size = size;
            this.texture = texture;
        }

        public Vector2 Position
        {
            get { return body.Position * CoordinateHelper.unitToPixel; }
            set { body.Position = value * CoordinateHelper.pixelToUnit; }
        }


        public Vector2 Size
        {
            get { return size * CoordinateHelper.unitToPixel; }
            set { size = value * CoordinateHelper.pixelToUnit; }
        }

        public Body Body { get => body; set => body = value; }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(Size.X / (float)texture.Width, Size.Y / (float)texture.Height);
            spriteBatch.Draw(texture, Position, null, Color.White, body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), scale, SpriteEffects.None, 0);
        }
    }
}
