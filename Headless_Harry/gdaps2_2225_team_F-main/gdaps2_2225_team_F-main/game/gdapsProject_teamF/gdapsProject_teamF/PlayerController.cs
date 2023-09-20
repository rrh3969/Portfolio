using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace gdapsProject_teamF
{
    enum PlayerState
    {
        Idle,
        WalkingLeft,
        WalkingRight,
        RunningLeft,
        RunningRight,
        InAir,
        Attacking,
        Dead,
    }

    enum PlayerFrameLocation
    {
        idle = 0,
    }

    internal delegate void Death();

    internal delegate void AnimationVoid(SpriteBatch sb);

    /// <summary>
    /// Basic Player Class that impliments its own movement
    /// </summary>
    internal class PlayerController : GameObject
    {
        int playerHealth;
        //Max speed the player can reach
        float playerMaxSpeed = 5.5f;
        //The lerp ammount by which we adjust the speed
        float speedAdjustment;
        //Gravity multiplier
        float gravityMultiplier;
        float minGravity = .24f;
        float maxGravity = .7f;
        float dashDistance = 20;
        float slideTime = 0;
        float speedMultiplier = 1;
        float coyoteTimer;
        float jumpHeight = 10;
        //reference to the previous keyboard state
        KeyboardState previousKeyboardState;
        //the player's velocity
        Vector2 velocity;
        float maxFallSpeed;
        bool onGround;
        bool canJump;
        int ogWidth = 0;
        int ogHeight = 0;
        bool onWall;
        bool hitTop;
        double timerAir;
        bool stay = false;
        bool control = true;
        bool forcedMomentum;
        bool canGlide;
        bool isImmortal;
        float jumpMultiplier = 1;

        //Feet Collider
        Rectangle feetCollider;

        //Head Collider
        Rectangle headCollider;

        //Death Delegate
        Death death;

        //Handels glitching out of the map
        int previousX;

        // Animation
        Texture2D playerSpriteSheet;
        Vector2 framePosition;
        SpriteEffects playerDirection;

        int frame;              // The current animation frame
        double timeCounter;     // The amount of time that has passed
        double fps;             // The speed of the animation
        double timePerFrame;    // The amount of time (in fractional seconds) per frame

        // Constants for "source" rectangle (inside the image)
        const int WalkFrameCount = 3;       // The number of frames in the animation
        const int PlayerRectOffsetYIdle = 0;   // How far down in the image are the frames?
        const int PlayerRectHeight = 310;     // The height of a single frame
        const int PlayerRectWidth = 250;      // The width of a single frame

        const int PlayerRectOffsetYRunning = 300;   // How far down in the image are the frames?
        const int PlayerRectWidthRunning = 270;      // The width of a single frame

        const int PlayerRectOffsetYJumping = 630;   // How far down in the image are the frames?
        const int PlayerRectWidthJumping = 270;      // The width of a single frame

        AnimationVoid animate;
        float playerScale = .22f;

        public PlayerController(Texture2D asset, Rectangle position, Death death) : base(asset, position)
        {
            ogWidth = position.Width;
            ogHeight = position.Height;
            //feetCollider = new Rectangle(position.X, position.Y + 40, ogWidth / 2, ogHeight / 2);
            //headCollider = new Rectangle(position.X, position.Y + 40, ogWidth / 4, ogHeight / 2);
            feetCollider = new Rectangle(position.X, position.Y + ogHeight, ogWidth, ogHeight/3);
            headCollider = new Rectangle(position.X, position.Y + 40, ogWidth, ogHeight / 2);
            this.death += death;
            this.death += () => velocity = Vector2.Zero;

            playerSpriteSheet = asset;

            fps = 10;                     // Will cycle through 10 walk frames per second
            timePerFrame = 1.0 / fps;       // Time per frame = amount of time in a single walk image

            animate = DrawIdle;
        }


        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            //Runs the player input void
            PlayerInput(gameTime, keyboardState);
            //Updates the players X position
            position.X += (int)(velocity.X);

            //Only update the player's Y position if they are not on the ground
            if (!onGround && stay == false)
            {
                //Coyote time that lets the player jump even if they are not on the ground, but can cause double jump issue
                gravityMultiplier = keyboardState.IsKeyDown(Keys.Space) && velocity.Y < 0 ? minGravity : maxGravity;
                velocity.Y += gravityMultiplier;
                position.Y += (int)(velocity.Y);

                Debug.WriteLine("Furth");
                coyoteTimer -= (float) gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (!onGround && stay == true)
            {
                coyoteTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            UpdateAnimation(gameTime);

            if (!OnGround)
            {
                if (velocity.Y < 0)
                {
                    animate = DrawJumping;
                }
                else
                {
                    animate = DrawFalling;
                }
            }
            else
            {
                if (velocity.X == 0)
                {
                    animate = DrawIdle;
                }
                else
                {
                    animate = DrawRunning;
                }
            }

            previousKeyboardState = keyboardState;
        }

        /// <summary>
        /// lerps the player speed from 0 to a max speed by speedAdjustment
        /// if the player is pressing any button then the speed increases or decreases 
        /// </summary>
        /// <param name="gameTime"></param>
        void PlayerInput(GameTime gameTime, KeyboardState kbState)
        {

            //Temp mulitplier, will be adjusted
            float increaseSpeedMultiplier = 4;


            float playerSpeed = Math.Lerp(0, playerMaxSpeed, speedAdjustment);


            //Controls the amount along the lerp
            speedAdjustment += (float)gameTime.ElapsedGameTime.TotalSeconds * increaseSpeedMultiplier;



           
            //If the player is hitting D then set the X velocity to the current player speed and make sure playerMaxSpeed is positive
            //If the player is hitting A then move them to the left then set velocity to the negative of the current player speed and player max
           
            if (kbState.IsKeyDown(Keys.D))
            {
                if (playerMaxSpeed < 0)
                {
                    ChangeDirection();
                    playerDirection = SpriteEffects.None;
                }
            }
            else if (kbState.IsKeyDown(Keys.A))
            {
                if (playerMaxSpeed > 0)
                {
                    ChangeDirection();
                    playerDirection = SpriteEffects.FlipHorizontally;
                }
            }
            else
            {
                speedAdjustment = 0;
            }

            //Checks if the player presses Space and he is on the ground
            if (SingleKeyPress(Keys.Space, kbState) && coyoteTimer > 0)
            {
                onGround = false;
                speedMultiplier = 1.5f;
                coyoteTimer = 0;
                canJump = false;
                Jump();
                onWall = false;
                canGlide = true;
            }

            //glide
            /*
            if (kbState.IsKeyDown(Keys.R) && coyoteTimer > 0)
            {
                if (canGlide = true)
                {
                    if(playerMaxSpeed > 0)
                    {
                        velocity.X -= 2;
                        velocity.Y -= 2;
                        gravityMultiplier = gravityMultiplier - 1;
                    }
                    if (playerMaxSpeed > 0)
                    {
                        velocity.X += 2;
                        velocity.Y -= 2;
                        gravityMultiplier = gravityMultiplier - 1;
                    }
                }
            }
            */

            //Lets the player slide in their given direction
            /*
            if (kbState.IsKeyDown(Keys.Q) && onGround)
            {
                playerSpeed = Math.Lerp(velocity.X, 0, slideTime);
                slideTime += (float)gameTime.ElapsedGameTime.TotalSeconds * .1f;
            }
            else
            {
                slideTime = 0;
            }
            */
            if (kbState.IsKeyDown(Keys.Q))
            {
                speedAdjustment = 1;
            }


            //Ground pounds

            /*if (SingleKeyPress(Keys.Z, kbState) && !onGround)
            {
                
                GroundPound(gameTime);
                control = true;
               
            }
            /*
            if (coyoteTimer < -.5 && control == true)
            {

                stay = false;

                velocity.Y = 10;
                gravityMultiplier = 35;
                velocity.Y = gravityMultiplier + velocity.Y;
                control = false;
            }
            */
            

            //Sets the velocity equal to the current players speed
            velocity.X = playerSpeed * speedMultiplier;


            //Moves the player instantaniously 
            /*if (SingleKeyPress(Keys.LeftShift, kbState) && !onGround && velocity.X != 0)
            {
                Dash();
            }*/
            //wall jump

            /*if (onGround != true && onWall == true)
            {
                
                if (SingleKeyPress(Keys.Space, kbState))
                {
                    velocity.Y = 0;
                    //velocity.Y -= jumpHeight;
                }
            }*/


            FollowPlayer();
           
        }

        


        void ResetForGround()
        {
            canJump = true;
            coyoteTimer = .1f;
            velocity.Y = 0;
            onGround = true;
            speedMultiplier = 1;
            hitTop = false;
        }

        void ResetForClip()
        {
            canJump = true;
            coyoteTimer = .1f;
            //velocity.Y = 0;
            //onGround = true;
            hitTop = false;
        }

        /// <summary>
        /// Adds -7 to the Y velocity
        /// </summary>
        void Jump()
        {
            velocity.Y = 0;
            velocity.Y -= jumpHeight * jumpMultiplier;
            jumpMultiplier = 1;
        }

        /// <summary>
        /// Returns the the current value of OnGround
        /// </summary>
        public bool OnGround
        {
            get { return onGround; }
        }
        /// <summary>
        /// Method that returns true if a key is pressed once
        /// </summary>
        /// <param name="key"></param>
        /// <param name="kbState"></param>
        /// <returns></returns>
        private bool SingleKeyPress(Keys key, KeyboardState kbState)
        {
            return kbState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }
        //method so that if a movement key is being pressed the abillity can also be pressed 
        private bool MovementKeyA(Keys keyAb, Keys keyMove,KeyboardState kbState)
        {
            return (SingleKeyPress(keyAb, kbState) && kbState.IsKeyDown(keyMove));
        }
       
        /// <summary>
        /// Gets the player's current health and also allows it to be modified
        /// </summary>
        public int PlayerHealth
        {
            get { return playerHealth; }
            set { playerHealth = value; }
        }
        /// <summary>
        /// displaces the charcter by x 
        /// </summary>
        /*private void Dash()
        {
            velocity.X += (int)(Math.NormalizeFloat(playerMaxSpeed) * dashDistance);
        }*/
        /// <summary>
        /// purpose to inflict gravity on the player 
        /// returns nothing
        /// takes in nothing
        /// </summary>
        /*private void GroundPound(GameTime groundPoundTime)
        {
            //controls phyics and if statment
            stay = true;
            bool control = true;
            //if statment that suspends player in the air 
            if (stay == true && control == true)
            {
                
                gravityMultiplier = 0;
                velocity.Y = 0;
                velocity.X = 0;
                

               
            }
            if (coyoteTimer < -.5)
            {
                control = false;
                stay = false;
                velocity.Y = 10;
                gravityMultiplier = 15;
                velocity.Y = gravityMultiplier + velocity.Y;
            }

        }*/
       

        /// <summary>
        /// Changes the direction the player is going
        /// </summary>
        private void ChangeDirection()
        {
            playerMaxSpeed *= -1;
            speedAdjustment = .1f;
            speedMultiplier = 1;
        }

        private void Die()
        {

        }

        /// <summary>
        /// Lets the feet colider follow the player
        /// </summary>
        private void FollowPlayer()
        {
            /*
            if (playerMaxSpeed > 0)
            {
                feetCollider.X = position.Right - position.Width;
            }
            else
            {
                feetCollider.X = (int)(position.Left + position.Width/2);
            }
            */
            //feetCollider.X = position.Right - position.Width + 10;
            //feetCollider.X = (position.X + position.Width/4);
            feetCollider.X = position.X;
            feetCollider.Y = (int)(position.Y + position.Height/1.2);
            headCollider.X = feetCollider.X;
            headCollider.Y = (int)(position.Y - position.Height/1.5);

            //Head Following
        }

        /// <summary>
        /// Handels basic collisions from things like gravity, walking into walls, ect...
        /// </summary>
        /// <param name="obj"></param>
        private void GeneralCollision(GameObject obj)
        {
            //Handels if there is ground above the player
            /*
            if (obj.Position.Top < position.Y && !onWall)
            {
                Debug.WriteLine("first");
                position.Y = obj.Position.Bottom;
                velocity.Y = 0;
            }

            //Handels player going through walls and ground
            else if (position.Y >= obj.Position.Y)
            {
                Debug.WriteLine("Second");
                position.X = position.Right > obj.Position.Right ? obj.Position.Right : obj.Position.Left - position.Width;
                velocity.X = 0;
            }
            */

            if (obj.Position.Top >= position.Bottom)
            {
                
                ResetForGround();
            }
            else
            {
                /*
                if (obj.Position.Top < position.Y && !onWall)
                {
                    Debug.WriteLine("Second");
                    position.Y = obj.Position.Bottom;
                    velocity.Y = 0;
                    hitTop = true;
                }
                */
                if (headCollider.Intersects(obj.Position))
                {
                    Debug.WriteLine("Second");
                    position.Y = obj.Position.Bottom;
                    velocity.Y = 0;
                    hitTop = true;
                }
                else
                {
                    hitTop = false;
                }
            }

            if (feetCollider.Intersects(obj.Position) && !onGround)
            {
                //Debug.WriteLine("Third");
                Debug.WriteLine("Furth");
                position.Y = obj.Position.Top - position.Height + 1;
                ResetForClip();
            }

            if (obj.Position.Top < position.Bottom - 6 && !hitTop)
            {
                //Collides Left
                if (position.X < obj.Position.Right && position.Right > obj.Position.Right)
                {
                    position.X = obj.Position.Right;
                }


                //Collides Right
                else if (position.Right > obj.Position.Left && position.Left < obj.Position.Left)
                {
                    Debug.WriteLine("Fifth");
                    position.X = obj.Position.Left - position.Width;
                }
                velocity.X = 0;
            }
        }


        /// <summary>
        /// Start of complete collision handeling with the player
        /// </summary>
        /// <param name="others"></param>
        public void PlayerCollisionHandeling(List<GameObject> others)
        {

            if (others.Count == 0)
            {
                Debug.WriteLine("Triggered");
                onGround = false;
            }
            foreach (GameObject obj in others)
            {
                GeneralCollision(obj);
                switch (obj.Layer)
                {
                    case ObjectLayers.Ground:
                        //controls wall jump
                        //turns off bounce
                        forcedMomentum = false;
                        if (feetCollider.Intersects(obj.Position))
                        {
                            //Debug.WriteLine("Third");
                            position.Y = obj.Position.Top - position.Height + 1;
                            ResetForClip();
                            onGround = true;
                            velocity.Y = 0;
                            jumpMultiplier = 1;
                        }
                        break;
                    case ObjectLayers.Wall:
                        //controls wall jump
                        onWall = true;
                        //turns off bounce
                        forcedMomentum = false;
                        break;
                    //only kills the player if the player is not on top of the enemy
                    case ObjectLayers.Enemy:
                        //if the player is on any part of the head
                        if (position.Bottom - 10 < obj.Position.Top && !(obj is ForkEnemy))
                        {
                            //use mometum to jump
                            //turns on bounce
                            gravityMultiplier = minGravity;
                            velocity.Y = -10;
                            jumpMultiplier = 1.3f;
                            //Propel();
                            break;
                        }

                        if (!isImmortal)
                        {
                            death();
                        }
                        break;
                    default:
                        break;
                }
            }
            onWall = false;
            if (position.X <= 0 || position.X >= 700)
            {
                position.X = previousX;
            }
            previousX = position.X;
        }

        /// <summary>
        /// Allows other scripts to set the player just above the bottom of the screen\
        /// </summary>
        public void NextLevel(int screenHeight)
        {
            position.Y = screenHeight - 1;
            velocity.Y = -10;
        }

        /// <summary>
        /// Allows other scripts to set the player just bellow the top of the screen
        /// </summary>
        public void PreviousLevel()
        {
            position.Y = 1;
        }

        public void ResetPosition(Rectangle initalPosition)
        {
            position = initalPosition;
        }

        public void EnemyBounce(GameObject obj)
        {
            if(obj.Position.Top > position.Bottom)
            {
                velocity.Y = velocity.Y + coyoteTimer;
            }
        }

        public void Propel()
        {
            //if this is turned on by being on a enemy
            if(forcedMomentum)
            {
                //reset velocity
                velocity.Y = 0;
                //increase based off of time
                velocity.Y -= jumpHeight - coyoteTimer;


            }
            else
            {
                //check
                forcedMomentum = false;
            }
        }
        public bool IsImmortal
        {
            get { return isImmortal; }
            set { isImmortal = value; }
        }

        /// <summary>
        /// Updates mario's animation as necessary
        /// </summary>
        /// <param name="gameTime">Time information</param>
        public void UpdateAnimation(GameTime gameTime)
        {
            // How much time has passed?  
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (timeCounter >= timePerFrame)
            {
                frame += 1;                  

                if (frame > WalkFrameCount)   
                    frame = 1;              

                timeCounter -= timePerFrame;  
                                            
            }
        }

        private void DrawIdle(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                playerSpriteSheet,                    
                new Vector2(position.X, position.Y),
                new Rectangle(
                    PlayerRectWidth * frame,
                    PlayerRectOffsetYIdle,
                    PlayerRectWidth,
                    PlayerRectHeight),
                Color.White,
                0,
                Vector2.Zero, 
                playerScale, 
                playerDirection,
                0); 
        }


        private void DrawRunning(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                playerSpriteSheet,                   
                new Vector2(position.X, position.Y),             
                new Rectangle(                
                    PlayerRectWidthRunning * frame,   
                    PlayerRectOffsetYRunning,           
                    PlayerRectWidthRunning,             
                    PlayerRectHeight),         
                Color.White,                    
                0,                              
                Vector2.Zero,                   
                playerScale,                          
                playerDirection,                    
                0);                          
        }

        private void DrawJumping(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                playerSpriteSheet,               
                new Vector2(position.X, position.Y),            
                new Rectangle(                
                    240,   
                    PlayerRectOffsetYJumping,       
                    PlayerRectWidthJumping,         
                    PlayerRectHeight),   
                Color.White,               
                0,                   
                Vector2.Zero,           
                playerScale,             
                playerDirection,      
                0);                 
        }

        private void DrawFalling(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                playerSpriteSheet,                 
                new Vector2(position.X, position.Y),                
                new Rectangle(           
                    10,    
                    PlayerRectOffsetYJumping,          
                    PlayerRectWidthJumping,         
                    PlayerRectHeight),         
                Color.White,                 
                0,                            
                Vector2.Zero,                 
                playerScale,                       
                playerDirection,             
                0);                       
        }


        public override void Draw(SpriteBatch sb)
        {
            //DrawIdle(SpriteEffects.None, sb);

            //DrawSingle(SpriteEffects.None, sb, PlayerFrameLocation.idle);

            //DrawRunning(SpriteEffects.None, sb);

            //DrawJumping(SpriteEffects.None, sb);

            //DrawFalling(SpriteEffects.None, sb);


            animate(sb);
        }
    }
}
