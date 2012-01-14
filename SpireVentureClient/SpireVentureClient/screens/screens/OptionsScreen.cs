﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpireVenture.screens.framework;
using SpireVenture.util;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SpireVenture.screens.screens
{
    class OptionsScreen : GameScreen
    {
        private StringBuilder usernamekeyboardInput, keywordkeyboardInput;
        private KeyboardStringBuilder usernameStringBuilder, keywordStringBuilder;
        private GameScreen ParentScreen;
        private const string titleText = "Options";

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;

        private bool usernameActive = false;
        private bool keywordActive = false;

        public OptionsScreen(GameScreen parentScreen)
        {
            usernamekeyboardInput = new StringBuilder();
            keywordkeyboardInput = new StringBuilder();
            usernameStringBuilder = new KeyboardStringBuilder();
            keywordStringBuilder = new KeyboardStringBuilder();

            loadSavedOptions();

            ParentScreen = parentScreen;
            ParentScreen.currentScreenState = ScreenState.Hidden;

            menuEntries.Add(new MenuEntry("Username:"));
            menuEntries.Add(new MenuEntry("Keyword:"));
            menuEntries.Add(new MenuEntry("Exit"));
            menuEntries[0].Active = true;
        }

        private void loadSavedOptions()
        {
            usernamekeyboardInput.Append(ClientOptions.Instance.Username);
            keywordkeyboardInput.Append(ClientOptions.Instance.Keyword);
        }

        public override void LoadContent() {}

        public override void HandleInput(InputState input)
        {
            if (input.IsNewKeyPress(Keys.Up) && !usernameActive && !keywordActive)
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }
            if (input.IsNewKeyPress(Keys.Down) && !usernameActive && !keywordActive)
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }
            if (input.IsNewKeyPress(Keys.Enter))
            {
                switch (selectedEntry)
                {
                    case (int)OptionsEntry.Username:
                        if (usernameActive)
                        {
                            // done typing
                            ClientOptions.Instance.setUsername(usernamekeyboardInput.ToString());
                            usernameActive = false;
                        }
                        else if (!usernameActive)
                        {
                            // start taking keystrokes
                            usernameActive = true;
                        }
                        break;
                    case (int)OptionsEntry.Keyword:
                        if (keywordActive)
                        {
                            // done typing
                            ClientOptions.Instance.setKeyword(keywordkeyboardInput.ToString());
                            keywordActive = false;
                        }
                        else if (!keywordActive)
                        {
                            // start taking keystrokes
                            keywordActive = true;
                        }
                        break;
                    case (int)OptionsEntry.Exit:
                        screenManager.RemoveScreen(this);
                        ParentScreen.currentScreenState = ScreenState.Active;
                        break;
                }
            }
            if (input.IsNewKeyPress(Keys.Escape))
            {
                if (usernameActive)
                {
                    usernamekeyboardInput.Clear();
                    usernamekeyboardInput.Append(ClientOptions.Instance.Username);
                    usernameActive = false;
                }
                else if (keywordActive)
                {
                    keywordkeyboardInput.Clear();
                    keywordkeyboardInput.Append(ClientOptions.Instance.Keyword);
                    keywordActive = false;
                }
                else
                {
                    screenManager.RemoveScreen(this);
                    ParentScreen.currentScreenState = ScreenState.Active;
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

            if (usernameActive)
                usernameStringBuilder.Process(Keyboard.GetState(), gameTime, usernamekeyboardInput);
            if (keywordActive)
                keywordStringBuilder.Process(Keyboard.GetState(), gameTime, keywordkeyboardInput);
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
            
            // draw options
            int x = 0;
            foreach (MenuEntry entry in menuEntries)
            {
                spriteBatch.DrawString(font, entry.Text, new Vector2(50, 140 + x), entry.getColor(), 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
                x += 30;
            }

            String usertxt, keywtxt;
            usertxt = (usernameActive) ? usernamekeyboardInput.ToString() + "_" : usernamekeyboardInput.ToString();
            keywtxt = (keywordActive) ? keywordkeyboardInput.ToString() + "_" : keywordkeyboardInput.ToString();

            spriteBatch.DrawString(font, usertxt, new Vector2(150, 140), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, keywtxt, new Vector2(150, 140 + 30), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}
