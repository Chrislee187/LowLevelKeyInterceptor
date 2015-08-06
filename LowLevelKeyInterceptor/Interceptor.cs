using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LowLevelKeyInterceptor
{
    public class Interceptor
    {
        public delegate void LowLevelKeyboardProcEventHandler(object sender, LowLevelKeyboardProcEventArgs e);

        public event LowLevelKeyboardProcEventHandler KeyDown;
        public event LowLevelKeyboardProcEventHandler KeyUp;

        private readonly IntPtr _hookId;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private readonly Dictionary<Keys, bool> _keyStates = new Dictionary<Keys, bool>();

        public Interceptor()
        {
            _hookId = SetHook(LowLevelKeyPressHandler);
            _hotKeys = new List<Tuple<IEnumerable<Keys>, IEnumerable<Keys>, IEnumerable<Keys>, Action>>();
        }

        ~Interceptor()
        {
            UnhookWindowsHookEx(_hookId);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr LowLevelKeyPressHandler(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0 ) return CallNextHookEx(_hookId, nCode, wParam, lParam);

            var keyData = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

            var keyDown = keyData.KeyDown();
            _keyStates[(Keys)keyData.vkCode] = keyDown;
            var evt = new LowLevelKeyboardProcEventArgs(keyData.vkCode, keyData.scanCode, keyData.flags, _keyStates);

            if (keyDown)
            {
                if (CheckHotKeys()) nCode = -1; // We've used this hotkey so stop it propagating any further

                KeyDown?.Invoke(null, evt);
            }
            else KeyUp?.Invoke(null, evt);

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private readonly List<Tuple<IEnumerable<Keys>, IEnumerable<Keys>, IEnumerable<Keys>, Action>> _hotKeys = new List<Tuple<IEnumerable<Keys>, IEnumerable<Keys>, IEnumerable<Keys>, Action>>();
        private readonly List<Tuple<IEnumerable<KeyGroup>, Action>> _hotKeys2 = new List<Tuple<IEnumerable<KeyGroup>, Action>>();

        private bool CheckHotKeys()
        {
            // Check the hot keys in descending number of keys order to ensure single modifier keys (i.e CTRL-J are not triggered by multi-modifiers CTRL-SHFT-J
            var action = _hotKeys2
                .OrderByDescending(hkl => hkl.Item1.Count())
                .FirstOrDefault(tuple => tuple.Item1.All(kg => kg.State(_keyStates)))?.Item2;

            if (action == null) return false;

            action();
            return true;
        }

        /// <summary>
        /// Register the action to be run if at least one key from each KeyGroup is currently depressed
        /// </summary>
        /// <example>
        ///     interceptor.Register(() => Console.WriteLine("Hello from CTRL-SHFT-J-I"), Keys.J, KeyGroups.ControlKeys);
        /// </example>
        /// <param name="action"></param>
        /// <param name="groups"></param>
        public void Register(Action action, params KeyGroup[] groups)
        {
            _hotKeys2.Add(new Tuple<IEnumerable<KeyGroup>, Action>(groups, action));
        }

        /// <summary>
        /// Register the action to be run if the specified key is pressed whilst at least one key from each of the modifier KeyGroups is currently depressed
        /// </summary>
        /// <example>
        ///     interceptor.Register(() => Console.WriteLine("Hello from CTRL-J"), Keys.J, KeyGroups.ControlKeys);
        /// </example>
        /// <param name="action"></param>
        /// <param name="modifiers"></param>
        public void Register(Action action, Keys key, params KeyGroup[] modifiers)
        {
            var keygroups = new List<KeyGroup>(modifiers);
            keygroups.Add(new KeyGroup(key));
            _hotKeys2.Add(new Tuple<IEnumerable<KeyGroup>, Action>(keygroups, action));
        }
        
        #region WINAPI Stuff
        private const int WH_KEYBOARD_LL = 13;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        #endregion
    }
}