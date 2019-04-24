﻿using Microsoft.Xna.Framework;
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

        private int[,] grid;
        //private List<CollisionTiles> collisionTiles = new List<CollisionTiles>();
        private List<Tile> collisionTiles = new List<Tile>();
        private List<Obstacle> obstacleTiles = new List<Obstacle>();
        private List<Enemy> enemies = new List<Enemy>();

        private Kid player;
        private int enemies_counter = 2;
        private int width, height;
        private World world;
        public Texture2D texture;
        private static ContentManager content;
        Random r;
        private bool finished_tutorial = false;
        

        /// delete font
        private SpriteFont font;
        /// 
     


        public Map(World world,Kid player)
        {
            this.world = world;
            this.player = player;
        }

        public List<Tile> CollisionTiles { get => collisionTiles; }

        public int Width { get => width; }
        public int Height { get => height; }
        public static ContentManager Content { protected get => content; set => content = value; }
        public List<Obstacle> ObstacleTiles { get => obstacleTiles; set => obstacleTiles = value; }
        public List<Enemy> Enemies { get => enemies; set => enemies = value; }
        public Dictionary<int, bool> passableDict =
                    new Dictionary<int, bool>();
        public bool Finished_tutorial { get => finished_tutorial; set => finished_tutorial = value; }

        //public List<CollisionTiles> CollisionTiles { get => collisionTiles; }
        public int Enemies_counter { get => enemies_counter; set => enemies_counter = value; }

        public void Generate(int[,] map, int size, SpriteFont font)
        {
            /////delete font//////
            this.font = font;

            int number = 0;
            passableDict.Add(0, true);
            passableDict.Add(8, true);

            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    number = map[y, x];
                    texture = Content.Load<Texture2D>("Tiles/HUD");

                    //if (number > 0 && number <= 2)
                    //{
                    //obs.Position = new Vector2(x * size, y * size);
                    /*
                    if (number == 2)
                        obs.Body.CollisionCategories = Category.Cat30;
                        */
                    //ObstacleTiles.Add(obs);

                    //collisionTiles.Add(new CollisionTiles(number, new Rectangle(x * size, y * size, size, size),world,new Vector2(64,64),font));
                    //collisionTiles.Add(new Tile(number, new Rectangle(x * size, y * size, size, size), world, new Vector2(64, 64)));
                    //    }


                    if (number == 1)//ground
                    {
                        Ground ground = new Ground(world, texture, new Vector2(64, 64), font,false);
                        ground.Position = new Vector2(x * size+(ground.Size.X / 2), y * size);
                        ObstacleTiles.Add(ground);
                        if (!passableDict.ContainsKey(1)) 
                        passableDict.Add(1, ground.Passable);
                    }
                    else if (number == 2)//locker
                    {
                        Locker locker = new Locker(world, texture, new Vector2(64, 64), font,false);
                        locker.Position = new Vector2(x * size+(locker.Size.X/2), y * size);
                        ObstacleTiles.Add(locker);
                        if (!passableDict.ContainsKey(2))         
                            passableDict.Add(2, locker.Passable);


                    }
                    else if (number == 3)//door
                    {
                        Door door = new Door(world, texture, new Vector2(64, 64), font,false);
                        door.Position = new Vector2(x * size + (door.Size.X / 2), y * size);

                        /*
                        if (number == 2)
                            obs.Body.CollisionCategories = Category.Cat30;
                            */
                        ObstacleTiles.Add(door);
                        if (!passableDict.ContainsKey(3))
                            passableDict.Add(3, door.Passable);

                    }
                    else if (number == 4)//bag
                    {
                        Bag bag = new Bag(world, texture, new Vector2(64, 64), font,false);
                        bag.Position = new Vector2(x * size + (bag.Size.X / 2), y * size);
                        ObstacleTiles.Add(bag);
                        if (!passableDict.ContainsKey(4))
                            passableDict.Add(4, bag.Passable);

                    }
                    else if (number == 5)//table
                    {
                        Table table = new Table(world, texture, new Vector2(64, 64), font,false);
                        table.Position = new Vector2(x * size + (table.Size.X / 2), y * size);
                        ObstacleTiles.Add(table);
                        if (!passableDict.ContainsKey(5))
                            passableDict.Add(5, table.Passable);


                    }
                    else if (number == 6)//chair
                    {
                        Chair chair = new Chair(world, texture, new Vector2(64, 64), font,false);
                        chair.Position = new Vector2(x * size + (chair.Size.X / 2), y * size);
                        ObstacleTiles.Add(chair);
                        if (!passableDict.ContainsKey(6))
                            passableDict.Add(6, chair.Passable);


                    }
                    else if (number == 7)//platfomr which is not floor
                    {
                        Obstacle obs = new Obstacle(world, texture, new Vector2(64, 64), font,false);
                        obs.Position = new Vector2(x * size + (obs.Size.X / 2), y * size);
                        ObstacleTiles.Add(obs);
                        obs.KinesisOn = false;
                        obs.Body.CollisionCategories = Category.Cat7;//////needs to be deleted 
                        if (!passableDict.ContainsKey(7))
                            passableDict.Add(7, obs.Passable);


                    }
                    else if (number == 8)//enemy
                    {
                        Console.WriteLine("x: " + x + " y: " + y);
                        Point startLocationGrid = new Point(y, x);
                        r = new Random();
                        int rInt = r.Next(192, 320);
                        Enemy en = new Enemy(world,
                        new Vector2(96, 96),
                        100,
                        new Vector2(y * 64, x * 64 - 600),startLocationGrid,10, font, rInt, this,player);
                        Point enemeyGridLocation = new Point(scaleToGrid(en.Position.X / 64.0f), (int)en.Position.Y / 64);
                        en.GridLocation = enemeyGridLocation;                       
                        Enemies.Add(en);
                    }
                    else if (number == 9)
                    {
                        Obstacle obs2 = new Obstacle(world, texture, new Vector2(64, 64), font,false);
                        obs2.Position = new Vector2(x * size + (obs2.Size.X / 2), y * size);
                        ObstacleTiles.Add(obs2);
                        obs2.KinesisOn = true;
                        if (!passableDict.ContainsKey(9))
                            passableDict.Add(9, obs2.Passable);

                    }
                    

                    width = (x + 1) * size;
                    height = (y + 1) * size;

                }
            }
            grid = map;

        }

        public void Update(GameTime gameTime)
        {
            
            
            player.GridLocation = new Point(scaleToGrid(player.Position.X / 64), (int)player.Position.Y / 64);

            foreach (Enemy en in Enemies)
            {

                en.Update(gameTime, player.Position, player.IsAlive);
                Point enemeyGridLocation = new Point(scaleToGrid(en.Position.X / 64), scaleToGrid(en.Position.Y / 64));
                en.GridLocation = enemeyGridLocation;
                if (en.Health == 0)
                {
                    enemies_counter--;
                }
            }
            Enemies.RemoveAll(Enemy => Enemy.Health == 0);

            foreach (Obstacle ob in obstacleTiles)
            {
                Point obstacleGridLocation = new Point((int)ob.Position.X / 64, (int)ob.Position.Y / 64);
                /*
                if (ob.Position.X / 64 > (int)ob.Position.X / 64)
                {
                    obstacleGridLocation.X++;
                }
                if (ob.Position.Y / 64 > (int)ob.Position.Y / 64)
                {
                    obstacleGridLocation.Y++;

                }
                */
                ob.GridLocation = obstacleGridLocation;

            }

        }

        public void DrawGrid(GameTime gameTime, int[,] gameMap, SpriteBatch spriteBatch, SpriteFont f)
        {
            for (int x = 0; x < gameMap.GetLength(1); x++)
            {
                for (int y = 0; y < gameMap.GetLength(0); y++)
                {
                    spriteBatch.DrawString(f, x + " / " + y, new Vector2(x * 64, y * 64), Color.White);
                }

            }
        }



        public void DrawObstacle(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Obstacle ob in ObstacleTiles)
                ob.Draw(gameTime, spriteBatch);
             
        }

        public void DrawEnemies(GameTime gameTime, SpriteBatch spriteBatch)
        {

            foreach (Enemy en in Enemies)
                en.Draw(gameTime, spriteBatch,font);
        }

        public int getGridObject(int x,int y)
        {
            if (x > 0 && y > 0)
                return grid[x, y];
            else
                return 0;
        }

        public bool isPassable(int x, int y)
        {
            if (passableDict.TryGetValue(grid[x, y], out bool check))
            {
                if (check)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;

            }

        }

        private int scaleToGrid(float x)
        {
            float tmp = x;
            int scale = (int)tmp;
            if (tmp > (float)scale)
                return scale;
            else
                return --scale;
            
        }


    }
}