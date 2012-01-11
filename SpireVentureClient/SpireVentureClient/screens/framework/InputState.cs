using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SpireVenture.screens.framework
{
    public class InputState
    {
        public KeyboardState CurrentKeyboardState;
        public KeyboardState LastKeyboardState;
        private int keyboardElapsedTime;

        public InputState()
        {
            CurrentKeyboardState = new KeyboardState();
            LastKeyboardState = new KeyboardState();
            keyboardElapsedTime = 0;
        }

        public void Update(GameTime gameTime)
        {
            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            keyboardElapsedTime -= gameTime.ElapsedGameTime.Milliseconds;
        }

        public bool IsNewKeyPress(Keys key)
        {
            if (keyboardElapsedTime <= 0)
            {
                if (CurrentKeyboardState.IsKeyDown(key))
                {
                    keyboardElapsedTime = 200;
                    return true;
                }
            }
            return false;
        }
    }
}
