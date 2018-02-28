using Renci.SshNet;
using System;

namespace IndieGoat.Net.SSH.App
{
    static class service
    {
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

            //SSH connection info
            PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(SSHIP, int.Parse(SSHPORT), SSHUSERNAME, SSHPASSWORD);

            //Declaring sshclient
            PublicResources.client = new SshClient(connectionInfo);

            if (PublicResources.client.IsConnected)
            {
                //Starts the console thread
                mConsole.ConsoleThread();

                //Loop so the application doesn't close
                while (true) { }
            }
        }
    }
}
