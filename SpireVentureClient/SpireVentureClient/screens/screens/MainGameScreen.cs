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
using Entities.framework;
using Entities.components;
using Lidgren.Network;

namespace SpireVenture.screens.screens
{
    /* This is the main game screen. hurf */

    //TODO F: figure out way to spawn player without jarring load of players (perhaps fade in?)

    class MainGameScreen : GameScreen
    {

        private StringBuilder keyboardInput;
        private KeyboardStringBuilder keyboardStringBuilder;
        private bool IsTypingMessage = false;
        Dictionary<string, Texture2D> spriteDict;

        double inputNow = 0;
        double inputNextUpdate = NetTime.Now;

        double updateNow = 0;
        double updateNextUpdate = NetTime.Now;

        private Inputs inputs;

        public MainGameScreen(string username)
        {
            keyboardInput = new StringBuilder();
            keyboardStringBuilder = new KeyboardStringBuilder();
            spriteDict = new Dictionary<string, Texture2D>();
            ClientGameManager.Instance.setUsername(username);
        }

        public override void LoadContent() 
        {
            spriteDict = screenManager.SpriteDict;
        }

        public override void HandleInput(InputState input) 
        {
            inputs.resetStates();

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
            if (input.IsNewKeyPress(Keys.Escape))
            {
                if (IsTypingMessage)
                {
                    IsTypingMessage = false;
                    keyboardInput.Clear();
                }
            }
            if (!IsTypingMessage)
            {
                inputs.Up = (input.CurrentKeyboardState.IsKeyDown(Keys.Up)) ? true : false;
                inputs.Down = (input.CurrentKeyboardState.IsKeyDown(Keys.Down)) ? true : false;
                inputs.Left = (input.CurrentKeyboardState.IsKeyDown(Keys.Left)) ? true : false;
                inputs.Right = (input.CurrentKeyboardState.IsKeyDown(Keys.Right)) ? true : false;
                inputs.Space = (input.CurrentKeyboardState.IsKeyDown(Keys.Space)) ? true : false;
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

            updateNow = NetTime.Now;
            if (updateNow > updateNextUpdate)
            {
                NetworkManager.Instance.CheckForNewMessages();  // get new packets
                updateNextUpdate += (1.0 / GameConstants.CLIENT_UPDATE_RATE);
            }

            inputNow = NetTime.Now;
            if (inputNow > inputNextUpdate)
            {
                ClientGameManager.Instance.PredictPlayerFromInput(inputs); 
                NetworkManager.Instance.HandleOutgoingMessages(inputs); // send new packets
                inputNextUpdate += (1.0 / GameConstants.CLIENT_INPUT_RATE);
            }
           

            // handle updating screen entities

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = screenManager.GraphicsDevice;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            spriteBatch.DrawString(font, "Game Screen", new Vector2(graphics.Viewport.Width / 2, 40), Color.White, 0, font.MeasureString("Game Screen") / 2, 2f, SpriteEffects.None, 0);

            // draw players
            foreach (Entity player in ClientGameManager.Instance.PlayerEntities.Values.ToList())
            {
                Vector2 pos = (player.GetComponent("Position") as Position).Vector2Pos;
                spriteBatch.Draw(spriteDict["bandit"], pos, spriteDict["bandit"].Bounds, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
            }

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
