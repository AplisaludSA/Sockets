using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cliente_F
{
    public partial class Form1 : Form
    {
        
        static private NetworkStream stream;
        static private StreamWriter streamw;
        static private StreamReader streamr;
        static private TcpClient cliente = new TcpClient();
        static private string nick = "Desconocido";

        private delegate void DAddItem(string s);
        private void AddItem(string s)
        {
            lstMensajes.Items.Add(s);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblUsuario.Visible = true;
            btnIngresar.Visible = true;
            txtUsuario.Visible = true;

            lstMensajes.Visible = false;
            btnEnviar.Visible = false;
            txtMessage.Visible = false;
        }

        private void btnIngresar_Click(object sender, EventArgs e) => Login();
        private void btnEnviar_Click(object sender, EventArgs e) => Enviar();

        void Listen()
        {
            while (cliente.Connected)
            {
                try
                {
                    this.Invoke(new DAddItem(AddItem), streamr.ReadLine());
                }
                catch
                {
                    Application.Exit();
                }
            }
        }

        void Conectar()
        {
            try
            {
                cliente.Connect(IPAddress.Parse("172.16.70.123"), 8080);
                if (cliente.Connected)
                {
                    Thread t = new Thread(Listen);

                    stream = cliente.GetStream();
                    streamw = new StreamWriter(stream);
                    streamr = new StreamReader(stream);

                    streamw.WriteLine(nick);
                    streamw.Flush();

                    t.Start();
                }
                else
                {
                    Application.Exit();
                }
            }
            catch
            {
                Application.Exit();
            }
        }


        private void Enviar()
        {
            streamw.WriteLine(txtMessage.Text);
            streamw.Flush();
            txtMessage.Clear();
        }

        private void Login()
        {
            lblUsuario.Visible = false;
            btnIngresar.Visible = false;
            txtUsuario.Visible = false;

            lstMensajes.Visible = true;
            btnEnviar.Visible = true;
            txtMessage.Visible = true;

            nick = txtUsuario.Text;
            Conectar();
        }
        

        private void txtUsuario_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Return)
                Login();
        }

        private void txtMessage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                Enviar();
        }
    }
}
