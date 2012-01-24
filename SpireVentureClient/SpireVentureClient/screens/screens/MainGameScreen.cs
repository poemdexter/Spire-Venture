using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpireVenture.screens.framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpireVenture.managers;

namespace SpireVenture.screens.screens
{
    /* This is the main game screen. hurf */

    class MainGameScreen : GameScreen
    {
        public MainGameScreen() { }

        public override void LoadContent() { }

        public override void HandleInput(InputState input) { }

        public override void Update(GameTime gameTime)
        {
            
            NetworkManager.Instance.CheckForNewMessages();
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = screenManager.GraphicsDevice;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            spriteBatch.DrawString(font, "Game Screen", new Vector2(graphics.Viewport.Width / 2, 40), Color.White, 0, font.MeasureString("Game Screen") / 2, 2f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
