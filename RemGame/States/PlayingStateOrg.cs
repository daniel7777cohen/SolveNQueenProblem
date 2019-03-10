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




namespace RemGame
{

    public sealed class PlayingState : BaseGameState, IPlayingState
    {
        // private string[] playList = { "General Music 1" };
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
        Texture2D playerLeft;
        Texture2D playerRight;
        Texture2D playerStand;
        Texture2D playerWalk;
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
        const int maxLayers = 12;
        Scrollingbackground Sc1;
        Scrollingbackground Sc2;
        Scrollingbackground Sc3;
        Scrollingbackground Sc4;
        Scrollingbackground Sc5;
        Scrollingbackground Sc6;
        Scrollingbackground Sc7;
        Scrollingbackground Sc8;
        Scrollingbackground Sc9;
        Scrollingbackground Sc10;
        Scrollingbackground Sc11;
        Scrollingbackground Sc12;

        Texture2D backGround1;
        Texture2D backGround2;
        Texture2D backGround3;
        Texture2D backGround4;
        Texture2D backGround5;


        //String[] mainMusicPlaylist;
        SoundEffect walking;
        SoundEffect jumping;
        SoundEffect hall;

        SoundEffectInstance walkingInstance;
        SoundEffectInstance jumpingInstance;
        SoundEffectInstance hallInstance;
        SoundManager soundManager;
        public bool IsRonAlive { get => isRonAlive; set => isRonAlive = value; }

        public PlayingState(Game game)
            : base(game)
        {
            if (game.Services.GetService(typeof(IPlayingState)) == null)
                game.Services.AddService(typeof(IPlayingState), this);
            soundManager = new SoundManager(game);
            //soundManager = (ISoundManager)game.Services.GetService(typeof(ISoundManager));
            //general = new SoundManager(game);
        }

