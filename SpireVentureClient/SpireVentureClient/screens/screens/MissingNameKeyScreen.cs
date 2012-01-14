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
    class MissingNameKeyScreen : GameScreen
    {
        private GameScreen ParentScreen;
        Texture2D backgroundTexture;

        public MissingNameKeyScreen(GameScreen parentScreen)
        {
            ParentScreen = parentScreen;
        }

        public override void LoadContent()
        {
            backgroundTexture = screenManager.Game.Content.Load<Texture2D>("screen/ipinput_bg");
        }

        public override void HandleInput(InputState input)
        {
            if (input.IsNewKeyPress(Keys.Enter) || input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.Escape))
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
            float scale = 4f;
            int midscreen = graphics.Viewport.Height / 2;
            String message1 = "Please enter username and keyword in";
            String message2 = "Options before playing Multiplayer.";

            spriteBatch.Draw(backgroundTexture, new Vector2(graphics.Viewport.Width / 2, midscreen), backgroundTexture.Bounds, Color.White, 0f, new Vector2(backgroundTexture.Width / 2, backgroundTexture.Height / 2), scale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, message1, new Vector2(graphics.Viewport.Width / 2, midscreen - 10), Color.White, 0, font.MeasureString(message1) / 2, 2f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, message2, new Vector2(graphics.Viewport.Width / 2, midscreen + 10), Color.White, 0, font.MeasureString(message2) / 2, 2f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
