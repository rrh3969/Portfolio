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
	internal class Button : UIController
	{
		// ----- Fields -----
		MouseState mouseState;
		MouseState prevMouse;

		// ----- Properties -----


		// ----- Constructor -----
		public Button(Texture2D texture, Rectangle position)
			: base(texture, position)
		{
			// Unused as of now	
		}

        // ----- Methods -----

		/// <summary>
		/// Checks if the cursor is within the bounds of a button, and if it was pressed
		/// </summary>
		/// <param name="mousePos"></param>
		/// <returns></returns>
        public virtual bool ButtonPressed(MouseState prevMouse)
        {
			// Temp vector to record current postion of the cursor as an single object
            Vector2 mousePoint = new Vector2(mouseState.X, mouseState.Y);
			// return the bool of the position and click check
			return position.Contains(mousePoint)
				&& mouseState.LeftButton != ButtonState.Pressed
                && prevMouse.LeftButton == ButtonState.Pressed;
        }

        public override void Update(GameTime gameTime)
        {
            mouseState = Mouse.GetState();

            // Event goes here
            // ButtonPressed(prevMouse) used as a param

            // Contains tester code
            Vector2 mousePoint = new Vector2(mouseState.X, mouseState.Y);
			// TODO: Update this to no be hardcoded, may be able to use LERP
			if (position.Contains(mousePoint))
			{
				position.Width = 143;
				position.Height = 72;
			}
			else
			{
				position.Width = 128;
				position.Height = 64;
			}

            prevMouse = mouseState;          
        }
    }
}

