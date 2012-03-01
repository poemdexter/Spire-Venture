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
                    if (!chat.Equals(""))  // no empty strings in chat
                    {
                        ChatMessagePacket msgPacket = new ChatMessagePacket();
                        msgPacket.message = chat;
                        NetworkManager.Instance.SendReliableData(msgPacket);
                        keyboardInput.Clear();
                    }
                }
            }
            if (input.IsNewKeyPress(Keys.Escape))
            {
                if (IsTypingMessage)
                {
                    IsTypingMessage = false;
                    keyboardInput.Clear();
                }
                else 
                {
                    screenManager.AddScreen(new MainGameOptionsScreen(this));
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
                // *** these two statements must be together or else we run risk of
                // *** destroying our input prediction!!!
                ClientGameManager.Instance.PredictPlayerFromInput(inputs); // predict our position
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

            // get our transformation matrix so we can center camera on player
            ClientGameManager.Instance.CheckForPlayerEntity(ClientGameManager.Instance.Username);
            Vector2 lerpPos = ClientGameManager.Instance.LerpPlayer(ClientGameManager.Instance.PlayerEntities[ClientGameManager.Instance.Username]);
            Matrix transformMatrix = Matrix.CreateTranslation(-lerpPos.X + graphics.Viewport.Width / 2,
                                                              -lerpPos.Y + graphics.Viewport.Height / 2,
                                                              1);

            // *** transformation spritebatch.  everything drawn here will be in relation to the player as the center of screen
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, transformMatrix);

            // draw lab floor
            spriteBatch.Draw(spriteDict["lab"], Vector2.Zero, spriteDict["lab"].Bounds, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0);

            //spriteBatch.DrawString(font, "Game Screen", new Vector2(graphics.Viewport.Width / 2, 40), Color.White, 0, font.MeasureString("Game Screen") / 2, 2f, SpriteEffects.None, 0);

            // draw self
            spriteBatch.Draw(spriteDict["bandit"], lerpPos, spriteDict["bandit"].Bounds, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0);

            // draw players
            foreach (Entity player in ClientGameManager.Instance.PlayerEntities.Values.ToList())
            {
                if ((player.GetComponent("Username") as Username).UserNm != ClientGameManager.Instance.Username)
                {
                    lerpPos = ClientGameManager.Instance.LerpPlayer(player);
                    spriteBatch.Draw(spriteDict["bandit"], lerpPos, spriteDict["bandit"].Bounds, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0);
                }
            }
            spriteBatch.End();

            // *** static position spritebatch.  everything drawn here will not move when player moves.  use for UI, HUD, etc.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            // draw messages
            List<ChatMessage> msgList = ChatManager.Instance.getTopMessagesToDisplay();
            if (msgList != null)
            {
                int n = 2;
                foreach (ChatMessage msg in msgList)
                {
                    if (msg.getChatNameString().Equals(""))
                        spriteBatch.DrawString(font, msg.getChatMessageString(), new Vector2(5 + (font.MeasureString(msg.getChatNameString()).X), graphics.Viewport.Height - (15 * n)), Color.Yellow, 0, Vector2.Zero, GameConstants.CHAT_SCALE, SpriteEffects.None, 0);
                    else if (msg.getRealNameString().Equals("<<"))
                        spriteBatch.DrawString(font, msg.getChatMessageString(), new Vector2(5 + (font.MeasureString(msg.getChatNameString()).X), graphics.Viewport.Height - (15 * n)), Color.White, 0, Vector2.Zero, GameConstants.CHAT_SCALE, SpriteEffects.None, 0);
                    else
                    {
                        spriteBatch.DrawString(font, msg.getChatNameString(), new Vector2(5, graphics.Viewport.Height - (15 * n)), Color.Yellow, 0, Vector2.Zero, GameConstants.CHAT_SCALE, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, msg.getChatMessageString(), new Vector2(5 + ((font.MeasureString(msg.getChatNameString()).X * GameConstants.CHAT_SCALE)), graphics.Viewport.Height - (15 * n)), Color.White, 0, Vector2.Zero, GameConstants.CHAT_SCALE, SpriteEffects.None, 0);
                    }
                    n++;
                }
            }

            // draw typing of message
            if (IsTypingMessage)
            {
                string chat = ChatManager.Instance.getTruncatedChatInput(keyboardInput.ToString());
                spriteBatch.DrawString(font, chat, new Vector2(5, graphics.Viewport.Height - 15), Color.White, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }
    }
}
