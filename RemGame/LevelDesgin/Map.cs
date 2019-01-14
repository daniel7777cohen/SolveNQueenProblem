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
        private int width, height;
        private World world;
        public Texture2D texture;
        private static ContentManager content;

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

        public void Generate(int[,] map, int size, SpriteFont font)
        {
            int number = 0;
            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    number = map[y, x];

                    if (number > 0)
                    {
                        texture = Content.Load<Texture2D>("HUD");
                        Obstacle obs = new Obstacle(world, texture, new Vector2(64, 64), font);
                        obs.Position = new Vector2(x * size, y * size);
                        ObstacleTiles.Add(obs);
                        
                        //collisionTiles.Add(new CollisionTiles(number, new Rectangle(x * size, y * size, size, size),world,new Vector2(64,64),font));
                        //collisionTiles.Add(new Tile(number, new Rectangle(x * size, y * size, size, size), world, new Vector2(64, 64)));
                    }
                    width = (x + 1) * size;
                    height = (y + 1) * size;

                }
            }
        }

        public void Draw(GameTime gameTime,SpriteBatch spriteBatch)
        {
           // foreach (CollisionTiles tile in collisionTiles)
                foreach (Obstacle ob in ObstacleTiles)
                    ob.Draw(gameTime,spriteBatch);
        }

    }
}
