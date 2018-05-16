using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using IndieGoat.InideClient.Default;
using IndieGoat.Net.SSH;

namespace testApp
{
    public partial class Form1 : Form
    {

        GlobalSSH sshService;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TcpClient client = new TcpClient();
            client.Connect("localhost", 5750);
            client.Client.Send(Encoding.UTF8.GetBytes("test"));
        }
    }
}
