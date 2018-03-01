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
                ssh.StartSSHService("indiegoat.us", "80", "public", "Public36");
                ssh.ForwardLocalPort("3390", "192.168.0.10");
                ssh.ForwardLocalPort("2445", "192.168.0.10");
            })); thread.Start();
        }
    }
}
