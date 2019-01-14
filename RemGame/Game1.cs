using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System.Collections.Generic;
using System;

namespace RemGame
{
    enum Movement
    {
        Left,
        Right,
        Jump,
        Stop
    }

    enum GameState
    {
        MainMenu,
        Options,
        Playing
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //GameState CurrentGameState = GameState.MainMenu;
        private Color _backgroundColor = Color.CornflowerBlue;
        private List<Component> _gameComponents;

        World world;
        Kid player;
        Enemy DemoEnemy;
        Floor floor;
        KeyboardState keyboardState;
        KeyboardState prevKeyboardState = Keyboard.GetState();
        MouseState currentMouseState;

        Texture2D playerLeft;
        Texture2D playerRight;


        SpriteFont font;

        Camera2D cam;
        Vector2 camLocation;
        /// <summary>
        /// ///////////////////
        /// </summary>
        //PhysicsObject[] plat;
        Obstacle[] plat;
        
        const int maxPlat = 4;
        /// <summary>
        /// ///////////////////
        /// </summary>
        Map map;

        Scrollingbackground[] sc;
        const int maxLayers = 6;
        Scrollingbackground Sc1;
        Scrollingbackground Sc2;
        Scrollingbackground Sc3;
        Scrollingbackground Sc4;
        Scrollingbackground Sc5;
        Scrollingbackground Sc6;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

           // graphics.PreferredBackBufferWidth = 1920;
           // graphics.PreferredBackBufferHeight = 1080;


        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            cam = new Camera2D(GraphicsDevice);
            //cam.

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            world = new World(new Vector2(0, 9.8f));

            map = new Map(world);

            font = Content.Load<SpriteFont>("Fonts/Font");

            //Tile.Content = Content;
            Map.Content = Content;
            /*
            map.Generate(new int[,]
            {
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
                {2,1,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,1,1,2,2},
                {2,2,0,0,0,0,0,0,0,1,1,1,2,2,2,1,0,0,0,0,2,2},
                {2,2,0,0,0,0,0,0,1,2,2,2,2,2,2,2,1,0,0,0,2,2},
                {2,0,0,0,0,0,1,1,2,2,2,2,2,2,2,2,2,1,1,1,2,2},
                {2,0,0,0,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,1,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            }, 64,font);
            */
            map.Generate(new int[,]
            {
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
                {2,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,1,1},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,1,1},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
                {2,0,0,0,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,1,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            }, 64, font);
            /*
            var randomButton = new Button(Content.Load<Texture2D>("Buttons/Button"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(350,200),
                Text = "Random",
            };

            randomButton.Click += RandomButton_Click;

            var quitButton = new Button(Content.Load<Texture2D>("Buttons/Button"), Content.Load<SpriteFont>("Fonts/Font"))
            {
                Position = new Vector2(350, 250),
                Text = "Quit",
            };

            quitButton.Click += QuitButton_Click;
            */


            Sc1 = new Scrollingbackground(Content.Load<Texture2D>("Layers/b-01_background"), new Rectangle(0, 0, 1280, 720));
            Sc2 = new Scrollingbackground(Content.Load<Texture2D>("Layers/b-02_back-A"), new Rectangle(0, 0, 1280, 720));
            Sc3 = new Scrollingbackground(Content.Load<Texture2D>("Layers/b-03_back-B"), new Rectangle(0, 0, 1280, 720));
            Sc4 = new Scrollingbackground(Content.Load<Texture2D>("Layers/b-04_ground"), new Rectangle(0, 0, 1280, 720));
            Sc5 = new Scrollingbackground(Content.Load<Texture2D>("Layers/f-01_ground-grass"), new Rectangle(0, 0, 1280, 720+200));
            Sc6 = new Scrollingbackground(Content.Load<Texture2D>("Layers/f-02_front"), new Rectangle(0, 0, 1280, 720));


            sc = new Scrollingbackground[maxLayers];
            sc[0] = Sc1;
            sc[1] = Sc2;
            sc[2] = Sc3;
            sc[3] = Sc4;
            sc[4] = Sc5;
            sc[5] = Sc6;