        protected override void LoadContent()
        {
            soundManager.LoadContent(@"Sound/Music", @"Sound/FX");
            //soundManager.Play("General Music 1");
            Texture2D hearts = Content.Load<Texture2D>("misc/heart");


            hall = Content.Load<SoundEffect>("Sound/FX/hallWay");
            hallInstance = hall.CreateInstance();
            hallInstance.IsLooped = true;

            hallInstance.Play();
            hallInstance.Volume = 0.04f;
            //  general.LoadContent(@"Sounds/Music/", "");
            //general.Play("General Music 1");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cam = new Camera2D(GraphicsDevice);
            world = new World(new Vector2(0, 9.8f));
            map = new Map(world);

            // mainMusicPlaylist = new string[] { "General Music 1" };
            // soundManager.StartPlayList(mainMusicPlaylist, 0);
            //soundManager.Play("MonoGame MusicTest - Accordion 1");
            //soundManager.Play("MonoGame MusicTest - Precussion 1");

            //soundManager.Play("General Music 1");
            font = Content.Load<SpriteFont>("Fonts/Font");

            Tile.Content = Content;
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
            /*
            map.Generate(new int[,]
            {
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,1,1,1,1,0,1,1,1,1,0,1,1,0,1,1,0,0,1,1,0,1,0,1,0,1,1,0,0,1,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,1,0,0,1,0,1,0,0,0,0,1,0,1,0,1,0,0,0,1,0,0,1,1,0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,1,1,1,1,0,1,1,1,1,0,1,0,0,0,1,0,1,1,1,0,1,1,0,1,0,0,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,1,1,0,0,0,1,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,1,0,1,0,0,1,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,1,0,0,1,0,1,1,1,1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,1,1,0,0,1,1,0,0,0,0,0,1,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,1,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,1,1,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            }, 64, font);
            */
            //REM
            /*
            map.Generate(new int[,]
            {
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,1,1,1,1,0,1,1,0,1,1,0,0,1,1,0,1,0,1,0,1,1,0,0,1,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,1,0,0,0,0,1,0,1,0,1,0,0,0,1,0,0,1,1,0,0,1,0,0,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,1,1,1,1,0,1,0,0,0,1,0,1,1,1,0,1,1,0,1,0,0,1,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,1,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,1,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,1,1,1,1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,1,1,0,0,1,1,0,0,0,0,0,1,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            }, 64, font);
            */
            ////Straight MAP
            ///

            //14
            map.Generate(new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                //{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,0,0,3,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                //{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                //{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            }, 64, font);

            Sc1 = new Scrollingbackground(Content.Load<Texture2D>("Layers/b-01_background"), new Rectangle((int)cam.BoundingRectangle.Left - 1920, -150, 1920, 1200), 0, null);
            Sc2 = new Scrollingbackground(Content.Load<Texture2D>("Layers/b-02_back-A"), new Rectangle((int)cam.BoundingRectangle.Left - 1920, 0, 1920, 1080), 2, null);
            Sc3 = new Scrollingbackground(Content.Load<Texture2D>("Layers/b-03_back-B"), new Rectangle((int)cam.BoundingRectangle.Left - 1920, 0, 1920, 1080), 2, null);
            Sc4 = new Scrollingbackground(Content.Load<Texture2D>("Layers/b-04_ground"), new Rectangle((int)cam.BoundingRectangle.Left - 1920, 0, 1920, 1080), 0, null);
            Sc5 = new Scrollingbackground(Content.Load<Texture2D>("Layers/f-01_ground-grass"), new Rectangle((int)cam.BoundingRectangle.Left - 1920, 0, 1920, 1080), 0, null);
            Sc6 = new Scrollingbackground(Content.Load<Texture2D>("Layers/f-02_front"), new Rectangle((int)cam.BoundingRectangle.Left - 1920, 0, 1920, 1080), 4, null);
            Sc7 = new Scrollingbackground(Content.Load<Texture2D>("Layers/b-01_background"), new Rectangle((int)cam.Position.X, -150, 1920, 1200), 0, Sc1);
            Sc8 = new Scrollingbackground(Content.Load<Texture2D>("Layers/b-02_back-A"), new Rectangle((int)cam.Position.X, 0, 1920, 1080), 2, Sc2);
            Sc9 = new Scrollingbackground(Content.Load<Texture2D>("Layers/b-03_back-B"), new Rectangle((int)cam.Position.X, 0, 1920, 1080), 2, Sc3);
            Sc10 = new Scrollingbackground(Content.Load<Texture2D>("Layers/b-04_ground"), new Rectangle((int)cam.Position.X, 0, 1920, 1080), 0, Sc4);
            Sc11 = new Scrollingbackground(Content.Load<Texture2D>("Layers/f-01_ground-grass"), new Rectangle((int)cam.Position.X, 0, 1920, 1080), 0, Sc5);
            Sc12 = new Scrollingbackground(Content.Load<Texture2D>("Layers/f-02_front"), new Rectangle((int)cam.Position.X, 0, 1920, 1080), 4, Sc6);

            sc = new Scrollingbackground[maxLayers];

            sc[0] = Sc1;
            sc[1] = Sc2;
            sc[2] = Sc3;
            sc[3] = Sc4;
            sc[4] = Sc5;
            sc[5] = Sc6;
            sc[6] = Sc7;
            sc[7] = Sc8;
            sc[8] = Sc9;
            sc[9] = Sc10;
            sc[10] = Sc11;
            sc[11] = Sc12;
            backGround1 = Content.Load<Texture2D>("Layers/level/row-1-col-1");
            backGround2 = Content.Load<Texture2D>("Layers/level/row-1-col-2");
            backGround3 = Content.Load<Texture2D>("Layers/level/row-1-col-3");
            backGround4 = Content.Load<Texture2D>("Layers/level/row-1-col-4");
            backGround5 = Content.Load<Texture2D>("Layers/level/row-1-col-5");
            for (int i = 0; i < 6; i++)
            {
                sc[i].setRighttwinSc(sc[i + 6]);
            }
            /*


            floor = new Floor(world, Content.Load<Texture2D>("cave_walk"), new Vector2(GraphicsDevice.Viewport.Width * 2, 60));
            floor.Position = new Vector2(0, GraphicsDevice.Viewport.Height - 60);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            plat = new Obstacle[maxPlat];
            /*
            for (int i = 0; i < maxPlat; i++)
            {
                plat[3-i] = new Obstacle(world, Content.Load<Texture2D>("HUD"), new Vector2(70,70),font);
                plat[3-i].Position = new Vector2(500 + 200 * i, 600 - 45 * i);
                plat[3-i].Body.BodyType = BodyType.Static;
            }
            */
            playerLeft = Content.Load<Texture2D>("Player/playerLeft");
            playerRight = Content.Load<Texture2D>("Player/playerRight");
            playerStand = Content.Load<Texture2D>("Player/Anim/Ron_Stand");
            playerWalk = Content.Load<Texture2D>("Player/Anim/Ron_Walk");
            SpriteEffects flip = SpriteEffects.FlipHorizontally;

            player = new Kid(hearts,world,
                Content.Load<Texture2D>("Player/Anim/Ron_Stand"),
                Content.Load<Texture2D>("Player/Anim/Ron_Stand"),
                Content.Load<Texture2D>("Player/bullet"),
                new Vector2(60, 60),
                100,
                cam.ScreenToWorld(new Vector2(50, 400)), false, font);


            // player.Position = new Vector2(player.Size.X, GraphicsDevice.Viewport.Height - 87);

            walking = Content.Load<SoundEffect>("Sound/FX/Footsteps Brick 1");
            walkingInstance = walking.CreateInstance();
            walkingInstance.IsLooped = true;
            walkingInstance.Volume = 0.009f;
            walkingInstance.Pitch = -0.2f;


            jumping = Content.Load<SoundEffect>("Sound/FX/Jump");
            jumpingInstance = jumping.CreateInstance();
            jumpingInstance.IsLooped = false;
            jumpingInstance.Volume = 0.01f;
            //jumpingInstance.Pitch = 0.1f;

            Rectangle anim3 = new Rectangle(-30, -65, 240, 160);
            Rectangle anim4 = new Rectangle(20, -50, 140, 130);


            player.Animations[0] = new AnimatedSprite(playerLeft, 1, 4, anim3, 0f);
            player.Animations[1] = new AnimatedSprite(playerRight, 1, 4, anim3, 0f);

            player.Animations[2] = new AnimatedSprite(playerStand, 4,13, anim3,0.017f);

            player.Animations[3] = new AnimatedSprite(playerWalk, 2, 12, anim3, 0.017f);
            

            map.setPlayerToMap(player);


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

            handleInput(gameTime);

            currentMouseState = Mouse.GetState();
            walkingInstance.Volume = 0.02f;


            //after componnet list is set THIS can be deleted



            if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                // Console.WriteLine(cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y)));
                foreach (Obstacle obj in map.ObstacleTiles)
                {
                    Vector2 mouseToWorld = cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y));
                    if (mouseToWorld.X >= obj.Position.X - 35 && mouseToWorld.X <= obj.Position.X + 35 && mouseToWorld.Y >= obj.Position.Y && mouseToWorld.Y <= obj.Position.Y + 70)
                        player.Kinesis(obj, currentMouseState);

                }
            }

