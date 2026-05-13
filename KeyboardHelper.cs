namespace LR1TrackEditor
{
    using Microsoft.Xna.Framework.Input;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides keyboard state via GetAsyncKeyState for MonoGame embedded in WinForms,
    /// where Keyboard.GetState() doesn't work because the game window is hidden.
    /// </summary>
    public static class KeyboardHelper
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private static bool IsDown(int vKey) => (GetAsyncKeyState(vKey) & 0x8000) != 0;

        public static KeyboardState GetState()
        {
            var keys = new System.Collections.Generic.List<Keys>();

            if (IsDown(0x57)) keys.Add(Keys.W);
            if (IsDown(0x41)) keys.Add(Keys.A);
            if (IsDown(0x53)) keys.Add(Keys.S);
            if (IsDown(0x44)) keys.Add(Keys.D);
            if (IsDown(0x45)) keys.Add(Keys.E);
            if (IsDown(0x51)) keys.Add(Keys.Q);
            if (IsDown(0x52)) keys.Add(Keys.R);
            if (IsDown(0x54)) keys.Add(Keys.T);
            if (IsDown(0x56)) keys.Add(Keys.V);
            if (IsDown(0x20)) keys.Add(Keys.Space);
            if (IsDown(0xA0)) keys.Add(Keys.LeftShift);
            if (IsDown(0xA2)) keys.Add(Keys.LeftControl);
            if (IsDown(0xA3)) keys.Add(Keys.RightControl);
            if (IsDown(0xA4)) keys.Add(Keys.LeftAlt);
            if (IsDown(0xA5)) keys.Add(Keys.RightAlt);
            if (IsDown(0x0D)) keys.Add(Keys.Enter);
            if (IsDown(0x1B)) keys.Add(Keys.Escape);
            if (IsDown(0x2E)) keys.Add(Keys.Delete);
            if (IsDown(0x4F)) keys.Add(Keys.O);
            if (IsDown(0x70)) keys.Add(Keys.F1);
            if (IsDown(0x71)) keys.Add(Keys.F2);

            return new KeyboardState(keys.ToArray());
        }
    }
}
