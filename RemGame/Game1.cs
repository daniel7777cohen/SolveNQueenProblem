using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        Floor floor;
        KeyboardState prevKeyboardState = Keyboard.GetState();
        MouseState currentMouseState;

        Texture2D playerLeft;
        Texture2D playerRight;





        /// <summary>
        /// ///////////////////
        /// </summary>
        PhysicsObject[] plat;
        const int maxPlat = 4;
        /// <summary>
        /// ///////////////////
        /// </summary>


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;


        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

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

            

            world = new World(new Vector2(0, 9.8f));



            player = new Kid(world,
                Content.Load<Texture2D>("Player"),
                Content.Load<Texture2D>("Player"),
                Content.Load<Texture2D>("Player/bullet"),
                new Vector2(58, 31),
                100,
                new Vector2(400, 0),this);

            playerLeft = Content.Load<Texture2D>("Player/playerLeft");
            playerRight = Content.Load<Texture2D>("Player/playerRight");
            // player.Position = new Vector2(player.Size.X, GraphicsDevice.Viewport.Height - 87);

            player.Animations[0] = new AnimatedSprite(playerLeft, 1, 4);
            player.Animations[1] = new AnimatedSprite(playerRight, 1, 4);

            floor = new Floor(world,Content.Load<Texture2D>("cave_walk"),new Vector2(GraphicsDevice.Viewport.Width,60));
            floor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 25);
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            plat = new PhysicsObject[maxPlat];
            for (int i = 0; i < maxPlat; i++)
            {
                plat[3-i] = new PhysicsObject(world, Content.Load<Texture2D>("HUD"), 70, 100);
                plat[3-i].Position = new Vector2(500 + 80 * i, 630 - 45 * i);
                plat[3-i].Body.BodyType = BodyType.Static;
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            _gameComponents = new List<Component>()
            {
                randomButton,
                quitButton,
                player
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

            if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                foreach (PhysicsObject obj in plat)
                {
                    Console.WriteLine(CoordinateHelper.pixelToUnit * currentMouseState.Position.ToVector2());
                    Console.WriteLine(obj.Body.Position);
                    if ((CoordinateHelper.pixelToUnit * currentMouseState.Position.X >= obj.Body.Position.X) && (CoordinateHelper.pixelToUnit * currentMouseState.Position.X <= obj.Body.Position.X+obj.Size.X))
                    {
                        player.Kinesis(obj);
                        
                    }
                }
            }

            foreach (var component in _gameComponents)
                component.Update(gameTime);



            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);

            spriteBatch.Begin();

            foreach (var component in _gameComponents)
                component.Draw(gameTime,spriteBatch);

            
            player.Draw(gameTime,spriteBatch);
            floor.Draw(spriteBatch);
            //////////////////////////////////////////////
            foreach(PhysicsObject p in plat)
            {
                p.Draw(spriteBatch);
            }
            ///////////////////////////////////////////////

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
