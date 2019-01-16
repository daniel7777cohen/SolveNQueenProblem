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
    class Enemy : Component
    {
        /// <summary>
        bool pingPong = false;
        bool Ghost = false;
        /// </summary>
        private World world;
        private Texture2D texture;
        private Vector2 size;
        private float mass;
        private Vector2 position;

        private PhysicsObject torso;

        private PhysicsObject wheel;
        /*
                /// <tmp>
                private PhysicsObject mele;
                private PhysicsObject shoot;

                Vector2 shootBase;
                Vector2 shootDirection;
                Texture2D shootTexture;
                /// <tmp>
                /// 
       */

        private PhysicsView pv1;
        private PhysicsView pv2;

        private bool isAttacking = false;
        private RevoluteJoint axis1;


        private float speed = 0.4f;
        private bool isMoving = false;
        private Movement direction = Movement.Right;



        private DateTime previousJump = DateTime.Now;   // time at which we previously jumped
        private const float jumpInterval = 0.7f;        // in seconds
        private Vector2 jumpForce = new Vector2(0, -5); // applied force when jumping

        private DateTime previousSlide = DateTime.Now;   // time at which we previously jumped
        private const float slideInterval = 0.1f;        // in seconds
        private Vector2 slideForce = new Vector2(4, 0); // applied force when jumping



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
        public Vector2 Position { get => torso.Position; }

        public Enemy(World world, Texture2D torsoTexture, Texture2D wheelTexture, Texture2D bullet, Vector2 size, float mass, Vector2 startPosition, bool isBent, SpriteFont f)
        {
            this.world = world;
            this.size = size;
            this.texture = torsoTexture;
            this.mass = mass / 2.0f;
            //shootTexture = bullet;

            isMoving = false;
            Vector2 torsoSize = new Vector2(size.X, size.Y - size.X / 2.0f);
            float wheelSize = size.X;

            // Create the torso
            torso = new PhysicsObject(world, torsoTexture, torsoSize.X, mass / 2.0f);
            torso.Position = startPosition;
            position = torso.Position;

            // Create the feet of the body
            wheel = new PhysicsObject(world, torsoTexture, wheelSize, mass / 2.0f);
            wheel.Position = torso.Position + new Vector2(0, torsoSize.X / 2);

            wheel.Body.Friction = 16.0f;

            // Create a joint to keep the torso upright
            JointFactory.CreateAngleJoint(world, torso.Body, new Body(world));


            // Connect the feet to the torso
            axis1 = JointFactory.CreateRevoluteJoint(world, torso.Body, wheel.Body, Vector2.Zero);
            axis1.CollideConnected = false;
            axis1.MotorEnabled = true;
            axis1.MotorSpeed = 0;
            axis1.MaxMotorTorque = 10;

            //mele = new PhysicsObject(world, bullet, 30, 1);
            //mele.Body.Mass = 1.5f;

            //shoot = new PhysicsObject(world, shootTexture, 30, 1);
            pv1 = new PhysicsView(torso.Body, torso.Position, torso.Size, f);
            pv2 = new PhysicsView(wheel.Body, wheel.Position, wheel.Size, f);

            torso.Body.CollisionCategories = Category.Cat10;
            wheel.Body.CollisionCategories = Category.Cat11;

            torso.Body.CollidesWith = Category.Cat1;
            wheel.Body.CollidesWith = Category.Cat1;

        }

        public void Move(Movement movement)
        {
            switch (movement)
            {
                case Movement.Left:
                    if (!keyboardState.IsKeyDown(Keys.LeftShift))
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
        /// <summary>
        /// /////////////////////////////////////////////////////////Abillities////////////////////////////////////////////////////////
        /// </summary>
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
           // mele.Position = new Vector2(torso.Position.X + torso.Size.X / 2, torso.Position.Y + torso.Size.Y / 2);
            //mele.Body.ApplyLinearImpulse(new Vector2(4, 0));
            //mele.Body.FixtureList[0].OnCollision = dispose;
            // IsAttacking = false;
        }

        public void Kinesis(Obstacle obj, MouseState currentMouseState)
        {
            obj.KinesisOn = true;

            if (obj.Position.Y > 0)
            {
                if (currentMouseState.RightButton == ButtonState.Pressed)
                {
                    obj.Body.BodyType = BodyType.Dynamic;
                    //obj.Body.Mass = 0.0f;
                    obj.Body.GravityScale = 0;
                    obj.Body.ApplyForce(new Vector2(0, -4.0f));
                }
                else
                {

                    //obj.Body.Mass = 0;
                    //obj.Body.ResetDynamics();
                    //obj.Body.BodyType = BodyType.Static;
                }
            }
            else
            {
                obj.Body.ResetDynamics();
                obj.Body.GravityScale = 1;
                //obj.Body.Mass = 0.0f;
                //obj.Body.Mass = 9.8f;
                if (currentMouseState.RightButton == ButtonState.Pressed)
                {
                    //obj.Body.BodyType = BodyType.Static;
                    //obj.Body.ApplyForce(new Vector2(4.0f, 0));
                    //obj.Body.BodyType = BodyType.Static;
                }
            }
            obj.KinesisOn = false;
        }

        public void bent()
        {
            torso.Body.IgnoreCollisionWith(wheel.Body);
            torso.Position = wheel.Position;
        }

        public override void Update(GameTime gameTime)
        {

            keyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            //bentPosition = new Vector2(torso.Position.X,torso.Position.Y-10);

                                   anim = animations[(int)direction];

                                    if (isMoving) // apply animation
                                        Anim.Update(gameTime);
                                    else //player will appear as standing with frame [1] from the atlas.
                                        Anim.CurrentFrame = 1;

                                    isMoving = false;
            if(keyboardState.IsKeyDown( Keys.Q)&&!(prevKeyboardState.IsKeyDown(Keys.Q)))
            {
                if (!Ghost)
                {
                    torso.Body.Enabled = false;
                    wheel.Body.Enabled = false;
                    isMoving=false;
                    Ghost = true;
                }
                else
                {

                    torso.Body.Enabled = true;
                    wheel.Body.Enabled = true;
                    Ghost = false;
                }

            }
           


            if (!(pingPong) && Position.X < 4000 - size.X / 2&&!(Ghost))
            {
                Move(Movement.Right);
                isMoving = true;
                direction = Movement.Right;
                
                /*
                Move(Movement.Left);
                isMoving = true;
                direction = Movement.Left;
                */
            }

            else if(!(Ghost))
            {
                pingPong = true;
                if (pingPong && Position.X -size.X/2> 200)
                {
                    Move(Movement.Left);
                    isMoving = true;
                    direction = Movement.Left;
                }
                else
                    pingPong = false;
                /*
                Move(Movement.Right);
                isMoving = true;
                direction = Movement.Right;
*/
            }
            /*
            else
            {
                Move(Movement.Stop);
                isMoving = false;

            }
            */
            /* 
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
                                        shootDirection = new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y);
                                        Console.WriteLine("start: " + currentMouseState.Position.X + " " + currentMouseState.Position.Y);

                                    }
                                    if (currentMouseState.LeftButton == ButtonState.Released && (previousMouseState.LeftButton == ButtonState.Pressed))
                                    {
                                        //IsAttacking = true;
                                        shootBase = new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y);
                                        Console.WriteLine("end: " + currentMouseState.Position.X + " " + currentMouseState.Position.Y);
                                        Vector2 shootForce = new Vector2((shootDirection.X - shootBase.X) / 4, (shootDirection.Y - shootBase.Y) / 4);
                                        shoot.Position = new Vector2(torso.Position.X + torso.Size.X / 2, torso.Position.Y + torso.Size.Y / 2);
                                        shoot.Body.ApplyForce(shootForce);

                                    }

                                    if (keyboardState.IsKeyDown(Keys.LeftControl) && !(prevKeyboardState.IsKeyDown(Keys.LeftControl)))
                                    {
                                        meleAttack();

                                    }


                                    if (keyboardState.IsKeyDown(Keys.Down))
                                    {
                                        bent();
                                    }
           */
            /*
            if (keyboardState.IsKeyUp(Keys.Down)&& prevKeyboardState.IsKeyDown(Keys.Down))
            {
                //torso.Body.Enabled = true;

            }
            */
            //mele.Update(gameTime);

            previousMouseState = currentMouseState;
            prevKeyboardState = keyboardState;

        }

        //needs to be changed

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            //torso.Draw(gameTime,spriteBatch);
            Rectangle dest = torso.physicsObjRecToDraw();
            //dest.Height = dest.Height+(int)wheel.Size.Y/2;
            //dest.Y = dest.Y + (int)wheel.Size.Y/2;

            anim.Draw(spriteBatch, dest, torso.Body);
            pv1.Draw(gameTime, spriteBatch);
            pv2.Draw(gameTime, spriteBatch);
            /*
                        if (isAttacking)
                            mele.Draw(gameTime, spriteBatch);

                            shoot.Draw(gameTime, spriteBatch);
            */



            //wheel.Draw(gameTime,spriteBatch);
        }


    }
}
