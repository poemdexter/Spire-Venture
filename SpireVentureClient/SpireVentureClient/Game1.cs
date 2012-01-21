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
using SpireVenture.managers;
using SpireVenture.screens.screens;
using Util.util;

namespace SpireVentureClient
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        int windowWidth, windowHeight;

        public void hey() { }
        public Game1()
        {
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);

            // TODO: Fix so that we set option correctly
            ClientOptions.Instance.initialize();
            int h = ClientOptions.Instance.ResolutionHeight;
            int w = ClientOptions.Instance.ResolutionWidth;
            bool full = ClientOptions.Instance.Fullscreen;

            if (h > 0 && w > 0)
            {
                windowWidth = w;
                windowHeight = h;
                graphics.IsFullScreen = false;
            }
            else
            {
                windowWidth = 800;
                windowHeight = 600;
                graphics.IsFullScreen = false;
            }

            graphics.PreferredBackBufferWidth = windowWidth;
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.IsFullScreen = full;
            

            screenManager = new ScreenManager(this, graphics);

            Components.Add(screenManager);

            // Activate the first screens
            screenManager.AddScreen(new MainMenuScreen());
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent() { }
        protected override void UnloadContent() { }
    }
}
