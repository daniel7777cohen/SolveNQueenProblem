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
using Microsoft.Xna.Framework.Content;

namespace RemGame
{
    class Enemy 
    {
        private static ContentManager content;
        Random random;

        bool pingPong = false;
        bool Ghost = false;

        private int health = 5;
        private World world;
        private Map map;
        private Vector2 size;
        private float mass;
        private Vector2 position;
        private Vector2 lastPosition;
        private Point gridLocation;

        private int distance;
        private int oldDistance;

        private PhysicsObject torso;
        private PhysicsObject wheel;

        private PhysicsObject mele;


        private DateTime previousWalk = DateTime.Now;   // time at which we previously jumped
        private const float walkInterval = 3.0f;        // in seconds

        private static Random r = new Random();

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
        private bool collideRight = false;
        private bool collideLeft = false;



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
        public int Health { get => health; set => health = value; }


        public Enemy(World world,Vector2 size, float mass, Vector2 startPosition, bool isBent, SpriteFont f,int newDistance,Map map)
        {
            this.world = world;
            this.map = map;
            this.size = size;
            this.mass = mass / 2.0f;

            isMoving = false;
            Vector2 torsoSize = new Vector2(size.X, size.Y - size.X / 2.0f);
            float wheelSize = size.X;

            // Create the torso
            torso = new PhysicsObject(world, null, torsoSize.X, mass / 2.0f);
            torso.Position = startPosition;
            position = torso.Position;
            lastPosition = position;

            int rInt = r.Next(192, 320);
            distance = rInt; 
            oldDistance = distance;

            // Create the feet of the body
            wheel = new PhysicsObject(world, null, wheelSize, mass / 2.0f);
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

            torso.Body.CollisionCategories = Category.Cat20;
            wheel.Body.CollisionCategories = Category.Cat21;

            torso.Body.CollidesWith = Category.Cat1 | Category.Cat28 | Category.Cat7;
            wheel.Body.CollidesWith = Category.Cat1 | Category.Cat28 | Category.Cat7;

            torso.Body.OnCollision += new OnCollisionEventHandler(Platform_OnCollision);

            torso.Body.OnCollision += new OnCollisionEventHandler(HitByPlayer);
            wheel.Body.OnCollision += new OnCollisionEventHandler(HitByPlayer);

            pv1 = new PhysicsView(torso.Body, torso.Position, torso.Size, f);
            pv2 = new PhysicsView(wheel.Body, wheel.Position, wheel.Size, f);

            Animations[0] = new AnimatedSprite(Content.Load<Texture2D>("Player/playerLeft"), 1, 4, new Rectangle(0, -20, 90, 90), 0.15f);
            Animations[1] = new AnimatedSprite(Content.Load<Texture2D>("Player/playerRight"), 1, 4, new Rectangle(0, -20, 90, 90), 0.15f);

            shootTexture = shootTexture = Content.Load<Texture2D>("Player/bullet");

        }
        public static ContentManager Content { protected get => content; set => content = value; }
        public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
        public AnimatedSprite Anim { get => anim; set => anim = value; }
        public AnimatedSprite[] Animations { get => animations; set => animations = value; }
        internal Movement Direction { get => direction; set => direction = value; }
        public Vector2 Position { get => torso.Position; }
        public int Distance { get => distance; set => distance = value; }
        public Point GridLocation { get => gridLocation; set => gridLocation = value; }

        public void Move(Movement movement)
        {
            switch (movement)
            {
                case Movement.Left:
                    lookingRight = false;
                     axis1.MotorSpeed = -MathHelper.TwoPi * speed;
                    anim = animations[0];
                    break;

                case Movement.Right:
                    lookingRight = true;
                    axis1.MotorSpeed = MathHelper.TwoPi * speed;
                    anim = animations[1];

                    break;

                case Movement.Stop:
                    axis1.MotorSpeed = 0;
                    break;
            }
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
            
            if ((DateTime.Now - previousShoot).TotalSeconds >= randomInterval && !Ghost)
            {
                isMeleAttacking = true;
                mele = new PhysicsObject(world, shootTexture, 15, 1);
                mele.Body.CollisionCategories = Category.Cat30;
                mele.Body.CollidesWith = Category.Cat10 | Category.Cat11 | Category.Cat1;
                
                mele.Body.Mass = 1.0f;
                mele.Body.IgnoreGravity = true;
                mele.Position = new Vector2(torso.Position.X + torso.Size.X / 2, torso.Position.Y);
                int dir;
                if (lookingRight)
                    dir = 1;
                else
                    dir = -1;
                mele.Body.ApplyLinearImpulse(new Vector2(10*dir, 0));
                if (isPlayerAlive)
                    mele.Body.OnCollision += new OnCollisionEventHandler(Mele_OnCollision);
                previousShoot = DateTime.Now;

            }
            else
            isAttacking = false;
            
        }

