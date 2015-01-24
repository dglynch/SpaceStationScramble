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

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Synchronizer synchronizer;

        PlayerNumber currentPlayer;

        //Textures
        Texture2D playerOneBackground;
        Texture2D playerTwoBackground;

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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            playerOneBackground = Content.Load<Texture2D>("gfx/inside-rough");
            playerTwoBackground = Content.Load<Texture2D>("gfx/outside-rough");
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            //Draw the background
            if (currentPlayer == PlayerNumber.ONE) {
                spriteBatch.Draw(playerOneBackground, Vector2.Zero, Color.White);
            }
            else {
                spriteBatch.Draw(playerTwoBackground, Vector2.Zero, Color.White)
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public enum PlayerNumber {
        ONE,
        TWO
    };
}
