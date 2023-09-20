using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace gdapsProject_teamF
{
    public enum ObjectLayers
    {
        Default,
        Ground,
        Wall,
        Enemy,
    }
    public abstract class GameObject
    {
        //fields to be assigned in constructor
        protected Texture2D asset;
        protected Rectangle position;
        protected ObjectLayers layer;

        /// <summary>
        /// Protected constructor that assigns fields
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="position"></param>
        protected GameObject(Texture2D asset, Rectangle position)
        {
            this.asset = asset;
            this.position = position;
            layer = ObjectLayers.Default;
        }

        /// <summary>
        /// Overloaded constructor to allow for specific layers if the object needs it
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="position"></param>
        /// <param name="layer"></param>
        protected GameObject(Texture2D asset, Rectangle position, ObjectLayers layer)
        {
            this.asset = asset;
            this.position = position;
            this.layer = layer;
        }

        /// <summary>
        /// Get only property for the position of the rectangle
        /// </summary>
        public Rectangle Position
        {
            get { return position; }
        }

        /// <summary>
        /// Public property for getting the GameObjects layer
        /// This will be helpful in determining the type of object we collide with
        /// </summary>
        public ObjectLayers Layer
        {
            get { return layer; }
        }

        /// <summary>
        /// Virtual void for drawing the given asset
        /// </summary>
        /// <param name="sb"></param>
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(asset, position, Color.White);
        }

        /// <summary>
        /// Abstract update to be later used
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);
    }
}
