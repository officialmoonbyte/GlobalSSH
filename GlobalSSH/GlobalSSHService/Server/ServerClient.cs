using IndieGoat.Net.SSH.App;
using Renci.SshNet;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
            ServerConsole();
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

        /// <summary>
        /// Connection to the server console.
        /// </summary>
        private void ServerConsole()
        {
            new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    //Check if the client stream is available
                    NetworkStream clientStream; try { clientStream = LocalClientConnection.GetStream(); }
                    catch { Console.WriteLine("Client has disconnected!"); break; }

                    //Gets the client request
                    string ClientResult = Receiver(clientStream);

                    if (ClientResult != null)
                    {
                        //Split the client result.
                        string[] CommandArgs = ClientResult.Split(' ');

                        //Check if the command is equal to forward
                        if (CommandArgs[0] == "FORWARD")
                        {
                            //Get command vars
                            string IP = CommandArgs[1];
                            string PORT = CommandArgs[2];

                            //Forwards the port
                            var port = new ForwardedPortLocal("127.0.0.1", uint.Parse(PORT), IP, uint.Parse(PORT));

                            //Starts the forward port
                            PublicResources.client.AddForwardedPort(port);
                            port.Start();

                            if (port.IsStarted)
                            {
                                service.server.SendClient(true.ToString());
                            }
                            else
                            {
                                service.server.SendClient(false.ToString());
                            }
                        }
                    }
                }
            })).Start();

        }

        //Receive a byte from the server and transcribe the byte into a string.
        private string Receiver(NetworkStream m)
        {
            try
            {
                //Checks if the NetworkStream can read from the socket request.
                if (m.CanRead)
                {
                    //Sets the byte value and then read the network stream.
                    byte[] bytes = new byte[LocalClientConnection.ReceiveBufferSize];
                    m.Read(bytes, 0, LocalClientConnection.ReceiveBufferSize);

                    //Encodes the bytes into a string, and than trim the string.
                    string ReceivedBytes = Encoding.UTF8.GetString(bytes);
                    ReceivedBytes = ReceivedBytes.Trim('\0');

                    //List in console, client response
                    Console.WriteLine("[INFO] Got client response! " + ReceivedBytes);

                    //Flushes the Network Stream
                    m.Flush();

                    //Return bytes
                    return ReceivedBytes;
                }
                else
                {
                    //Return error if Network Stream does not support reading.
                    Console.WriteLine("[Error] Client does not support reading!");
                }
            }
            catch
            {
                //Display a error, Client Disconnect.
                Console.WriteLine("[Server] Client disconnected!");
                LocalClientConnection.Close();
            }

            //Return null (method should not get up to this point, unless there is an error.
            return null;
        }
    }
}