        bool Platform_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.CollisionCategories == Category.Cat7)
            {
                if (direction == Movement.Right)
                {
                    collideRight = true;
                    
                }
                else
                {
                    collideLeft = true;

                }

                return true;
            }

            return false;

        }

        bool Mele_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            
            isMeleAttacking = false;
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
                    }
                    else
                    {
                        torso.Body.Dispose();
                        wheel.Body.Dispose();

                    }
                    return true;
                }
            
            return true;

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

            anim = Animations[0];
            anim = Animations[(int)direction];

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
                if (gridLocation.X > 3 && gridLocation.Y >3)
                for (int i = 1; i <= 2; i++)
                {
                    if (map.getGridObject(gridLocation.X + i, gridLocation.Y) == 7 || map.getGridObject(gridLocation.X + i, gridLocation.Y + 1) == 7 || map.getGridObject(gridLocation.X + i, gridLocation.Y - 1) == 7)
                    {
                        collideRight = true;
                    }
                    if (map.getGridObject(gridLocation.X - i, gridLocation.Y) == 7 || map.getGridObject(gridLocation.X - i, gridLocation.Y + 1) == 7 || map.getGridObject(gridLocation.X - i, gridLocation.Y - 1) == 7)
                    {
                        collideLeft = true;
                    }

                }

                int dir = 0;
                if (playerPosition.X > Position.X - 200 && playerPosition.X < Position.X + 200 && isPlayerAlive &&(playerPosition.Y < position.Y + size.X * 2 && playerPosition.Y + 3 * size.X > position.Y))
                    {
                    speed = SPEED;
                    
                    isBackToLastPos = false;
                
                        if (playerPosition.X < Position.X - 150 )
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

                    if (!(pingPong) && Position.X <= lastPosition.X + distance - size.X / 2 && !(Ghost)&&!collideRight)
                    {
                        
                            Move(Movement.Right);
                            isMoving = true;
                            direction = Movement.Right;
                        
                    }

                    else if (!(Ghost))
                    {
                        pingPong = true;
                        if (pingPong && Position.X >= lastPosition.X + size.X / 2 - distance &&!collideLeft)

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
                    collideLeft = false;
                    collideRight = false;
                }
                
            }

            if (isMeleAttacking)
                mele.Update(gameTime);
                
            previousMouseState = currentMouseState;
            prevKeyboardState = keyboardState;

        }
  
        //needs to be changed

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch,SpriteFont font)
        {

            //torso.Draw(gameTime,spriteBatch);
            Rectangle dest = torso.physicsObjRecToDraw();
            //dest.Height = dest.Height+(int)wheel.Size.Y/2;
            //dest.Y = dest.Y + (int)wheel.Size.Y/2;
            if(!torso.Body.IsDisposed&& anim!=null)
                anim.Draw(spriteBatch, dest, torso.Body,false);
            //pv1.Draw(gameTime, spriteBatch);
            //pv2.Draw(gameTime, spriteBatch);

            if (isMeleAttacking && !(mele.Body.IsDisposed))
                mele.Draw(gameTime, spriteBatch);

            //wheel.Draw(gameTime,spriteBatch);

            //////////////////////////////////////////FOR CHEACKING ENEMY INDICATORS OF SORRUNDING////////////////////////////////////
            if (gridLocation.X > 5)
            {
                Console.WriteLine(gridLocation);
                //Console.WriteLine("looking to the right:");
                for (int i = 1; i <= 2; i++)
                {
                    if (map.getGridObject(gridLocation.X + i, gridLocation.Y) == 7 || map.getGridObject(gridLocation.X + i, gridLocation.Y+1) == 7 || map.getGridObject(gridLocation.X + i, gridLocation.Y - 1) == 7)
                    {
                        spriteBatch.DrawString(font, "COLLISION RIGHT", new Vector2(gridLocation.X*64 , gridLocation.Y*64 + 40), Color.White);
                        collideRight = true;
                        // Console.Write("COLLISION RIGHT");
                    }

                }
               //Console.WriteLine("looking to the left:");
                for (int i = 1; i <= 4; i++)
                {
                    if (map.getGridObject(gridLocation.X - i, gridLocation.Y) == 7)
                    {
                       // Console.Write("COLLISION Left");
                    }
                    //Console.WriteLine("looking at index: " + (gridLocation.X - i) + " " + map.getGridObject(gridLocation.X - i, gridLocation.Y));

                }
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }


    }
}
