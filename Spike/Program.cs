using System;
using System.Windows.Forms;
using LowLevelKeyInterceptor;

namespace Spike
{
    class Program
    {

        public static void Main()
        {
            Console.WriteLine("Interceptor example");
            Console.WriteLine("CTRL-J, CTRL-SHIFT-J and CTRL-I all have simple console writeline global hot keys");
            var h = new Interceptor();
            h.KeyDown += InterceptKeys_KeyDown;
            h.KeyUp += InterceptKeys_KeyUp;

            var ctrlKeysGroup = KeyGroup.ControlKeys;
            var shftKeysGroup = KeyGroup.ShiftKeys;
            var iKeyGroup = new KeyGroup(Keys.I);
            var jKeyGroup = new KeyGroup(Keys.J);

            h.Register(() => Console.WriteLine("Hello from CTRL-J"), Keys.J, ctrlKeysGroup);
            h.Register(() => Console.WriteLine("Hello from CTRL-SHFT-J"), Keys.J, ctrlKeysGroup, shftKeysGroup);
            h.Register(() => Console.WriteLine("Hello from CTRL-SHFT-I"), ctrlKeysGroup, shftKeysGroup, iKeyGroup);
            h.Register(() => Console.WriteLine("Hello from CTRL-SHFT-I-J"), ctrlKeysGroup, shftKeysGroup, iKeyGroup, jKeyGroup);
            Application.Run();
        }

        private static void InterceptKeys_KeyDown(object sender, LowLevelKeyboardProcEventArgs e)
        {
            DumpState(e);
        }

        private static void InterceptKeys_KeyUp(object sender, LowLevelKeyboardProcEventArgs e)
        {
            DumpState(e);
        }

        private static void DumpState(LowLevelKeyboardProcEventArgs e)
        {
            if (e.ControlKeyState()) Console.Write("Control-");
            if (e.ShiftKeyState) Console.Write("Shift-");
            if (e.AltKeyState) Console.Write("Alt-");
            if (e.WindowsKeyState) Console.Write("Windows-");
            if (e.MenuKeyState) Console.Write("Menu-");

            if (!e.IsModifier)
            {
                Console.Write($"{(Keys)e.KeyCode}");
            }
            Console.WriteLine(e.KeyUp() ? " released" : " pressed");
        }
    }
}
