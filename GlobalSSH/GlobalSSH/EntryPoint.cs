using System.IO;
using System.Net;
using System.Threading;
using System.IO.Compression;
using System.Diagnostics;

namespace IndieGoat.Net.SSH
{
    public class GlobalSSH
    {

        #region Global vars

        const string ApplicationDirectory = @"C:\IndieGoat\SSH\GlobalService\";
        const string ApplicationZipDirectory = @"C:\IndieGoat\SSH\GlobalService\install.zip";
        const string ApplicationName = @"GlobalSSHService.exe";
        const string ApplicationURL = "";

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
                    ZipFile.ExtractToDirectory(ApplicationZipDirectory, ApplicationDirectory);

                })); thread.Start();
            }
        }

        #endregion

        #region Startup of the SSH service

        //Starts the ssh service, on command
        public void StartSSHService(string SSHIP, string SSHPORT, string SSHUSERNAME, string SSHPASSWORD)
        {
            //Starts the process
            SSHServiceProcess = Process.Start(ApplicationDirectory + ApplicationName, SSHIP + " " + SSHPORT + " " + SSHUSERNAME + " " + SSHPASSWORD);
        }

        #endregion

    }
}
