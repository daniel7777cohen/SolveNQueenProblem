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
        private World world;
        private Texture2D texture;
        private Vector2 size;
        private float mass;
        private Vector2 bentPosition;

        private PhysicsObject torso;
        private PhysicsObject bentTorso;
        private PhysicsObject wheel;
        private PhysicsObject bentWheel;

        /// <tmp>
        private PhysicsObject mele;
        /// <tmp>
        /// 
        private bool isAttacking = false;
        private RevoluteJoint axis1;
        private RevoluteJoint axis2;

        private float speed = 1.3f;
        private bool isMoving = false;
        private bool isStanding = true;
        private Movement direction = Movement.Right;



        private DateTime previousJump = DateTime.Now;   // time at which we previously jumped
        private const float jumpInterval = 0.7f;        // in seconds
        private Vector2 jumpForce = new Vector2(0, -2); // applied force when jumping

        private DateTime previousSlide = DateTime.Now;   // time at which we previously jumped
        private const float slideInterval = 0.1f;        // in seconds
        private Vector2 slideForce = new Vector2(2, 0); // applied force when jumping



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

        public Kid(World world, Texture2D torsoTexture, Texture2D wheelTexture,Texture2D bullet, Vector2 size, float mass, Vector2 startPosition,Game game)
        {
            this.world = world;
            this.size = size;
            this.texture = torsoTexture;
            this.mass = mass / 2.0f;

            isMoving = false;
            Vector2 torsoSize = new Vector2(size.X, size.Y - size.X / 2.0f);
            float wheelSize = torsoSize.X ;

            Vector2 bentTorsoSize = new Vector2(size.X, size.Y - size.X / 2.0f);
            float bentWeheelSize = torsoSize.X;

            // Create the torso
            torso = new PhysicsObject(world, torsoTexture, torsoSize.X, mass / 2.0f);
            torso.Position = startPosition;

            bentTorso = new PhysicsObject(world, torsoTexture, torsoSize.X, mass / 2.0f);
            bentTorso.Position = startPosition;

            // Create the feet of the body
            wheel = new PhysicsObject(world, torsoTexture, wheelSize, mass / 2.0f);
            wheel.Position = torso.Position + new Vector2(0, torsoSize.Y / 2.0f);

            bentWheel = new PhysicsObject(world, torsoTexture, wheelSize, mass / 2.0f);
            bentWheel.Position = torso.Position + new Vector2(0, torsoSize.Y / 2.0f);

            wheel.Body.Friction = 10.0f;
            bentWheel.Body.Friction = 10.0f;
            // Create a joint to keep the torso upright
            JointFactory.CreateAngleJoint(world, torso.Body,new Body(world));
            JointFactory.CreateAngleJoint(world, bentTorso.Body, new Body(world));

            // Connect the feet to the torso
            axis1 = JointFactory.CreateRevoluteJoint(world, torso.Body, wheel.Body, Vector2.Zero);
            axis2 = JointFactory.CreateRevoluteJoint(world, bentTorso.Body, bentWheel.Body, Vector2.Zero);

            axis1.CollideConnected = false;
            axis2.CollideConnected = false;


            axis1.MotorEnabled = true;
            axis2.MotorEnabled = true;

            axis1.MotorSpeed = 0;
            axis2.MotorSpeed = 0;

            axis1.MaxMotorTorque = 10;
            axis2.MaxMotorTorque = 10;
            

            mele = new PhysicsObject(world, bullet, 30, 200);
            mele.Body.Mass = 0.5f;
            
        }

        public void Move(Movement movement)
        {
            switch (movement)
            {
                case Movement.Left:
                    if(!keyboardState.IsKeyDown(Keys.LeftShift))
                    axis1.MotorSpeed = -MathHelper.TwoPi * speed;
                    break;

                case Movement.Right:
                    axis1.MotorSpeed = MathHelper.TwoPi * speed;
                    break;

                case Movement.Stop:
                    axis1.MotorSpeed = 0;
                    break;
            }
        }

        //private bool dispose()
        //{
            //isAttacking = false;
          //  return true;
        //}

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
            if ((DateTime.Now - previousSlide).TotalSeconds >= slideInterval)
            {

                if (dir == Movement.Right)
                    torso.Body.ApplyLinearImpulse(slideForce);
                else
                {
                    slideForce.X = slideForce.X * -1; 
                    torso.Body.ApplyLinearImpulse(slideForce);
                    slideForce.X = slideForce.X * -1;
                }

                previousSlide = DateTime.Now;

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

        public void Kinesis(PhysicsObject obj)
        {
            obj.Body.BodyType = BodyType.Dynamic;
            obj.Body.ApplyForce (new Vector2(0, -5f));
        }
        /*
        public void bent(Vector2 position)
        {
            //torso.Size = new Vector2(torso.Size.X, torso.Size.Y - 15f);
            AdjustBodeis()

        }

        public void standUp(Vector2 position)
        {
            //torso.Size = new Vector2(torso.Size.X, torso.Size.Y + 15f);
        }
        */
        //1 to adjust body to normal , -1 to maek body smaller
        


        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            bentPosition = new Vector2(torso.Position.X,torso.Position.Y-10);
            

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

            if (keyboardState.IsKeyDown(Keys.LeftControl) && !(prevKeyboardState.IsKeyDown(Keys.LeftControl)))
            {
                isStanding = false;
            }

            if (keyboardState.IsKeyUp(Keys.LeftControl) && (prevKeyboardState.IsKeyDown(Keys.LeftControl)))
            {
                isStanding = true;
            }


            if ((currentMouseState.LeftButton == ButtonState.Pressed) && !(previousMouseState.LeftButton == ButtonState.Pressed))
            {
                meleAttack();
            }
            prevKeyboardState = keyboardState;

            
        }

        //needs to be changed

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {   

            //torso.Draw(spriteBatch);
            //if (isStanding)
                anim.Draw(spriteBatch, torso.Position, new Vector2(torso.Size.X, torso.Size.Y - 10));
            //else
                anim.Draw(spriteBatch, bentTorso.Position, new Vector2(bentTorso.Size.X, bentTorso.Size.Y - 10));
            if (isAttacking)
            mele.Draw(gameTime, spriteBatch);
            
            //wheel.Draw(spriteBatch);
        }

        
    }
}
