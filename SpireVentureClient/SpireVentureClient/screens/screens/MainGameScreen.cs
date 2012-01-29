using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpireVenture.screens.framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpireVenture.managers;
using Util.util;
using Microsoft.Xna.Framework.Input;

namespace SpireVenture.screens.screens
{
    /* This is the main game screen. hurf */

    //TODO B: figure out way to spawn player without jarring load of players (perhaps fade in?)

    class MainGameScreen : GameScreen
    {

        private StringBuilder keyboardInput;
        private KeyboardStringBuilder keyboardStringBuilder;
        private bool IsTypingMessage = false;

        public MainGameScreen()
        {
            keyboardInput = new StringBuilder();
            keyboardStringBuilder = new KeyboardStringBuilder();
        }

        public override void LoadContent() { }

        public override void HandleInput(InputState input) 
        {
            if (input.IsNewKeyPress(Keys.Enter))
            {
                if (!IsTypingMessage)
                    IsTypingMessage = true;
                else if (IsTypingMessage)
                {
                    IsTypingMessage = false;
                    string chat = keyboardInput.ToString();
                    ChatMessagePacket msgPacket = new ChatMessagePacket();
                    msgPacket.message = chat;
                    NetworkManager.Instance.SendReliableData(msgPacket);
                    keyboardInput.Clear();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // handle chat string building
            if (IsTypingMessage)
            {
                keyboardStringBuilder.Process(Keyboard.GetState(), gameTime, keyboardInput);
            }

            ChatManager.Instance.updateQueue(gameTime.ElapsedGameTime.Milliseconds);  // refresh for chat in game
            NetworkManager.Instance.CheckForNewMessages();  // get new packets
            // send new packets
            // handle updating screen entities

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = screenManager.GraphicsDevice;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            spriteBatch.DrawString(font, "Game Screen", new Vector2(graphics.Viewport.Width / 2, 40), Color.White, 0, font.MeasureString("Game Screen") / 2, 2f, SpriteEffects.None, 0);

            // draw messages
            List<ChatMessage> msgList = ChatManager.Instance.getTopMessagesToDisplay();
            if (msgList != null)
            {
                int n = 2;
                foreach (ChatMessage msg in msgList)
                {
                    spriteBatch.DrawString(font, msg.getChatString(), new Vector2(5, graphics.Viewport.Height - (15*n)), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
                    n++;
                }
            }

            // draw typing of message
            if (IsTypingMessage)
                spriteBatch.DrawString(font, keyboardInput.ToString() + "_", new Vector2(5, graphics.Viewport.Height - 15), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}
