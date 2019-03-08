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
using FarseerPhysics.Dynamics.Contacts;

namespace RemGame
{
    class Enemy 
    {
        Random random;
        /// <summary>
        bool pingPong = false;
        bool Ghost = false;
        /// </summary>
        private int health = 5;
        private World world;
        private Texture2D texture;
        private Vector2 size;
        private float mass;
        private Vector2 position;
        private Vector2 lastPosition;
        private int distance;
        private int oldDistance;

        private PhysicsObject torso;
        private PhysicsObject wheel;

        private PhysicsObject mele;


        private DateTime previousWalk = DateTime.Now;   // time at which we previously jumped
        private const float walkInterval = 3.0f;        // in seconds

        private static Random r = new Random();
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
        private bool isMeleAttacking = false;
        private RevoluteJoint axis1;

        private bool isPlayerAlive = true;
        
        private const float SPEED = 0.3f;
        private float speed = SPEED;
        private bool isMoving = false;
        private bool isBackToLastPos = true;
        private Movement direction = Movement.Right;
        private bool lookingRight = true;



        private DateTime previousJump = DateTime.Now;   // time at which we previously jumped
        private const float jumpInterval = 0.7f;        // in seconds
        private Vector2 jumpForce = new Vector2(0, -5); // applied force when jumping

        private DateTime previousSlide = DateTime.Now;   // time at which we previously jumped
        private const float slideInterval = 0.1f;        // in seconds
        private Vector2 slideForce = new Vector2(4, 0); // applied force when jumping

        private DateTime previousShoot = DateTime.Now;   // time at which we previously jumped
        private const float shootInterval = 3.0f;        // in seconds

        private AnimatedSprite anim;
        private AnimatedSprite[] animations = new AnimatedSprite[2];


        KeyboardState keyboardState;
        KeyboardState prevKeyboardState = Keyboard.GetState();

        MouseState currentMouseState;
        MouseState previousMouseState = Mouse.GetState();

        Texture2D shootTexture;


        public Enemy(World world, Texture2D torsoTexture, Texture2D wheelTexture, Texture2D bullet, Vector2 size, float mass, Vector2 startPosition, bool isBent, SpriteFont f,int newDistance)
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

            lastPosition = position;

            //r = new Random();
            int rInt = r.Next(192, 320);

            distance = rInt;
            

            oldDistance = distance;


            shootTexture = bullet;
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

            torso.Body.CollisionCategories = Category.Cat20;
            wheel.Body.CollisionCategories = Category.Cat21;

            torso.Body.CollidesWith = Category.Cat1 | Category.Cat28;
            wheel.Body.CollidesWith = Category.Cat1 | Category.Cat28;


            torso.Body.OnCollision += new OnCollisionEventHandler(HitByPlayer);
            wheel.Body.OnCollision += new OnCollisionEventHandler(HitByPlayer);

        }

        public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
        public AnimatedSprite Anim { get => anim; set => anim = value; }
        public AnimatedSprite[] Animations { get => animations; set => animations = value; }
        internal Movement Direction { get => direction; set => direction = value; }
        public Vector2 Position { get => torso.Position; }
        public int Distance { get => distance; set => distance = value; }

        public void Move(Movement movement)
        {
            switch (movement)
            {
                case Movement.Left:
                    lookingRight = false;
                     axis1.MotorSpeed = -MathHelper.TwoPi * speed;
                    break;

                case Movement.Right:
                    lookingRight = true;
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
            random = new Random();
            double randomInterval = (random.NextDouble() * shootInterval + 1);
            

            Console.WriteLine(randomInterval);
            if ((DateTime.Now - previousShoot).TotalSeconds >= randomInterval && !Ghost)
            {
                isMeleAttacking = true;
                mele = new PhysicsObject(world, shootTexture, 15, 1);
                mele.Body.CollisionCategories = Category.Cat30;
                mele.Body.CollidesWith = Category.Cat10 | Category.Cat11 | Category.Cat1;
                
                //mele.Body.CollidesWith = Category.Cat1;

                mele.Body.Mass = 1.0f;
                mele.Body.IgnoreGravity = true;
                mele.Position = new Vector2(torso.Position.X + torso.Size.X / 2, torso.Position.Y);
                int dir;
                if (lookingRight)
                    dir = 1;
                else
                    dir = -1;
                mele.Body.ApplyLinearImpulse(new Vector2(10*dir, 0));
                //mele.Body.FixtureList[0].OnCollision = dispose;
                if (isPlayerAlive)
                    mele.Body.OnCollision += new OnCollisionEventHandler(Mele_OnCollision);
                previousShoot = DateTime.Now;

            }
            else
            isAttacking = false;
            
        }

        bool Mele_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            
            //Console.WriteLine("Mele_OnCollision");

            isMeleAttacking = false;
            //mele.Body.Enabled = false;
            mele.Body.Dispose();
            return true;
        

        }
        
