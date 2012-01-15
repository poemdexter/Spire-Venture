using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpireVenture.screens.framework;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpireVenture.screens.screens
{
    class ProfileScreen : GameScreen
    {
        private GameScreen ParentScreen;
        List<MenuEntry> menuEntries = new List<MenuEntry>();
        List<MenuEntry> profileEntries = new List<MenuEntry>();
        int selectedMenuEntry = -1;
        int selectedProfileEntry = -1;
        private const string titleText = "Profiles";
        bool onProfiles = false, onMenu = false;

        public ProfileScreen(GameScreen parentScreen)
        {
            //usernamekeyboardInput = new StringBuilder();
            //keywordkeyboardInput = new StringBuilder();
            //usernameStringBuilder = new KeyboardStringBuilder();
            //keywordStringBuilder = new KeyboardStringBuilder();

            //loadSavedOptions();

            ParentScreen = parentScreen;
            ParentScreen.currentScreenState = ScreenState.Hidden;

            findProfiles();
            menuEntries.Add(new MenuEntry("New"));
            menuEntries.Add(new MenuEntry("Cancel"));

            if (profileEntries.Count > 0)
            {
                onProfiles = true;
                selectedProfileEntry = 0;
                profileEntries[0].Active = true;
            }
            else
            {
                onMenu = true;
                selectedMenuEntry = 0;
                menuEntries[0].Active = true;
            }
        }

        public override void LoadContent() { }

        public override void HandleInput(InputState input)
        {
            if (input.IsNewKeyPress(Keys.Up))
            {
                if (onProfiles)
                {
                    selectedProfileEntry--;

                    if (selectedProfileEntry < 0)
                    {
                        selectedProfileEntry = -1;
                        selectedMenuEntry = menuEntries.Count - 1;
                        onProfiles = false;
                        onMenu = true;
                    }
                }
                else if (onMenu)
                {
                    selectedMenuEntry--;

                    if (selectedMenuEntry < 0)
                    {
                        if (profileEntries.Count > 0)
                        {
                            selectedMenuEntry = -1;
                            selectedProfileEntry = profileEntries.Count - 1;
                            onProfiles = true;
                            onMenu = false;
                        }
                        else
                        {
                            selectedMenuEntry = menuEntries.Count - 1;
                        }
                    }
                }
            }
            if (input.IsNewKeyPress(Keys.Down))
            {
                if (onProfiles)
                {
                    selectedProfileEntry++;

                    if (selectedProfileEntry >= profileEntries.Count)
                    {
                        selectedProfileEntry = -1;
                        selectedMenuEntry = 0;
                        onProfiles = false;
                        onMenu = true;
                    }
                }
                else if (onMenu)
                {
                    selectedMenuEntry++;

                    if (selectedMenuEntry >= menuEntries.Count)
                    {
                        if (profileEntries.Count > 0)
                        {
                            selectedMenuEntry = -1;
                            selectedProfileEntry = 0;
                            onProfiles = true;
                            onMenu = false;
                        }
                        else
                        {
                            selectedMenuEntry = 0;
                        }
                    }
                }
            }
            if (input.IsNewKeyPress(Keys.Enter))
            {
                if (onMenu)
                {
                    switch (selectedMenuEntry)
                    {
                        case (int)ProfileEntry.New:
                            // create new profile
                            break;
                        case (int)ProfileEntry.Cancel:
                            screenManager.RemoveScreen(this);
                            ParentScreen.currentScreenState = ScreenState.Active;
                            break;
                    }
                }
                else if (onProfiles)
                {
                    // load selected profile *START GAME OH GOD*
                }
            }
            if (input.IsNewKeyPress(Keys.Escape))
            {
                screenManager.RemoveScreen(this);
                ParentScreen.currentScreenState = ScreenState.Active;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                menuEntries[i].Active = (i == selectedMenuEntry) ? true : false;
            }
            if (profileEntries.Count > 0)
            {
                for (int j = 0; j < profileEntries.Count; j++)
                {
                    profileEntries[j].Active = (j == selectedProfileEntry) ? true : false;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = screenManager.GraphicsDevice;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

            // draw background
            float scale = 4f;

            // draw title
            spriteBatch.DrawString(font, titleText, new Vector2(graphics.Viewport.Width / 2, 40), Color.White, 0, font.MeasureString(titleText) / 2, scale, SpriteEffects.None, 0);

            // draw profiles
            if (profileEntries.Count > 0)
            {
                int x1 = 0;
                foreach (MenuEntry entry in profileEntries)
                {
                    spriteBatch.DrawString(font, entry.Text, new Vector2(50, 150 + x1), entry.getColor(), 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
                    x1 += 30;
                }
            }

            // draw options
            int x = 0;
            foreach (MenuEntry entry in menuEntries)
            {
                spriteBatch.DrawString(font, entry.Text, new Vector2(50, graphics.Viewport.Height - 20 - (30 * menuEntries.Count) + x), entry.getColor(), 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
                x += 30;
            }

            spriteBatch.End();
        }

        private void findProfiles()
        {
            String documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            String clientPath = Path.Combine(documentsPath, "SpireVenture");

            string[] profileFiles = Directory.GetFiles(clientPath, "*.sav");

            if (profileFiles.Length > 0)
            {
                foreach (string filename in profileFiles)
                {
                    profileEntries.Add(new MenuEntry(Path.GetFileNameWithoutExtension(filename)));
                }
            }
        }
    }
}
