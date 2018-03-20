using System.IO;
using System.Net;
using System.Threading;
using System.IO.Compression;
using System.Diagnostics;
using Ionic.Zip;
using System;
using System.Net.Sockets;
using System.Text;

namespace IndieGoat.Net.SSH
{
    public class GlobalSSH
    {

        #region Global vars

        const string ApplicationDirectory = @"C:\IndieGoat\SSH\GlobalService\";
        const string ApplicationZipDirectory = @"C:\IndieGoat\SSH\GlobalService\install.zip";
        const string ApplicationName = @"GlobalSSHService.exe";
        const string ApplicationDefaultName = "GlobalSSHService";
        const string ApplicationURL = "https://dl.dropboxusercontent.com/s/i5mbboap1n3t81q/install.zip?dl=0";
        bool RedirectStandardOutput = false;

        Process SSHServiceProcess;

        bool IsRunning = false;

        TcpLocalClient LocalClient;
        
        #endregion

        #region Initialization

        /// <summary>
        /// Initialize the ssh service
        /// </summary>
        public GlobalSSH()
        {
            Console.WriteLine("bb1211");
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

            LocalClient = new TcpLocalClient();
        }

        #endregion

        #region Commands

        public void ShutdownApplication()
        { SSHServiceProcess.Close(); }
        public bool ForwardLocalPort(string PORT, string LOCALHOST)
        {
            //Checks if the application has exited
            if (SSHServiceProcess.HasExited)
            {
                //Returns from the method if application is currently not running.
                Console.WriteLine("Process is currently closed!");
                return false;
            }

            //Get input of the command
            StreamWriter stream = SSHServiceProcess.StandardInput;
            stream.WriteLine("FORWARD " + PORT + " " + LOCALHOST);

            //Gets the result from the server.
            string ServerResult = LocalClient.WaitForResult();
            return bool.Parse(ServerResult);
        }

        public bool CheckPort(string PORT)
        {
            //Checks if the application has exited
            if (SSHServiceProcess.HasExited)
            {
                //Returns from the method if application is currently not running
                Console.WriteLine("Process is currently closed!");
                return false;
            }

            //Gets the result from the server
            string ServerResult = LocalClient.WaitForResult();
            return bool.Parse(ServerResult);
        }
        #endregion

        #region Startup of the SSH service

        //Starts the ssh service, on command
        public void StartSSHService(string SSHIP, string SSHPORT, string SSHUSERNAME, string SSHPASSWORD, bool CreateNoWindow = true, bool RedirectOutput = true)
        {
            Process[] Processes = Process.GetProcessesByName(ApplicationDefaultName);
            Console.WriteLine(Processes.Length);
           
            if (Processes.Length > 0)
            {
                SSHServiceProcess = Processes[0];
                
            }
            else
            {
                RedirectStandardOutput = RedirectOutput;

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

            LocalClient.ConnectToRemoteServer();
        }

        #endregion

        #region Client

        public class TcpLocalClient
        {
            private TcpClient client;

            /// <summary>
            /// Connects to a local command server.
            /// </summary>
            public void ConnectToRemoteServer(string serverIP=  "localhost", int serverPort = 4850)
            {
                //Connects to the server
                client = new TcpClient();
                client.Connect(serverIP, serverPort);
            }

            /// <summary>
            /// Disconnects from a local command server.
            /// </summary>
            public void Disconnect()
            {
                //Checks if client is connected, if true closes the connection
                if (client.Connected) client.Close();
            }

            /// <summary>
            /// Waits for a string from the server and then returns that string.
            /// </summary>
            /// <returns>Return value of the server</returns>
            public string WaitForResult()
            {
                //Gets data from the server.
                Console.WriteLine("[SSH] Receiving Server Data! Please Wait...");
                byte[] data = new byte[client.ReceiveBufferSize];
                int receivedDataLength = client.Client.Receive(data);

                //Converts the value to a string
                string stringData = Encoding.ASCII.GetString(data, 0, receivedDataLength);

                //Returns the server response
                Console.WriteLine("[SSH] Server Response : " + stringData);
                return stringData;
            }

            /// <summary>
            /// Sends a command to the server
            /// </summary>
            public void SendCommand(string Command, string[] Args)
            {
                //Gets the string to send the server
                string sendCommandServer; sendCommandServer = Command; for (int i = 0; i < Args.Length; i++)
                { sendCommandServer += " " + Args[i]; }

                //Sends the server data
                client.Client.Send(Encoding.UTF8.GetBytes(sendCommandServer));
            }
        }

        #endregion

    }
}
