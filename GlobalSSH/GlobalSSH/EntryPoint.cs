﻿using System.IO;
using System.Net;
using System.Threading;
using System.IO.Compression;
using System.Diagnostics;
using Ionic.Zip;
using System;

namespace IndieGoat.Net.SSH
{
    public class GlobalSSH
    {

        #region Global vars

        const string ApplicationDirectory = @"C:\IndieGoat\SSH\GlobalService\";
        const string ApplicationZipDirectory = @"C:\IndieGoat\SSH\GlobalService\install.zip";
        const string ApplicationName = @"GlobalSSHService.exe";
        const string ApplicationURL = "https://dl.dropboxusercontent.com/s/i5mbboap1n3t81q/install.zip?dl=0";

        Process SSHServiceProcess;

        bool IsRunning = false;
        
        #endregion

        #region Initialization

        /// <summary>
        /// Initialize the ssh service
        /// </summary>
        public GlobalSSH()
        {
            //Check if application directory exist's
            if (!Directory.Exists(ApplicationDirectory))
            {
                //Creates the empty directory
                Directory.CreateDirectory(ApplicationDirectory);
            }

            //Checks if the application file exists
            if (!File.Exists(ApplicationDirectory + ApplicationName))
            {
                //Initialize the new web client
                WebClient client = new WebClient();

                //Download's the file in the application directory
                client.DownloadFile(ApplicationURL, ApplicationZipDirectory);

                //Extracts the update
                using (ZipFile zip = ZipFile.Read(ApplicationZipDirectory))
                {
                    zip.ExtractAll(ApplicationDirectory);
                }
            }
        }

        #endregion

        #region Commands

        public void ShutdownApplication()
        { SSHServiceProcess.Close(); }
        public void ForwardLocalPort(string PORT, string LOCALHOST)
        {
            //Checks if the application has exited
            if (SSHServiceProcess.HasExited)
            {
                //Returns from the method if application is currently not running.
                Console.WriteLine("Process is currently closed!");
                return;
            }

            //Get input of the command
            StreamWriter stream = SSHServiceProcess.StandardInput;
            stream.WriteLine("FORWARD " + PORT + " " + LOCALHOST);

            //Get output of the command
            StreamReader o_stream = SSHServiceProcess.StandardOutput;
            Console.WriteLine(o_stream.ReadLine());
        }

        public void CheckPort(string PORT)
        {
            //Checks if the application has exited
            if (SSHServiceProcess.HasExited)
            {
                //Returns from the method if application is currently not running
                Console.WriteLine("Process is currently closed!");
                return;
            }

            //Inputs command
            StreamWriter stream = SSHServiceProcess.StandardInput;
            stream.WriteLine("CHECK " + PORT);

            //Get output of the command
            StreamReader o_Stream = SSHServiceProcess.StandardOutput;
            Console.WriteLine(o_Stream.ReadLine());
        }
        #endregion

        #region Startup of the SSH service

        //Starts the ssh service, on command
        public void StartSSHService(string SSHIP, string SSHPORT, string SSHUSERNAME, string SSHPASSWORD, bool CreateNoWindow = true)
        {
            SSHServiceProcess = new Process();
            SSHServiceProcess.StartInfo.UseShellExecute = false;
            SSHServiceProcess.StartInfo.RedirectStandardInput = true;
            SSHServiceProcess.StartInfo.RedirectStandardOutput = true;

            SSHServiceProcess.StartInfo.FileName = ApplicationDirectory + ApplicationName;
            SSHServiceProcess.StartInfo.CreateNoWindow = CreateNoWindow;

            SSHServiceProcess.StartInfo.Arguments = SSHIP + " " + SSHPORT + " " + SSHUSERNAME + " " + SSHPASSWORD;
            SSHServiceProcess.Start();

            Console.WriteLine("started process");
            IsRunning = true;
        }

        #endregion

    }
}
