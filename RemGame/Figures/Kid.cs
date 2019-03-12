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
        private bool GameOver = false;

        private World world;
        private Texture2D texture;
        private Vector2 size;
        private float mass;
        private Vector2 position;

        private SpriteFont f;

        PhysicsView pv1;
        PhysicsView pv2;
        PhysicsView pv3;



        private PhysicsObject upBody;
        private PhysicsObject midBody;
        private PhysicsObject wheel;

        /// <tmp>
        private PhysicsObject shot;
        private List<PhysicsObject> shotList = new List<PhysicsObject>();

        private PhysicsObject shoot;

        Vector2 shootBase;
        Vector2 shootDirection;
        Texture2D shootTexture;
        Texture2D hearts;
        /// <tmp>
        /// 
        private bool isMeleAttacking = false;
        private bool isRangeAttacking = false;

        private RevoluteJoint axis1;
        private Joint axis2;


        private int health = 8;
        private bool isAlive = true;
        private const float SPEED = 12.0f;
        private float speed = SPEED;
        private float actualMovningSpeed=0;
        private bool isMoving = false;
        private bool isJumping = false;
        private bool isSliding = false;
        private bool isBending = false;
        private Movement direction = Movement.Right;
        private bool lookRight = true;

        private bool showText = false;

        private DateTime previousJump = DateTime.Now;   // time at which we previously jumped
        private const float jumpInterval = 1.2f;        // in seconds
        private Vector2 jumpForce = new Vector2(0, -5); // applied force when jumping

        private DateTime previousSlide = DateTime.Now;   // time at which we previously jumped
        private const float slideInterval = 1.0f;        // in seconds
        private Vector2 slideForce = new Vector2(5, 0); // applied force when jumping

        private DateTime previousShoot = DateTime.Now;   // time at which we previously jumped
        private const float shootInterval = 0.3f;        // in seconds

        private DateTime previousBend = DateTime.Now;   // time at which we previously jumped
        private const float bendInterval = 0.1f;        // in seconds

        


        private AnimatedSprite anim = null;
        private AnimatedSprite[] animations = new AnimatedSprite[4];


        KeyboardState keyboardState;
        KeyboardState prevKeyboardState = Keyboard.GetState();

        MouseState currentMouseState;
        MouseState previousMouseState = Mouse.GetState();


        public AnimatedSprite Anim { get => anim; set => anim = value; }
        public AnimatedSprite[] Animations { get => animations; set => animations = value; }
        internal Movement Direction { get => direction; set => direction = value; }
        public Vector2 Position { get => upBody.Position; }
        public bool IsJumping { get => isJumping; set => isJumping = value; }
        public bool IsMoving { get => isMoving; set => isMoving = value; }
        public bool IsBending { get => isBending; set => isBending = value; }
        public int Health { get => health; set => health = value; }
        public bool IsAlive { get => isAlive; set => isAlive = value; }
        public float ActualMovningSpeed { get => actualMovningSpeed; set => actualMovningSpeed = value; }
        
        
        public Kid(Texture2D hearts ,World world, Texture2D torsoTexture, Texture2D wheelTexture,Texture2D bullet, Vector2 size, float mass, Vector2 startPosition,bool isBent,SpriteFont f)
        {
            this.hearts = hearts;
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
            upBody = new PhysicsObject(world, torsoTexture, torsoSize.X, mass / 2.0f);
            upBody.Position = startPosition;
            position = upBody.Position;

            midBody = new PhysicsObject(world, torsoTexture, torsoSize.X, mass / 2.0f);
            midBody.Position = upBody.Position + new Vector2(0, 64);
            // Create the feet of the body

            wheel = new PhysicsObject(world, torsoTexture, wheelSize, mass / 2.0f);
            wheel.Position = midBody.Position + new Vector2(0, 64);

            upBody.Body.Friction = 50.0f;
            midBody.Body.Friction = 50.0f;
            wheel.Body.Friction = 50.0f;

            
            // Create a joint to keep the torso upright
            JointFactory.CreateAngleJoint(world, upBody.Body,new Body(world));
            JointFactory.CreateAngleJoint(world, midBody.Body, new Body(world));


            upBody.Body.CollisionCategories = Category.Cat10;
            wheel.Body.CollisionCategories = Category.Cat11;

            upBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
            midBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
            wheel.Body.CollidesWith = Category.Cat1 | Category.Cat30;

            upBody.Body.OnCollision += new OnCollisionEventHandler(HitByEnemy);
            midBody.Body.OnCollision += new OnCollisionEventHandler(HitByEnemy);
            wheel.Body.OnCollision += new OnCollisionEventHandler(HitByEnemy);


            // Connect the feet to the torso
            axis1 = JointFactory.CreateRevoluteJoint(world, midBody.Body, wheel.Body, Vector2.Zero);
            axis1.CollideConnected = true;
            axis1.MotorEnabled = true;
            axis1.MotorSpeed = 0.0f;
            axis1.MaxMotorTorque = 4.0f;

            axis2 = JointFactory.CreateRevoluteJoint(world, upBody.Body, midBody.Body, Vector2.Zero);
            //axis2 = JointFactory.CreateAngleJoint(world,upBody.Body,midBody.Body);

            axis2.CollideConnected = true;
            //axis2.MotorEnabled = false;




            pv1 = new PhysicsView(upBody.Body, upBody.Position, upBody.Size, f);
            pv2 = new PhysicsView(midBody.Body, midBody.Position, wheel.Size, f);
            pv3 = new PhysicsView(wheel.Body,wheel.Position, wheel.Size, f);
        }

        

        public void Move(Movement movement)
        {   
            if(!IsBending)
            speed = SPEED;
            switch (movement)
            {
                case Movement.Left:
                    lookRight = false;
                    axis1.MotorSpeed = -MathHelper.TwoPi * speed;
                    anim = animations[3];
                    break;

                case Movement.Right:
                    lookRight = true;
                    axis1.MotorSpeed = MathHelper.TwoPi * speed;
                    anim = animations[3];
                    break;

                case Movement.Stop:

                    //if (axis1.MotorSpeed != 0)
                    //{
                    
                    if (!IsJumping)
                    {
                        axis1.MotorSpeed = 0;
                        axis1.BodyB.ResetDynamics();
                        axis1.BodyA.ResetDynamics();
                        upBody.Body.ResetDynamics();
                    }


                    //}

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
                isJumping = true;
                isMoving = true;
                wheel.Body.ApplyLinearImpulse(jumpForce);
                previousJump = DateTime.Now;
            }
            else
            {
                isJumping = true;
                isMoving = true;
            }



        }

        //should create variables for funciton
        public void Slide(Movement dir)
        {
            speed = SPEED;
            if (!isJumping)
            {
                if ((DateTime.Now - previousSlide).TotalSeconds >= slideInterval)
                {
                    isSliding = true;
                    IsMoving = true;
                    upBody.Body.CollidesWith = Category.None;
                    midBody.Body.CollidesWith = Category.None;
                    if (dir == Movement.Right)
                        wheel.Body.ApplyLinearImpulse(slideForce);
                    else
                    {
                        slideForce.X = slideForce.X * -1;
                        wheel.Body.ApplyLinearImpulse(slideForce);
                        slideForce.X = slideForce.X * -1;
                    }

                    previousSlide = DateTime.Now;

                }
            }
        }

        public void Shoot()
        {
            if ((DateTime.Now - previousShoot).TotalSeconds >= shootInterval)
            {

                isMeleAttacking = true;
                shot = new PhysicsObject(world, shootTexture, 15, 1);
                shot.Body.CollisionCategories = Category.Cat28;
                shot.Body.CollidesWith = Category.Cat20 | Category.Cat21 | Category.Cat1;
                shot.Body.Mass = 2.0f;
                shot.Body.IgnoreGravity = true;
                shot.Position = new Vector2(upBody.Position.X + upBody.Size.X / 2, upBody.Position.Y + upBody.Size.Y / 2);
                int shootingDirection;
                if (lookRight)
                    shootingDirection = 1;
                else
                    shootingDirection = -1;
                shot.Body.ApplyLinearImpulse(new Vector2(15 * shootingDirection, 0));
                shot.Body.OnCollision += new OnCollisionEventHandler(Mele_OnCollision);
                shotList.Add(shot);

                previousShoot = DateTime.Now;

            }
        }

        bool Mele_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
               
            PhysicsObject tmp = null;
            foreach(PhysicsObject p in shotList)
            {
                if (p.Body.BodyId == fixtureA.Body.BodyId)
                    tmp = p;
            }
 
            if (tmp != null)
            {
                shotList.Remove(tmp);
                tmp.Body.Dispose();
            }
            
            return true;
            
            
            
        }
        public void rangedShoot(Vector2 shootForce)
        {
            isRangeAttacking = true;
            shoot = new PhysicsObject(world, shootTexture, 30, 1);
            shoot.Body.IgnoreCollisionWith(upBody.Body);
            shoot.Body.IgnoreCollisionWith(wheel.Body);

            //Console.WriteLine("end: " + currentMouseState.Position.X + " " + currentMouseState.Position.Y);
            shoot.Position = new Vector2(upBody.Position.X + upBody.Size.X / 2, upBody.Position.Y + upBody.Size.Y / 2);
            shoot.Body.Mass = 2.0f;
            shoot.Body.ApplyForce(shootForce);
            shoot.Body.OnCollision += new OnCollisionEventHandler(Shoot_OnCollision);

        }
        bool Shoot_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.CollisionCategories == Category.Cat20 || fixtureB.CollisionCategories == Category.Cat21)
            {
                
                isRangeAttacking = false;
                //shoot.Body.Enabled = false;
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
                if (IsMoving)
                {
                    upBody.Body.CollidesWith = Category.None;
                    speed = SPEED / 2;
                    anim = animations[1];
                }
                else
                {
                    upBody.Body.CollidesWith = Category.None;
                    anim = animations[0];
                }
                //////shot single image crouch
            }

        }

        public void Scene()
        {

            Move(Movement.Right);
            isMoving = true;
            direction = Movement.Right;

        }

        bool HitByEnemy(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            
            if (fixtureB.CollisionCategories == Category.Cat30)
            {
                if (Health > 0)
                {
                    Health--;
                    //Console.WriteLine(Health);
                }
                if (Health == 0)
                {
                    IsAlive = false;
                    upBody.Body.Enabled = false;
                    upBody.Body.Enabled = false;
                    //torso.Body.Dispose();
                    //wheel.Body.Dispose();
                    Health = 8;
                }
            }
        
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            if (isAlive)
            {
                keyboardState = Keyboard.GetState();
                currentMouseState = Mouse.GetState();
                actualMovningSpeed = upBody.Body.AngularVelocity;
                //bentPosition = new Vector2(torso.Position.X,torso.Position.Y-10);

                if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
                {
                    isJumping = false;
                    isMoving = false;
                }
               
                if ((DateTime.Now - previousSlide).TotalSeconds >= slideInterval)
                {
                    isSliding = false;
                    isMoving = false;
                    upBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
                    midBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
                }


                foreach (PhysicsObject s in shotList)
                {
                    s.Update(gameTime);
                }
                

                //if (IsMoving) // apply animation
                //else //player will appear as standing with frame [1] from the atlas.
                  //  Anim.CurrentFrame = 1;

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
                    if (shootForce.X > 5 || shootForce.X < -5 || shootForce.Y > 5 || shootForce.Y < -5)
                        rangedShoot(shootForce * 4);
                }


                if (keyboardState.IsKeyDown(Keys.LeftControl) && !(prevKeyboardState.IsKeyDown(Keys.LeftControl)))
                {
                    Shoot();

                }



                if (Position.X < 400)
                {
                    wheel.Body.ApplyLinearImpulse(new Vector2(1, 0));
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
                    bend();
                    IsBending = true;

                }

                if (keyboardState.IsKeyUp(Keys.Down) && prevKeyboardState.IsKeyDown(Keys.Down))
                {
                    IsBending = false;
                    upBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
                }

                if (!isMoving && !IsBending)
                    anim = animations[2];

                Anim.Update(gameTime);


                previousMouseState = currentMouseState;
                prevKeyboardState = keyboardState;


            }
            else
            GameOver = true;
        }

        //needs to be changed

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (!GameOver)
            {
                //upBody.Draw(gameTime, spriteBatch);
                Rectangle dest = upBody.physicsObjRecToDraw();
                dest.Height = dest.Height+(int)wheel.Size.Y/2;
                dest.Y = dest.Y + (int)wheel.Size.Y/2;
                if (Anim != null)
                {
                    if(direction == Movement.Right)
                        Anim.Draw(spriteBatch, dest, upBody.Body,false);
                    else
                        Anim.Draw(spriteBatch, dest, upBody.Body, true);

                }
                foreach (PhysicsObject s in shotList)
                {
                    s.Draw(gameTime, spriteBatch);
                }
                if (isRangeAttacking && !(shoot.Body.IsDisposed))
                    shoot.Draw(gameTime, spriteBatch);

                if (showText)
                {

                    spriteBatch.DrawString(f, "ho HEY,im ron i got schyzofrenia", new Vector2(Position.X + size.X, Position.Y), Color.White);
                }

                //spriteBatch.Begin();
                for (int i = 0; i < Health; i++)
                {
                    spriteBatch.Draw(hearts, new Vector2(Position.X - 900 + i * 60, Position.Y - 600), Color.White);
                }
                // spriteBatch.End();

                pv1.Draw(gameTime, spriteBatch);
                //pv2.Draw(gameTime, spriteBatch);
                //pv3.Draw(gameTime, spriteBatch);

                //spriteBatch.DrawString(f, WheelSpeed.ToString(), new Vector2(Position.X + size.X, Position.Y), Color.White);


                //wheel.Draw(gameTime,spriteBatch);
            }
            else
            {
                
                //needs to add a while loop for waiting 5 seconds before exiting to StartMenu
                spriteBatch.DrawString(f, "GAME OVER!!!!!!", new Vector2(Position.X + size.X, Position.Y), Color.White);

            }

        }
        public static void MyDelay(int seconds)
        {
            
        }
    }
}
