using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LowLevelKeyInterceptor
{
    public class LowLevelKeyboardProcEventArgs : EventArgs
    {
        public Keys KeyCode;            // virtual keycode https://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx
        public Int32 ScanCode;          // hardward scancode
        public KBDLLHOOKSTRUCTFlags Flags;
//        public Int32 time;              // message timestamp
//        public IntPtr dwExtraInfo;

        public bool KeyUp() => ((KBDLLHOOKSTRUCTFlags.LLKHF_KEYUP & this.Flags) > 0);
        public bool KeyDown() => !KeyUp();


        public bool ControlKeyState() => _keyStates.Any(kvp => CtrlKeyVirtualCodes.Contains(kvp.Key) && kvp.Value);
        public bool ShiftKeyState => _keyStates.Any(kvp => ShiftKeyVirtualCodes.Contains(kvp.Key) && kvp.Value);
        public bool AltKeyState => _keyStates.Any(kvp => AltKeyVirtualCodes.Contains(kvp.Key) && kvp.Value);
        public bool WindowsKeyState => _keyStates.Any(kvp => WinKeyVirtualCodes.Contains(kvp.Key) && kvp.Value);
        public bool MenuKeyState => _keyStates.Any(kvp => ContextKeyVirtualCodes.Contains(kvp.Key) && kvp.Value);

        public bool IsControl => CtrlKeyVirtualCodes.Contains(KeyCode);
        public bool IsShift => ShiftKeyVirtualCodes.Contains(KeyCode);
        public bool IsAlt => AltKeyVirtualCodes.Contains(KeyCode);
        public bool IsWindows => WinKeyVirtualCodes.Contains(KeyCode);
        public bool IsMenu => ContextKeyVirtualCodes.Contains(KeyCode);

        public bool IsModifier => IsControl || IsShift || IsAlt || IsWindows || IsMenu;

        private readonly Dictionary<Keys, bool> _keyStates;

        private static readonly List<Keys> CtrlKeyVirtualCodes = new List<Keys> { Keys.LControlKey, Keys.RControlKey, Keys.Control, Keys.ControlKey };
        private static readonly List<Keys> ShiftKeyVirtualCodes = new List<Keys> { Keys.LShiftKey, Keys.RShiftKey, Keys.Shift, Keys.ShiftKey };
        private static readonly List<Keys> WinKeyVirtualCodes = new List<Keys> { Keys.LWin, Keys.RWin };
        private static readonly List<Keys> AltKeyVirtualCodes = new List<Keys> { Keys.LMenu, Keys.RMenu };
        private static readonly List<Keys> ContextKeyVirtualCodes = new List<Keys> { Keys.Apps };

        internal LowLevelKeyboardProcEventArgs(int code, int scanCode, int flags, Dictionary<Keys, bool> keyStates)
        {
            KeyCode = (Keys) code;
            ScanCode = scanCode;
            Flags = (KBDLLHOOKSTRUCTFlags) flags;
            _keyStates = keyStates;
        }
    }
}