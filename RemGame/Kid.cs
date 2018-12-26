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
    class Kid:Component
    {
        //private Body body;
        //private Texture2D texture;
        //private Vector2 size;
        private PhysicsObject torso;
        private PhysicsObject wheel;
        /// <tmp>
        private PhysicsObject mele;
        /// <tmp>
        private bool isAttacking = false;
        private RevoluteJoint axis;
        private float speed = 1.3f;
        private DateTime previousJump = DateTime.Now;   // time at which we previously jumped
        private const float jumpInterval = 0.7f;        // in seconds
        private Vector2 jumpForce = new Vector2(0, -2); // applied force when jumping
        private bool isMoving = false;

        private Movement direction = Movement.Right;

        private AnimatedSprite anim;
        private AnimatedSprite[] animations = new AnimatedSprite[2];


        KeyboardState keyboardState;
        KeyboardState prevKeyboardState = Keyboard.GetState();

        MouseState currentMouseState;
        MouseState previousMouseState = Mouse.GetState();


        public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
        public AnimatedSprite Anim { get => anim; set => anim = value; }
        public AnimatedSprite[] Animations { get => animations; set => animations = value; }
        internal Movement Direction { get => direction; set => direction = value; }

        //private Movement direction = Move.Right;

        public Kid(World world, Texture2D torsoTexture, Texture2D wheelTexture,Texture2D bullet, Vector2 size, float mass, Vector2 startPosition,Game game)
        {
            isMoving = false;
            Vector2 torsoSize = new Vector2(size.X, size.Y - size.X / 2.0f);
            float wheelSize = size.X ;

            // Create the torso
            torso = new PhysicsObject(world, torsoTexture, torsoSize.X, mass / 2.0f);
            torso.Position = startPosition;

            // Create the feet of the body
            wheel = new PhysicsObject(world, torsoTexture, wheelSize, mass / 2.0f);
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

            mele = new PhysicsObject(world, bullet, 30, 200);
            mele.Body.Mass = 0.5f;
            
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
                    if(!keyboardState.IsKeyDown(Keys.LeftShift))
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

        public void meleAttack()
        {
            IsAttacking = true;
            mele.Position = new Vector2(torso.Position.X + torso.Size.X / 2, torso.Position.Y + torso.Size.Y / 2);
            mele.Body.ApplyLinearImpulse(new Vector2(1, 0));
            //mele.Body.FixtureList[0].OnCollision = dispose;
           // IsAttacking = false;
        }

        private bool dispose()
        {
            isAttacking = false;
            return true;
        }

        public void Jump()

        {
            if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
            {
                torso.Body.ApplyLinearImpulse(jumpForce);
                previousJump = DateTime.Now;
            }
        }
        //should create variables for funciton
        public void Slide(Movement dir)
        {
            if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
            {
                
                if (dir == Movement.Right) 
                    torso.Body.ApplyLinearImpulse(new Vector2(2,0));
                else 
                    torso.Body.ApplyLinearImpulse(new Vector2(-2, 0));

                previousJump = DateTime.Now;

            }
        }

        public void Kinesis(PhysicsObject obj)
        {
            obj.Body.BodyType = BodyType.Dynamic;
            obj.Body.ApplyForce (new Vector2(0, -5f));
        }

        //public void bent()
        //{
          //  torso = new PhysicsObject(world, torsoTexture, torsoSize.X, mass / 2.0f);
        //}


        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            anim = animations[(int)direction];

            if (isMoving) // apply animation
                Anim.Update(gameTime);
            else //player will appear as standing with frame [1] from the atlas.
                Anim.CurrentFrame = 1;

            isMoving = false;

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                Move(Movement.Left);
                isMoving = true;
                direction = Movement.Left;

            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                Move(Movement.Right);
                isMoving = true;
                direction = Movement.Right;

            }
            else
            {
                Move(Movement.Stop);
                isMoving = false;

            }

            //if statment should changed
            if (keyboardState.IsKeyDown(Keys.Space) && !(prevKeyboardState.IsKeyDown(Keys.Space)))
            {
                isMoving = true;
                Jump();
            }

            if (keyboardState.IsKeyDown(Keys.LeftShift) && !(prevKeyboardState.IsKeyDown(Keys.LeftShift)))
            {
                if (keyboardState.IsKeyDown(Keys.Right))
                    Slide(Movement.Right);
                else if (keyboardState.IsKeyDown(Keys.Left))
                    Slide(Movement.Left);

            }

            if (currentMouseState.LeftButton == ButtonState.Pressed && !(previousMouseState.LeftButton == ButtonState.Pressed))
            {
                meleAttack();
            }
            prevKeyboardState = keyboardState;

            
        }

        //needs to be changed

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //torso.Draw(spriteBatch);
            anim.Draw(spriteBatch, torso.Position, new Vector2(torso.Size.X, torso.Size.Y-10));
            if (isAttacking)
            mele.Draw(spriteBatch);
            
            //wheel.Draw(spriteBatch);
        }

        
    }
}