        bool HitByPlayer(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
                if (fixtureB.CollisionCategories == Category.Cat28)
                {
                    if (health > 0)
                    {
                        health--;
                       // Console.WriteLine(health);
                    }
                    else
                    {
                        //torso.Body.Enabled = false;
                        //wheel.Body.Enabled = false;
                        torso.Body.Dispose();
                        wheel.Body.Dispose();

                    }
                    return true;
                }
            
            return true;

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



        public void Update(GameTime gameTime,Vector2 playerPosition, bool PlayerAlive)
        {
            if (!PlayerAlive)
                isPlayerAlive = false;
            keyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            //bentPosition = new Vector2(torso.Position.X,torso.Position.Y-10);

            anim = animations[(int)direction];

            if (isMoving) // apply animation
                Anim.Update(gameTime);
            else //player will appear as standing with frame [1] from the atlas.
                Anim.CurrentFrame = 1;


            isMoving = false;
            if (keyboardState.IsKeyDown(Keys.Q) && !(prevKeyboardState.IsKeyDown(Keys.Q)))
            {
                if (!Ghost)
                {
                    torso.Body.Enabled = false;
                    wheel.Body.Enabled = false;
                    isMoving = false;
                    Ghost = true;
                }
                else
                {

                    torso.Body.Enabled = true;
                    wheel.Body.Enabled = true;
                    Ghost = false;
                }

            }

            if (!torso.Body.IsDisposed) {
                if (playerPosition.X > Position.X - 200 && playerPosition.X < Position.X + 200 && isPlayerAlive)
                {
                    speed = SPEED;
                    int dir = 0;
                    isBackToLastPos = false;
                    if (playerPosition.X < Position.X - 150)
                    {
                        Move(Movement.Left);
                        isMoving = true;
                        direction = Movement.Left;
                        this.meleAttack();

                    }
                    else if (playerPosition.X > Position.X + 150)
                    {
                        Move(Movement.Right);
                        isMoving = true;
                        direction = Movement.Right;
                        this.meleAttack();

                    }

                    else
                    {
                        Move(Movement.Stop);
                        this.meleAttack();
                    }

                }
                else if (!isBackToLastPos)
                {
                    speed = 0.2f;
                    if (lastPosition.X + 4 < Position.X)
                    {

                        Move(Movement.Left);
                        isMoving = true;
                        direction = Movement.Left;
                    }
                    else if (lastPosition.X - 4 > Position.X)
                    {
                        Move(Movement.Right);
                        isMoving = true;
                        direction = Movement.Right;
                    }
                    else
                    {
                        Move(Movement.Stop);
                        isBackToLastPos = true;
                    }
                }
                else if (isBackToLastPos)
                {

                    speed = SPEED;

                    //torso.Body.OnCollision += OnCollisionEventHandler()             
                    if (!(pingPong) && Position.X <= lastPosition.X + distance - size.X / 2 && !(Ghost))
                    {
                        //Console.WriteLine("RIGHT")
                        Move(Movement.Right);
                        isMoving = true;
                        direction = Movement.Right;

                    }

                    else if (!(Ghost))
                    {
                        pingPong = true;
                        if (pingPong && Position.X >= lastPosition.X + size.X / 2 - distance)
                        {
                            Move(Movement.Left);
                            isMoving = true;
                            direction = Movement.Left;
                        }
                        else
                            pingPong = false;

                    }



                    else
                    {
                        Move(Movement.Stop);
                    }
                }
            }
            
            
                //}
            /*
                if (keyboardState.IsKeyUp(Keys.Down)&& prevKeyboardState.IsKeyDown(Keys.Down))
                {
                    //torso.Body.Enabled = true;

                }
                */
                if(isMeleAttacking)
                mele.Update(gameTime);
                
                if(!isPlayerAlive)

            previousMouseState = currentMouseState;
            prevKeyboardState = keyboardState;

        }

   
        //needs to be changed

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            //torso.Draw(gameTime,spriteBatch);
            Rectangle dest = torso.physicsObjRecToDraw();
            //dest.Height = dest.Height+(int)wheel.Size.Y/2;
            //dest.Y = dest.Y + (int)wheel.Size.Y/2;
            if(!torso.Body.IsDisposed)
            anim.Draw(spriteBatch, dest, torso.Body);
            //pv1.Draw(gameTime, spriteBatch);
            //pv2.Draw(gameTime, spriteBatch);

            if (isMeleAttacking && !(mele.Body.IsDisposed))
                mele.Draw(gameTime, spriteBatch);






            //wheel.Draw(gameTime,spriteBatch);
        }


    }
}
