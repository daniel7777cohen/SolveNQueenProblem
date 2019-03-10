using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;

namespace RemGame
{
    class Map
    {
        // private List<CollisionTiles> collisionTiles = new List<CollisionTiles>();
        private List<Tile> collisionTiles = new List<Tile>();
        private List<Obstacle> obstacleTiles = new List<Obstacle>();
        private List<Enemy> enemies = new List<Enemy>();
        private Kid player;

        private int width, height;
        private World world;
        public Texture2D texture;
        private static ContentManager content;
        Random r;
        private bool finished_tutorial = false;
        public bool Finished_tutorial { get => finished_tutorial; set => finished_tutorial = value; }

        //public List<CollisionTiles> CollisionTiles { get => collisionTiles; }



        public Map(World world)
        {
            this.world = world;

        }

        public List<Tile> CollisionTiles { get => collisionTiles; }

        public int Width { get => width; }
        public int Height { get => height; }
        public static ContentManager Content { protected get => content; set => content = value; }
        public List<Obstacle> ObstacleTiles { get => obstacleTiles; set => obstacleTiles = value; }
        internal List<Enemy> Enemies { get => enemies; set => enemies = value; }

        public void setPlayerToMap(Kid player)
        {
            this.player = player;
        }

        public void Generate(int[,] map, int size, SpriteFont font)
        {
            int number = 0;
            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    number = map[y, x];

                    if (number > 0 && number <= 2)
                    {
                        texture = Content.Load<Texture2D>("Tiles/HUD");
                        Obstacle obs = new Obstacle(world, texture, new Vector2(64, 64), font);
                        obs.Position = new Vector2(x * size, y * size);
                        /*
                        if (number == 2)
                            obs.Body.CollisionCategories = Category.Cat30;
                            */
                        ObstacleTiles.Add(obs);

                        //collisionTiles.Add(new CollisionTiles(number, new Rectangle(x * size, y * size, size, size),world,new Vector2(64,64),font));
                        //collisionTiles.Add(new Tile(number, new Rectangle(x * size, y * size, size, size), world, new Vector2(64, 64)));
                    }
                    else if (number == 3)
                    {
                        r = new Random();
                        int rInt = r.Next(192, 320);
                        Enemy en = new Enemy(world,
                        Content.Load<Texture2D>("Player/playerLeft"),
                        Content.Load<Texture2D>("Player/playerRight"),
                        Content.Load<Texture2D>("Player/bullet"),
                        new Vector2(96, 96),
                        100,
                        new Vector2(x * size, y * size), false, font, rInt);
                        en.Animations[0] = new AnimatedSprite(Content.Load<Texture2D>("Player/playerLeft"), 1, 4,new Rectangle(0,0,0,0),0.0f);
                        en.Animations[1] = new AnimatedSprite(Content.Load<Texture2D>("Player/playerRight"), 1, 4, new Rectangle(0, 0, 0, 0), 0.0f);
                        enemies.Add(en);
                    }
                    else if (number == 4)
                    {
                        texture = Content.Load<Texture2D>("Tiles/HUD");
                        //Door door = new Door(world, texture, new Vector2(64, 64), font);
                        //door.Position = new Vector2(x * size, y * size);
                        /*
                        if (number == 2)
                            obs.Body.CollisionCategories = Category.Cat30;
                            */
                       // ObstacleTiles.Add(door);
                    }
                    width = (x + 1) * size;
                    height = (y + 1) * size;



                }
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Enemy en in enemies)
                en.Update(gameTime, player.Position, player.IsAlive);

        }

        public void DrawObstacle(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // foreach (CollisionTiles tile in collisionTiles)
            foreach (Obstacle ob in ObstacleTiles)
                ob.Draw(gameTime, spriteBatch);


        }

        public void DrawEnemies(GameTime gameTime, SpriteBatch spriteBatch)
        {


            foreach (Enemy en in enemies)
                en.Draw(gameTime, spriteBatch);
        }



    }
}