using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void OnAcceptedHandler(Socket clientSocket)
        {
            try
            {
                // 네트워크에서 데이터 통신의 단위 : byte, bitStream 은 하드웨어적

                // byte[] recvBuffer = new byte[BUFFERSIZE];
                // int recvCnt = clientSocket.Receive(recvBuffer); // 배열은 Call by ref, Winsock > recv();

                // // recvBuffer 의 0 부터 recvCnt 까지 인코딩함
                // string recvString = Encoding.UTF8.GetString(recvBuffer, 0, recvCnt);

                // System.Console.WriteLine($"받은 데이터: {recvString}");



                byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to Arch linux Server");

                Session session = new Session();
                session.Init(clientSocket);
                session.Send(sendBuffer);

                Thread.Sleep(1000);
                session.Disconnect();

                // GetBytes  = string => bytes
                // GetString = bytes  => string

                // clientSocket.Send(sendBuffer); // Winsock > send();

                // // recv, send 둘 다 안함.
                // clientSocket.Shutdown(SocketShutdown.Both); // Winsock > shutdown();
                // clientSocket.Close();
            }
            catch(Exception e)
            {
                System.Console.WriteLine(e);
            }
        }

        public static ushort BUFFERSIZE = 1024;
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            // 입장을 담당할 리스너
            string host = Dns.GetHostName();
            System.Console.WriteLine(host);

            // 해당 Host 의 IP Entry 를 알아냄, 호스트에 진입할 수 있는 모든 IP
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            System.Console.WriteLine(ipAddr);

            // 엔드포인트는 최종적으로 IP 주소와 포트를 바인딩시켜서 만드는 도착점이다.
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 54000);

            // Socket listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // Winsock > socket();

            _listener.Init(endPoint, OnAcceptedHandler);

            while (true)
            {
                // System.Console.WriteLine("Listening.....");
            }

            // try
            // {
            //     // listenSocket.Bind(endPoint); // Winsock > bind();
            //     // listenSocket.Listen(10); // Winsock > listen();
            //     // // 인자 > 대기 숫자 ( 이 이상 대기가 들어가면 안 받음 )
            //     while(true)
            //     {
            //         System.Console.WriteLine("Listening.....");
            //         // Socket clientSocket = listenSocket.Accept(); // Winsock > accept();
            //         Socket clientSocket = _listener.Accept();
            //     }
            // }
            // catch(Exception e)
            // {
            //     System.Console.WriteLine(e);
            // }

        }
    }
}
