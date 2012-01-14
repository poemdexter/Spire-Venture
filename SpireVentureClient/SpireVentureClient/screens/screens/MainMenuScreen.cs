using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpireVenture.screens.framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpireVenture.screens.screens
{
    class MainMenuScreen : GameScreen
    {
        private const string titleText = "Spire Venture 0.1a";

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;

        public MainMenuScreen()
        {
            menuEntries.Add(new MenuEntry("Singleplayer"));
            menuEntries.Add(new MenuEntry("Multiplayer"));
            menuEntries.Add(new MenuEntry("Options"));
            menuEntries.Add(new MenuEntry("Exit"));
            menuEntries[0].Active = true;
        }

        // handle key presses
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
                    case (int)MainMenuEntry.Singleplayer:
                        // start local server and join
                        break;
                    case (int)MainMenuEntry.Multiplayer:
                        screenManager.AddScreen(new IPInputScreen(this));
                        break;
                    case (int)MainMenuEntry.Options:
                        screenManager.AddScreen(new OptionsScreen(this));
                        break;
                    case (int)MainMenuEntry.Exit:
                        screenManager.Game.Exit();
                        break;
                }
            }
        }

        // update menu entry to signal which is selected
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

            // draw title
            spriteBatch.DrawString(font, titleText, new Vector2(graphics.Viewport.Width / 2, 100), Color.White, 0, font.MeasureString(titleText) / 2, 4f, SpriteEffects.None, 0);

            // draw options
            int x = 0;
            foreach (MenuEntry entry in menuEntries)
            {
                spriteBatch.DrawString(font, entry.Text, new Vector2(50, graphics.Viewport.Height - 140 + x), entry.getColor(), 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
                x += 30;
            }

            spriteBatch.End();
        }
    }
}
