using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSIS_Cource
{
    class Server
    {
        public static BindingList<TaskView> Tasks = new BindingList<TaskView>();

        public static Stopwatch sw = new Stopwatch();
        private static readonly IPAddress serverAddress = IPAddress.Parse("127.0.0.1");
        private static readonly int Port = 7777;
        private static TcpListener server = null;
        public static Task Listen = new Task(Listener);
        public static BindingList<ClientObject> Clients = new BindingList<ClientObject>();
        public static Queue<(string, string, string, MessageType)> queue
            = new Queue<(string, string, string, MessageType)>();
        static (string, string, string, MessageType) Current;
        public static byte[][] CompleteBytes { get; set; }
        public static bool[] IsComplete { get; set; }
        static bool inProcess = false;
        public static long CurrentLength;
        private static void Listener()
        {
            try
            {
                if (server == null)
                {
                    server = new TcpListener(serverAddress, Port);
                    server.Start();
                    AddPointTask.Start();
                }

                while (true)
                {
                    if (queue.Count > 0 && inProcess == false)
                    {
                        sw.Start();
                        inProcess = true;
                        Task Writer = new Task(Write);
                        Writer.Start();
                        Current = queue.Dequeue();
                        FileInfo fi = new FileInfo(Current.Item1);
                        CurrentLength = fi.Length / 1000 + 1;
                        IsComplete = new bool[CurrentLength];
                        CompleteBytes = new byte[CurrentLength][];
                        int count = 0;
                        using (FileStream fs = new FileStream(fi.FullName, FileMode.Open))
                        {
                            while (count < CurrentLength)
                            {
                                bool stop = false;
                                foreach (var client in Clients)
                                {
                                    if (client.IsFree)
                                    {
                                        byte[] data = new byte[1000];
                                        int len = fs.Read(data, 0, data.Length);
                                        if (count == CurrentLength-1)
                                        {
                                            client.Append(data, Encoding.UTF8.GetBytes(Current.Item2), 0, count, Current.Item4, len, true);
                                            stop = true;
                                        }
                                        else
                                            client.Append(data, Encoding.UTF8.GetBytes(Current.Item2), 0, count, Current.Item4, len, false);
                                        count++;
                                        Console.WriteLine(count);
                                    }
                                }
                                if (stop)
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                MessageBox.Show(ex.Message + ex.StackTrace);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
            finally
            {
                if (server != null)
                    server.Stop();
                foreach (var c in Clients)
                {
                    c.client.Close();
                }
                Clients.Clear();
            }
        }
        private static Task AddPointTask = new Task(AddPointLoop);
        public static void AddPointLoop()
        { 
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                //Console.WriteLine($"Подключен клиент {client.Client.RemoteEndPoint.ToString()}");
                //MessageBox.Show($"Подключен клиент {client.Client.RemoteEndPoint.ToString()}");

                ClientObject co = new ClientObject(client);
                Clients.Add(co);
            }          
        }
        
        private static void Write()
        {
            if (inProcess)
            {
                using (FileStream fs = new FileStream(Current.Item3, FileMode.OpenOrCreate))
                {
                    int i = 0;
                    while (i != CurrentLength)
                    {
                        if (IsComplete[i])
                        {
                            fs.Write(CompleteBytes[i], 0, CompleteBytes[i].Length);
                            CompleteBytes[i] = null;
                            i++;
                            //Console.WriteLine($"i={i}");
                        }
                    }
                    inProcess = false;
                }
            }     
        }
    }
}
