namespace LR1TrackEditor
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using XnaButtonState = Microsoft.Xna.Framework.Input.ButtonState;
    using XnaMouseState = Microsoft.Xna.Framework.Input.MouseState;

    /// <summary>
    /// Provides mouse state via Win32 APIs for MonoGame embedded in WinForms,
    /// where Mouse.GetState() doesn't work because the game window is hidden
    /// and never receives WM_LBUTTONDOWN/WM_RBUTTONDOWN messages.
    /// Scroll wheel is captured via an application-level message filter.
    /// </summary>
    public static class MouseHelper
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        private const int VK_LBUTTON = 0x01;
        private const int VK_RBUTTON = 0x02;
        private const int VK_MBUTTON = 0x04;

        private static int scrollWheelValue = 0;

        /// <summary>
        /// Call once at startup to install the scroll wheel message filter.
        /// </summary>
        public static void Install()
        {
            Application.AddMessageFilter(new ScrollWheelFilter());
        }

        /// <summary>
        /// Fallback for direct scroll delta injection.
        /// </summary>
        public static void AddScrollDelta(int delta)
        {
            scrollWheelValue += delta;
        }

        private static bool IsDown(int vKey) => (GetAsyncKeyState(vKey) & 0x8000) != 0;

        public static XnaMouseState GetState(IntPtr windowHandle)
        {
            GetCursorPos(out POINT screenPt);
            POINT clientPt = screenPt;
            ScreenToClient(windowHandle, ref clientPt);

            return new XnaMouseState(
                clientPt.X,
                clientPt.Y,
                scrollWheelValue,
                IsDown(VK_LBUTTON) ? XnaButtonState.Pressed : XnaButtonState.Released,
                IsDown(VK_MBUTTON) ? XnaButtonState.Pressed : XnaButtonState.Released,
                IsDown(VK_RBUTTON) ? XnaButtonState.Pressed : XnaButtonState.Released,
                XnaButtonState.Released,
                XnaButtonState.Released
            );
        }

        public static void SetPosition(IntPtr windowHandle, int x, int y)
        {
            POINT pt = new POINT { X = x, Y = y };
            ClientToScreen(windowHandle, ref pt);
            SetCursorPos(pt.X, pt.Y);
        }

        private class ScrollWheelFilter : IMessageFilter
        {
            private const int WM_MOUSEWHEEL = 0x020A;

            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == WM_MOUSEWHEEL)
                {
                    int delta = (short)((m.WParam.ToInt64() >> 16) & 0xFFFF);
                    scrollWheelValue += delta;
                }
                return false;
            }
        }
    }
}
