using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpireVenture.screens.framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SpireVenture.screens.screens
{
    class PopUpWarningScreen : GameScreen
    {
        private GameScreen ParentScreen;
        private Texture2D backgroundTexture;
        private string Message = "";

        public PopUpWarningScreen(GameScreen parentScreen, string message)
        {
            ParentScreen = parentScreen;
            Message = message;
        }

        public override void LoadContent()
        {
            backgroundTexture = screenManager.Game.Content.Load<Texture2D>("screen/ipinput_bg");
        }

        public override void HandleInput(InputState input)
        {
            if (input.IsNewKeyPress(Keys.Enter) || input.IsNewKeyPress(Keys.Escape) || input.IsNewKeyPress(Keys.Space))
            {
                screenManager.RemoveScreen(this);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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

            // check message too long, need to split it in half.
            if (font.MeasureString(Message).X > 200)
            {
                string[] words = Message.Split(' ');
                int half = (int)Math.Floor((double)words.Length / 2);
                string firsthalf = StringSplitter(words, 0, half - 1);
                string secondhalf = StringSplitter(words, half, words.Length - half - 1);
                spriteBatch.DrawString(font, firsthalf, new Vector2(graphics.Viewport.Width / 2, midscreen - 10), Color.White, 0, font.MeasureString(firsthalf) / 2, 2f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, secondhalf, new Vector2(graphics.Viewport.Width / 2, midscreen + 10), Color.White, 0, font.MeasureString(secondhalf) / 2, 2f, SpriteEffects.None, 0);
            }
            else 
            {
                spriteBatch.DrawString(font, Message, new Vector2(graphics.Viewport.Width / 2, midscreen + 10), Color.White, 0, font.MeasureString(Message) / 2, 2f, SpriteEffects.None, 0);
            }
            
            spriteBatch.End();
        }

        private string StringSplitter(string[] source, int startPos, int length)
        {
            StringBuilder builder = new StringBuilder();
            for (int x = 0; x <= length; x++)
            {
                builder.Append(source[startPos + x]).Append(' ');
            }
            return builder.ToString();
        }
    }
}
