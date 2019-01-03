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
        Kid playerBent;
        Floor floor;
        Floor plat1;
        KeyboardState keyboardState;
        KeyboardState prevKeyboardState = Keyboard.GetState();
        MouseState currentMouseState;

        Texture2D playerLeft;
        Texture2D playerRight;


        SpriteFont font;


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

            font = Content.Load<SpriteFont>("Fonts/Font");

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
                new Vector2(96, 96),
                100,
                new Vector2(0, 0),false,this);
            



            playerLeft = Content.Load<Texture2D>("Player/playerLeft");
            playerRight = Content.Load<Texture2D>("Player/playerRight");
            // player.Position = new Vector2(player.Size.X, GraphicsDevice.Viewport.Height - 87);

            player.Animations[0] = new AnimatedSprite(playerLeft, 1, 4);
            player.Animations[1] = new AnimatedSprite(playerRight, 1, 4);


            floor = new Floor(world,Content.Load<Texture2D>("cave_walk"),new Vector2(GraphicsDevice.Viewport.Width*2, 60));
            floor.Position = new Vector2(0, GraphicsDevice.Viewport.Height-60);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            plat = new PhysicsObject[maxPlat];
            for (int i = 0; i < maxPlat; i++)
            {
                plat[3-i] = new PhysicsObject(world, Content.Load<Texture2D>("HUD"), 70, 10);
                plat[3-i].Position = new Vector2(500 + 200 * i, 600 - 45 * i);
                plat[3-i].Body.BodyType = BodyType.Static;
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            _gameComponents = new List<Component>()
            {
                randomButton,
                quitButton,
                player,
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
                    
                    if ((currentMouseState.Position.X >= obj.Position.X-35) && (currentMouseState.Position.X <= obj.Position.X+35)&&
                        (currentMouseState.Position.Y >= obj.Position.Y) && (currentMouseState.Position.Y <= obj.Position.Y +70))
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

            spriteBatch.DrawString(font, "Mouse Position"+currentMouseState.Position.X+" ,"+currentMouseState.Position.Y, new Vector2(GraphicsDevice.Viewport.Width / 2.0f - 120f, -GraphicsDevice.Viewport.Height + 900), Color.White);

            foreach (var component in _gameComponents)
                component.Draw(gameTime,spriteBatch);

            
            floor.Draw(spriteBatch);
            spriteBatch.DrawString(font, "*", new Vector2(floor.Position.X+100, floor.Position.Y), Color.White);

            //////////////////////////////////////////////
            foreach (PhysicsObject p in plat)
            {
                p.Draw(gameTime,spriteBatch);
                spriteBatch.DrawString(font, "*", new Vector2(p.Position.X,p.Position.Y), Color.White);
                spriteBatch.DrawString(font, "*" , new Vector2(p.Position.X, (p.Position.Y + 70)), Color.White);
                spriteBatch.DrawString(font, "*", new Vector2(p.Position.X-35, p.Position.Y+35), Color.White);
                spriteBatch.DrawString(font, "*", new Vector2(p.Position.X+35, p.Position.Y+35), Color.White);

            }
            ///////////////////////////////////////////////
            //spriteBatch.DrawString(font, "Player Position" + player.Position.X + " ," + player.Position.Y, new Vector2(player.Position.X, player.Position.Y), Color.White);
           spriteBatch.DrawString(font, "*", new Vector2(player.Position.X, player.Position.Y), Color.White);
           spriteBatch.DrawString(font, "*", new Vector2(player.Position.X, (player.Position.Y+96)), Color.White);
           spriteBatch.DrawString(font, "*", new Vector2(player.Position.X-48, player.Position.Y+48), Color.White);
           spriteBatch.DrawString(font, "*", new Vector2(player.Position.X+48, player.Position.Y+48), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
