using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpireVenture.screens.framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using SpireVenture.managers;

namespace SpireVenture.screens.screens
{
    class MainGameOptionsScreen : GameScreen
    {
        private GameScreen ParentScreen;
        private Texture2D backgroundTexture;
        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;

        public MainGameOptionsScreen(GameScreen parentScreen)
        {
            ParentScreen = parentScreen;
            menuEntries.Add(new MenuEntry("Exit to Main Menu"));
            menuEntries.Add(new MenuEntry("Exit to Desktop"));
            menuEntries.Add(new MenuEntry("Cancel"));
            menuEntries[0].Active = true;
        }

        public override void LoadContent()
        {
            backgroundTexture = screenManager.Game.Content.Load<Texture2D>("screen/ipinput_bg");
        }

        public override void HandleInput(InputState input)
        {
            if (input.IsNewKeyPress(Keys.Up))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }
            if (input.IsNewKeyPress(Keys.Down))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }
            if (input.IsNewKeyPress(Keys.Enter))
            {
                switch (selectedEntry)
                {
                    case (int)MainGameOptionsEntry.ExitMain:
                        NetworkManager.Instance.StopSingleplayerServer();
                        screenManager.AddScreen(new MainMenuScreen());
                        this.ExitScreen();
                        ParentScreen.ExitScreen();
                        break;
                    case (int)MainGameOptionsEntry.ExitDesktop:
                        NetworkManager.Instance.StopSingleplayerServer();
                        screenManager.Game.Exit();
                        break;
                    case (int)MainGameOptionsEntry.Cancel:
                        this.ExitScreen();
                        break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            for (int i = 0; i < menuEntries.Count; i++)
            {
                menuEntries[i].Active = (i == selectedEntry) ? true : false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = screenManager.GraphicsDevice;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

            // draw background
            float bgscale = 4f;
            int midscreen = graphics.Viewport.Height / 2;

            spriteBatch.Draw(backgroundTexture, new Vector2(graphics.Viewport.Width / 2, midscreen), backgroundTexture.Bounds, Color.White, 0f, new Vector2(backgroundTexture.Width / 2, backgroundTexture.Height / 2), bgscale, SpriteEffects.None, 0f);

            // draw options
            int x = 0;
            foreach (MenuEntry entry in menuEntries)
            {
                spriteBatch.DrawString(font, entry.Text, new Vector2(graphics.Viewport.Width / 2 - 20, midscreen - 30 + x), entry.getColor(), 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
                x += 30;
            }

            spriteBatch.End();
        }
    }
}
