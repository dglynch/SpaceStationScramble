using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceStationScramble {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SpaceStationScrambleGame : Microsoft.Xna.Framework.Game {
        const int SCREEN_WIDTH = 1280;
        const int SCREEN_HEIGHT = 720;

        //Input
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private GamePadState currentGamepadState;
        private GamePadState previousGamepadState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Synchronizer synchronizer;
        string keyCode;

        PlayerNumber currentPlayer;

        //Textures
        Texture2D characterSelectionBackground;
        Texture2D menuSelector;
        Texture2D keyCodeBackground;
        Texture2D readyStartBackground;
        Texture2D playerOneBackground;
        Texture2D playerTwoBackground;

        SpriteFont font;

        Vector2 playerOneMenuPosition = new Vector2(450, 280);
        Vector2 playerTwoMenuPosition = new Vector2(450, 380);

        //Station info
        Dictionary<SpaceStationSection, Vector2> insideNodePositions;

        ScreenContext context = ScreenContext.CHARACTER_SELECTION;

        //Player One info
        Texture2D playerOneSprite; //For animation this will need to updated
        Vector2 playerOnePosition;
        Vector2 playerOneOffset;
        float playerOneMoveSpeed;
        Vector2 playerOneMoveStep;
        PlayerOneState playerOneState;

        //Player Two info
        Texture2D playerTwoSprite; //For animation this will need to updated
        Vector2 playerTwoPosition;
        float playerTwoMoveSpeed;

        public SpaceStationScrambleGame() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            synchronizer = new Synchronizer();

            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            //Start with player one for now
            currentPlayer = PlayerNumber.ONE;

            //Setup space station information
            insideNodePositions = new Dictionary<SpaceStationSection, Vector2>();
            insideNodePositions.Add(SpaceStationSection.CENTER, new Vector2(640, 360));
            insideNodePositions.Add(SpaceStationSection.NORTH, new Vector2(640, 60));
            insideNodePositions.Add(SpaceStationSection.SOUTH, new Vector2(640, 660));
            insideNodePositions.Add(SpaceStationSection.EAST, new Vector2(980, 360));
            insideNodePositions.Add(SpaceStationSection.WEST, new Vector2(300, 360));

            //initialize player one properties
            playerOneMoveSpeed = 3.0f;
            playerOneMoveStep = Vector2.Zero;

            playerOnePosition = insideNodePositions[SpaceStationSection.CENTER];
            playerOneState = PlayerOneState.AtCenter;

            //initialize player two properties
            playerTwoMoveSpeed = 3.0f;
            playerTwoPosition = new Vector2(960, 540);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            characterSelectionBackground = Content.Load<Texture2D>("gfx/character-selection");
            menuSelector = Content.Load<Texture2D>("gfx/player");

            keyCodeBackground = Content.Load<Texture2D>("gfx/key-code-background");
            readyStartBackground = Content.Load<Texture2D>("gfx/ready-start");

            playerOneBackground = Content.Load<Texture2D>("gfx/inside-rough");
            playerTwoBackground = Content.Load<Texture2D>("gfx/outside-rough");

            playerOneSprite = Content.Load<Texture2D>("gfx/player");
            playerTwoSprite = Content.Load<Texture2D>("gfx/player");

            font = Content.Load<SpriteFont>("font/Segoe UI Mono");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            previousGamepadState = currentGamepadState;
            currentGamepadState = GamePad.GetState(PlayerIndex.One);

            switch (context) {
                case ScreenContext.CHARACTER_SELECTION:
                    if (isNewlyPressedUp()) {
                        if (currentPlayer == PlayerNumber.TWO) {
                            currentPlayer = PlayerNumber.ONE;
                        }
                    } else if (isNewlyPressedDown()) {
                        if (currentPlayer == PlayerNumber.ONE) {
                            currentPlayer = PlayerNumber.TWO;
                        }
                    }
                    if (isNewlyPressedStart()) {
                        if (currentPlayer == PlayerNumber.ONE) {
                            keyCode = synchronizer.GenerateKeyCode();
                        }
                        context = ScreenContext.KEY_CODE;
                    }
                    break;
                case ScreenContext.KEY_CODE:
                    if (isNewlyPressedStart()) {
                        context = ScreenContext.READY_TO_START;
                    }
                    break;
                case ScreenContext.READY_TO_START:
                    if (isNewlyPressedStart()) {
                        context = ScreenContext.GAME_PLAY;
                    }
                    break;
                case ScreenContext.GAME_PLAY:
                    // Allows the game to exit
                    if (currentGamepadState.Buttons.Back == ButtonState.Pressed
                        || currentKeyboardState.IsKeyDown(Keys.Escape)) {
                        this.Exit();
                    }

                    //Update player one
                    if (currentPlayer == PlayerNumber.ONE) {
                        InputKey pressedKey = InputKey.None;

                        if (currentGamepadState.IsButtonDown(Buttons.DPadUp) || currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W)) {
                            pressedKey = InputKey.MoveUp;
                        }
                        if (currentGamepadState.IsButtonDown(Buttons.DPadDown) || currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S)) {
                            pressedKey = InputKey.MoveDown;
                        }
                        if (currentGamepadState.IsButtonDown(Buttons.DPadRight) || currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D)) {
                            pressedKey = InputKey.MoveRight;
                        }
                        if (currentGamepadState.IsButtonDown(Buttons.DPadLeft) || currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A)) {
                            pressedKey = InputKey.MoveLeft;
                        }

                        processPlayerOneInput(pressedKey);
                        checkPlayerPosition();

                        //Update player1 position
                        playerOnePosition += playerOneMoveStep;
                    } else {
                        if (currentGamepadState.IsButtonDown(Buttons.DPadUp) || currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W)) {
                            playerTwoPosition.Y -= playerTwoMoveSpeed;
                        }
                        if (currentGamepadState.IsButtonDown(Buttons.DPadDown) || currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.S)) {
                            playerTwoPosition.Y += playerTwoMoveSpeed;
                        }
                        if (currentGamepadState.IsButtonDown(Buttons.DPadRight) || currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.D)) {
                            playerTwoPosition.X += playerTwoMoveSpeed;
                        }
                        if (currentGamepadState.IsButtonDown(Buttons.DPadLeft) || currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A)) {
                            playerTwoPosition.X -= playerTwoMoveSpeed;
                        }
                    }
                    break;
            }

            base.Update(gameTime);
        }

        private void processPlayerOneInput(InputKey key) {
            switch (playerOneState) {
                case PlayerOneState.AtCenter:
                    switch (key) {
                        case InputKey.MoveUp:
                            playerOneState = PlayerOneState.CenterToNorth;
                            playerOneMoveStep = new Vector2(0.0f, -playerOneMoveSpeed);
                            break;
                        case InputKey.MoveDown:
                            playerOneState = PlayerOneState.CenterToSouth;
                            playerOneMoveStep = new Vector2(0.0f, playerOneMoveSpeed);
                            break;
                        case InputKey.MoveLeft:
                            playerOneState = PlayerOneState.CenterToWest;
                            playerOneMoveStep = new Vector2(-playerOneMoveSpeed, 0.0f);
                            break;
                        case InputKey.MoveRight:
                            playerOneState = PlayerOneState.CenterToEast;
                            playerOneMoveStep = new Vector2(playerOneMoveSpeed, 0.0f);
                            break;
                    }
                    break;
                case PlayerOneState.CenterToNorth:
                case PlayerOneState.AtNorth:
                    if (key == InputKey.MoveDown) {
                        playerOneState = PlayerOneState.NorthToCenter;
                        playerOneMoveStep = new Vector2(0.0f, playerOneMoveSpeed);
                    }
                    break;
                case PlayerOneState.CenterToSouth:
                case PlayerOneState.AtSouth:
                    if (key == InputKey.MoveUp) {
                        playerOneState = PlayerOneState.SouthToCenter;
                        playerOneMoveStep = new Vector2(0.0f, -playerOneMoveSpeed);
                    }
                    break;
                case PlayerOneState.CenterToWest:
                case PlayerOneState.AtWest:
                    if (key == InputKey.MoveRight) {
                        playerOneState = PlayerOneState.WestToCenter;
                        playerOneMoveStep = new Vector2(playerOneMoveSpeed, 0.0f);
                    }
                    break;
                case PlayerOneState.CenterToEast:
                case PlayerOneState.AtEast:
                    if (key == InputKey.MoveLeft) {
                        playerOneState = PlayerOneState.EastToCenter;
                        playerOneMoveStep = new Vector2(-playerOneMoveSpeed, 0.0f);
                    }
                    break;
                case PlayerOneState.NorthToCenter:
                    if (key == InputKey.MoveUp) {
                        playerOneState = PlayerOneState.CenterToNorth;
                        playerOneMoveStep = new Vector2(0.0f, -playerOneMoveSpeed);
                    }
                    break;
                case PlayerOneState.SouthToCenter:
                    if (key == InputKey.MoveDown) {
                        playerOneState = PlayerOneState.CenterToSouth;
                        playerOneMoveStep = new Vector2(0.0f, playerOneMoveSpeed);
                    }
                    break;
                case PlayerOneState.EastToCenter:
                    if (key == InputKey.MoveRight) {
                        playerOneState = PlayerOneState.CenterToEast;
                        playerOneMoveStep = new Vector2(playerOneMoveSpeed, 0.0f);
                    }
                    break;
                case PlayerOneState.WestToCenter:
                    if (key == InputKey.MoveLeft) {
                        playerOneState = PlayerOneState.CenterToWest;
                        playerOneMoveStep = new Vector2(-playerOneMoveSpeed, 0.0f);
                    }
                    break;
            }
        }

        private void checkPlayerPosition() {
            switch (playerOneState) {
                case PlayerOneState.CenterToNorth:
                    if (Vector2.Distance(playerOnePosition, insideNodePositions[SpaceStationSection.NORTH]) < playerOneMoveSpeed) {
                        playerOnePosition = insideNodePositions[SpaceStationSection.NORTH];
                        playerOneState = PlayerOneState.AtNorth;
                        playerOneMoveStep = Vector2.Zero;
                    }
                    break;
                case PlayerOneState.CenterToSouth:
                    if (Vector2.Distance(playerOnePosition, insideNodePositions[SpaceStationSection.SOUTH]) < playerOneMoveSpeed) {
                        playerOnePosition = insideNodePositions[SpaceStationSection.SOUTH];
                        playerOneState = PlayerOneState.AtSouth;
                        playerOneMoveStep = Vector2.Zero;
                    }
                    break;
                case PlayerOneState.CenterToWest:
                    if (Vector2.Distance(playerOnePosition, insideNodePositions[SpaceStationSection.WEST]) < playerOneMoveSpeed) {
                        playerOnePosition = insideNodePositions[SpaceStationSection.WEST];
                        playerOneState = PlayerOneState.AtWest;
                        playerOneMoveStep = Vector2.Zero;
                    }
                    break;
                case PlayerOneState.CenterToEast:
                    if (Vector2.Distance(playerOnePosition, insideNodePositions[SpaceStationSection.EAST]) < playerOneMoveSpeed) {
                        playerOnePosition = insideNodePositions[SpaceStationSection.EAST];
                        playerOneState = PlayerOneState.AtEast;
                        playerOneMoveStep = Vector2.Zero;
                    }
                    break;
                case PlayerOneState.NorthToCenter:
                case PlayerOneState.SouthToCenter:
                case PlayerOneState.EastToCenter:
                case PlayerOneState.WestToCenter:
                    if (Vector2.Distance(playerOnePosition, insideNodePositions[SpaceStationSection.CENTER]) < playerOneMoveSpeed) {
                        playerOnePosition = insideNodePositions[SpaceStationSection.CENTER];
                        playerOneState = PlayerOneState.AtCenter;
                        playerOneMoveStep = Vector2.Zero;
                    }
                    break;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (context) {
                case ScreenContext.CHARACTER_SELECTION:
                    spriteBatch.Draw(characterSelectionBackground, Vector2.Zero, Color.White);
                    if (currentPlayer == PlayerNumber.ONE) {
                        spriteBatch.Draw(menuSelector, playerOneMenuPosition, Color.White);
                    } else {
                        spriteBatch.Draw(menuSelector, playerTwoMenuPosition, Color.White);
                    }
                    break;
                case ScreenContext.KEY_CODE:
                    spriteBatch.Draw(keyCodeBackground, Vector2.Zero, Color.White);
                    if (currentPlayer == PlayerNumber.ONE) {
                        spriteBatch.DrawString(font, keyCode, new Vector2(640 - font.MeasureString(keyCode).X / 2, 360), Color.Yellow);
                    } else {

                    }
                    break;
                case ScreenContext.READY_TO_START:
                    spriteBatch.Draw(readyStartBackground, Vector2.Zero, Color.White);
                    break;
                case ScreenContext.GAME_PLAY:
                    //Draw the background
                    if (currentPlayer == PlayerNumber.ONE) {
                        spriteBatch.Draw(playerOneBackground, Vector2.Zero, Color.White);

                        //For debugging node positions
                        spriteBatch.Draw(playerOneSprite, insideNodePositions[SpaceStationSection.NORTH], Color.Green);
                        spriteBatch.Draw(playerOneSprite, insideNodePositions[SpaceStationSection.SOUTH], Color.Green);
                        spriteBatch.Draw(playerOneSprite, insideNodePositions[SpaceStationSection.EAST], Color.Green);
                        spriteBatch.Draw(playerOneSprite, insideNodePositions[SpaceStationSection.WEST], Color.Green);
                        spriteBatch.Draw(playerOneSprite, insideNodePositions[SpaceStationSection.CENTER], Color.Green);

                        //Draw the player
                        spriteBatch.Draw(playerOneSprite, playerOnePosition, Color.White);
                    } else {
                        spriteBatch.Draw(playerTwoBackground, Vector2.Zero, Color.White);

                        //Draw the player
                        spriteBatch.Draw(playerTwoSprite, playerTwoPosition, Color.White);
                    }
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private bool isNewlyPressedUp() {
            return (currentGamepadState.IsButtonDown(Buttons.DPadUp) && !previousGamepadState.IsButtonDown(Buttons.DPadUp))
                || (currentKeyboardState.IsKeyDown(Keys.Up) && !previousKeyboardState.IsKeyDown(Keys.Up))
                || (currentKeyboardState.IsKeyDown(Keys.W) && !previousKeyboardState.IsKeyDown(Keys.W));
        }

        private bool isNewlyPressedDown() {
            return (currentGamepadState.IsButtonDown(Buttons.DPadDown) && !previousGamepadState.IsButtonDown(Buttons.DPadDown))
                || (currentKeyboardState.IsKeyDown(Keys.Down) && !previousKeyboardState.IsKeyDown(Keys.Down))
                || (currentKeyboardState.IsKeyDown(Keys.S) && !previousKeyboardState.IsKeyDown(Keys.S));
        }

        private bool isNewlyPressedStart() {
            return (currentGamepadState.IsButtonDown(Buttons.Start) && !previousGamepadState.IsButtonDown(Buttons.Start))
                || (currentKeyboardState.IsKeyDown(Keys.Enter) && !previousKeyboardState.IsKeyDown(Keys.Enter))
                || (currentKeyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.Space));
        }
    
    }

    public enum PlayerNumber {
        ONE,
        TWO
    };

    public enum SpaceStationSection {
        NORTH,
        SOUTH,
        EAST,
        WEST,
        CENTER
    };

    public enum PlayerOneState {
        AtCenter,
        AtNorth,
        AtSouth,
        AtEast,
        AtWest,
        CenterToNorth,
        CenterToSouth,
        CenterToEast,
        CenterToWest,
        NorthToCenter,
        SouthToCenter,
        EastToCenter,
        WestToCenter
    };

    public enum InputKey {
        None,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight
    };

    public enum ScreenContext {
        CHARACTER_SELECTION,
        KEY_CODE,
        READY_TO_START,
        GAME_PLAY
    }
}
