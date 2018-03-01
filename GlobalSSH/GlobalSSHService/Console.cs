using System.Threading;
using System;
using Renci.SshNet;

namespace IndieGoat.Net.SSH.App
{
    public class mConsole
    {
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
                                Console.WriteLine(true.ToString());
                            }
                            else
                            {
                                Console.WriteLine(false.ToString());
                            }
                        }
                    }
                    catch { Console.WriteLine(false.ToString()); Console.WriteLine("Error!"); }
                }
            })); thread.Start();
        }
    }
}
