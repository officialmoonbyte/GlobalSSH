using GlobalDYNUpdater;
using GlobalSSHService.Server;
using Renci.SshNet;
using System;
using System.Threading;

namespace IndieGoat.Net.SSH.App
{
    static class service
    {
        public static Server server;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string SSHIP = args[0];
            string SSHPORT = args[1];
            string SSHUSERNAME = args[2];
            string SSHPASSWORD = args[3];

            ILogger.AddToLog("SSH", "Started SSH request!");
            ILogger.AddToLog("SSH", "SSHIP : " + SSHIP);
            ILogger.AddToLog("SSH", "SSHPORT : " + SSHPORT);
            ILogger.AddToLog("SSH", "SSHUSERNAME : " + SSHUSERNAME);
            ILogger.AddToLog("SSH", "SSHPASSWORD : " + SSHPASSWORD);

            ILogger.SetLoggingEvents();

            //SSH connection info
            PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(SSHIP, int.Parse(SSHPORT), SSHUSERNAME, SSHPASSWORD);

            //Declaring sshclient
            PublicResources.client = new SshClient(connectionInfo);
            PublicResources.client.Connect();

            if (PublicResources.client.IsConnected)
            {
                //Start the tcp local server
                server = new Server();
                server.StartServer();
            }
            else
            {
                ILogger.AddToLog("SSH", "GlobalSSH is currently not connected to the server!");
                ILogger.WriteLog();
            }

            while (true) { Thread.Sleep(50000); }
        }
    }
}
