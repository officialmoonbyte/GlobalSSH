using System.Net.Sockets;
using System.Text;

namespace GlobalSSHService.Server
{
    class ServerClient
    {
        //Local values for the server
        TcpClient LocalClientConnection;
        TcpListener LocalServerListener;

        /// <summary>
        /// Sets all local values of the ServerClient
        /// </summary>
        public ServerClient(TcpClient client, TcpListener ServerListener)
        {
            LocalClientConnection = client;
            LocalServerListener = ServerListener;
        }

        /// <summary>
        /// Sends the client information.
        /// </summary>
        public void SendClientInformation(string Information)
        {
            try
            {
                //Sends client information
                LocalClientConnection.Client.Send(Encoding.UTF8.GetBytes(Information));
            } catch { }
        }
    }
}
