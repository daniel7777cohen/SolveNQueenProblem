using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using XELibrary;

namespace RemGame
{
    enum Movement
    {
        Left,
        Right,
        Jump,
        Stop
    }
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;


        public SpriteBatch SpriteBatch { get; private set; }
       
        CelAnimationManager celAnimationManager;
        ScrollingBackgroundManager scrollingBackgroundManager;
        InputHandler inputHandler;
        //SoundManager soundManager;
        GameStateManager stateManager;

        public ITitleIntroState TitleIntroState;
        public IStartMenuState StartMenuState;
        public IPlayingState PlayingState;
        public IMissionOne Mission1;
        public IEscapeState EscapeState;
        public IPausedState PausedState;
        public IOptionsMenuState OptionsMenuState;

        public bool EnableSoundFx { get; set; }
        public bool EnableMusic { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            //graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";

            inputHandler = new InputHandler(this);
            Components.Add(inputHandler);

            celAnimationManager = new CelAnimationManager(this, "Textures\\");
            Components.Add(celAnimationManager);

            scrollingBackgroundManager = new ScrollingBackgroundManager(this, "Textures\\");
            Components.Add(scrollingBackgroundManager);

            // soundManager = new SoundManager(this);
            //Components.Add(soundManager);

            stateManager = new GameStateManager(this);
            Components.Add(stateManager);

            TitleIntroState = new TitleIntroState(this);
            StartMenuState = new StartMenuState(this);
            PausedState = new PausedState(this);
            OptionsMenuState = new OptionsMenuState(this);
            PlayingState = new PlayingState(this);
            Mission1 = new Mission1(this);
            EscapeState = new EscapeState(this);
            EnableSoundFx = true;
            EnableMusic = true;

        }
        public void Reset()
        {
            base.Initialize();
            base.BeginRun();
            TitleIntroState = new TitleIntroState(this);
            StartMenuState = new StartMenuState(this);
            PausedState = new PausedState(this);
            OptionsMenuState = new OptionsMenuState(this);
            PlayingState = new PlayingState(this);
            Mission1 = new Mission1(this);
            EscapeState = new EscapeState(this);
        }
        protected override void Initialize()
        {
            IsMouseVisible = true;
            //scrollingBackgroundManager.setCamera(cam);
            base.Initialize();
        }

        protected override void BeginRun()
        {
            stateManager.ChangeState(TitleIntroState.Value);
            IsMouseVisible = true;

            base.Initialize();
            base.BeginRun();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Load sounds
            string musicPath = @"Sound/Music/";
            string fxPath = @"Sound/FX/";

            //soundManager.LoadContent(musicPath, fxPath);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin();
            base.Draw(gameTime);
            SpriteBatch.End();
        }
    }
}