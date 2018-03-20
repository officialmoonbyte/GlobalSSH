using System.Threading;
using System;
using Renci.SshNet;
using System.Collections.Generic;

namespace IndieGoat.Net.SSH.App
{
    public class mConsole
    {

        public static List<int> PortForward = new List<int>();

        public static void ConsoleThread()
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                //Console thread loop
                while (true)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        //Gets the command from the console
                        string[] Command = Console.ReadLine().Split(new string[] { " " }, StringSplitOptions.None);

                        //FORWAD PORT[ARGS1] LOCALHOST[ARGS2]
                        if (Command[0] == "FORWARD")
                        {
                            //Get the local values
                            string PORT = Command[1];
                            string LOCALHOST = Command[2];

                            Console.WriteLine("Port : " + PORT);
                            Console.WriteLine("IP : " + LOCALHOST);

                            //Forwards the port
                            var port = new ForwardedPortLocal("127.0.0.1", uint.Parse(PORT), LOCALHOST, uint.Parse(PORT));

                            //Starts the forward port
                            PublicResources.client.AddForwardedPort(port);
                            port.Start();

                            if (port.IsStarted)
                            {
                                service.server.SendClient(true.ToString());
                                PortForward.Add(int.Parse(PORT));
                            }
                            else
                            {
                                service.server.SendClient(false.ToString());
                            }
                        }
                        if (Command[0] == "CHECK")
                        {
                            string CHECKPORT = Command[1];
                            bool SendCommand = false;
                            for(int i = 0; i > PortForward.Count; i++)
                            {
                                if (int.Parse(CHECKPORT) == PortForward[i])
                                {
                                    service.server.SendClient(true.ToString()); SendCommand = true;
                                }
                            }
                            if (!SendCommand) service.server.SendClient(false.ToString());
                        }
                    }
                    catch { service.server.SendClient(false.ToString()); Console.WriteLine("Error!"); }
                }
            })); thread.Start();
        }
    }
}
