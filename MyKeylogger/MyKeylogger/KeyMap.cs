using System.Windows.Forms;

namespace MyKeylogger
{
    struct KeyMap
    {
        public Keys Key { get; private set; }

        public string Modified { get; private set; }

        public string Original { get; private set; }

        public KeyMap(Keys key, string original, string modified = null) : this()
        {
            Key = key;
            Original = original;
            Modified = modified;
        }
    }
}
