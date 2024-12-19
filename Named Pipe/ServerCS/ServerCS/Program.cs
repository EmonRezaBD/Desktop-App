using System;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Collections;
namespace CSNamedPipe
{
    public class NamedPipeServer
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeFileHandle CreateNamedPipe(String pipeName, uint dwOpenMode, uint dwPipeMode, uint nMaxInstances, uint nOutBufferSize, uint nInBufferSize, uint nDefaultTimeOut, IntPtr lpSecurityAttributes);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int ConnectNamedPipe(SafeFileHandle hNamedPipe, IntPtr lpOverlapped);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int DisconnectNamedPipe(SafeFileHandle hNamedPipe);
        public
        const uint DUPLEX = (0x00000003);
        public
        const uint FILE_FLAG_OVERLAPPED = (0x40000000);

         public struct shihab{
                public Int32 intVal;
                public string str;
                };

        public class Client
        {
            public SafeFileHandle handle;
            public FileStream stream;
        }
        public
        const int BUFFER_SIZE = 100;
        public Client clientse = null;
        public string pipeName;
        Thread listenThread;
        SafeFileHandle clientHandle;
        public NamedPipeServer(string PName)
        {
            pipeName = PName;
        }
        public void Start()
        {
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
            //ListenForClients();
            //Read();
        }
        private void ListenForClients()
        {
            while (true)
            {
                clientHandle = CreateNamedPipe(this.pipeName, DUPLEX | FILE_FLAG_OVERLAPPED, 0, 255, BUFFER_SIZE, BUFFER_SIZE, 0, IntPtr.Zero);
                //could not create named pipe
                if (clientHandle.IsInvalid) return;
                int success = ConnectNamedPipe(clientHandle, IntPtr.Zero);
                //could not connect client
                if (success == 0) return;
                clientse = new Client();
                clientse.handle = clientHandle;
                clientse.stream = new FileStream(clientse.handle, FileAccess.Read, BUFFER_SIZE, true);
                Thread readThread = new Thread(new ThreadStart(Read));
                readThread.Start();
            }
        }
        private void Read()
        {
            string temp;
            Queue q = new Queue();
            byte[] buffer = null;
            ASCIIEncoding encoder = new ASCIIEncoding();
            while (true)
            {
                int bytesRead = 0;
                try
                {
                    buffer = new byte[BUFFER_SIZE];
                    bytesRead = clientse.stream.Read(buffer, 0, BUFFER_SIZE);
                }
                catch
                {
                    Console.WriteLine("read error");
                    break;
                }
                //client has disconnected
                if (bytesRead == 0) break;
                int ReadLength = 0;
                for (int i = 0; i < BUFFER_SIZE; i++)
                {
                    if (buffer[i].ToString("x2") != "cc")
                    {
                        ReadLength++;
                    }
                    else break;
                }
                if (ReadLength > 0)
                {
                    byte[] Rc = new byte[ReadLength];
                    shihab xyz;
                    Buffer.BlockCopy(buffer, 0, Rc, 0, ReadLength);
                    temp = encoder.GetString(Rc, 0, ReadLength);

                   // int eatthatFrog =
                            Int32 eatthatFrog = (
                                    (Int32)Rc[0] << 24) |
                                    ((Int32)Rc[1] << 16) |
                                    ((Int32)Rc[2] << 8) |
                                    ((Int32)Rc[3]);
                    xyz.intVal = eatthatFrog;
                    xyz.str = encoder.GetString(Rc, 4, ReadLength-4);

                    /*                    string strr = "";
                                        for(int i =0; i<=1;i++)
                                        {
                                            strr += temp[i];
                                        }
                                        //q.Enqueue(strr);
                                        int num = int.Parse(strr);
                                        //Console.Write(num);
                                        strr = "";
                                        for (int i = 2; i < 6; i++)
                                        {
                                            strr += temp[i];
                                        }
                                        q.Enqueue(strr);
                                        strr = "";
                                        for (int i = 6; i <= 7; i++)
                                        {
                                            strr += temp[i];
                                        }
                                        num = int.Parse(strr);*/
                    //Console.Write(num);
                    //q.Enqueue(strr);
                    //q.Enqueue(temp);
                    Console.WriteLine("Element Inserted :" + xyz.intVal );
                    Console.WriteLine("Element Inserted :" + xyz.str);
                    //Console.WriteLine("Current queue");
                    foreach (string c in q)
                    {
                        //Console.Write(c + " ");
                    }
                    //buffer.Initialize();
                }
            }
            clientse.stream.Close();
            clientse.handle.Close();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            NamedPipeServer PServer1 = new NamedPipeServer(@"\\.\pipe\myNamedPipe1");
            PServer1.Start();
        }
    }
}