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
    class Kid
    {
        private Body body;
        private Texture2D texture;
        private Vector2 size;
        private bool isMoving;
        private Move direction = Move.Right;

        public Kid(World world,Vector2 size,Texture2D texture)
        {
            isMoving = false;
            this.Size = size;
            this.Texture = texture;
            body = BodyFactory.CreateRectangle(world, size.X * CoordinateHelper.pixelToUnit, size.Y * CoordinateHelper.pixelToUnit, 1);
        }

        public Body Body { get => body; set => body = value; }
        public Texture2D Texture { get => texture; set => texture = value; }
        public Vector2 Size { get => size; set => size = value; }
        public Vector2 Position { get => body.Position * CoordinateHelper.unitToPixel; set => body.Position = value * CoordinateHelper.pixelToUnit; }

        public void update(GameTime gameTime)
        {
            KeyboardState kstate = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(Keys.Right))
            {
                direction = Move.Right;
                isMoving = true;
            }

            if (kstate.IsKeyDown(Keys.Left))
            {
                direction = Move.Left;
                isMoving = true;
            }

            if (kstate.IsKeyDown(Keys.Space))
            {
                direction = Move.Jump;
                isMoving = true;
            }


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 scale = new Vector2(Size.X / (float)texture.Width, Size.Y / (float)texture.Height);
            spriteBatch.Draw(texture, Position, null, Color.White, body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), scale, SpriteEffects.None, 0);
        }
    }
}
