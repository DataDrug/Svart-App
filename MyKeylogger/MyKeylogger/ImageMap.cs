using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKeylogger
{
    struct ImageMap
    {
        public string _fileName { get; private set; }
        public string _filePath { get; private set; }
        public Bitmap _bitmap { get; private set; }

        public ImageMap (string fileName, string filePath, Bitmap bitmap)
        {
            _fileName = fileName;
            _filePath = filePath;
            _bitmap = bitmap;
        }
    }
}
