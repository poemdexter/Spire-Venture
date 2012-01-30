using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpireVenture.screens.framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SpireVenture.managers
{
    public class ScreenManager : DrawableGameComponent
    {
        List<GameScreen> screens = new List<GameScreen>();
        List<GameScreen> screensToUpdate = new List<GameScreen>();
        InputState input = new InputState();
        bool isInitialized;
        public GraphicsDeviceManager Graphics { get; set; }

        // shared amongst screen components
        public SpriteBatch SpriteBatch { get; set; }
        public SpriteFont Font { get; set; }
        public Dictionary<string, Texture2D> SpriteDict { get; set; }

        // constructor
        public ScreenManager(Game game, GraphicsDeviceManager graphics)
            : base(game)
        {
            Graphics = graphics;
        }

        public override void Initialize()
        {
            base.Initialize();
            isInitialized = true;
        }

        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            ContentManager content = Game.Content;

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Font = content.Load<SpriteFont>("font/lofi_font");

            loadSpriteDictionary(content);

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
        }

        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }

        public override void Update(GameTime gameTime)
        {
            input.Update(gameTime);

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            screensToUpdate.Clear();

            foreach (GameScreen screen in screens)
                screensToUpdate.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;

            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];
                screensToUpdate.Remove(screensToUpdate[screensToUpdate.Count - 1]);
                // Update the screen.
                screen.Update(gameTime);

                if (screen.currentScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);

                        otherScreenHasFocus = true;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                if (screen.currentScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }

        public void AddScreen(GameScreen screen)
        {
            screen.screenManager = this;

            if (isInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (isInitialized)
            {
                screen.UnloadContent();
            }

            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }

        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }

        // *** add new graphics here
        private void loadSpriteDictionary(ContentManager content)
        {
            SpriteDict = new Dictionary<string, Texture2D>();

            SpriteDict.Add("bandit", content.Load<Texture2D>("entity/char_bandit"));
        }
    }
}
