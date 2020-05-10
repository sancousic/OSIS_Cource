using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSIS_Cource
{
    class ClientObject
    {
        public TcpClient client;
        public int Id { get => Server.Clients.IndexOf(this); }
        public Task Proc;
        public bool IsReady = false;
        public bool isFree = true;
        public string Source = "";
        NetworkStream networkStream = null;
        Message lastMessage = new Message();
        public bool IsFree
        {
            get => isFree;
            set
            {
                isFree = value;
            }
        }
        public string Client_str
        {
            get 
            {
                return $"{client.Client.RemoteEndPoint.ToString()} {(IsFree ? "free" : $"{Source}")}";
            }
        }
        public ClientObject(TcpClient client)
        {
            this.client = client;
            networkStream = client.GetStream();
            Proc = new Task(Process);
            Proc.Start();
        }
        public void Append(byte[] data, byte[] key, int id_source, int counter, MessageType type, int len, bool isLast)
        {
            Message message = new Message()
            {
                Counter = counter,
                Data = data.Take(len).ToArray(),
                IdSource = id_source,
                Key = key,
                IsLast = isLast,
                MessageType = type,
            };
            Source = id_source.ToString();
            byte[] message_data = message.ToByteArray();
            networkStream.Write(message_data, 0, message_data.Length);
            IsFree = false;
            IsReady = false;
        }
        public void Process()
        {
            try
            {

                while (true)
                {
                    byte[] data = new byte[1024];
                    int len = 0;
                    if (networkStream.DataAvailable)
                    {
                        len = networkStream.Read(data, 0, data.Length);
                        if (len > 1024)
                            throw new Exception("ex");
                        lastMessage = Message.FromByteArray(data, len);
                        if (lastMessage.MessageType == MessageType.RemoveComputingPoint)
                            break;
                        Server.CompleteBytes[lastMessage.Counter] = lastMessage.Data;
                        Server.IsComplete[lastMessage.Counter] = true;
                        if (lastMessage.Counter == Server.CurrentLength - 1)
                        {
                            Server.sw.Stop();
                            Console.WriteLine($"algoritm was stoped");
                            Console.WriteLine($"nodes: {Server.Clients.Count}");
                            Console.WriteLine($"time: {Server.sw.ElapsedMilliseconds}");
                        }
                        IsFree = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            finally
            {
                if (networkStream != null)
                    networkStream.Close();
                if (client != null)
                {
                    client.Close();
                    Server.Clients.Remove(this);
                    Console.WriteLine("REMOVED");
                }
            }

        }
    }
}
