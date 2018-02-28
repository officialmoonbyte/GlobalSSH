using System.IO;
using System.Net;
using System.Threading;
using System.IO.Compression;
using System.Diagnostics;
using Ionic.Zip;

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
                Thread thread = new Thread(new ThreadStart(() =>
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

                })); thread.Start();
            }
        }

        #endregion

        #region Commands

        public void ShutdownApplication()
        { SSHServiceProcess.Close(); }
        public bool ForwardLocalPort(string PORT, string LOCALHOST)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                while (!IsRunning) { }
                StreamWriter stream = SSHServiceProcess.StandardInput;
                stream.WriteLine("FORWARD " + PORT + " " + LOCALHOST);
                stream.Close();

            })); thread.Start();

            StreamReader o_stream = SSHServiceProcess.StandardOutput;
            string Output = o_stream.ReadLine();

            return bool.Parse(Output);
        }

        #endregion

        #region Startup of the SSH service

        //Starts the ssh service, on command
        public void StartSSHService(string SSHIP, string SSHPORT, string SSHUSERNAME, string SSHPASSWORD)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                while(!File.Exists(ApplicationDirectory + ApplicationName)) { }

                Process[] tmpProcess;

                tmpProcess = Process.GetProcessesByName(ApplicationName);

                try
                {
                    if (tmpProcess[0] == null)
                    {
                        //Starts the process
                        SSHServiceProcess = Process.Start(ApplicationDirectory + ApplicationName, SSHIP + " " + SSHPORT + " " + SSHUSERNAME + " " + SSHPASSWORD);
                    }
                    else
                    {
                        SSHServiceProcess = tmpProcess[0];
                    }
                }
                catch { SSHServiceProcess = Process.Start(ApplicationDirectory + ApplicationName, SSHIP + " " + SSHPORT + " " + SSHUSERNAME + " " + SSHPASSWORD); }

                IsRunning = true;
            })); thread.Start();
        }

        #endregion

    }
}
