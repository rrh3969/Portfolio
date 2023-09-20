using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gdapsProject_teamF
{
    internal class LevelLoader
    {
        //fields
        //level data
        string[,] level;
        StreamReader reader = null;
        //width and height of level data
        int width;
        int height;
        //scale to multiply by
        int scale = 20;

        //constructor
        public LevelLoader(int width, int height)
        {
            this.width = width;
            this.height = height;
            level = new string[width, height];
        }

        //read method
        private void Read(string filename)
        {
            reader = new StreamReader("../../../" + filename);
            try
            {
                //fill array with level data
                for (int h = 0; h < height; h++)
                {
                    string line = reader.ReadLine();
                    for (int w = 0; w < width; w++)
                    {
                        level[w, h] = line[w].ToString();
                    }
                }
            }
            catch 
            {
                
            }
            if (reader != null)
            {
                reader.Close();
            }
        }

        //load method
        public List<GameObject> Load(string filename, Texture2D platformtexture,Texture2D walltexture, Texture2D playertexture, Texture2D enemytexture, Texture2D forktexture, Texture2D hovertexture, SpriteBatch spriteBatch)
        {
            //list of objects
            List<GameObject> gameObjects = new List<GameObject>();
            //Dictionary of walls
            Dictionary<int, Dictionary<int, int>> walls = new Dictionary<int, Dictionary<int, int>>();
            //count of individual platforms
            int platforms = 0;
            //int to store coordinate of platform
            int widthCoord = 0;
            //read and fill array
            Read(filename);
            //use level date to draw level
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    switch (level[w, h])
                    {
                        case "X":
                            //add platform to count
                            platforms++;
                            widthCoord = w;
                            break;
                        case "P":
                            //add player
                            //gameObjects.Add(new PlayerController(playertexture, new Rectangle(w * scale, h * scale, scale, scale)));
                            break;
                        case "E":
                            //add enemy
                            gameObjects.Add(new BaseEnemy(enemytexture, new Rectangle(w * scale, h * scale, scale, scale), ObjectLayers.Enemy));
                            break;
                        case "F":
                            //add enemy
                            gameObjects.Add(new ForkEnemy(forktexture, new Rectangle(w * scale, h * scale, scale, scale), ObjectLayers.Enemy));
                            break;
                        case "H":
                            //add enemy
                            gameObjects.Add(new HoverEnemy(hovertexture, new Rectangle(w * scale, h * scale, scale, scale), ObjectLayers.Enemy));
                            break;
                        case "W":
                            //add wall to dictionary
                            //gameObjects.Add(new StaticColliders(platformtexture, new Rectangle(w * scale, h * scale, scale, scale), ObjectLayers.Wall));
                            if(walls.ContainsKey(w))
                            {
                                if (walls[w].ContainsKey(h - walls[w].Count))
                                {
                                    if (walls[w].Count == walls[w][h - walls[w].Count])
                                    {
                                        walls[w][h - walls[w].Count]++;
                                    }
                                    else
                                    {
                                        walls[w].Add(h, 1);
                                    }
                                }
                                else
                                {
                                    walls[w].Add(h, 1);
                                }
                            }
                            else
                            {
                                walls.Add(w, new Dictionary<int, int>());
                                walls[w].Add(h, 1);
                            }
                            break;
                        default:
                            //add long platform
                            if (platforms != 0)
                            {
                                gameObjects.Add(new StaticColliders(platformtexture, new Rectangle(((widthCoord- platforms) * scale) + scale, h * scale, scale * platforms, scale), ObjectLayers.Ground));
                                platforms = 0;
                            }
                            break;
                    }
                }
                //add long platform
                if (platforms != 0)
                {
                    gameObjects.Add(new StaticColliders(platformtexture, new Rectangle(((widthCoord-platforms) * scale) + scale, h * scale, scale * platforms, scale), ObjectLayers.Ground));
                    platforms = 0;
                }
            }
            //add walls
            for(int i = 0; i < width; i++)
            {
                if (walls.ContainsKey(i))
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (walls[i].ContainsKey(j))
                        {
                            gameObjects.Add(new StaticColliders(walltexture, new Rectangle(i * scale, j * scale, scale, walls[i][j] * scale), ObjectLayers.Wall));
                        }
                    }
                }
            }
            //return list
            return gameObjects;
        }
    }
}
