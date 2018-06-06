using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Servidor
{
    class Program
    {

        public static TcpListener server;
        public static TcpClient cliente = new TcpClient();
        public static IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("172.16.70.123"), 8080);
        public static List<Connection> list = new List<Connection>();

        public static Connection con;

        public struct Connection
        {
            public NetworkStream stream;
            public StreamWriter streamw;
            public StreamReader streamr;
            public string nick;
        }


        static void Main(string[] args)
        {
            Inicio();
        }


        /* public static void Iniciar(Socket _listen, IPEndPoint _connect)
         {    
             Socket _conexion = _listen.Accept();
             Console.WriteLine("Conexion Aceptada");

             byte[] info = new byte[100];
             string data = string.Empty;
             int data_tam = 0;

             data_tam = _conexion.Receive(info, 0, info.Length, 0);
             Array.Resize(ref info, data_tam);
             data = Encoding.Default.GetString(info);

             Console.WriteLine($"La info recibida es: { data }");
         }*/

        public static void Inicio()
        {
            Console.WriteLine("Servidor Iniciado... \n");
            server = new TcpListener(ipEndPoint);
            server.Start();

            while (true)
            {
                cliente = server.AcceptTcpClient();
                con = new Connection();
                con.stream = cliente.GetStream();
                con.streamr = new StreamReader(con.stream);
                con.streamw = new StreamWriter(con.stream);

                con.nick = con.streamr.ReadLine();
                list.Add(con);
                Console.WriteLine("{0} esta conectado", con.nick);

                Thread t = new Thread(Escuchar);
                t.Start();
            }
        }

        static void Escuchar()
        {
            Connection hcon = con;
            do
            {
                try
                {
                    string tmp = hcon.streamr.ReadLine();
                    Console.WriteLine("{0} : {1}", hcon.nick, tmp);
                    foreach (Connection c in list)
                    {
                        try
                        {
                            c.streamw.WriteLine("{0} : {1}", hcon.nick, tmp);
                            c.streamw.Flush();
                        }
                        catch { }
                    }
                }
                catch
                {
                    list.Remove(hcon);
                    Console.WriteLine("{0} se ha desconectado", con.nick);
                    break;
                }
            } while (true);
        }
    }
}
