using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace gdapsProject_teamF
{
    public enum EnemyStateP
    {
        Standing,
        Walking,
        Attacking,
        Dead,
    }
    public abstract class EnemyClass : GameObject
    {
        protected bool attacking;
        protected bool dead;


        public EnemyClass(Texture2D asset, Rectangle position, ObjectLayers layer) : base(asset, position, layer)
        {
            layer = ObjectLayers.Enemy;
        }

        public abstract void Attack();

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public virtual void Move(GameTime gameTime)
        {

        }
        public virtual void Die()
        {

        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}
