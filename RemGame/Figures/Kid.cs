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
using FarseerPhysics.Collision;


namespace RemGame
{
    class Kid:Component
    {
        private World world;
        private Texture2D texture;
        private Vector2 size;
        private float mass;
        private Vector2 position;

        private SpriteFont f;

        PhysicsView pv1;
        PhysicsView pv2;


        private PhysicsObject torso;

        private PhysicsObject wheel;

        /// <tmp>
        private PhysicsObject mele;
        private PhysicsObject shoot;

        Vector2 shootBase;
        Vector2 shootDirection;
        Texture2D shootTexture;
        /// <tmp>
        /// 
        private bool isMeleAttacking = false;
        private bool isRangeAttacking = false;

        private RevoluteJoint axis1;



        private const float SPEED = 1.0f;
        private float speed = SPEED;
        float wheelSpeed = 0;
        private bool isMoving = false;
        private bool isJumping = false;
        private bool isSliding = false;
        private bool isBending = false;
        private Movement direction = Movement.Right;

        private bool showText = false;

        private DateTime previousJump = DateTime.Now;   // time at which we previously jumped
        private const float jumpInterval = 1.1f;        // in seconds
        private Vector2 jumpForce = new Vector2(0, -8); // applied force when jumping

        private DateTime previousSlide = DateTime.Now;   // time at which we previously jumped
        private const float slideInterval = 0.7f;        // in seconds
        private Vector2 slideForce = new Vector2(7, 0); // applied force when jumping

        private DateTime previousBend = DateTime.Now;   // time at which we previously jumped
        private const float bendInterval = 0.1f;        // in seconds
        

        private AnimatedSprite anim;
        private AnimatedSprite[] animations = new AnimatedSprite[2];


        KeyboardState keyboardState;
        KeyboardState prevKeyboardState = Keyboard.GetState();

        MouseState currentMouseState;
        MouseState previousMouseState = Mouse.GetState();


        public AnimatedSprite Anim { get => anim; set => anim = value; }
        public AnimatedSprite[] Animations { get => animations; set => animations = value; }
        internal Movement Direction { get => direction; set => direction = value; }
        public Vector2 Position { get => torso.Position; }
        public bool IsJumping { get => isJumping; set => isJumping = value; }
        public bool IsMoving { get => isMoving; set => isMoving = value; }
        public bool IsBending { get => isBending; set => isBending = value; }
        public float WheelSpeed { get => wheelSpeed; set => wheelSpeed = value; }

        public Kid(World world, Texture2D torsoTexture, Texture2D wheelTexture,Texture2D bullet, Vector2 size, float mass, Vector2 startPosition,bool isBent,SpriteFont f)
        {
            this.world = world;
            this.size = size;
            this.texture = torsoTexture;
            this.mass = mass / 2.0f;
            shootTexture = bullet;
            this.f = f;

            IsMoving = false;
            Vector2 torsoSize = new Vector2(size.X, size.Y-size.X/2.0f);
            float wheelSize = size.X ;

            
            // Create the torso
            torso = new PhysicsObject(world, torsoTexture, torsoSize.X, mass / 2.0f);
            torso.Position = startPosition;
            position = torso.Position;
            
            
            // Create the feet of the body
            
            wheel = new PhysicsObject(world, torsoTexture, wheelSize, mass / 2.0f);
            wheel.Position = torso.Position + new Vector2(0, 96);

            torso.Body.Friction = 50.0f;
            wheel.Body.Friction = 50.0f;

            
            // Create a joint to keep the torso upright
            JointFactory.CreateAngleJoint(world, torso.Body,new Body(world));

            torso.Body.CollisionCategories = Category.Cat10;
            wheel.Body.CollisionCategories = Category.Cat11;

            torso.Body.CollidesWith = Category.Cat1;
            wheel.Body.CollidesWith = Category.Cat1;

            // Connect the feet to the torso
            axis1 = JointFactory.CreateRevoluteJoint(world, torso.Body, wheel.Body, Vector2.Zero);
            axis1.CollideConnected = false;
            axis1.MotorEnabled = true;
            axis1.MotorSpeed = 0.0f;
            axis1.MaxMotorTorque = 12.0f;

            

            

            pv1 = new PhysicsView(torso.Body,torso.Position,torso.Size, f);
            pv2 = new PhysicsView(wheel.Body,wheel.Position, wheel.Size, f);
        }

        

