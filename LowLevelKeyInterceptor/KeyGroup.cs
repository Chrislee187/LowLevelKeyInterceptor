using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LowLevelKeyInterceptor
{
    public class KeyGroup : IReadOnlyCollection<Keys>
    {
        public string GroupName { get; }
        private readonly List<Keys> _keys;

        public static readonly KeyGroup ControlKeys = new KeyGroup("Control", Keys.LControlKey, Keys.RControlKey, Keys.Control, Keys.ControlKey);
        public static readonly KeyGroup ShiftKeys = new KeyGroup("Shift", Keys.LShiftKey, Keys.RShiftKey, Keys.Shift, Keys.ShiftKey);
        public static readonly KeyGroup WindowKeys = new KeyGroup("Windows", Keys.LWin, Keys.RWin);
        public static readonly KeyGroup AltKeys = new KeyGroup("Alt", Keys.LMenu, Keys.RMenu);
        public static readonly KeyGroup AppsKeys = new KeyGroup("Apps", Keys.Apps);

        public KeyGroup(params Keys[] keys) : this(string.Join(", ", keys), keys)
        {

        }

        public KeyGroup(string groupName, params Keys[] keys )
        {
            GroupName = groupName;
            _keys = new List<Keys>(keys);
        }

        public bool State(IDictionary<Keys, bool> keyStates) => this.Any(k => keyStates.ContainsKey(k) && keyStates[k]);

        public IEnumerator<Keys> GetEnumerator() => _keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => _keys.Count;

        public override string ToString() => GroupName;
    }
}