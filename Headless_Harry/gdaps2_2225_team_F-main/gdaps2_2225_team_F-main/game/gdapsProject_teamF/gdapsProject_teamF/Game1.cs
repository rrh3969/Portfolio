using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace gdapsProject_teamF
{
    // Delegate for controlling what should be drawn
    internal delegate void CurrentDraw();

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        // To save keyboard
        private KeyboardState prevKeyboard;
        private KeyboardState currentKeyboard;
        private MouseState prevMouse;
        private MouseState currentMouse;
        private GameState current = GameState.Menu;
        // Title texture and location
        private Texture2D titleLogo;
        private Rectangle titleLoc;
        // Title info and location 
        private SpriteFont titleFont;
        private Vector2 titleSize;
        private Vector2 pauseInfo;
        private Vector2 pausePos;
        private Vector2 overPos;
        private Vector2 overInfo;
        private Rectangle Screen;
        // Player test texture
        private Texture2D playerTexture;
        private Texture2D overDisplay;
        private Texture2D pauseDisplay;
        private Texture2D optionsMenu;
        private Texture2D optionsButtonAsset;
        private Texture2D yesButtonAsset;
        private Texture2D noButtonAsset;
        private Texture2D startButtonAsset;
        private Texture2D godmodeButtonAsset;
        private Texture2D winScreen;
        private Texture2D titleScreen;
        private Texture2D titleScreen2;
        private Texture2D backround;

        // Buttons
        Button startButton;
        Button optionsButton;
        Button backButton;
        Button godmodeButton;
        Button yesButton;
        Button noButton;

        // Managers
        PlayerController playerController;
        LevelManager levelManager;

        // Inital Player Position
        Rectangle startingPlayerPosition = new Rectangle(20, 820, 55, 55);

        // A delegate for what we should currently be drawing
        CurrentDraw currentDraw;

        // A list of delegates that is indexed 
        List<CurrentDraw> drawMethods;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        // Basic FSM
        private enum GameState
        {
            Menu,
            Game,
            Pause,
            Over,
            Win,
            Options,
            GMSelect
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //assigns value
            titleLoc = new Rectangle(145, 100, 420, 300);
            titleSize = new Vector2(275, 400);
            overPos = new Vector2(300, 300);
            overInfo = new Vector2(240, 400);
            pauseInfo = new Vector2(280, 400);
            pausePos = new Vector2(320, 300);
            Screen = new Rectangle(0, 0, 700, 900);


            // Initializes all draw methods
            AddDrawMethods();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // General Assets
            titleLogo = Content.Load<Texture2D>("title");
            titleFont = Content.Load<SpriteFont>("arial12");
            
            // Player Character Assets
            playerTexture = Content.Load<Texture2D>("Redone Run");
            
            // Full Screen Assets
            winScreen = Content.Load<Texture2D>("WinTemp");
            optionsMenu = Content.Load<Texture2D>("GodModeSelect");
            pauseDisplay = Content.Load<Texture2D>("pauseMenuEdit");
            overDisplay = Content.Load<Texture2D>("GameOverScreenEdit");
            backround = Content.Load<Texture2D>("backround");

            // Button Assets
            optionsButtonAsset = Content.Load<Texture2D>("OptionsButton");
            godmodeButtonAsset = Content.Load<Texture2D>("GodModeButton");
            startButtonAsset = Content.Load<Texture2D>("StartButton");
            yesButtonAsset = Content.Load<Texture2D>("YesButton");
            noButtonAsset = Content.Load<Texture2D>("NoButton");

            titleScreen = Content.Load<Texture2D>("TitleScreen3");
            titleScreen2 = Content.Load<Texture2D>("TitleScreen");


            _graphics.PreferredBackBufferWidth = 700;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();

            // Loads the player
            playerController = new PlayerController(playerTexture, startingPlayerPosition, () => ChangeState(GameState.Over));

            startButton = new Button(
                startButtonAsset,
                new Rectangle(
                    _graphics.PreferredBackBufferWidth / 2 - 60,
                    _graphics.PreferredBackBufferHeight / 2 - 50,
                    256,
                    128));
            optionsButton = new Button(
                optionsButtonAsset,
                new Rectangle(
                    _graphics.PreferredBackBufferWidth / 2 - 60,
                    _graphics.PreferredBackBufferHeight / 2 + 100,
                    256,
                    128));
            yesButton = new Button(
                yesButtonAsset,
                new Rectangle(
                    _graphics.PreferredBackBufferWidth / 2 - 60,
                    _graphics.PreferredBackBufferHeight - 175,
                    128,
                    64));
            noButton = new Button(
                noButtonAsset,
                new Rectangle(
                    _graphics.PreferredBackBufferWidth / 2 - 60,
                    _graphics.PreferredBackBufferHeight - 240,
                    128,
                    64));
            godmodeButton = new Button(
                godmodeButtonAsset,
                new Rectangle(
                    _graphics.PreferredBackBufferWidth / 2 - 55,
                    _graphics.PreferredBackBufferHeight - 400,
                    256,
                    128));

            //Controls the transitioning and drawing of levels
            levelManager = new LevelManager(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, Content, _spriteBatch, playerController);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            currentKeyboard = Keyboard.GetState();
            currentMouse = Mouse.GetState();


            switch (current)
            {
                case GameState.Menu:
                    IsMouseVisible = true;
                    if (currentKeyboard.IsKeyDown(Keys.Enter) && prevKeyboard.IsKeyUp(Keys.Enter))
                    {
                        ChangeState(GameState.Game);
                    }
                    startButton.Update(gameTime);
                    if (startButton.ButtonPressed(prevMouse))
                    {
                        ChangeState(GameState.Game);
                    }
                    optionsButton.Update(gameTime);
                    if (optionsButton.ButtonPressed(prevMouse)) 
                    { 
                        ChangeState(GameState.Options);
                    }
                    break;
                case GameState.Game:
                    {
                        playerController.Update(gameTime);
                        playerController.PlayerCollisionHandeling(levelManager.CurrentPartition.Query(playerController.Position));

                        if (currentKeyboard.IsKeyDown(Keys.P) && prevKeyboard.IsKeyUp(Keys.P))
                        {
                            ChangeState(GameState.Pause);
                        }
                        if (currentKeyboard.IsKeyDown(Keys.K) && prevKeyboard.IsKeyUp(Keys.K))
                        {
                            ChangeState(GameState.Over);
                        }
                        if (levelManager.hasWon)
                        {
                            ChangeState(GameState.Win);
                        }

                        levelManager.Update(gameTime);
                        IsMouseVisible = false;
                    }
                    break;
                case GameState.Pause:
                    {
                        if (currentKeyboard.IsKeyDown(Keys.P) && prevKeyboard.IsKeyUp(Keys.P))
                        {
                            ChangeState(GameState.Game);
                        }
                    }
                    break;

                case GameState.Over:
                    {
                        if (currentKeyboard.IsKeyDown(Keys.Enter) && prevKeyboard.IsKeyUp(Keys.Enter))
                        {
                            ChangeState(GameState.Menu);
                            ResetGame();
                        }
                    }
                    break;
                case GameState.Win:
                    {
                        levelManager.hasWon = false;
                        if (currentKeyboard.IsKeyDown(Keys.Enter) && prevKeyboard.IsKeyUp(Keys.Enter))
                        {
                            ChangeState(GameState.Menu);
                            ResetGame();
                        }
                    }
                    break;
                case GameState.Options:
                    {
                        godmodeButton.Update(gameTime);
                        if (godmodeButton.ButtonPressed(prevMouse))
                        {
                            ChangeState(GameState.GMSelect);
                        }
                    }
                    break;
                case GameState.GMSelect:
                    {
                        // Allow for the option of yes and no 
                        yesButton.Update(gameTime);
                        if (yesButton.ButtonPressed(prevMouse))
                        {
                            // If yes is clicked disable
                            playerController.IsImmortal = false;
                            ChangeState(GameState.Menu);
                        }
                        noButton.Update(gameTime);
                        if (noButton.ButtonPressed(prevMouse))
                        {
                            // If no is clicked enable 
                            playerController.IsImmortal = true;
                            ChangeState(GameState.Menu);
                        }
                    }
                    break;
            }
            
            prevMouse = currentMouse;
            prevKeyboard = currentKeyboard;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            currentDraw();
           

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Changes the state of the game and changes what is currently being drawn
        /// </summary>
        /// <param name="gameState"></param>
        void ChangeState(GameState gameState)
        {
            current = gameState;
            currentDraw = drawMethods[(int)gameState];
        }

        /// <summary>
        /// Adds all code/method functionality to the draw list
        /// </summary>
        void AddDrawMethods()
        {
            drawMethods = new List<CurrentDraw>();

            //Menu
            drawMethods.Add(() =>
            {
                _spriteBatch.Draw(titleScreen, new Vector2(0, 0), Color.White);
                _spriteBatch.Draw(titleLogo, titleLoc, Color.White);
                startButton.Draw(_spriteBatch);
                optionsButton.Draw(_spriteBatch);
                //string intro = "press enter to begin";
                //_spriteBatch.DrawString(titleFont, intro, titleSize, Color.White);
            });

            // Game
            drawMethods.Add(() =>
            {
                _spriteBatch.Draw(backround, new Vector2(0, 0), Color.White);
                if(playerController.IsImmortal)
                {
                    _spriteBatch.Draw(godmodeButtonAsset, new Rectangle(25, 25, 128, 64), Color.White);
                }
                playerController.Draw(_spriteBatch);
                levelManager.DrawLevel(_spriteBatch);
            });

            // Pause
            drawMethods.Add(() =>
            {
                _spriteBatch.Draw(pauseDisplay, Screen, Color.White);
            });

            // Game Over
            drawMethods.Add(() =>
            {
                _spriteBatch.Draw(overDisplay, Screen, Color.White);
            });

            // Win
            drawMethods.Add(() =>
            {
                _spriteBatch.Draw(winScreen, Screen, Color.White);
            });

            // Options
            drawMethods.Add(() =>
            {
                _spriteBatch.Draw(titleScreen2, Screen, Color.White);
                godmodeButton.Draw(_spriteBatch);
            });

            // GMSelect 
            drawMethods.Add(() =>
            {
                _spriteBatch.Draw(optionsMenu, Screen, Color.White);
                if(playerController.IsImmortal)
                {
                    yesButton.Draw(_spriteBatch);
                }
                else
                {
                    noButton.Draw(_spriteBatch);
                }
                
            });

            //Sets the current draw to our menu
            currentDraw = drawMethods[0];
        }

        void ResetGame()
        {
            playerController.ResetPosition(startingPlayerPosition);
            levelManager.ResetGame();
        }


    }
}