using System;

// ReSharper disable InconsistentNaming

namespace LowLevelKeyInterceptor
{
    internal struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public IntPtr dwExtraInfo;
    }

    //    Bits Description for Flags
    //      0	Specifies whether the key is an extended key, such as a function key or a key on the numeric keypad.The value is 1 if the key is an extended key; otherwise, it is 0.
    //      1	Specifies whether the event was injected from a process running at lower integrity level.The value is 1 if that is the case; otherwise, it is 0. Note that bit 4 is also set whenever bit 1 is set.
    //      2-3	Reserved.
    //      4	Specifies whether the event was injected. The value is 1 if that is the case; otherwise, it is 0. Note that bit 1 is not necessarily set when bit 4 is set.
    //      5	The context code.The value is 1 if the ALT key is pressed; otherwise, it is 0.
    //      6	Reserved.
    //      7	The transition state.The value is 0 if the key is pressed and 1 if it is being released.
    [Flags]
    public enum KBDLLHOOKSTRUCTFlags : uint
    {
        LLKHF_EXTENDED = 0x01,
        LLKHF_LOWER_IL_INJECTED = 0x02,
        LLKHF_RESERVED1 = 0x04,
        LLKHF_RESERVED2 = 0x08,
        LLKHF_INJECTED = 0x10,
        LLKHF_ALTDOWN = 0x20,
        LLKHF_RESERVED3 = 0x40,
        LLKHF_KEYUP = 0x80,
    }

    static class KBDLLHOOKSTRUCTExtensions
    {
        public static bool KeyUp(this KBDLLHOOKSTRUCT kbd)
        {
            return (((KBDLLHOOKSTRUCTFlags)kbd.flags & KBDLLHOOKSTRUCTFlags.LLKHF_KEYUP) > 0);
        }
        public static bool KeyDown(this KBDLLHOOKSTRUCT kbd)
        {
            return !KeyUp(kbd);
        }
    }
}