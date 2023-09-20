using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gdapsProject_teamF
{
    internal class BaseEnemy : EnemyClass
    {
        int maxDist = 30;
        int moveUnit = 1;
        int distance = 0;
        SpriteEffects direction;

        bool isResting = true;
        double restTime;
        int restTimeMultiplier = 3;


        Random random;

        public BaseEnemy(Texture2D asset, Rectangle position, ObjectLayers layer) : base(asset, position, layer)
        {
            random = new Random();
            restTime = random.NextDouble() * restTimeMultiplier;
        }

        /// <summary>
        /// Moves the enemy
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Move(GameTime gameTime)
        {


            if (!isResting)
            {
                position.X += moveUnit;
                distance += moveUnit;

                if (distance >= maxDist && maxDist > 0)
                {
                    distance = 0;
                    maxDist = -maxDist;
                    moveUnit = -moveUnit;
                    isResting = true;
                    restTime = random.NextDouble() * restTimeMultiplier;
                    direction = SpriteEffects.FlipHorizontally;
                }
                else if (distance <= maxDist && maxDist < 0)
                {
                    distance = 0;
                    maxDist = -maxDist;
                    moveUnit = -moveUnit;
                    isResting = true;
                    restTime = random.NextDouble() * restTimeMultiplier;
                    direction = SpriteEffects.None;
                }
            }
            else
            {
                restTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (restTime <= 0)
                {
                    isResting = false;
                }
            }
        }

        public override void Attack()
        {
            throw new NotImplementedException();
        }

        public void Bounce()
        {

        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(asset, new Vector2(position.X, position.Y), new Rectangle(0, 75, 400, 400), Color.White, 0, Vector2.Zero, .08f, direction, 0);
        }

    }
}
