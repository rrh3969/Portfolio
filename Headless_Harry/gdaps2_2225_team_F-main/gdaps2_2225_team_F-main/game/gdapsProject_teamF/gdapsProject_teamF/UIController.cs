using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gdapsProject_teamF
{
	internal abstract class UIController
	{
		// ----- Fields -----
		protected Texture2D texture;
		protected Rectangle position;

		// ----- Properties -----
		public Rectangle Position { get { return position; } }

		// ----- Constructors -----
		protected UIController(Texture2D texture, Rectangle position)
		{
			this.texture = texture;
			this.position = position;
		}

		// ----- Methods -----

		/// <summary>
		/// Blueprint for drawing the sprite
		/// </summary>
		/// <param name="sprite"></param>
		public virtual void Draw(SpriteBatch sprite)
		{
			sprite.Draw(
				texture,
				position,
				Color.White);
		}

		/// <summary>
		/// Abstract update to be utilized by child classes
		/// </summary>
		/// <param name="gameTime"></param>
		public abstract void Update(GameTime gameTime);
	}
}

