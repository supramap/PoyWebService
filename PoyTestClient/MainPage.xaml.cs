using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PoyTestClient.ServiceReference1;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;

namespace PoyTestClient
{
    public partial class MainPage : UserControl
    {
        List<FileInfo> files = new List<FileInfo>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void UpLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.ShowDialog();
            files.AddRange(dlg.Files);

            StringBuilder sb = new StringBuilder();
            foreach (FileInfo fi in files)
            {
                sb.Append(fi.Name + "\r");
            }
            FileList.Text = sb.ToString();
        }

        int numberOfFiles;
        Service1SoapClient client;
        int jobId;
        object lockable = new object();

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            Submit.IsEnabled=false;
            UpLoad.IsEnabled=false;
            FileList.IsEnabled=false;
            Progress.Text = "Starting";
            numberOfFiles = files.Count;

            client = new Service1SoapClient();
            client.AddFileCompleted +=new EventHandler<AddFileCompletedEventArgs>(client_AddFileCompleted);
            client.InitCompleted +=new EventHandler<InitCompletedEventArgs>(client_InitCompleted);
            client.InitAsync();
          
        }


        void client_InitCompleted(object sender, InitCompletedEventArgs e)
        {
            Progress.Text = "uploading files";
            jobId = e.Result;
            foreach (FileInfo fi in files)
            {
                FileStream fileStream = fi.OpenRead();
                byte[] filedata = new byte[(int)fileStream.Length];
                fileStream.Read(filedata,0,(int)fileStream.Length);
                 
               client.AddFileAsync(jobId,filedata , fi.Name);
            }

            //int numberOfPeriods = 0;
            //while (numberOfFiles != 0)
            //{
            //    System.Threading.Thread.Sleep(300);
            //    numberOfPeriods = (numberOfPeriods + 1) % 4;
            //    Progress.Text = "UpLoading Files";
            //    for (int i = 0; i < numberOfPeriods; i++)
            //        Progress.Text = Progress.Text = ".";
            //}

            //Submit.IsEnabled = false;
            //UpLoad.IsEnabled = false;
            //FileList.IsEnabled = false;
            //Progress.Text = "Done";
        }


        void  client_AddFileCompleted(object sender, AddFileCompletedEventArgs e)
        {
            
            lock ( lockable)
            {
                numberOfFiles--;
            }

            if(numberOfFiles == 0)
            {
                Progress.Text = "All Files uploaded";
                client.SubmitCompleted += new EventHandler<SubmitCompletedEventArgs>(client_SubmitCompleted);
                client.SubmitAsync(jobId);
            }
        }


        IsolatedStorageFile resultsFiles;
        void client_SubmitCompleted(object sender, SubmitCompletedEventArgs e)
        {
 
             resultsFiles =  IsolatedStorageFile.GetUserStoreForApplication();
         
            resultsFiles.OpenFile("temp", FileMode.Create).Write(e.Result, 0, e.Result.Length);

           // Progress.Text = "Done";
           // ResultFile.Visibility = System.Windows.Visibility.Visible;



            Submit.IsEnabled = true;
            UpLoad.IsEnabled = true;
            FileList.IsEnabled = true;
            Progress.Text = "";
            

        }


        private void ResultFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            //sfd.SafeFileName ="";
            
            sfd.ShowDialog();
            Stream destStream = sfd.OpenFile();
            IsolatedStorageFileStream sourceStream = resultsFiles.OpenFile("temp",FileMode.Open);
            byte[] tempBuffer = new byte[sourceStream.Length];
            sourceStream.Read(tempBuffer,0,tempBuffer.Length);
            destStream.Write(tempBuffer,0,tempBuffer.Length);

            Submit.IsEnabled = true;
            UpLoad.IsEnabled = true;
            FileList.IsEnabled = true; 
            Progress.Text = "";
            ResultFile.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
