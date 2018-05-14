using System.Threading;
using System;
using System.Net.Sockets;
using Renci.SshNet;

namespace IndieGoat.Net.SSH
{
    public class GlobalSSH
    {

        #region Vars

        public SshClient SSHClient;

        #endregion

        #region Startup

        /// <summary>
        /// Connects to the SSH server on startup
        /// </summary>
        public GlobalSSH(string SSHIP, int SSHPORT, string Username, string Password, bool AutoConnect = false)
        {
            if (AutoConnect) ConnectSSH(SSHIP, SSHPORT, Username, Password);
        }

        #endregion

        #region Connect

        /// <summary>
        /// Connects to the SSH server
        /// </summary>
        public void ConnectSSH(string SSHIP, int SSHPORT, string Username, string Password)
        {
            if (!this.IsConnected())
            {
                PasswordConnectionInfo connectionInfo = new PasswordConnectionInfo(SSHIP, SSHPORT, Username, Password);

                SSHClient = new SshClient(connectionInfo);
                SSHClient.Connect();
            } else { Console.WriteLine("[GlobalSSH] SSH client is currently connected!"); }
        }

        /// <summary>
        /// Gets the status of the SSH server to see if it is connected
        /// </summary>
        /// <returns></returns>
        public bool IsConnected() { try { if (SSHClient != null) { return SSHClient.IsConnected; } else { return false; } } catch { return false; } }

        #endregion

        #region Port

        #region Legacy

        /// <summary>
        /// Returns true if the port can be used.
        /// </summary>
        public bool CheckLocalPort(int PORT)
        {
            bool IsPortOpen = false;
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    client.Connect("127.0.0.1", PORT);
                    IsPortOpen = false;
                    client.Close();
                }
                catch (Exception e)
                {
                    IsPortOpen = true;
                }
            }

            return IsPortOpen;
        }

        public bool TunnelLocalPort(string IP, string PORT, bool Async = false)
        {
            if (CheckLocalPort(int.Parse(PORT)) && this.IsConnected())
            {
                var forPort = new ForwardedPortLocal("127.0.0.1", uint.Parse(PORT), IP, uint.Parse(PORT));

                SSHClient.AddForwardedPort(forPort);
                forPort.Start();

                return forPort.IsStarted;
            }
            else
            {
                Console.WriteLine("Port is currently in use! Ignoring incase port is open in diffrent ssh tunnel.");

                if (Async) ThreadedPortLocal(IP, PORT);
                return false;
            }
        }

        #endregion

        #region Threaded

        private void ThreadedPortLocal(string IP, string PORT)
        {
            Console.WriteLine("Started a Threaded Local Port thread... Waiting for port to be avaialble.");

            new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (CheckLocalPort(int.Parse(PORT)) && this.IsConnected())
                    {
                        var forPort = new ForwardedPortLocal("127.0.0.1", uint.Parse(PORT), IP, uint.Parse(PORT));

                        SSHClient.AddForwardedPort(forPort);
                        forPort.Start();
                    }

                    Thread.Sleep(1000);
                }
            })).Start();
        }

        #endregion

        #endregion

    }
}
