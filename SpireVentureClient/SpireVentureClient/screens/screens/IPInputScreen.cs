using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpireVenture.screens.framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using SpireVenture.util;

namespace SpireVenture.screens.screens
{
    class IPInputScreen : GameScreen
    {
        private const string titleText = "Enter Server IP:";
        private StringBuilder keyboardInput;
        private KeyboardStringBuilder keyboardStringBuilder;
        private GameScreen ParentScreen;
        Texture2D backgroundTexture;

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
            if (input.IsNewKeyPress(Keys.Enter) && keyboardInput.Length > 0)
            {
                // accept ip and start connect
                
            }
            if (input.IsNewKeyPress(Keys.Escape))
            {
                screenManager.RemoveScreen(this);
            }
        }

        // update menu entry to signal which is selected
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            keyboardStringBuilder.Process(Keyboard.GetState(), gameTime, keyboardInput);
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
            spriteBatch.Draw(backgroundTexture, new Vector2(graphics.Viewport.Width / 2, midscreen), backgroundTexture.Bounds, Color.White, 0f, new Vector2(backgroundTexture.Width / 2, backgroundTexture.Height / 2), scale, SpriteEffects.None, 0f);
            // draw title
            spriteBatch.DrawString(font, titleText, new Vector2(graphics.Viewport.Width / 2, midscreen - 15), Color.White, 0, font.MeasureString(titleText) / 2, 4f, SpriteEffects.None, 0);
            // draw current ip as typed
            spriteBatch.DrawString(font, keyboardInput, new Vector2(graphics.Viewport.Width / 2, midscreen + 30), Color.White, 0, font.MeasureString(keyboardInput) / 2, 4f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