        public void Move(Movement movement)
        {   
            if(!IsBending)
            speed = SPEED;
            switch (movement)
            {
                case Movement.Left:
                    axis1.MotorSpeed = -MathHelper.TwoPi * speed;
                    break;

                case Movement.Right:
                    axis1.MotorSpeed = MathHelper.TwoPi * speed;
                    break;

                case Movement.Stop:
                    
                    if (axis1.MotorSpeed != 0)
                    { 
                       axis1.MotorSpeed = 0;
                       axis1.BodyB.ResetDynamics();        
                    }

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
            isJumping = true;
            
                if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
                {

                    torso.Body.ApplyLinearImpulse(jumpForce);
                    previousJump = DateTime.Now;
                }
            
            
        }
        
        //should create variables for funciton
        public void Slide(Movement dir)
        {
            
            speed = SPEED;
            if (!isJumping)
            {
                isSliding = true;
                IsMoving = true;
                torso.Body.CollidesWith = Category.None;

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
        }

        public void meleAttack()
        {
            isMeleAttacking = true;
            mele = new PhysicsObject(world, shootTexture, 30, 1);
            mele.Body.Mass = 4.0f;
            mele.Body.IgnoreGravity = true;
            mele.Position = new Vector2(torso.Position.X + torso.Size.X / 2, torso.Position.Y + torso.Size.Y / 2);
            mele.Body.ApplyLinearImpulse(new Vector2(30, 0));
            //mele.Body.FixtureList[0].OnCollision = dispose;
            mele.Body.OnCollision += new OnCollisionEventHandler(Mele_OnCollision);
 
        }

        bool Mele_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            //if (fixtureB.CollisionCategories != Category.Cat1)
            //{
                isMeleAttacking = false;
                mele.Body.Dispose();
                return true;
            //}
            
            
        }
        public void rangedShoot(Vector2 shootForce)
        {
            isRangeAttacking = true;
            shoot = new PhysicsObject(world, shootTexture, 30, 1);
            //Console.WriteLine("end: " + currentMouseState.Position.X + " " + currentMouseState.Position.Y);
            shoot.Position = new Vector2(torso.Position.X + torso.Size.X / 2, torso.Position.Y + torso.Size.Y / 2);
            shoot.Body.Mass = 2.0f;
            shoot.Body.ApplyForce(shootForce);
            shoot.Body.OnCollision += new OnCollisionEventHandler(Shoot_OnCollision);

        }
        bool Shoot_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.CollisionCategories == Category.Cat10 || fixtureB.CollisionCategories == Category.Cat11)
            {

                isRangeAttacking = false;
                shoot.Body.Dispose();
                return true;
            }
            else
            {

                return true;
            }
        }

