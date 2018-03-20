using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IndieGoat.Net.SSH;

namespace testApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                GlobalSSH ssh = new GlobalSSH();
                ssh.StartSSHService("indiegoat.us", "80", "public", "Public36", false, true);
                bool b1 = ssh.ForwardLocalPort("3390", "192.168.0.11");
                bool b2 = ssh.ForwardLocalPort("2445", "192.168.0.11");
                bool b3 = ssh.ForwardLocalPort("3389", "192.168.0.11");

                Console.WriteLine("b1 : " + b1);
                Console.WriteLine("b2 : " + b2);
                Console.WriteLine("b3 : " + b3);

                Console.WriteLine("DONE!");
            })); thread.Start();
        }
    }
}
