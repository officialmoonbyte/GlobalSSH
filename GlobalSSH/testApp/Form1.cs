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

        GlobalSSH sshService;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Port Tunnel : " + sshService.TunnelLocalPort("192.168.0.11", "3389", true));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Port Available : " + sshService.CheckLocalPort(445));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sshService = new GlobalSSH("indiegoat.us", 80, "public", "Public36", false);
        }
    }
}