        public void Kinesis(Obstacle obj,MouseState currentMouseState)
        {
            obj.KinesisOn = true;

            if (obj.Position.Y > 0 )
            {
                if (currentMouseState.RightButton == ButtonState.Pressed)
                {
                    obj.Body.CollidesWith = Category.None; 
                    obj.Body.BodyType = BodyType.Dynamic;
                    //obj.Body.Mass = 0.0f;
                    obj.Body.GravityScale = 0.1f;
                    obj.Body.Rotation = 0.1f;
                    obj.Body.ApplyForce(new Vector2(0, -6.0f));
                    
                    
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

        public void bend()
        {
            if (!isJumping)
            {
                //torso.Body.IgnoreCollisionWith(wheel.Body);
                torso.Position = new Vector2(wheel.Position.X, wheel.Position.Y );
                speed = SPEED / 2;
                
            }

        }

        public void Scene()
        {
            
                Move(Movement.Right);
                isMoving = true;
                direction = Movement.Right;
            
        }

        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
            
            //bentPosition = new Vector2(torso.Position.X,torso.Position.Y-10);

            if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
                isJumping = false;
            if ((DateTime.Now - previousSlide).TotalSeconds >= slideInterval)
            {
                isSliding = false;
                torso.Body.CollidesWith = Category.All;
            }

            anim = animations[(int)direction];

            if (IsMoving) // apply animation
                Anim.Update(gameTime);
            else //player will appear as standing with frame [1] from the atlas.
                Anim.CurrentFrame = 1;

            IsMoving = false;

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                Move(Movement.Left);
                direction = Movement.Left;
                IsMoving = true;

            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                Move(Movement.Right);
                direction = Movement.Right;
                IsMoving = true;

            }
            else
            {
                IsMoving = false;
                Move(Movement.Stop);
            }
            
            //if statment should changed
            if (keyboardState.IsKeyDown(Keys.Space) && !(prevKeyboardState.IsKeyDown(Keys.Space)))
            {
                Jump();
            }
           
            if (isJumping)
            {
                if (keyboardState.IsKeyDown(Keys.Right) && (!prevKeyboardState.IsKeyDown(Keys.Right)))
                {
                    wheel.Body.ApplyLinearImpulse(new Vector2(1.5f, 0));
                }

                else if (keyboardState.IsKeyDown(Keys.Left) && (!prevKeyboardState.IsKeyDown(Keys.Left)))
                {
                    wheel.Body.ApplyLinearImpulse(new Vector2(-1.5f, 0));
                }
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

                //Console.WriteLine("start: " + currentMouseState.Position.X + " " + currentMouseState.Position.Y);

            }
            if (currentMouseState.LeftButton == ButtonState.Released && (previousMouseState.LeftButton == ButtonState.Pressed))
            {
                shootBase = new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y);
                Vector2 shootForce = new Vector2((shootDirection.X - shootBase.X), (shootDirection.Y - shootBase.Y));
                if(shootForce.X > 5 || shootForce.X < -5 || shootForce.Y > 5 || shootForce.Y < -5)
                rangedShoot(shootForce*4);
            }
             

            if (keyboardState.IsKeyDown(Keys.LeftControl) && !(prevKeyboardState.IsKeyDown(Keys.LeftControl)))
            {
                meleAttack();

            }


            if (Position.X < 400)
            {
                wheel.Body.ApplyLinearImpulse(new Vector2(1,0));
                Scene();
            }
            if (Position.X > 400 && Position.X < 410)
            {
                showText = true;
            }
            else
                showText = false;



            if (keyboardState.IsKeyDown(Keys.Down))
            {
               // axis1.BodyA.IgnoreCollisionWith(axis1.BodyB);
                //axis1.BodyB.IgnoreCollisionWith(axis1.BodyA);
                if(torso.Position.Y +96 != wheel.Position.Y+48)
                bend();
                else
                {
                    torso.Position = new Vector2(torso.Position.X, torso.Position.Y - 48); 
                }

                IsBending = true;
                /*
                if(keyboardState.IsKeyDown(Keys.Space) && !(prevKeyboardState.IsKeyDown(Keys.Space)))
                {
                    wheel.Body.CollidesWith = Category.Cat30;
                }
                */
            }
            
            if (keyboardState.IsKeyUp(Keys.Down)&& prevKeyboardState.IsKeyDown(Keys.Down))
            {
                IsBending = false;

            }

           
            WheelSpeed = wheel.Body.AngularVelocity/4;
            
            //mele.Update(gameTime);
            previousMouseState = currentMouseState;
            prevKeyboardState = keyboardState;

            

        }

        //needs to be changed

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
            torso.Draw(gameTime,spriteBatch);
            //Rectangle dest = torso.physicsObjRecToDraw();
            //dest.Height = dest.Height+(int)wheel.Size.Y/2;
            //dest.Y = dest.Y + (int)wheel.Size.Y/2;

            //anim.Draw(spriteBatch, dest, torso.Body);

            if (isMeleAttacking && !(mele.Body.IsDisposed))
                mele.Draw(gameTime, spriteBatch);

           if (isRangeAttacking && !(shoot.Body.IsDisposed))
                shoot.Draw(gameTime, spriteBatch);

            if (showText)
            {
                
                spriteBatch.DrawString(f, "ho HEY,im ron i got schyzofrenia", new Vector2(Position.X + size.X, Position.Y), Color.White);
            }
            

            pv1.Draw(gameTime, spriteBatch);
            pv2.Draw(gameTime, spriteBatch);
            //spriteBatch.DrawString(f, WheelSpeed.ToString(), new Vector2(Position.X + size.X, Position.Y), Color.White);


            //wheel.Draw(gameTime,spriteBatch);
        }


    }
}