            foreach (var component in _gameComponents)
                component.Update(gameTime);




            if (player.IsMoving && player.ActualMovningSpeed != 0)
            {
                //  Console.WriteLine(player.ActualMovningSpeed);

                foreach (var scrolling in sc)
                {
                    if (player.Direction == Movement.Right)
                        scrolling.Update(cam, -1);
                    else
                        scrolling.Update(cam, 1);
                }
            }

            if (!player.IsBending)
            {

                //walkingInstance.Pitch = 0.0f;
                if (player.IsMoving && !player.IsJumping)
                {
                    walkingInstance.Play();
                }

                else
                {
                    walkingInstance.Stop();
                }
            }
            else
            {
                walkingInstance.Pitch = -0.5f;
                walkingInstance.Volume = 0.01f;
            }

            if (player.IsJumping)
            {
                jumpingInstance.Play();
            }

            camLocation = new Vector2(player.Position.X, player.Position.Y - 100);

            cam.LookAt(camLocation);
            //cam.Rotate(0.0005f);
            //cam.ZoomOut(0.0001f);
            map.Update(gameTime);
            // soundManager.Update(gameTime);
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);


            base.Update(gameTime);
        }

        private void handleInput(GameTime gameTime)
        {
            if (!player.IsAlive)
            {
                StateManager.PopState();
                OurGame.Reset();
                StateManager.PushState(OurGame.StartMenuState.Value);
                player.IsAlive = true;
            }
            if (Input.KeyboardHandler.WasKeyPressed(Keys.Escape))
                StateManager.PushState(OurGame.EscapeState.Value);
            if (Input.KeyboardHandler.WasKeyPressed(Keys.P))
            {
                StateManager.PushState(OurGame.PausedState.Value);
                soundManager.PauseSong();
            }
            if (!(StateManager.State == OurGame.PausedState.Value))
                soundManager.ResumeSong();

            /*
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (Obstacle obj in map.ObstacleTiles)
                {
                    Vector2 mouseToWorld = cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y));
                    if (mouseToWorld.X >= obj.Position.X - 35 && mouseToWorld.X <= obj.Position.X + 35 && mouseToWorld.Y >= obj.Position.Y && mouseToWorld.Y <= obj.Position.Y + 70)
                        player.Kinesis(obj, currentMouseState);

                }
            }
            */






        }

        public override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(_backgroundColor);
            spriteBatch.Begin(transformMatrix: cam.GetViewMatrix());
            Rectangle backgroundREC1 = new Rectangle(0, -700, 5693, 1969);
            Rectangle backgroundREC2 = new Rectangle(4097, -700, 5693, 1969);
            Rectangle backgroundREC3 = new Rectangle(8193, -700, 5693, 1969);
            Rectangle backgroundREC4 = new Rectangle(12990, -700, 5693, 1969);
            Rectangle backgroundREC5 = new Rectangle(16387, -700, 5693, 1969);

            //sc[0].Draw(spriteBatch);
            //sc[6].Draw(spriteBatch);
            /*
            for (int i = 1; i < 3; i++)
            {
                sc[i].Draw(spriteBatch);
            }
            //////////////////////////////////////////////
            for (int i = 7; i < 9; i++)
            {
                sc[i].Draw(spriteBatch);

            }
            */

            ///////////////////////////////////////
            ///
            //map.DrawEnemies(gameTime, spriteBatch);
            map.Update(gameTime);

            //////////////////////////////////////////////


            ///////////////////////////////////////////////
            ///


            spriteBatch.DrawString(font, cam.Position.X + "/" + cam.Position.Y, new Vector2(cam.Position.X, cam.Position.Y), Color.White);

            map.DrawObstacle(gameTime, spriteBatch);

            spriteBatch.Draw(backGround1, backgroundREC1, null, Color.White);
            spriteBatch.Draw(backGround2, backgroundREC2, null, Color.White);
            spriteBatch.Draw(backGround3, backgroundREC3, null, Color.White);
            spriteBatch.Draw(backGround4, backgroundREC4, null, Color.White);
            spriteBatch.Draw(backGround5, backgroundREC5, null, Color.White);

            map.DrawEnemies(gameTime, spriteBatch);

            /*
            for (int i = 3; i <= 5; i++)

            {

                sc[i].Draw(spriteBatch);

            }

            for (int i = 9; i <= 11; i++)

            {

                sc[i].Draw(spriteBatch);

            }
            */
            //spriteBatch.DrawString(font, "Mouse Position" + cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y)), new Vector2(GraphicsDevice.Viewport.Width / 2.0f - 120f, -GraphicsDevice.Viewport.Height + 900), Color.White);

            //spriteBatch.DrawString(font, "Mouse Position" + cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y)), new Vector2(cam.Position.X + cam.BoundingRectangle.Width / 2, -cam.Position.Y + cam.BoundingRectangle.Height / 4), Color.White);

            foreach (var component in _gameComponents)
            {
                component.Draw(gameTime, spriteBatch);
            }

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