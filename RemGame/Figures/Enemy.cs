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

    enum Movement
    {
        Left,
        Right,
        Jump,
        Stop
    }
    class Enemy
    {

        static int x = 1;

        public enum Mode { Idle, Patrol, WalkToPlayer, Attack, Evade }// what mode of behavior the monster AI is using 
        private int itrator = 0;
        private bool colorPicked = false;

        private static ContentManager content;
        Random random;
        private Point startLocationGrid;

        bool pingPong = false;//***
        bool Ghost = false;//***

        private int health = 5;
        private World world;
        private Map map;
        private Vector2 size;
        private float mass;
        private Vector2 position;
        private Vector2 lastPosition;
        private Point gridLocation;
        private Kid player;
        private int playerDistanceToAttack;

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

        private bool playerDetected = false;
        private bool playerInAttackRange = false;
        private bool isAttacking = false;
        private bool isMeleAttacking = false;
        private RevoluteJoint axis1;
        private int attackingrange;

        private bool isPlayerAlive = true;

        private const float SPEED = 0.3f;
        private float speed = SPEED;
        private bool isMoving = false;
        private bool isBackToLastPos = true;
        private Movement direction = Movement.Right;
        private bool lookingRight = true;


        private Mode mode;
        private Mode previuosMode;



        private bool grounded;

        private int patrolRange;
        bool goBack = false;
        private int inspectionSightRange;

        private float idleInterval;

        private float evasionLuck;

        List<Vector2> path;
        private Vector2[] patrolGridPath;
        private Vector2[] playerGridPath;
        private Vector2[] selectedPath;
        private int patrolDirection = 1;

        private DateTime previousWander = DateTime.Now;   // time at which we previously jumped
        private const float wanderInterval = 3.0f;

        private bool endOfPatrol = false;

        private bool wandered = true;

        Texture2D gridColor;


        /// //////////////////////////////Working on new behavoir - Ai//////////////////////////////////////////////////////////////////////////////


        private DateTime previousJump = DateTime.Now;   // time at which we previously jumped
        private const float jumpInterval = 0.7f;        // in seconds
        private Vector2 jumpForce = new Vector2(0, -5); // applied force when jumping

        private DateTime previousSlide = DateTime.Now;   // time at which we previously jumped
        private const float slideInterval = 0.1f;        // in seconds
        private Vector2 slideForce = new Vector2(4, 0); // applied force when jumping

        private DateTime previousShoot = DateTime.Now;   // time at which we previously jumped
        private const float shootInterval = 3.0f;        // in seconds

        private AnimatedSprite anim;
        private AnimatedSprite[] animations = new AnimatedSprite[4];


        Texture2D shootTexture;


        public Enemy(World world, Vector2 size, float mass, Vector2 startPosition, Point startLocationGrid, int patrolRange, SpriteFont f, int newDistance, Map map, Kid player, int playerDistanceToAttack)
        {

            this.world = world;
            this.map = map;
            this.size = size;
            this.mass = mass / 2.0f;
            this.player = player;
            ////
            this.startLocationGrid = startLocationGrid;
            this.patrolRange = patrolRange;
            this.playerDistanceToAttack = playerDistanceToAttack;
            mode = Mode.Idle;
            ////
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

            torso.Body.OnCollision += new OnCollisionEventHandler(HitByPlayer);
            wheel.Body.OnCollision += new OnCollisionEventHandler(HitByPlayer);

            pv1 = new PhysicsView(torso.Body, torso.Position, torso.Size, f);
            pv2 = new PhysicsView(wheel.Body, wheel.Position, wheel.Size, f);

            Animations[0] = new AnimatedSprite(Content.Load<Texture2D>("Figures/Level1/Principal/Anim/Principal_Walk"), 2, 8, new Rectangle(0, -170, 250, 250), 0.05f);
            Animations[1] = new AnimatedSprite(Content.Load<Texture2D>("Figures/Level1/Principal/Anim/Principal_Walk"), 2, 8, new Rectangle(0, -170, 250, 250), 0.05f);
            Animations[3] = new AnimatedSprite(Content.Load<Texture2D>("Figures/Level1/Principal/Anim/Principal_Stand"), 2, 17, new Rectangle(0, -170, 250, 250), 0.05f);

            shootTexture = shootTexture = Content.Load<Texture2D>("Player/bullet");

        }
        public int Health { get => health; set => health = value; }
        public static ContentManager Content { protected get => content; set => content = value; }
        public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
        public AnimatedSprite Anim { get => anim; set => anim = value; }
        public AnimatedSprite[] Animations { get => animations; set => animations = value; }
        public Movement Direction { get => direction; set => direction = value; }
        public Vector2 Position { get => torso.Position; }
        public int Distance { get => distance; set => distance = value; }
        public Point GridLocation { get => gridLocation; set => gridLocation = value; }

        public virtual void Move(Movement movement)
        {
            switch (movement)
            {
                case Movement.Left:
                    lookingRight = false;
                    axis1.MotorSpeed = -MathHelper.TwoPi * speed;
                    anim = animations[1];
                    break;

                case Movement.Right:
                    lookingRight = true;
                    axis1.MotorSpeed = MathHelper.TwoPi * speed;
                    anim = animations[1];

                    break;

                case Movement.Stop:
                    axis1.MotorSpeed = 0;
                    anim = animations[3];
                    break;
            }
        }

        /*
                public void Jump()

                {
                    if (motion == Act.jump) return;
                    motion = Act.jump;
                    wheel.Body.ApplyLinearImpulse(jumpForce * new Vector2(0, 1));

                    if ((DateTime.Now - previousJump).TotalSeconds >= jumpInterval)
                    {
                        torso.Body.ApplyLinearImpulse(jumpForce);
                        previousJump = DateTime.Now;
                    }
                }
         */
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
                mele.Body.ApplyLinearImpulse(new Vector2(10 * dir, 0));
                if (isPlayerAlive)
                    mele.Body.OnCollision += new OnCollisionEventHandler(Mele_OnCollision);
                previousShoot = DateTime.Now;

            }
            else
                isAttacking = false;

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

        public void Update(GameTime gameTime, Vector2 playerPosition, bool PlayerAlive, int patrolbound)
        {

            bool reached = false;

            if (gridLocation == startLocationGrid)
                PathFinder.SetMap(map);

            UpdateAI();

            anim = Animations[3];
            anim = Animations[(int)direction];

            if (isMoving || direction == Movement.Stop) // apply animation
                Anim.Update(gameTime);
            else //player will appear as standing with frame [1] from the atlas.
                Anim.CurrentFrame = 1;

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {
            if (patrolGridPath != null)
            {
                Color c = Color.Red;

                if (!colorPicked)
                {
                    switch (x)
                    {
                        case 1:
                            c = Color.Red;
                            break;
                        case 2:
                            c = Color.Purple;
                            break;
                        case 3:
                            c = Color.Pink;
                            break;
                        case 4:
                            c = Color.Plum;
                            break;
                        case 5:
                            c = Color.RosyBrown;
                            break;
                    }
                    x++;
                    if (x == 5)
                        x = 1;
                }
                /*DRAWS A* PATH
                for (int i = 0; i < patrolGridPath.Length; i++)
                {
                    Rectangle gridloc = new Rectangle((int)patrolGridPath[i].X * 64, (int)patrolGridPath[i].Y * 64, 64, 64);
                    if (gridLocation.ToVector2() != patrolGridPath[i])
                        spriteBatch.Draw(gridColor, gridloc, c);
                    else
                        spriteBatch.Draw(gridColor, gridloc, Color.Green);
                }
                */
            }

            /*dRAWS PATH TO PLAYER
            if (playerGridPath != null)
            {

                for (int i = 0; i < playerGridPath.Length; i++)
                {
                    Rectangle gridloc = new Rectangle((int)playerGridPath[i].X * 64, (int)playerGridPath[i].Y * 64, 40, 40);
                    if (gridLocation.ToVector2() != playerGridPath[i])
                        spriteBatch.Draw(gridColor, gridloc, Color.Green);
                    else
                        spriteBatch.Draw(gridColor, gridloc, Color.GreenYellow);
                }
            }
            */
            //torso.Draw(gameTime,spriteBatch);
            Rectangle dest = torso.physicsRectnagleObjRecToDraw();
            //dest.Height = dest.Height+(int)wheel.Size.Y/2;
            //dest.Y = dest.Y + (int)wheel.Size.Y/2;
            if (!torso.Body.IsDisposed && anim != null && direction != Movement.Left)
                anim.Draw(spriteBatch, dest, torso.Body, true);
            if(direction == Movement.Left)
                anim.Draw(spriteBatch, dest, torso.Body, false);


            //pv1.Draw(gameTime, spriteBatch);
            //pv2.Draw(gameTime, spriteBatch);

            if (isMeleAttacking && !(mele.Body.IsDisposed))
                mele.Draw(gameTime, spriteBatch);

            //wheel.Draw(gameTime,spriteBatch);
            /*
           spriteBatch.DrawString(font, this.GridLocation.ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 - 20), Color.White);
            if (selectedPath != null)
                spriteBatch.DrawString(font, selectedPath[selectedPath.Length - 1].ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 + 20), Color.White);
            
            */
            //spriteBatch.DrawString(font, itrator.ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 + 20), Color.White);

            //spriteBatch.DrawString(font, this.mode.ToString(), new Vector2(this.GridLocation.X * 64 + 90, this.GridLocation.Y * 64 + 40), Color.White);
           
        }

        public virtual void UpdateAI()
        {
            if (selectedPath == null)
                selectedPath = new Vector2[] { Vector2.Zero };


            //Borders for chcking path to the player,to reduce calculations
            if ((player.GridLocation.X < GridLocation.X + 50 && player.GridLocation.X > GridLocation.X - 50) && (player.GridLocation.Y > 0) && player.GridLocation != null)
            {
                playerGridPath = findPathToPlayer();
            }

            if (playerGridPath == null)
                playerGridPath = new Vector2[] { gridLocation.ToVector2() };


            if (mode != Mode.WalkToPlayer && mode != Mode.Attack && (playerGridPath.Length < 6 && playerGridPath.Length > 1))//sight range
            {
                if (playerGridPath.Length < 2 && mode != Mode.Attack) //attack range                
                    mode = Mode.Attack;

                else
                    mode = Mode.WalkToPlayer;
            }

            else
            {
                if (endOfPatrol)
                {
                    if(mode != Mode.Idle)
                        mode = Mode.Idle;

                    if ((DateTime.Now - previousWander).TotalSeconds >= wanderInterval)
                    {
                        itrator = 0;
                        endOfPatrol = false;
                        wandered = true;
                    }
                }

                else if (mode != Mode.Patrol)
                    mode = Mode.Patrol;

            }
            
            patrolDirection *= -1;

            //condition needts to change by enemies abilities like attack up or down etc...           
            //if (playerGridPath.Length > 2)//if player not in range do: patrol idle,else do : attack,walktoplayer,evade
            //{            //}

            switch (mode)
            {
                case Mode.Idle:
                    Move(Movement.Stop);
                    direction = Movement.Stop;
                    break;

                case Mode.Patrol:

                    if (itrator == 0 && wandered)
                    {
                        patrolGridPath = findPathToPatrol(patrolDirection*20);
                        selectedPath = patrolGridPath;
                        endOfPatrol = false;
                    }

                    else if (itrator == selectedPath.Length - 1 && selectedPath[selectedPath.Length - 1] == gridLocation.ToVector2())
                    {
                        endOfPatrol = true;
                        previousWander = DateTime.Now;
                        wandered = false;
                    }
                    break;

                case Mode.WalkToPlayer:

                    if (itrator == 0 && wandered == false)
                    {
                        patrolGridPath = findPathToPatrol(patrolDirection * 20);
                        selectedPath = patrolGridPath;

                    }

                    else if (itrator == selectedPath.Length - 1 && selectedPath[selectedPath.Length - 1] == gridLocation.ToVector2())
                    {

                        if (!wandered)
                        {
                            if ((map.isPassable(gridLocation.X + 1, gridLocation.Y) || map.isPassable(gridLocation.X - 1, gridLocation.Y)))
                            {
                                wandered = true;
                                previousWander = DateTime.Now;
                            }
                        }
                        if ((DateTime.Now - previousWander).TotalSeconds >= wanderInterval)
                        {
                            itrator = 0;
                            wandered = false;
                        }

                    }
                    break;

                case Mode.Attack:
                    break;

                case Mode.Evade:
                    break;

                default:
                    selectedPath = new Vector2[] { gridLocation.ToVector2() };
                    break;

            }

            if (gridLocation.ToVector2() != selectedPath[selectedPath.Length - 1] && (mode == Mode.Patrol || mode == Mode.WalkToPlayer))
            {
                isMoving = true;

                if (gridLocation.ToVector2() == selectedPath[itrator])
                {

                    if (selectedPath[itrator + 1].X > gridLocation.X)
                    {
                        direction = Movement.Right;
                        Move(Movement.Right);
                    }


                    if (selectedPath[itrator + 1].X < gridLocation.X)
                    {
                        direction = Movement.Left;
                        Move(Movement.Left);
                    }

                    if (selectedPath[itrator + 1].Y < gridLocation.Y)
                    {
                        wheel.Body.ApplyLinearImpulse(new Vector2(0, -6));
                        isMoving = false;

                    }

                    itrator++;

                }

            }
            else
            {
                Move(Movement.Stop);
                isMoving = false;

            }


            /*
            if (patrolGridPath[itrator].Y == gridLocation.Y && map.isPassable((int)patrolGridPath[itrator].X + 1, (int)patrolGridPath[itrator].Y))
            {

                if (gridLocation.ToVector2() != patrolGridPath[itrator + 1])
                {
                    Move(Movement.Right);
                    direction = Movement.Right;
                    isMoving = true;
                }
                else
                    reached = true;

                if(reached)
                {
                    itrator++;
                    //Move(Movement.Stop);
                    //isMoving = false;

                }

                if (patrolGridPath[itrator + 1].Y < gridLocation.Y)
                {
                    isMoving = false;
                    wheel.Body.ApplyLinearImpulse(new Vector2(0, -6));
                }
            }
            */
            /*
            if (gridLocation.ToVector2() != gridpath[itrator])
            {
                Move(Movement.Right);
                direction = Movement.Right;
                isMoving = true;
            }
            else
            */

            // Console.WriteLine("grid vector :" + patrolGridPath[itrator] + "enemy vector :" + gridLocation);
            //Console.WriteLine(itrator);
        }
        /*
        else if(gridpath[itrator].Y == y && gridLocation.X == gridpath[itrator].X)
        {
            Move(Movement.Left);
            direction = Movement.Left;
            isMoving = true;
        }

        else if (gridpath[itrator].Y < gridLocation.Y && gridpath[itrator].X == gridLocation.X + 1)
        {
            wheel.Body.ApplyLinearImpulse(new Vector2(0, -2));
            itrator++;
            Console.WriteLine(" WANTS TO JUMP grid vector :" + gridpath[itrator] + "enemy vector :" + gridLocation);

        }
        */



        public virtual Vector2[] findPathToPatrol(int dest)
        {
            int maxDestanationValue;
            if (dest < 0)
            {
                maxDestanationValue = r.Next(5, dest * -1);
                maxDestanationValue *= -1;
            }
            else
                maxDestanationValue = r.Next(5, dest);


            Vector2[] arr;
            path = PathFinder.FindPath(gridLocation.ToVector2(), new Vector2(gridLocation.X + maxDestanationValue, gridLocation.Y), "Euclidain");
            if (path == null)
                arr = new Vector2[] { gridLocation.ToVector2() };
            else
                arr = path.ToArray();

            return arr;
        }

        public virtual Vector2[] findPathToPlayer()
        {
            Vector2[] arr;

            path = PathFinder.FindPath(gridLocation.ToVector2(), player.GridLocation.ToVector2(), "Manhattan");
            if (path == null)
                arr = new Vector2[] { gridLocation.ToVector2() };
            else
                arr = path.ToArray();

            return arr;
        }

        public void setAstarsquare(Texture2D t)
        {
            gridColor = t;
        }

        private void swtichLookingDirection()
        {
            wheel.Body.ApplyLinearImpulse(new Vector2(0, -0.2f));
        }
        /*
        private float GetRandomSpeed()
        {
            float x = Game1.rnd.Next(12) - 6;
            if (x > 0) if (x < 4) x = 4;
            if (x < 0) if (x > -4) x = -4;
            return x;
        }
        */
    }
}