using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gdapsProject_teamF
{
    /// <summary>
    /// Mostly Empty Class for static colliders that will be generated from a file
    /// </summary>
    internal class StaticColliders : GameObject
    {
        public StaticColliders(Texture2D asset, Rectangle position, ObjectLayers layer) : base(asset, position, layer)
        {

        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
        public override void Update(GameTime gameTime)
        {

        }
    }
}
