using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Reflection.Metadata;

namespace gdapsProject_teamF
{
    internal class LevelManager 
    {
        //platform test texture
        private Texture2D platform;
        //wall test texture
        private Texture2D wall;

        private Texture2D playerTexture;

        //enemy temp textures
        private Texture2D enemyTexture;
        private Texture2D forkTexture;
        private Texture2D hoverTexture;

        List<SpatialPartition<GameObject>> spatialPartitions = new List<SpatialPartition<GameObject>>();
        SpatialPartition<GameObject> currentPartition;
        LevelLoader levelLoader = new LevelLoader(35, 45);
        List<List<GameObject>> gameObjects = new List<List<GameObject>>();

        int currentLevel;
        int screenHeight;

        PlayerController playerController;

        //win property
        public bool hasWon
        {
            get;
            set;
        }


        public LevelManager(int screenWidth, int screenHeight, ContentManager content, SpriteBatch _spriteBatch, PlayerController playerController)
        {
            this.screenHeight = screenHeight;
            hasWon = false;
            wall = content.Load<Texture2D>("walltile");
            platform = content.Load<Texture2D>("platformtile");
            playerTexture = content.Load<Texture2D>("playeridle");
            enemyTexture = content.Load<Texture2D>("BaseEnemy");
            forkTexture = content.Load<Texture2D>("forkenemy");
            hoverTexture = content.Load<Texture2D>("hoverenemy");

            //the second and third enemy textures are textures for the other two enemy types
            gameObjects.Add(levelLoader.Load("level11.txt", platform, wall, playerTexture, enemyTexture, forkTexture, hoverTexture, _spriteBatch));
            gameObjects.Add(levelLoader.Load("level12.txt", platform, wall, playerTexture, enemyTexture, forkTexture, hoverTexture, _spriteBatch));
            gameObjects.Add(levelLoader.Load("level13.txt", platform, wall, playerTexture, enemyTexture, forkTexture, hoverTexture, _spriteBatch));

            //Adds the levels to the spacial partitions
            for (int i = 0; i < gameObjects.Count; i++)
            {
                SpatialPartition<GameObject> partition = new SpatialPartition<GameObject>(new Rectangle(0, 0, screenWidth, screenHeight), 0);
                foreach (GameObject gameObject in gameObjects[i])
                {
                    partition.Insert(gameObject, gameObject.Position);
                }
                spatialPartitions.Add(partition);
            }
            currentPartition = spatialPartitions[0];
            this.playerController = playerController;
        }

        public SpatialPartition<GameObject> CurrentPartition
        {
            get { return currentPartition; }
        }

        public void DrawLevel(SpriteBatch _spriteBatch)
        {
            foreach (GameObject gameObject in gameObjects[currentLevel])
            {
                gameObject.Draw(_spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (playerController.Position.Bottom < 0)
            {
                UpALevel();
            }
            else if (playerController.Position.Top > screenHeight)
            {
                DownALevel();
            }
            foreach (GameObject gameObject in gameObjects[currentLevel])
            {
                if(gameObject is BaseEnemy)
                {
                    BaseEnemy enemy = (BaseEnemy)gameObject;
                    enemy.Move(gameTime);
                }
                else if (gameObject is ForkEnemy)
                {
                    ForkEnemy enemy = (ForkEnemy)gameObject;
                    enemy.Move(gameTime);
                }
            }
        }

        void UpALevel()
        {
            if (currentLevel < gameObjects.Count - 1)
            {
                currentLevel++;
                currentPartition = spatialPartitions[currentLevel];
                playerController.NextLevel(screenHeight);
            }
            else
            {
                hasWon = true;
            }
        }
        void DownALevel()
        {
            if (currentLevel >= 1)
            {
                currentLevel--;
                currentPartition = spatialPartitions[currentLevel];
                playerController.PreviousLevel();
            }
        }

        public void ResetGame()
        {
            currentLevel = 0;
            currentPartition = spatialPartitions[0];
        }

    }
}
