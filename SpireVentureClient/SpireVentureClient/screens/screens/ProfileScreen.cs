using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpireVenture.screens.framework;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization.Formatters.Binary;
using SpireVenture.managers;
using System.Threading;
using Util.util;

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
        private bool newProfileActive = false;
        private StringBuilder keyboardInput;
        private KeyboardStringBuilder profileStringBuilder;
        private string[] profileFiles;

        public ProfileScreen(GameScreen parentScreen)
        {
            keyboardInput = new StringBuilder();
            profileStringBuilder = new KeyboardStringBuilder();

            ParentScreen = parentScreen;
            ParentScreen.currentScreenState = ScreenState.Hidden;

            profileFiles = FileGrabber.findLocalProfiles();
            createProfilesMenuEntries(profileFiles);
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
                            if (newProfileActive)
                            {
                                // done typing (save it)
                                string newProfileName = keyboardInput.ToString();
                                if (profileFiles != null && profileFiles.Length > 0)
                                {
                                    if (doesProfileExist(newProfileName))
                                    {
                                        screenManager.AddScreen(new PopUpWarningScreen(this, StringConstants.DuplicateProfile));
                                    }
                                    else
                                    {
                                        profileFiles.Concat(new string[] { newProfileName });
                                        FileGrabber.createNewProfile(newProfileName);
                                        profileEntries.Add(new MenuEntry(newProfileName));
                                    }
                                }
                                else
                                {
                                    profileFiles = new string[] { newProfileName };
                                    FileGrabber.createNewProfile(newProfileName);
                                    profileEntries.Add(new MenuEntry(newProfileName));
                                }
                                keyboardInput.Clear();
                                newProfileActive = false;
                            }
                            else if (!newProfileActive)
                            {
                                // start taking keystrokes
                                newProfileActive = true;
                            }
                            break;
                        case (int)ProfileEntry.Cancel:
                            screenManager.RemoveScreen(this);
                            ParentScreen.currentScreenState = ScreenState.Active;
                            break;
                    }
                }
                else if (onProfiles)
                {
                    NetworkManager.Instance.SingleplayerStart();

                    // time to send off profile name and "local" keyword
                    UsernameKeywordComboPacket packet = new UsernameKeywordComboPacket();
                    packet.username = Path.GetFileNameWithoutExtension(profileFiles[selectedProfileEntry]);
                    packet.keyword = "local";
                    Thread.Sleep(1000);
                    NetworkManager.Instance.SendData(packet);

                    // stupid thing so we can stay at the same handshake process as multiplayer
                    while (NetworkManager.Instance.Verified.Equals("")) {}

                    //TODO D: get Server data for singleplayer so we can start game (before screens)
                    
                    screenManager.AddScreen(new MainGameScreen());
                    screenManager.RemoveScreen(this);
                }
            }
            if (input.IsNewKeyPress(Keys.Escape))
            {
                if (newProfileActive)
                {
                    keyboardInput.Clear();
                    newProfileActive = false;
                }
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

            if (newProfileActive)
            {
                profileStringBuilder.Process(Keyboard.GetState(), gameTime, keyboardInput);
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

            if (newProfileActive)
            {
                String newtxt = keyboardInput.ToString() + "_";
                spriteBatch.DrawString(font, newtxt, new Vector2(150, graphics.Viewport.Height - 20 - (30 * (menuEntries.Count + (int)ProfileEntry.New))), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }

        private bool doesProfileExist(string profileName)
        {
            if (profileFiles.Length > 0)
            {
                foreach (string filename in profileFiles)
                {
                    if (profileName.Equals(Path.GetFileNameWithoutExtension(filename)))
                        return true;
                }
            }
            return false;
        }

        private void createProfilesMenuEntries(string[] profileFiles)
        {
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
