using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpireVenture.screens.framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using SpireVenture.util;
using SpireVenture.managers;

namespace SpireVenture.screens.screens
{
    enum ConnectionStatus
    {
        NotStarted,
        Connecting,
        Discovered,
        Connected,
        NotFound,
        Stopped
    }

    class IPInputScreen : GameScreen
    {
        private const string titleText = "Enter Server IP:";
        private StringBuilder keyboardInput;
        private KeyboardStringBuilder keyboardStringBuilder;
        private GameScreen ParentScreen;
        Texture2D backgroundTexture;

        // connection vars
        private double connectingStart = 0;
        private double connectingElapsed = 0;
        private double timeout = 10;

        ConnectionStatus currentConnectionStatus = ConnectionStatus.NotStarted;

        public IPInputScreen(GameScreen parentScreen)
        {
            keyboardInput = new StringBuilder();
            keyboardStringBuilder = new KeyboardStringBuilder();
            ParentScreen = parentScreen;
        }

        public override void LoadContent()
        {
            backgroundTexture = screenManager.Game.Content.Load<Texture2D>("screen/ipinput_bg");
        }

        public override void HandleInput(InputState input)
        {
            if (input.IsNewKeyPress(Keys.Enter) && keyboardInput.Length > 0 && currentConnectionStatus == ConnectionStatus.NotStarted)
            {
                // accept ip and start connect ***
                String ip = keyboardInput.ToString();
                NetworkManager.Instance.Start(ip);
                currentConnectionStatus = ConnectionStatus.Connecting;
            }
            if (input.IsNewKeyPress(Keys.Escape))
            {
                if (currentConnectionStatus != ConnectionStatus.NotStarted)
                {
                    // didn't find, just clear message
                    if (currentConnectionStatus == ConnectionStatus.NotFound || currentConnectionStatus == ConnectionStatus.Stopped)
                    {
                        currentConnectionStatus = ConnectionStatus.NotStarted;
                    }
                    else
                    {
                        // stop trying to connect immediately if were trying
                        currentConnectionStatus = ConnectionStatus.Stopped;
                    }
                }
                else
                {
                    screenManager.RemoveScreen(this);
                }
            }
        }

        // update menu entry to signal which is selected
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (currentConnectionStatus == ConnectionStatus.NotStarted)
            {
                keyboardStringBuilder.Process(Keyboard.GetState(), gameTime, keyboardInput);
            }

            if (currentConnectionStatus == ConnectionStatus.Connecting)
            {
                if (connectingStart == 0)
                {
                    connectingStart = gameTime.TotalGameTime.TotalSeconds;
                }

                connectingElapsed = gameTime.TotalGameTime.TotalSeconds - connectingStart;

                if (NetworkManager.Instance.Discovered)
                    currentConnectionStatus = ConnectionStatus.Discovered;
                else if (connectingElapsed > timeout)
                {
                    currentConnectionStatus = ConnectionStatus.NotFound;
                    connectingStart = 0;
                }
            }
            else if (currentConnectionStatus == ConnectionStatus.Discovered)
            {
                // time to send off username/keyword
                UsernameKeywordComboPacket packet = new UsernameKeywordComboPacket();
                packet.username = ClientOptions.Instance.Username;
                packet.keyword = ClientOptions.Instance.Keyword;
                NetworkManager.Instance.SendData(packet);

                // ***temp just to stop it
                currentConnectionStatus = ConnectionStatus.NotFound;
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
            int midscreen = graphics.Viewport.Height / 2;
            String message;

            spriteBatch.Draw(backgroundTexture, new Vector2(graphics.Viewport.Width / 2, midscreen), backgroundTexture.Bounds, Color.White, 0f, new Vector2(backgroundTexture.Width / 2, backgroundTexture.Height / 2), scale, SpriteEffects.None, 0f);
            switch (currentConnectionStatus)
            {
                case (ConnectionStatus.NotStarted):
                    // draw title
                    spriteBatch.DrawString(font, titleText, new Vector2(graphics.Viewport.Width / 2, midscreen - 15), Color.White, 0, font.MeasureString(titleText) / 2, 4f, SpriteEffects.None, 0);
                    // draw current ip as typed
                    spriteBatch.DrawString(font, keyboardInput, new Vector2(graphics.Viewport.Width / 2, midscreen + 30), Color.White, 0, font.MeasureString(keyboardInput) / 2, 4f, SpriteEffects.None, 0);
                    break;
                case (ConnectionStatus.Connecting):
                    message = "Connecting...";
                    spriteBatch.DrawString(font, message, new Vector2(graphics.Viewport.Width / 2, midscreen), Color.White, 0, font.MeasureString(message) / 2, 4f, SpriteEffects.None, 0);
                    break;
                case (ConnectionStatus.Connected):
                    message = "Connected!";
                    spriteBatch.DrawString(font, message, new Vector2(graphics.Viewport.Width / 2, midscreen), Color.White, 0, font.MeasureString(message) / 2, 4f, SpriteEffects.None, 0);
                    break;
                case (ConnectionStatus.NotFound):
                    message = "Server not found.";
                    spriteBatch.DrawString(font, message, new Vector2(graphics.Viewport.Width / 2, midscreen), Color.White, 0, font.MeasureString(message) / 2, 4f, SpriteEffects.None, 0);
                    break;
                case (ConnectionStatus.Stopped):
                    message = "Connection cancelled.";
                    spriteBatch.DrawString(font, message, new Vector2(graphics.Viewport.Width / 2, midscreen), Color.White, 0, font.MeasureString(message) / 2, 4f, SpriteEffects.None, 0);
                    break;

            }
            spriteBatch.End();
        }
    }
}
