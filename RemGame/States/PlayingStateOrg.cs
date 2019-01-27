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


        //String[] mainMusicPlaylist;
        SoundEffect walking;
        SoundEffect jumping;
        SoundEffect hall;

        SoundEffectInstance walkingInstance;
        SoundEffectInstance jumpingInstance;
        SoundEffectInstance hallInstance;
        SoundManager soundManager;
        public PlayingState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IPlayingState), this);
            soundManager = new SoundManager(game);

            //soundManager = (ISoundManager)game.Services.GetService(typeof(ISoundManager));
            //general = new SoundManager(game);
        }

        protected override void LoadContent()
        {
            soundManager.LoadContent("Sound/Music", "Sound/SoundFX");
            soundManager.Play("General Music 1");
            

            hall = Content.Load<SoundEffect>("Sound/SoundFX/hallWay");
            hallInstance = hall.CreateInstance();
            
            hallInstance.Play();
            hallInstance.Volume = 0.06f;
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

            map.Generate(new int[,]
            {
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,3,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,1,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,3,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,0,0,0,3,0,0,0,0,0,1,0,0,0,0,3,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
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
            player = new Kid(world,
                Content.Load<Texture2D>("Player/Ron_standing"),
                Content.Load<Texture2D>("Player"),
                Content.Load<Texture2D>("Player/bullet"),
                new Vector2(96, 96),
                100,
                cam.ScreenToWorld(new Vector2(50, 575)), false, font);

            playerLeft = Content.Load<Texture2D>("Player/playerLeft");
            playerRight = Content.Load<Texture2D>("Player/playerRight");
            // player.Position = new Vector2(player.Size.X, GraphicsDevice.Viewport.Height - 87);

            walking = Content.Load<SoundEffect>("Sound/SoundFX/Footsteps Brick 1");
            walkingInstance = walking.CreateInstance();
            walkingInstance.IsLooped = true;
            walkingInstance.Volume = 0.02f;

            jumping = Content.Load<SoundEffect>("Sound/SoundFX/Jump");
            jumpingInstance = jumping.CreateInstance();
            jumpingInstance.IsLooped = false;
            jumpingInstance.Volume = 0.02f;
            //jumpingInstance.Pitch = 0.1f;




            player.Animations[0] = new AnimatedSprite(playerLeft, 1, 4);
            player.Animations[1] = new AnimatedSprite(playerRight, 1, 4);
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


            //after componnet list is set THIS can be deleted



            if (currentMouseState.RightButton == ButtonState.Pressed)
                {
                    Console.WriteLine(cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y)));
                    foreach (Obstacle obj in map.ObstacleTiles)
                    {
                        Vector2 mouseToWorld = cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y));
                        if (mouseToWorld.X >= obj.Position.X - 35 && mouseToWorld.X <= obj.Position.X + 35 && mouseToWorld.Y >= obj.Position.Y && mouseToWorld.Y <= obj.Position.Y + 70)
                            player.Kinesis(obj, currentMouseState);

                    }
                }

            foreach (var component in _gameComponents)
                component.Update(gameTime);




            if (player.IsMoving)
            {

                foreach (var scrolling in sc)
                {
                    if (player.Direction == Movement.Right)
                        scrolling.Update(cam, -1, player.WheelSpeed);
                    else
                        scrolling.Update(cam, 1, player.WheelSpeed);
                }
            }

            if (!player.IsBending)
            {

                walkingInstance.Pitch = 0.0f;
                if (player.IsMoving)
                {

                    walkingInstance.Play();

                }

                else
                {
                    walkingInstance.Stop();
                }
            }
            else
                walkingInstance.Pitch = -0.5f;

            if (player.IsJumping)
            {
                jumpingInstance.Play();

            }

            camLocation = new Vector2(player.Position.X, player.Position.Y - 200);

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
            
            if (Input.KeyboardHandler.WasKeyPressed(Keys.Escape))
                StateManager.PushState(OurGame.StartMenuState.Value);
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
     
            sc[0].Draw(spriteBatch);
            sc[6].Draw(spriteBatch);

            for (int i = 1; i < 3; i++)
            {
                sc[i].Draw(spriteBatch);
            }
            //////////////////////////////////////////////
            for (int i = 7; i < 9; i++)
            {
                sc[i].Draw(spriteBatch);

            }


            ///////////////////////////////////////
            ///
            //map.DrawEnemies(gameTime, spriteBatch);
            map.Update(gameTime);
        
            map.DrawEnemies(gameTime, spriteBatch);



            //////////////////////////////////////////////

            
            ///////////////////////////////////////////////
            ///


            spriteBatch.DrawString(font, cam.Position.X + "/" + cam.Position.Y, new Vector2(cam.Position.X, cam.Position.Y), Color.White);

            map.DrawObstacle(gameTime, spriteBatch);

            for (int i = 3; i <= 5; i++)

            {

                sc[i].Draw(spriteBatch);

            }

            for (int i = 9; i <= 11; i++)

            {

                sc[i].Draw(spriteBatch);

            }

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
