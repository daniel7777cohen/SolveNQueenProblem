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

    public sealed class PlayingState : BaseGameState, IPlayingState
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
        MouseState previousMouseState;

        Texture2D playerCrouch;
        Texture2D playerCrouchWalk;
        Texture2D playerStand;
        Texture2D playerWalk;
        Texture2D[] jumpSetAnim = new Texture2D[5];
        Texture2D[] slideSetAnim = new Texture2D[3];

        SpriteFont font;
        Camera2D cam;
        Vector2 camLocation;

        Map map;

        Rectangle closingWall;

        Texture2D backGround1;
        Texture2D backGround2;
        Texture2D backGround3;
        Texture2D backGround4;
        Texture2D backGround5;

        Rectangle backgroundREC1;
        Rectangle backgroundREC2;
        Rectangle backgroundREC3;
        Rectangle backgroundREC4;
        Rectangle backgroundREC5;

        Texture2D hearts;

        Texture2D wall;


        SoundEffect footstep;
        SoundEffect jump_Up;
        SoundEffect jump_Down;
        SoundEffect idle;
        SoundEffect crouch;
        SoundEffect slide;

        SoundEffect hall;

        Song GeneralMusic;



        bool isJumpSoundPlayed = false;


        SoundEffectInstance walkingInstance;
        SoundEffectInstance jumpingUpInstance;
        SoundEffectInstance jumpingDownInstance;
        SoundEffectInstance idleInstance;
        SoundEffectInstance crouchingInstance;
        SoundEffectInstance slidingInstance;
       

        SoundEffectInstance hallInstance;

        public bool IsRonAlive { get => isRonAlive; set => isRonAlive = value; }

        public PlayingState(Game game)
            : base(game)
        {
            if (game.Services.GetService(typeof(IPlayingState)) == null)
                game.Services.AddService(typeof(IPlayingState), this);
   
        }

        protected override void LoadContent()
        {
            GeneralMusic = Content.Load<Song>("Sound/Music/General_Music_1");
            MediaPlayer.Volume = 0.09f;
            if(OurGame.EnableMusic==true)
            MediaPlayer.Play(GeneralMusic);
            hearts = Content.Load<Texture2D>("misc/heart");
            hall = Content.Load<SoundEffect>("Sound/FX/Level1/Hallway_Atmosphere");
            hallInstance = hall.CreateInstance();
            hallInstance.IsLooped = true;
            hallInstance.Play();
            hallInstance.Volume = 0.05f;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cam = new Camera2D(GraphicsDevice);
            world = new World(new Vector2(0, 9.8f));
            map = new Map(world);
            closingWall = new Rectangle((int)cam.Position.X,0,200,600);
            //soundManager.Play("General Music 1");
            font = Content.Load<SpriteFont>("Fonts/Font");
            Tile.Content = Content;
            Map.Content = Content;
            
            map.Generate(new int[,]
               {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,0,0,0,0,0,0,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,6,6,6,0,0,0,0,6,6,0,0,0,0,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0,0,0,0,0,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,6,6,6,0,0,0,0,6,6,6,6,6,0,0,0,0,0,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,5,5,5,5,5,0,0,0,0,0,0,0,0,0,0,6,6,6,6,6,0,0,0,0,0,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0,2,2,2,0,0,0,0,0,0,2,2,0,0,0,0,5,5,5,5,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,8,0,0,0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            }, 64, font);





            backGround1 = Content.Load<Texture2D>("Layers/level/tmpBack1");
            backGround2 = Content.Load<Texture2D>("Layers/level/tmpBack2");
            backGround3 = Content.Load<Texture2D>("Layers/level/tmpBack3");
            backGround4 = Content.Load<Texture2D>("Layers/level/tmpBack4");
            backGround5 = Content.Load<Texture2D>("Layers/level/tmpBack5");

            backgroundREC1 = new Rectangle(0, -700, 5693, 1969);
            backgroundREC2 = new Rectangle(4097, -700, 5693, 1969);
            backgroundREC3 = new Rectangle(8193, -700, 5693, 1969);
            backgroundREC4 = new Rectangle(12990, -700, 5693, 1969);
            backgroundREC5 = new Rectangle(16387, -700, 5693, 1969);

            wall = Content.Load<Texture2D>("Layers/level/closingWall");

            playerCrouch = Content.Load<Texture2D>("Player/Anim/Ron_Crouch");
            playerCrouchWalk = Content.Load<Texture2D>("Player/Anim/Ron_Crouch_Walk");
            playerStand = Content.Load<Texture2D>("Player/Anim/Ron_Stand");
            playerWalk = Content.Load<Texture2D>("Player/Anim/Ron_Walk");

            SpriteEffects flip = SpriteEffects.FlipHorizontally;

            player = new Kid(hearts,world,
                Content.Load<Texture2D>("Player/Anim/Ron_Stand"),
                Content.Load<Texture2D>("Player/Anim/Ron_Stand"),
                Content.Load<Texture2D>("Player/bullet"),
                new Vector2(60, 60),
                100,
                cam.ScreenToWorld(new Vector2(650, 440)), false, font);



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

            


            Rectangle anim3 = new Rectangle(-110, -65, 240, 160);
            Rectangle anim4 = new Rectangle(0, -50, 140, 130);


            player.Animations[0] = new AnimatedSprite(playerCrouch, 1, 1, anim3, 0f);
            player.Animations[1] = new AnimatedSprite(playerCrouchWalk, 2 , 16 , anim3, 0.03f);

            player.Animations[2] = new AnimatedSprite(playerStand, 4,13, anim3,0.017f);

            player.Animations[3] = new AnimatedSprite(playerWalk, 2, 12, anim3, 0.03f);

            jumpSetAnim[0] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_01_start");
            jumpSetAnim[1] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_02_up");
            jumpSetAnim[2] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_03_mid");
            jumpSetAnim[3] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_04_down");
            jumpSetAnim[4] = Content.Load<Texture2D>("Player/Anim/Jump/Ron_Jump_05_end");

            player.Animations[4] = new AnimatedSprite(jumpSetAnim[0], 1, 1, anim3, 0f);
            player.Animations[5] = new AnimatedSprite(jumpSetAnim[1], 1, 1, anim3, 0f);
            player.Animations[6] = new AnimatedSprite(jumpSetAnim[2], 1, 3, anim3, 0.6f);
            player.Animations[7] = new AnimatedSprite(jumpSetAnim[3], 1, 1, anim3, 0f);
            player.Animations[8] = new AnimatedSprite(jumpSetAnim[4], 1, 1, anim3, 0f);

            slideSetAnim[0] = Content.Load<Texture2D>("Player/Anim/Slide/Ron_Slide_01_start");
            slideSetAnim[1] = Content.Load<Texture2D>("Player/Anim/Slide/Ron_Slide_02_slide");
            slideSetAnim[2] = Content.Load<Texture2D>("Player/Anim/Slide/Ron_Slide_03_end");

            player.Animations[9] = new AnimatedSprite(slideSetAnim[0], 1, 4, anim3, 0.4f);
            player.Animations[10] = new AnimatedSprite(slideSetAnim[1], 1, 6, anim3, 0.3f);
            player.Animations[11] = new AnimatedSprite(slideSetAnim[2], 1, 4, anim3, 0.4f);


            map.setPlayerToMap(player);



            _gameComponents = new List<Component>()
            {
                
                
                player,

            };
        }

        public override void Update(GameTime gameTime)
        {
            if (closingWall.X + closingWall.Width +20 > player.Position.X)
                player.IsAlive = false;

            handleInput(gameTime);
            currentMouseState = Mouse.GetState();
            walkingInstance.Volume = 0.1f;

            //closingWall.X+=4;

            if ((currentMouseState.LeftButton == ButtonState.Pressed) && (currentMouseState.RightButton == ButtonState.Pressed ))
            {
                Console.WriteLine("kinesis!!!!!");
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

                //walkingInstance.Pitch = 0.0f;
                if (player.IsMoving && !player.IsJumping && !player.IsBending && !player.IsSliding&&OurGame.EnableSoundFx==true)
                {
                    walkingInstance.Play();
                }

                else
                {
                    walkingInstance.Pause();
                }
            
        

            if (player.IsJumping && !isJumpSoundPlayed&&OurGame.EnableSoundFx==true)
            {
                isJumpSoundPlayed = true;
                jumpingUpInstance.Play();
            }

            if (!player.IsJumping)
                isJumpSoundPlayed = false;

            if (player.HasLanded && player.PlayLandingSound&&OurGame.EnableSoundFx==true)
            {
                jumpingDownInstance.Play();
                player.PlayLandingSound = false;
            }

            if (!player.IsMoving&&!player.IsJumping&&OurGame.EnableSoundFx==true)
                idleInstance.Play();
            else
                idleInstance.Stop();

            if (player.IsBending&&OurGame.EnableSoundFx==true)
                crouchingInstance.Play();
            else
                crouchingInstance.Stop();


            if (player.IsSliding&&OurGame.EnableSoundFx==true)
                slidingInstance.Play();

            camLocation = new Vector2(player.Position.X, player.Position.Y - 100);

            cam.LookAt(camLocation);
            //cam.Rotate(0.0005f);
            //cam.ZoomOut(0.0001f);
            map.Update(gameTime);
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);


            base.Update(gameTime);
        }

        private void handleInput(GameTime gameTime)
        {
            if (!player.IsAlive)
            {
                StateManager.PopState();
                OurGame.Reset();
                StateManager.PushState(OurGame.GameOverState.Value);
                player.IsAlive = true;
            }
            if (Input.KeyboardHandler.WasKeyPressed(Keys.Escape))
            {
                StateManager.PushState(OurGame.EscapeState.Value);
                
            }
            if (Input.KeyboardHandler.WasKeyPressed(Keys.P))
            {
                StateManager.PushState(OurGame.PausedState.Value);
                hallInstance.Stop();
                MediaPlayer.Pause();
            }
            if (!(StateManager.State == OurGame.PausedState.Value))
            {
                hallInstance.Play();
                MediaPlayer.Resume();
            }

                previousMouseState = currentMouseState;

            if (OurGame.EnableMusic == false)
                MediaPlayer.Pause();
            else MediaPlayer.Resume();
            
        }

        public override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(_backgroundColor);
            spriteBatch.Begin(transformMatrix: cam.GetViewMatrix());

            //map.DrawEnemies(gameTime, spriteBatch);

            spriteBatch.DrawString(font, cam.Position.X + "/" + cam.Position.Y, new Vector2(cam.Position.X, cam.Position.Y), Color.White);


            spriteBatch.Draw(backGround1, backgroundREC1, null, Color.White);
            spriteBatch.Draw(backGround2, backgroundREC2, null, Color.White);
            spriteBatch.Draw(backGround3, backgroundREC3, null, Color.White);
            spriteBatch.Draw(backGround4, backgroundREC4, null, Color.White);
            spriteBatch.Draw(backGround5, backgroundREC5, null, Color.White);

            spriteBatch.Draw(wall, closingWall, null, Color.White);

            map.DrawObstacle(gameTime, spriteBatch);

            map.DrawEnemies(gameTime, spriteBatch);


            spriteBatch.DrawString(font, "Mouse Position" + cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y)), new Vector2(GraphicsDevice.Viewport.Width / 2.0f - 120f, -GraphicsDevice.Viewport.Height + 900), Color.White);
            spriteBatch.DrawString(font, "Mouse Position" + cam.ScreenToWorld(new Vector2(currentMouseState.Position.X, currentMouseState.Position.Y)), new Vector2(cam.Position.X + cam.BoundingRectangle.Width / 2, -cam.Position.Y + cam.BoundingRectangle.Height / 4), Color.White);

            foreach (var component in _gameComponents)
            {
                component.Draw(gameTime, spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }


    }
}