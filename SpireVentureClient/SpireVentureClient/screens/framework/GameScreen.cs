using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpireVenture.managers;
using Microsoft.Xna.Framework;

namespace SpireVenture.screens.framework
{
    public enum ScreenState
    {
        Active,
        Hidden
    }

    public abstract class GameScreen
    {
        // get the screen manager this screen belongs to
        public ScreenManager screenManager { get; set; }

        public ScreenState currentScreenState = ScreenState.Active;

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void HandleInput(InputState input) { }
        public virtual void Draw(GameTime gameTime) { }
        public virtual void Update(GameTime gameTime) { }

        public void ExitScreen()
        {
            screenManager.RemoveScreen(this);
        }
    }
}
