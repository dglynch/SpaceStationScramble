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
        TimeSpan deathTime;

        PlayerNumber currentPlayer;
        MenuItem currentMenuItem;

        //Textures
        Texture2D titleScreenMenu;
        Texture2D instructions;
        Texture2D credits;
        Texture2D characterSelectionBackground;
        Texture2D menuSelector;
        Texture2D keyCodeBackground;
        Texture2D readyStartBackground;
        Texture2D playerOneBackground;
        Texture2D playerTwoBackground;
        Texture2D valvePanel;

        SpriteFont font;

        Vector2 playerOneMenuPosition = new Vector2(450, 280);
        Vector2 playerTwoMenuPosition = new Vector2(450, 380);
        Vector2 playerThreeMenuPosition = new Vector2(450, 480);
        Vector2 playerFourMenuPosition = new Vector2(450, 580);

        //Station info
        Dictionary<SpaceStationSection, Vector2> insideNodePositions;

        ScreenContext context = ScreenContext.TITLE_SCREEN_MENU;

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

        //Disaster event info
        //eww eww eww, gross, gross, gross
        public static Texture2D steamTexture; 

        EventGenerator eventGenerator;
        DisasterEvent nextEvent;
        List<DisasterEvent> disasterEvents;
        double elapsedRoundTime;
        private GasValve currentlyClosingValve;
        private double valveClosingEndTime;

        SoundEffect valveTurn1;
        SoundEffect valveTurn2;
        SoundEffectInstance valve;
        bool valveSwap;

        SoundEffect affirmativeFeedback;
        SoundEffect negativeFeedback;

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
            currentMenuItem = MenuItem.NEW_GAME;

            //Setup space station information
            insideNodePositions = new Dictionary<SpaceStationSection, Vector2>();
            insideNodePositions.Add(SpaceStationSection.CENTER, new Vector2(640, 360));
            insideNodePositions.Add(SpaceStationSection.NORTH, new Vector2(640, 60));
            insideNodePositions.Add(SpaceStationSection.SOUTH, new Vector2(640, 660));
            insideNodePositions.Add(SpaceStationSection.EAST, new Vector2(980, 360));
            insideNodePositions.Add(SpaceStationSection.WEST, new Vector2(300, 360));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            titleScreenMenu = Content.Load<Texture2D>("gfx/title-screen-menu");
            instructions = Content.Load<Texture2D>("gfx/instructions");
            credits = Content.Load<Texture2D>("gfx/credits");
            characterSelectionBackground = Content.Load<Texture2D>("gfx/character-selection");
            menuSelector = Content.Load<Texture2D>("gfx/player");

            keyCodeBackground = Content.Load<Texture2D>("gfx/key-code-background");
            readyStartBackground = Content.Load<Texture2D>("gfx/ready-start");


            playerOneBackground = Content.Load<Texture2D>("gfx/inside-rough");
            playerTwoBackground = Content.Load<Texture2D>("gfx/outside-rough");

            playerOneSprite = Content.Load<Texture2D>("gfx/player");
            playerTwoSprite = Content.Load<Texture2D>("gfx/player");

            steamTexture = Content.Load<Texture2D>("gfx/steam");

            font = Content.Load<SpriteFont>("font/Segoe UI Mono");

            valvePanel = Content.Load<Texture2D>("gfx/valve-panel");

            valveTurn1 = Content.Load<SoundEffect>("sound/Valve Turn 1");
            valveTurn2 = Content.Load<SoundEffect>("sound/Valve Turn 2");

            affirmativeFeedback = Content.Load<SoundEffect>("sound/Affirmative FB");
            negativeFeedback = Content.Load<SoundEffect>("sound/Negative FB");
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
                case ScreenContext.TITLE_SCREEN_MENU:
                    if (isNewlyPressedUp()) {
                        if (currentMenuItem != MenuItem.NEW_GAME) {
                            currentMenuItem--;
                        }
                    }
                    if (isNewlyPressedDown()) {
                        if (currentMenuItem != MenuItem.EXIT) {
                            currentMenuItem++;
                        }
                    }
                    if (isNewlyPressedStart()) {
                        switch (currentMenuItem) {
                            case MenuItem.NEW_GAME:
                                context = ScreenContext.CHARACTER_SELECTION;
                                break;
                            case MenuItem.INSTRUCTIONS:
                                context = ScreenContext.INSTRUCTIONS;
                                break;
                            case MenuItem.CREDITS:
                                context = ScreenContext.CREDITS;
                                break;
                            case MenuItem.EXIT:
                                this.Exit();
                                break;
                        }
                    }
                    break;
                case ScreenContext.INSTRUCTIONS:
                case ScreenContext.CREDITS:
                case ScreenContext.DEATH:
                    if (isNewlyPressedStart() || isNewlyPressedBack()) {
                        context = ScreenContext.TITLE_SCREEN_MENU;
                    }
                    break;
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
                        } else {
                            keyCode = string.Empty;
                        }
                        context = ScreenContext.KEY_CODE;
                    }
                    if (isNewlyPressedBack()) {
                        context = ScreenContext.TITLE_SCREEN_MENU;
                    }
                    break;
                case ScreenContext.KEY_CODE:
                    if (currentPlayer == PlayerNumber.ONE) {
                        if (isNewlyPressedStart()) {
                            context = ScreenContext.READY_TO_START;
                        }
                        if (isNewlyPressedBack()) {
                            context = ScreenContext.CHARACTER_SELECTION;
                        }
                    } else {
                        keyCode += getTypedCharacter();
                        if (isNewlyPressedStart()) {
                            try {
                                synchronizer.AcceptKeyCode(keyCode);
                                context = ScreenContext.READY_TO_START;
                            } catch (InvalidKeyCodeException) {
                                keyCode = string.Empty;
                            }
                        }
                        if (isNewlyPressedBack()) {
                            if (keyCode.Length > 0) {
                                keyCode = keyCode.Remove(keyCode.Length - 1);
                            } else {
                                context = ScreenContext.CHARACTER_SELECTION;
                            }
                        }
                    }
                    break;
                case ScreenContext.READY_TO_START:
                    if (isNewlyPressedStart()) {
                        setInitialGameState();
                        context = ScreenContext.GAME_PLAY;
                    }
                    if (isNewlyPressedBack()) {
                        context = ScreenContext.TITLE_SCREEN_MENU;
                    }
                    break;
                case ScreenContext.GAME_PLAY:
                    // Allows the game to exit
                    if (currentGamepadState.Buttons.Back == ButtonState.Pressed
                            || currentKeyboardState.IsKeyDown(Keys.Escape)) {
                        context = ScreenContext.TITLE_SCREEN_MENU;
                    }

                    elapsedRoundTime += gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (nextEvent.StartTime <= elapsedRoundTime) {
                        disasterEvents.Add(nextEvent);
                        nextEvent = eventGenerator.NextEvent();
                    }

                    List<DisasterEvent> eventsToRemove = new List<DisasterEvent>();
                    foreach (DisasterEvent theEvent in disasterEvents) {
                        if (theEvent.EndTime <= elapsedRoundTime) {
                            eventsToRemove.Add(theEvent);
                            if (theEvent.VisibleToPlayer != currentPlayer) {
                                deathTime = gameTime.TotalGameTime;
                                context = ScreenContext.DEATH;
                            }
                        } else {
                            theEvent.Update();
                        }
                    }

                    foreach (DisasterEvent theEvent in eventsToRemove) {
                       disasterEvents.Remove(theEvent);
                    }
                    eventsToRemove.Clear();

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

                        if (playerOneState == PlayerOneState.AtCenter
                                || playerOneState == PlayerOneState.AtEast
                                || playerOneState == PlayerOneState.AtNorth
                                || playerOneState == PlayerOneState.AtSouth
                                || playerOneState == PlayerOneState.AtWest) {
                            if (currentGamepadState.IsButtonDown(Buttons.X) || currentKeyboardState.IsKeyDown(Keys.D1)) {
                                beginClosingGasValve(GasValve.BLUE);
                            } else if (currentGamepadState.IsButtonDown(Buttons.Y) || currentKeyboardState.IsKeyDown(Keys.D2)) {
                                beginClosingGasValve(GasValve.YELLOW);
                            } else if (currentGamepadState.IsButtonDown(Buttons.A) || currentKeyboardState.IsKeyDown(Keys.D3)) {
                                beginClosingGasValve(GasValve.GREEN);
                            } else if (currentGamepadState.IsButtonDown(Buttons.B) || currentKeyboardState.IsKeyDown(Keys.D4)) {
                                beginClosingGasValve(GasValve.RED);
                            }
                        } else {
                            stopClosingValve();
                        }

                        if (currentlyClosingValve != GasValve.NONE && valveClosingEndTime < elapsedRoundTime) {
                            finishClosingGasValve(currentlyClosingValve);
                        }
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

        private void setInitialGameState() {
            currentlyClosingValve = GasValve.NONE;
            valveClosingEndTime = Double.MaxValue;

            //initialize player one properties
            playerOneMoveSpeed = 3.0f;
            playerOneMoveStep = Vector2.Zero;

            playerOnePosition = insideNodePositions[SpaceStationSection.CENTER];
            playerOneState = PlayerOneState.AtCenter;

            //initialize player two properties
            playerTwoMoveSpeed = 3.0f;
            playerTwoPosition = new Vector2(960, 540);

            disasterEvents = new List<DisasterEvent>();
            eventGenerator = new EventGenerator(synchronizer);
            elapsedRoundTime = 0;
            nextEvent = eventGenerator.NextEvent();
        }

        private void beginClosingGasValve(GasValve gasValve) {
            if (currentlyClosingValve != gasValve) {
                currentlyClosingValve = gasValve;
                valveClosingEndTime = elapsedRoundTime + 2000;
                valve = getValveInstance();
                valve.Play();
            }
        }

        private SoundEffectInstance getValveInstance() {
            valveSwap = !valveSwap;
            if (valveSwap) {
                return valveTurn1.CreateInstance();
            } else {
                return valveTurn2.CreateInstance();
            }
        }

        private void stopClosingValve() {
            currentlyClosingValve = GasValve.NONE;
            valveClosingEndTime = Double.MaxValue;
            if (valve != null) {
                valve.Stop();
            }
        }
        private void finishClosingGasValve(GasValve gasValve) {
            currentlyClosingValve = GasValve.NONE;
            valveClosingEndTime = Double.MaxValue;
            removePendingGasLeak(gasValve, playerOneState);
        }

        private void removePendingGasLeak(GasValve gasValve, PlayerOneState playerOneState) {
            var eventsToRemove = new List<DisasterEvent>();
            foreach (var disaster in disasterEvents) {
                GasLeakDisaster gld = (GasLeakDisaster) disaster;
                if (sameColor(gasValve, gld.SteamColor) && sameLocation(playerOneState, gld.Position)) {
                    eventsToRemove.Add(disaster);
                }
            }
            if (eventsToRemove.Count > 0) {
                affirmativeFeedback.Play();
            } else {
                negativeFeedback.Play();
            }
            disasterEvents = disasterEvents.Except(eventsToRemove).ToList();
        }

        private bool sameColor(GasValve gasValve, SteamColor steamColor) {
            if (gasValve == GasValve.BLUE && steamColor == SteamColor.Blue) {
                return true;
            } else if (gasValve == GasValve.YELLOW && steamColor == SteamColor.Yellow) {
                return true;
            } else if (gasValve == GasValve.GREEN && steamColor == SteamColor.Green) {
                return true;
            } else if (gasValve == GasValve.RED && steamColor == SteamColor.Red) {
                return true;
            } else {
                return false;
            }
        }

        private bool sameLocation(PlayerOneState playerOneState, Vector2 position) {
            if (playerOneState == PlayerOneState.AtCenter && position == insideNodePositions[SpaceStationSection.CENTER]) {
                return true;
            } else if (playerOneState == PlayerOneState.AtEast && position == insideNodePositions[SpaceStationSection.EAST]) {
                return true;
            } else if (playerOneState == PlayerOneState.AtNorth && position == insideNodePositions[SpaceStationSection.NORTH]) {
                return true;
            } else if (playerOneState == PlayerOneState.AtSouth && position == insideNodePositions[SpaceStationSection.SOUTH]) {
                return true;
            } else if (playerOneState == PlayerOneState.AtWest && position == insideNodePositions[SpaceStationSection.WEST]) {
                return true;
            } else {
                return false;
            }
        }

        private string getTypedCharacter() {
            Keys[] pressedKeys = currentKeyboardState.GetPressedKeys();
            foreach (Keys key in pressedKeys) {
                if (previousKeyboardState.IsKeyUp(key)) {
                    string value = key.ToString();
                    if (value.Length == 1) {
                        if (value.CompareTo("A") >= 0 && value.CompareTo("Z") <= 0) {
                            if (currentKeyboardState.IsKeyDown(Keys.LeftShift) || currentKeyboardState.IsKeyDown(Keys.RightShift)) {
                                return value;
                            } else {
                                return value.ToLower();
                            }
                        }
                    }
                }
            }
            return string.Empty;
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
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            switch (context) {
                case ScreenContext.TITLE_SCREEN_MENU:
                    spriteBatch.Draw(titleScreenMenu, Vector2.Zero, Color.White);
                    switch (currentMenuItem) {
                        case MenuItem.NEW_GAME:
                            spriteBatch.Draw(menuSelector, playerOneMenuPosition, Color.White);
                            break;
                        case MenuItem.INSTRUCTIONS:
                            spriteBatch.Draw(menuSelector, playerTwoMenuPosition, Color.White);
                            break;
                        case MenuItem.CREDITS:
                            spriteBatch.Draw(menuSelector, playerThreeMenuPosition, Color.White);
                            break;
                        case MenuItem.EXIT:
                            spriteBatch.Draw(menuSelector, playerFourMenuPosition, Color.White);
                            break;
                    }
                    break;
                case ScreenContext.INSTRUCTIONS:
                    spriteBatch.Draw(instructions, Vector2.Zero, Color.White);
                    break;
                case ScreenContext.CREDITS:
                    spriteBatch.Draw(credits, Vector2.Zero, Color.White);
                    break;
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
                        spriteBatch.DrawString(font, keyCode, new Vector2(640 - font.MeasureString(keyCode).X / 2, 360), Color.Yellow);
                        if (gameTime.TotalGameTime.TotalMilliseconds % 1000 > 500) {
                            spriteBatch.DrawString(font, "_", new Vector2(640 + font.MeasureString(keyCode).X / 2, 360), Color.Yellow);
                        }
                        if (Cheater.CheatsOn) {
                            spriteBatch.DrawString(font, "GadgetKeyDustChild", new Vector2(20, 20), Color.Yellow, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        }
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

                        Vector2 centreValves = new Vector2(740, 470);
                        Vector2 eastValves = new Vector2(1100, 360);
                        Vector2 northValves = new Vector2(740, 20);
                        Vector2 southValves = new Vector2(740, 650);
                        Vector2 westValves = new Vector2(130, 360);
                        Vector2 offScreenValves = new Vector2(-1000, -1000);

                        Vector2 currentValves;

                        switch(playerOneState) {
                            case PlayerOneState.AtCenter:
                                currentValves = centreValves;
                                break;
                            case PlayerOneState.AtEast:
                                currentValves = eastValves;
                                break;
                            case PlayerOneState.AtNorth:
                                currentValves = northValves;
                                break;
                            case PlayerOneState.AtSouth:
                                currentValves = southValves;
                                break;
                            case PlayerOneState.AtWest:
                                currentValves = westValves;
                                break;
                            default:
                                currentValves = offScreenValves;
                                break;
                        }
                        spriteBatch.Draw(valvePanel, currentValves, Color.White);

                        float rotation = (float) (gameTime.TotalGameTime.TotalMilliseconds % 500 * 2 * Math.PI / 500);
                        switch (currentlyClosingValve) {
                            case GasValve.NONE:
                                break;
                            case GasValve.BLUE:
                                spriteBatch.DrawString(font, "O", new Vector2(currentValves.X + 19, currentValves.Y + 34), Color.White, rotation, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
                                break;
                            case GasValve.YELLOW:
                                spriteBatch.DrawString(font, "O", new Vector2(currentValves.X + 34, currentValves.Y + 19), Color.White, rotation, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
                                break;
                            case GasValve.GREEN:
                                spriteBatch.DrawString(font, "O", new Vector2(currentValves.X + 36, currentValves.Y + 52), Color.White, rotation, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
                                break;
                            case GasValve.RED:
                                spriteBatch.DrawString(font, "O", new Vector2(currentValves.X + 52, currentValves.Y + 34), Color.White, rotation, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
                                break;
                        }

                        //Draw the player
                        spriteBatch.Draw(playerOneSprite, playerOnePosition, Color.White);
                    } else {
                        spriteBatch.Draw(playerTwoBackground, Vector2.Zero, Color.White);

                        //Draw the player
                        spriteBatch.Draw(playerTwoSprite, playerTwoPosition, Color.White);
                    }
                    TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, (int) elapsedRoundTime);
                    spriteBatch.DrawString(font, string.Format("Time: {0,2:00}:{1,2:00}", timeSpan.Minutes, timeSpan.Seconds), new Vector2(10, 10), Color.White);
                    foreach (DisasterEvent disaster in disasterEvents) {
                        if (disaster.VisibleToPlayer == currentPlayer) {
                            disaster.Draw(spriteBatch);
                        }
                        if (Cheater.CheatsOn) {
                            spriteBatch.DrawString(font, "Event [" + disaster + "] time left: "
                                + (int)((disaster.EndTime - elapsedRoundTime) / 1000),
                                new Vector2(20, 50 + (disasterEvents.IndexOf(disaster) * 14)), Color.Red, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                        }
                    }
                    break;
                case ScreenContext.DEATH:
                    Vector2 measureString = font.MeasureString("GAME OVER");
                    spriteBatch.DrawString(font, "GAME OVER", new Vector2(640 - measureString.X / 2, 360 - measureString.Y / 2), Color.Red);
                    spriteBatch.DrawString(font, string.Format("Time: {0,2:00}:{1,2:00}", deathTime.Minutes, deathTime.Seconds), new Vector2(10, 10), Color.White);
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

        private bool isNewlyPressedBack() {
            return (currentGamepadState.IsButtonDown(Buttons.Back) && !previousGamepadState.IsButtonDown(Buttons.Back))
                || (currentKeyboardState.IsKeyDown(Keys.Back) && !previousKeyboardState.IsKeyDown(Keys.Back))
                || (currentKeyboardState.IsKeyDown(Keys.Escape) && !previousKeyboardState.IsKeyDown(Keys.Escape));
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
        TITLE_SCREEN_MENU,
        INSTRUCTIONS,
        CREDITS,
        CHARACTER_SELECTION,
        KEY_CODE,
        READY_TO_START,
        GAME_PLAY,
        DEATH
    }

    public enum MenuItem {
        NEW_GAME,
        INSTRUCTIONS,
        CREDITS,
        EXIT
    }

    public enum GasValve {
        NONE,
        BLUE,
        YELLOW,
        GREEN,
        RED
    }
}
