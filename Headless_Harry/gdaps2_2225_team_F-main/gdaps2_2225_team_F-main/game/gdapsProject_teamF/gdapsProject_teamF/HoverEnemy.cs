using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gdapsProject_teamF
{
    internal class HoverEnemy:EnemyClass
    {
        SpriteEffects direction;
        
        public HoverEnemy(Texture2D asset, Rectangle position, ObjectLayers layer) : base(asset, position, layer)
        {
 
        }

        public override void Attack()
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(asset, new Vector2(position.X, position.Y), new Rectangle(0, 0, 400, 400), Color.White, 0, Vector2.Zero, .08f, direction, 0);
        }
    }
}
