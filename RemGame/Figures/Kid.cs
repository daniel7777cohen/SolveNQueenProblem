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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace RemGame
{

    enum Animation
    {
        Crouch,
        CrouchWalk,
        Idle,
        Walk,
        JumpStart,
        JumpUp,
        JumpMid,
        JumpDown,
        JumpEnd,
        SlideStart,
        SlideIn,
        SlideEnd
    }

    class Kid:Component
    {
        private static ContentManager content;

        private bool GameOver = false;

        private World world;
        private Vector2 size;
        private float mass;
        private Vector2 position;

        private Vector2 followingPlayerPoint;
        private bool isFalling = false;
        private bool firstMove = false;
        private SpriteFont f;

        /// <summary>
        /// Farseer body coordinates
        /// </summary>
        PhysicsView pv1;
        PhysicsView pv2;
        PhysicsView pv3;



        private PhysicsObject upBody;
        private PhysicsObject midBody;
        private PhysicsObject wheel;

        private RevoluteJoint axis1;
        private Joint axis2;

        /// <tmp>
        private PhysicsObject shot;
        private List<PhysicsObject> shotList = new List<PhysicsObject>();

        private PhysicsObject rangedShot;
        private List<PhysicsObject> rangedShotList = new List<PhysicsObject>();


        Vector2 shootBase;
        Vector2 shootDirection;

        private bool isMeleAttacking = false;
        private bool isRangeAttacking = false;

        Texture2D shootTexture;
        Texture2D hearts;
      
        private int health = 8;
        private bool isAlive = true;

        private const float SPEED = 2.0f;
        private float walkTracker = 0;


        private float speed = SPEED;
        private float actualMovningSpeed=0;

        private bool isMoving = false;
        private bool isJumping = false;
        private bool isSliding = false;
        private bool isBending = false;

        private Movement direction = Movement.Right;
        private bool lookRight = true;

        private Vector2 cameraToFollow;


        private bool showText = false;

        private DateTime previousJump = DateTime.Now;   // time at which we previously jumped
        private const float jumpInterval = 0.9f;        // in seconds
        private Vector2 jumpForce = new Vector2(0, -5); // applied force when jumping
        private float takeOffPoi = 0;
        private float liftOff = 0;
        private float beforeLanding = 0;
        private bool goingDown = false;
        private bool hasLanded = true;
        private bool playLandingSound = false;



        private DateTime previousSlide = DateTime.Now;   // time at which we previously jumped
        private const float slideInterval = 1.3f;        // in seconds
        private Vector2 slideForce = new Vector2(7, 0); // applied force when jumping
        private float startPoint = 0;
        private float slideTracker = 0;
        private bool slideOver = true;

        private DateTime previousShoot = DateTime.Now;   // time at which we previously jumped
        private const float shootInterval = 0.3f;        // in seconds

        private DateTime previousBend = DateTime.Now;   // time at which we previously jumped
        private const float bendInterval = 0.1f;        // in seconds

        /// <summary>
        /// /////////////////////////////Art Assignment
        /// </summary>
        private AnimatedSprite anim = null;
        private AnimatedSprite[] animations = new AnimatedSprite[12];

        Texture2D playerCrouch;
        Texture2D playerCrouchWalk;
        Texture2D playerStand;
        Texture2D playerWalk;
        Texture2D[] jumpSetAnim = new Texture2D[5];
        Texture2D[] slideSetAnim = new Texture2D[3];

        SoundEffect footstep;
        SoundEffect jump_Up;
        SoundEffect jump_Down;
        SoundEffect idle;
        SoundEffect crouch;
        SoundEffect slide;


        bool isJumpSoundPlayed = false;


        SoundEffectInstance walkingInstance;
        SoundEffectInstance jumpingUpInstance;
        SoundEffectInstance jumpingDownInstance;
        SoundEffectInstance idleInstance;
        SoundEffectInstance crouchingInstance;
        SoundEffectInstance slidingInstance;


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
        public bool HasLanded { get => hasLanded; set => hasLanded = value; }
        public bool IsSliding { get => isSliding; set => isSliding = value; }
        public bool PlayLandingSound { get => playLandingSound; set => playLandingSound = value; }
        public static ContentManager Content { protected get => content; set => content = value; }
        public bool FirstMove { get => firstMove; set => firstMove = value; }
        public Vector2 CameraToFollow { get => cameraToFollow; set => cameraToFollow = value; }
        public Texture2D Hearts { get => hearts; set => hearts = value; }

        public Kid(World world,Vector2 size, float mass, Vector2 startPosition,bool isBent,SpriteFont f)
        {
            this.world = world;
            this.size = size;
            this.mass = mass / 4.0f;
            this.f = f;

            IsMoving = false;
            Vector2 torsoSize = new Vector2(size.X, size.Y-size.X/2.0f);
            float wheelSize = size.X ;


            // Create the upper body
            upBody = new PhysicsObject(world, null, torsoSize.X, mass / 2.0f);
            upBody.Position = startPosition;
            position = upBody.Position;
            followingPlayerPoint = position;

            // Create the midlle body
            midBody = new PhysicsObject(world, null, torsoSize.X, mass / 2.0f);
            midBody.Position = upBody.Position + new Vector2(0, 64);

            // Create the feet("Engine") of the body
            wheel = new PhysicsObject(world, null, wheelSize, mass / 2.0f);
            wheel.Position = midBody.Position + new Vector2(0, 64);

            cameraToFollow = new Vector2(position.X+100,position.Y-50);


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

            direction = Movement.Right;
            
            axis1 = JointFactory.CreateRevoluteJoint(world, midBody.Body, wheel.Body, Vector2.Zero);
            axis1.CollideConnected = false;
            axis1.MotorEnabled = true;
            axis1.MotorSpeed = 0.0f;
            axis1.MaxMotorTorque = 3.0f;

            axis2 = JointFactory.CreateRevoluteJoint(world, upBody.Body, midBody.Body, Vector2.Zero);

            axis2.CollideConnected = true;

            ////Art Init
            Hearts = Content.Load<Texture2D>("misc/heart");
            shootTexture = Content.Load<Texture2D>("Player/bullet");

            playerCrouch = Content.Load<Texture2D>("Player/Anim/Ron_Crouch");
            playerCrouchWalk = Content.Load<Texture2D>("Player/Anim/Ron_Crouch_Walk");
            playerStand = Content.Load<Texture2D>("Player/Anim/Ron_Stand");
            playerWalk = Content.Load<Texture2D>("Player/Anim/Ron_Walk");

            footstep = Content.Load<SoundEffect>("Sound/FX/Player/Ron_Footsteps");
            walkingInstance = footstep.CreateInstance();
            walkingInstance.IsLooped = true;
            walkingInstance.Pitch = 0.18f;


            jump_Up = Content.Load<SoundEffect>("Sound/FX/Player/Ron_Jump_Up");
            jumpingUpInstance = jump_Up.CreateInstance();
            jumpingUpInstance.IsLooped = false;
            jumpingUpInstance.Volume = 0.02f;

            jump_Down = Content.Load<SoundEffect>("Sound/FX/Player/Ron_Jump_Down");
            jumpingDownInstance = jump_Down.CreateInstance();
            jumpingDownInstance.IsLooped = false;
            jumpingDownInstance.Volume = 0.04f;

            idle = Content.Load<SoundEffect>("Sound/FX/Player/Ron_Idle");
            idleInstance = idle.CreateInstance();
            idleInstance.IsLooped = true;
            idleInstance.Volume = 0.1f;
            idleInstance.Pitch = 0.3f;


            crouch = Content.Load<SoundEffect>("Sound/FX/Player/Ron_Crouch");
            crouchingInstance = crouch.CreateInstance();
            crouchingInstance.IsLooped = true;
            crouchingInstance.Volume = 0.03f;
            crouchingInstance.Pitch = 0.25f;


            slide = Content.Load<SoundEffect>("Sound/FX/Player/Ron_Slide");
            slidingInstance = slide.CreateInstance();
            slidingInstance.IsLooped = false;
            slidingInstance.Volume = 0.04f;

            Rectangle anim3 = new Rectangle(-110, -65, 200, 160);
            Rectangle anim4 = new Rectangle(0, -50, 140, 130);


            Animations[(int)Animation.Crouch] = new AnimatedSprite(playerCrouch, 1, 1, anim3, 0f);
            Animations[(int)Animation.CrouchWalk] = new AnimatedSprite(playerCrouchWalk, 2, 16, anim3, 0.03f);

            Animations[(int)Animation.Idle] = new AnimatedSprite(playerStand, 4, 13, anim3, 0.017f);

            Animations[(int)Animation.Walk] = new AnimatedSprite(playerWalk, 2, 12, anim3, 0.03f);

            jumpSetAnim[0] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_01_start");
            jumpSetAnim[1] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_02_up");
            jumpSetAnim[2] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_03_mid");
            jumpSetAnim[3] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_04_down");
            jumpSetAnim[4] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_05_end");

            Animations[(int)Animation.JumpStart] = new AnimatedSprite(jumpSetAnim[0], 1, 1, anim3, 0f);
            Animations[(int)Animation.JumpUp] = new AnimatedSprite(jumpSetAnim[1], 1, 1, anim3, 0f);
            Animations[(int)Animation.JumpMid] = new AnimatedSprite(jumpSetAnim[2], 1, 3, anim3, 0.6f);
            Animations[(int)Animation.JumpDown] = new AnimatedSprite(jumpSetAnim[3], 1, 1, anim3, 0f);
            Animations[(int)Animation.JumpEnd] = new AnimatedSprite(jumpSetAnim[4], 1, 1, anim3, 0f);

            slideSetAnim[0] = Content.Load<Texture2D>("Player/Anim/Slide/Ron_Slide_01_start");
            slideSetAnim[1] = Content.Load<Texture2D>("Player/Anim/Slide/Ron_Slide_02_slide");
            slideSetAnim[2] = Content.Load<Texture2D>("Player/Anim/Slide/Ron_Slide_03_end");

            Animations[(int)Animation.SlideStart] = new AnimatedSprite(slideSetAnim[0], 1, 4, anim3, 0.4f);
            Animations[(int)Animation.SlideIn] = new AnimatedSprite(slideSetAnim[1], 1, 6, anim3, 0.3f);
            Animations[(int)Animation.SlideEnd] = new AnimatedSprite(slideSetAnim[2], 1, 4, anim3, 0.6f);


            pv1 = new PhysicsView(upBody.Body, upBody.Position, upBody.Size, f);
            pv2 = new PhysicsView(midBody.Body, midBody.Position, wheel.Size, f);
            pv3 = new PhysicsView(wheel.Body,wheel.Position, wheel.Size, f);
        }

        ///////////////////////////////////////////////////////////Abillities////////////////////////////////////////////////////////
        public void Move(Movement movement)
        {   
            if(!IsBending && !isJumping )
            speed = SPEED;

            if (!IsSliding && !IsJumping )
            {
                switch (movement)
                {
                    case Movement.Left:
                       
                        lookRight = false;
                        axis1.MotorSpeed = -MathHelper.TwoPi * speed;
                        anim = animations[(int)Animation.Walk];
                        break;

                    case Movement.Right:
                       
                        lookRight = true;
                        axis1.MotorSpeed = MathHelper.TwoPi * speed;
                        anim = animations[(int)Animation.Walk];
                        break;
                        
                    case Movement.Stop:
                        
                        axis1.MotorSpeed = 0;
                        if(!isFalling)
                            ResetPlayerDynamics();
                        break;
                }

            }
        }


        public void Jump()

        {
            
            if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval && hasLanded)
            {
                anim = animations[(int)Animation.JumpStart];
                isJumping = true;
                takeOffPoi = wheel.Position.Y;
                liftOff = wheel.Position.Y;
                wheel.Body.ApplyLinearImpulse(jumpForce);
                goingDown = false;
                HasLanded = false;
                previousJump = DateTime.Now;

            }

        }

        public void Slide(Movement dir)
        {
            speed = SPEED;
            if (!isJumping && !isBending)
            {
                if ((DateTime.Now - previousSlide).TotalSeconds >= slideInterval)
                {
                    anim = animations[(int)Animation.SlideStart];
                    slideOver = false;
                    startPoint = wheel.Position.X;
                    slideTracker = startPoint;
                    IsSliding = true;
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

        public void bend()
        {
            if (!isJumping && !IsSliding)
            {
                if (IsMoving)
                {
                    upBody.Body.CollidesWith = Category.None;
                    speed = SPEED / 2;
                    anim = animations[(int)Animation.CrouchWalk];
                }
                else
                {
                    upBody.Body.CollidesWith = Category.None;
                    anim = animations[(int)Animation.Crouch];
                }
                //////shot single image crouch
            }

        }

        //////////////////////////////////////// Actions////////////////////////////////////////////////
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
            rangedShot = new PhysicsObject(world, shootTexture, 20, 1);
            rangedShot.Body.CollisionCategories = Category.Cat28;
            rangedShot.Body.CollidesWith = Category.Cat20 | Category.Cat21 | Category.Cat1;
            rangedShot.Body.IgnoreCollisionWith(upBody.Body);
            rangedShot.Body.IgnoreCollisionWith(wheel.Body);

            //Console.WriteLine("end: " + currentMouseState.Position.X + " " + currentMouseState.Position.Y);
            rangedShot.Position = new Vector2(upBody.Position.X + upBody.Size.X / 2, upBody.Position.Y + upBody.Size.Y / 2);
            rangedShot.Body.Mass = 2.0f;
            rangedShot.Body.ApplyForce(shootForce);
            rangedShot.Body.OnCollision += new OnCollisionEventHandler(Shoot_OnCollision);
            rangedShotList.Add(rangedShot);

        }

        bool Shoot_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
    
            PhysicsObject tmp = null;
            foreach (PhysicsObject p in rangedShotList)
            {
                if (p.Body.BodyId == fixtureA.Body.BodyId)
                    tmp = p;
            }

            if (tmp != null)
            {
                rangedShotList.Remove(tmp);
                tmp.Body.Dispose();
            }

            return true;
        }

        public void Kinesis(Obstacle obj,MouseState currentMouseState)
        {
            
           if( obj.KinesisOn == true)
            if (obj.Position.Y > 0 )
            {
                if (currentMouseState.RightButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Pressed)
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
            //obj.KinesisOn = false;
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

                }
                if (Health == 0)
                {
                    IsAlive = false;
                    upBody.Body.Enabled = false;
                    upBody.Body.Enabled = false;
                    Health = 8;
                }
            }
        
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            
            walkingInstance.Volume = 0.1f;

            if (isAlive)
            {
                keyboardState = Keyboard.GetState();
                currentMouseState = Mouse.GetState();
                actualMovningSpeed = upBody.Body.AngularVelocity;
                //set the coordinates for camera to follow              
                Vector2 moveTo = Position - cameraToFollow;
                Vector2 fixedPosition;

                if (!IsBending)
                    fixedPosition = new Vector2(moveTo.X + 150, moveTo.Y);
                else
                     fixedPosition = new Vector2(moveTo.X + 150, moveTo.Y+80);

                cameraToFollow += fixedPosition * (float)gameTime.ElapsedGameTime.TotalSeconds*2;
              
                

                ///////////Check for falling
                if (upBody.Position.Y > followingPlayerPoint.Y && !isJumping)
                    isFalling = true;
                else
                    isFalling = false;

                if (isFalling == true && upBody.Position.Y > 1300)
                    isAlive = false;

                followingPlayerPoint = upBody.Position;

                //////////////////////Jump Animation ///////////////////
                if (!HasLanded && isJumping)
                {

                    if (wheel.Position.Y <= liftOff)
                    {
                        liftOff = wheel.Position.Y;
                        anim = animations[(int)Animation.JumpStart];
                        if (wheel.Position.Y <= takeOffPoi - 60)
                        {
                            anim = animations[(int)Animation.JumpUp];
                        }

                    }

                    else if (wheel.Position.Y > liftOff && !goingDown)
                    {
                        goingDown = true;
                        beforeLanding = wheel.Position.Y;
                        anim = animations[(int)Animation.JumpMid];

                    }
                    else if (wheel.Position.Y > beforeLanding)
                    {
                        beforeLanding = wheel.Position.Y;
                        if (wheel.Position.Y > takeOffPoi - 140)
                            anim = animations[(int)Animation.JumpDown];
                        if(wheel.Position.Y > takeOffPoi - 50)
                            anim = animations[(int)Animation.JumpEnd];
                    }
                    else
                    {
                        PlayLandingSound = true;
                        anim = animations[(int)Animation.Idle];
                        goingDown = false;
                        HasLanded = true;
                        isJumping = false;
                        liftOff = 0;
                        beforeLanding = 0;
                    }


                }

                //////////////////////Slide Animation//////////////////////////////
                if (IsSliding)
                {
                    if (wheel.Position.X == slideTracker || wheel.Position.X < slideTracker && direction == Movement.Right || wheel.Position.X > slideTracker && direction == Movement.Left)
                    {
                        slideOver = true;
                        ResetPlayerDynamics();
                    }
                    if (wheel.Position.X > startPoint + 100 || wheel.Position.X < startPoint - 100)
                    {
                        anim = animations[(int)Animation.SlideIn];
                    }

                    if (wheel.Position.X > startPoint + 350 || wheel.Position.X < startPoint - 350)
                    {
                        anim = animations[(int)Animation.SlideEnd];
                    }
                    if (wheel.Position.X > startPoint + 500 || wheel.Position.X < startPoint - 500 || slideOver)
                    {
                        IsSliding = false;
                        slideOver = true;
                        upBody.Body.ResetDynamics();

                        upBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
                        midBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
                    }
                    slideTracker = wheel.Position.X;
                }


                foreach (PhysicsObject s in shotList)
                {
                    s.Update(gameTime);
                }

                foreach (PhysicsObject r in rangedShotList)
                {
                    r.Update(gameTime);
                }

                IsMoving = false;

                
                ////////////////////////Key Mangment///////////////////////////////////////////
                /////Movments
                ///
                ///Move Right
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    if(direction == Movement.Right)
                        ResetPlayerDynamics();

                    Move(Movement.Left);
                    direction = Movement.Left;
                    IsMoving = true;

                }

                ///Move Left
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    if (direction == Movement.Left)
                        ResetPlayerDynamics();

                    Move(Movement.Right);
                    direction = Movement.Right;
                    IsMoving = true;
                    firstMove = true;

                }
                ///No Moving
                else
                {
                    IsMoving = false;
                    Move(Movement.Stop);

                }
                ///Jump
                if (keyboardState.IsKeyDown(Keys.Space) && (!prevKeyboardState.IsKeyDown(Keys.Space)))
                {
                    Jump();
                }
                ///Move While jump
                if (isJumping)
                {
                    if (keyboardState.IsKeyDown(Keys.D))
                    {
                        wheel.Body.ApplyLinearImpulse(new Vector2(0.03f, 0));
                    }

                    else if (keyboardState.IsKeyDown(Keys.A))
                    {
                        wheel.Body.ApplyLinearImpulse(new Vector2(-0.03f, 0));
                    }
                }

                ///Slide
                if (keyboardState.IsKeyDown(Keys.LeftShift) && (!prevKeyboardState.IsKeyDown(Keys.LeftShift)))
                {

                    if (keyboardState.IsKeyDown(Keys.D))
                        Slide(Movement.Right);

                    else if (keyboardState.IsKeyDown(Keys.A))
                        Slide(Movement.Left);

                }
                ///Bend
                if (keyboardState.IsKeyDown(Keys.S))
                {
                    bend();
                    IsBending = true;

                }
                ///Check for leaving bend pose
                if (keyboardState.IsKeyUp(Keys.S) && prevKeyboardState.IsKeyDown(Keys.S))
                {
                    IsBending = false;
                    upBody.Body.CollidesWith = Category.Cat1 | Category.Cat30;
                }

                /////////Actions///////////////////////
                ///Ranged Shot
                ///Calculate Direction For Shooting
                if (currentMouseState.RightButton == ButtonState.Pressed && !(previousMouseState.RightButton == ButtonState.Pressed))
                {
                    shootDirection = new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y);

                }
                ///Calculate Motion Vector For Shooting
                if (currentMouseState.RightButton == ButtonState.Released && (previousMouseState.RightButton == ButtonState.Pressed))
                {
                    shootBase = new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y);
                    Vector2 shootForce = new Vector2((shootDirection.X - shootBase.X), (shootDirection.Y - shootBase.Y));
                    if (shootForce.X > 5 || shootForce.X < -5 || shootForce.Y > 5 || shootForce.Y < -5)
                        rangedShoot(shootForce * 4);
                }

                ///Sraight Shot / might change to Mele
                if (currentMouseState.LeftButton == ButtonState.Pressed && !(previousMouseState.LeftButton == ButtonState.Pressed) && !(currentMouseState.RightButton == ButtonState.Pressed))

                {
                    Shoot();
                }

                ///////////////////////////////////////////////////////////////Sound Effects///////////////////////////////////////////////////////////////////
                if (direction != Movement.Stop && isMoving)
                {
                    walkingInstance.Play();
                }

                else
                {
                    walkingInstance.Stop();
                }

                if (IsJumping && !isJumpSoundPlayed)
                {
                    isJumpSoundPlayed = true;
                    jumpingUpInstance.Play();
                }

                if (!IsJumping)
                    isJumpSoundPlayed = false;

                if (HasLanded && PlayLandingSound)
                {
                    jumpingDownInstance.Play();
                    PlayLandingSound = false;
                }

                if (!IsMoving && !IsJumping)
                    idleInstance.Play();
                else
                    idleInstance.Stop();

                if (IsBending)
                    crouchingInstance.Play();
                else
                    crouchingInstance.Stop();


                if (IsSliding)
                    slidingInstance.Play();



                //////////////////////////////////////////SCENE/////////////////////////////////////////////////////////////////////////////
                /*
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
                */


                

                if (!isMoving && !IsBending && !isJumping && !IsSliding)
                    anim = animations[(int)Animation.Idle];

                Anim.Update(gameTime);

                previousMouseState = currentMouseState;
                prevKeyboardState = keyboardState;


            }
            else
            GameOver = true;
        }


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

                foreach (PhysicsObject r in rangedShotList)
                {
                    r.Draw(gameTime, spriteBatch);
                }


                /*
                if (showText)
                {

                    spriteBatch.DrawString(f, "ho HEY,im ron i got schyzofrenia", new Vector2(Position.X + size.X, Position.Y), Color.White);
                }
                */
            
                //pv1.Draw(gameTime, spriteBatch);
                //pv2.Draw(gameTime, spriteBatch);
                //pv3.Draw(gameTime, spriteBatch);
          
            }
            else
            {
                
                //needs to add a while loop for waiting 5 seconds before exiting to StartMenu
               // spriteBatch.DrawString(f, "GAME OVER!!!!!!", new Vector2(Position.X + size.X, Position.Y), Color.White);

            }

        }

        public void ResetPlayerDynamics()
        {
            upBody.Body.ResetDynamics();
            midBody.Body.ResetDynamics();
            wheel.Body.ResetDynamics();
        }

        public static void MyDelay(int seconds)
        {
            
        }


    }
}
