using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace gdapsProject_teamF
{
    internal class Info
    {
        //fields
        Texture2D texture;
        Rectangle box;
        bool isOn;
        SpriteFont font;
        float coyoteTimer;
        float maxTime = 5.00F;
        private delegate void currentInput(GameTime game);
        
        public Info(Texture2D texture, Rectangle box, bool isOn,SpriteFont font)
        {
            this.texture = texture;
            this.box = box;
            this.isOn = isOn;
        }


        public enum infoState
        {
            Dash,
            Slide,
            GroundPound,
            Start,
            Jump
        }
        infoState current = new infoState();
        //properties 

        public Texture2D Texture
        {
            get { return texture; }

            set
            {
                texture = value;
            }
        }

        public SpriteFont Font
        {
            get { return font; }

            set
            {
                font = value;
            }
        }
        public Rectangle Box
        {
            get { return box; }

            set
            {
                box = value;
            }
        }

        public bool IsOn
        {
            get { return isOn; }

            set
            {
                isOn = value;
            }
        }

        public void InputUpdate(GameTime gameTime)
        {
            Info Directions = new Info(this.Texture,this.Box,this.isOn,this.font);
            switch(current)
            {
                case infoState.Start:
                    break;
                case infoState.Dash:
                    break;
                case infoState.Slide:
                    break;
                case infoState.GroundPound:
                    break;
                case infoState.Jump:
                     break;
            }
        }

        public void Placement(int x,int y)
        {
           
        }

        public void TimeStart(GameTime gameTime)
        {
            coyoteTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void TimeEnd()
        {
            if(coyoteTimer >= maxTime)
            {
                maxTime = coyoteTimer;
                coyoteTimer= 0;
            }
        }

        public void EventCall()
        {

        }

       
    }
}