            floor = new Floor(world,Content.Load<Texture2D>("cave_walk"),new Vector2(GraphicsDevice.Viewport.Width*2, 60));
            floor.Position = new Vector2(0, GraphicsDevice.Viewport.Height-60);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            /*plat = new Obstacle[maxPlat];
            for (int i = 0; i < maxPlat; i++)
            {
                plat[3-i] = new Obstacle(world, Content.Load<Texture2D>("HUD"), new Vector2(70,70),font);
                plat[3-i].Position = new Vector2(500 + 200 * i, 600 - 45 * i);
                plat[3-i].Body.BodyType = BodyType.Static;
            }
            */
            player = new Kid(world,
                Content.Load<Texture2D>("Player/Ron_standing"),
                Content.Load<Texture2D>("Player"),
                Content.Load<Texture2D>("Player/bullet"),
                new Vector2(96, 96),
                100,
                new Vector2(100, 0), false, font);


            DemoEnemy = new Enemy(world,
                Content.Load<Texture2D>("Player"),
                Content.Load<Texture2D>("Player"),
                Content.Load<Texture2D>("Player/bullet"),
                new Vector2(96, 96),
                100,
                new Vector2(100, 0), false, font);



            playerLeft = Content.Load<Texture2D>("Player/playerLeft");
            playerRight = Content.Load<Texture2D>("Player/playerRight");
            // player.Position = new Vector2(player.Size.X, GraphicsDevice.Viewport.Height - 87);

            player.Animations[0] = new AnimatedSprite(playerLeft, 1, 4);
            player.Animations[1] = new AnimatedSprite(playerRight, 1, 4);

            DemoEnemy.Animations[0] = new AnimatedSprite(playerLeft, 1, 4);
            DemoEnemy.Animations[1] = new AnimatedSprite(playerRight, 1, 4);
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            _gameComponents = new List<Component>()
            {
                //randomButton,
                //quitButton,
                player,
                DemoEnemy,

            };
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void RandomButton_Click(object sender, EventArgs e)
        {
            var random = new Random();

            _backgroundColor = new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            currentMouseState = Mouse.GetState();

            //after componnet list is set THIS can be deleted
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            /*
            for(int i =0; i <= 5; i++)
            {
                if (sc[i].rectangle.X + sc[i].texture.Width <= 0)
                {
                    sc[i].rectangle.X = sc[i].rectangle.X + sc[i].texture.Width;
                }

                sc[i].Update();
            }
            */

            if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                foreach (Obstacle obj in map.ObstacleTiles)
                {
                    Vector2 mouseToWorld = cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y));
                    if (mouseToWorld.X>=obj.Position.X-35&& mouseToWorld.X<=obj.Position.X+35&&mouseToWorld.Y>=obj.Position.Y&&mouseToWorld.Y<=obj.Position.Y+70)
                           player.Kinesis(obj, currentMouseState);


                        // if ((currentMouseState.Position.X >= obj.Position.X-35) && (currentMouseState.Position.X <= obj.Position.X+35)&&
                        //     (currentMouseState.Position.Y >= obj.Position.Y) && (currentMouseState.Position.Y <= obj.Position.Y +70))
                        // {
                        //    player.Kinesis(obj, currentMouseState);
                        // }
                }
            }

            foreach (var component in _gameComponents)
                component.Update(gameTime);

            
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            camLocation = new Vector2(player.Position.X, player.Position.Y - 100);
            cam.LookAt(camLocation);

            //cam.Rotate(0.0005f);
            //cam.ZoomOut(0.0001f);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);

            spriteBatch.Begin(transformMatrix: cam.GetViewMatrix());

            map.Draw(gameTime,spriteBatch);
            //floor.Draw(spriteBatch);
            /*
            for (int i = 0; i < 4; i++)
            {
                sc[i].Draw(spriteBatch);
            }
            */
            spriteBatch.DrawString(font, "Mouse Position"+cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y)), new Vector2(GraphicsDevice.Viewport.Width / 2.0f - 120f, -GraphicsDevice.Viewport.Height + 900), Color.White);

            //floor.Draw(spriteBatch);
            spriteBatch.DrawString(font, "*", new Vector2(floor.Position.X+100, floor.Position.Y), Color.White);

            //////////////////////////////////////////////
            ///
           /*
            foreach (Obstacle p in plat)
            {
                p.Draw(gameTime,spriteBatch);
            }
*/
            foreach (var component in _gameComponents)
                component.Draw(gameTime, spriteBatch);
            ///////////////////////////////////////////////
            ///


            spriteBatch.DrawString(font, cam.Position.X + "/" + cam.Position.Y, new Vector2(cam.Position.X, cam.Position.Y), Color.White);
            /*
            for (int i = 4; i <= 5; i++)
            {
                
                sc[i].Draw(spriteBatch);
                
           }
           */
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
