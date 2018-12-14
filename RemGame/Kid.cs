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
using FarseerPhysics.Dynamics.Joints;

namespace RemGame
{
    class Kid
    {
        //private Body body;
        //private Texture2D texture;
        //private Vector2 size;
        private PhysicsObject torso;
        private PhysicsObject wheel;
        private RevoluteJoint axis;
        private float speed = 2.0f;
        private DateTime previousJump = DateTime.Now;   // time at which we previously jumped
        private const float jumpInterval = 1.0f;        // in seconds
        private Vector2 jumpForce = new Vector2(0, -2); // applied force when jumping
        private bool isMoving;
        //private Movement direction = Move.Right;

        public Kid(World world, Texture2D torsoTexture, Texture2D wheelTexture, Vector2 size, float mass, Vector2 startPosition)
        {
            isMoving = false;
            Vector2 torsoSize = new Vector2(size.X, size.Y - size.X / 2.0f);
            float wheelSize = size.X;

            // Create the torso
            torso = new PhysicsObject(world, torsoTexture, torsoSize.X, mass / 2.0f);
            torso.Position = startPosition;

            // Create the feet of the body
            wheel = new PhysicsObject(world, wheelTexture, wheelSize, mass / 2.0f);
            wheel.Position = torso.Position + new Vector2(0, torsoSize.Y / 2.0f);

            wheel.Body.Friction = 10.0f;

            // Create a joint to keep the torso upright
            JointFactory.CreateAngleJoint(world, torso.Body,new Body(world));

            // Connect the feet to the torso
            axis = JointFactory.CreateRevoluteJoint(world, torso.Body, wheel.Body, Vector2.Zero);
            axis.CollideConnected = false;

            axis.MotorEnabled = true;
            axis.MotorSpeed = 0;
            axis.MaxMotorTorque = 10;
        }

        //public Body Body { get => body; set => body = value; }
        //public Texture2D Texture { get => texture; set => texture = value; }
        //public Vector2 Size { get => size; set => size = value; }
        //public Vector2 Position { get => body.Position * CoordinateHelper.unitToPixel; set => body.Position = value * CoordinateHelper.pixelToUnit; }
        /*
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
        */
        public void Move(Movement movement)
        {
            switch (movement)
            {
                case Movement.Left:
                    axis.MotorSpeed = -MathHelper.TwoPi * speed;
                    break;

                case Movement.Right:
                    axis.MotorSpeed = MathHelper.TwoPi * speed;
                    break;

                case Movement.Stop:
                    axis.MotorSpeed = 0;
                    break;
            }
        }

        public void Jump()
        {
            if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
            {
                torso.Body.ApplyLinearImpulse(ref jumpForce);
                previousJump = DateTime.Now;
            }
        }

        //needs to be changed
        public void Draw(SpriteBatch spriteBatch)
        {
            torso.Draw(spriteBatch);
            //wheel.Draw(spriteBatch);
        }
    }
}
