using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace RemGame
{
    enum Movement
    {
        Jump,
        Left,
        Right,
        Stop
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        World world;
        Kid player;
        Floor floor;
        private KeyboardState prevKeyboardState = Keyboard.GetState();

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
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            world = new World(new Vector2(0, 9.8f));

            player = new Kid(world,
                Content.Load<Texture2D>("Player"),
                Content.Load<Texture2D>("Player"),
                new Vector2(58, 62),
                100,
                new Vector2(430, 0));
           // player.Position = new Vector2(player.Size.X, GraphicsDevice.Viewport.Height - 87);

            floor = new Floor(world,Content.Load<Texture2D>("cave_walk"),new Vector2(GraphicsDevice.Viewport.Width,60));
            floor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 25);





        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                player.Move(Movement.Left);
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                player.Move(Movement.Right);
            }
            else
            {
                player.Move(Movement.Stop);
            }
            //if statment should changed
            if (keyboardState.IsKeyDown(Keys.Space) && !(prevKeyboardState.IsKeyDown(Keys.Space)))
{
                player.Jump();
            }
            prevKeyboardState = keyboardState;

            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            player.Draw(spriteBatch);
            floor.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
