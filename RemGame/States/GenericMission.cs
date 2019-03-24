using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using XELibrary;
using Microsoft.Xna.Framework.Media;

namespace RemGame
{

    public sealed class GenericMissions : BaseGameState, IMissionOne
    {
        SpriteBatch spriteBatch;

        private Color _backgroundColor = Color.CornflowerBlue;
        private List<Component> _gameComponents;
        private bool isRonAlive = true;

        World world;
        Kid player;
        Enemy DemoEnemy;
        Floor floor;
        KeyboardState keyboardState;
        KeyboardState prevKeyboardState = Keyboard.GetState();
        MouseState currentMouseState;

        SpriteFont font;
        Camera2D cam;
        Vector2 camLocation;

        Map map;

        public bool IsRonAlive { get => isRonAlive; set => isRonAlive = value; }

        public GenericMissions(Game game)
            : base(game)
        {
            if (game.Services.GetService(typeof(IMissionOne)) == null)
                game.Services.AddService(typeof(IMissionOne), this);

        }

        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            cam = new Camera2D(GraphicsDevice);
            world = new World(new Vector2(0, 9.8f));

            font = Content.Load<SpriteFont>("Fonts/Font");

            Tile.Content = Content;
            Map.Content = Content;
            Enemy.Content = Content;
            Kid.Content = Content;



            player = new Kid(cam,world,
                new Vector2(60, 60),
                100,
                cam.ScreenToWorld(new Vector2(650, 440)), false, font);


            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            _gameComponents = new List<Component>()
            {
                //randomButton,
                //quitButton,
                
                player,

            };
        }

        public override void Update(GameTime gameTime)
        {


            currentMouseState = Mouse.GetState();

            foreach (var component in _gameComponents)
                component.Update(gameTime);


            camLocation = new Vector2(player.Position.X, player.Position.Y - 100);

            cam.LookAt(camLocation);
            //cam.Rotate(0.0005f);
            //cam.ZoomOut(0.0001f);
            // soundManager.Update(gameTime);
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);


            base.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(_backgroundColor);
            spriteBatch.Begin(transformMatrix: cam.GetViewMatrix());

            //spriteBatch.DrawString(font, cam.Position.X + "/" + cam.Position.Y, new Vector2(cam.Position.X, cam.Position.Y), Color.White);

            foreach (var component in _gameComponents)
            {
                component.Draw(gameTime, spriteBatch);
            }

            //spriteBatch.DrawString(font, "Mouse Position" + cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y)), new Vector2(GraphicsDevice.Viewport.Width / 2.0f - 120f, -GraphicsDevice.Viewport.Height + 900), Color.White);
            //spriteBatch.DrawString(font, "Mouse Position" + cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y)), new Vector2(cam.Position.X + cam.BoundingRectangle.Width / 2, -cam.Position.Y + cam.BoundingRectangle.Height / 4), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            base.StateChanged(sender, e);
            /*
            // Change to visible if not at the top of the stack
            if (StateManager.State != this.Value)
            {
                Visible = true;
                soundManager.StopPlayList();
            }
            else
            {
                if (OurGame.EnableMusic)
                    soundManager.StartPlayList(playList);
            }
            */
        }

    }
}