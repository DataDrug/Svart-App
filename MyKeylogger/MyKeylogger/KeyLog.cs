using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKeylogger
{
    public class KeyLog
    {
        public string text { get; private set; }

        public KeyLog()
        {
            text = "";
        }
    }
}
