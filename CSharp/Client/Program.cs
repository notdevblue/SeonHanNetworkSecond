using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        static public ushort BUFFERSIZE = 1024;

        static void Main(string[] args)
        {
            string host = Dns.GetHostName();

            // 해당 Host 의 IP Entry 를 알아냄, 호스트에 진입할 수 있는 모든 IP
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            System.Console.WriteLine(ipAddr);

            // 엔드포인트는 최종적으로 IP 주소와 포트를 바인딩시켜서 만드는 도착점이다.
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 54000);


            while(true)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // Winsock > socket();

                try
                {
                    socket.Connect(endPoint); // Winsock > connect();

                    // 연결이 됨

                    System.Console.WriteLine($"Connected to {socket.RemoteEndPoint}");

                    for (int i = 0; i < 5; ++i)
                    {
                        byte[] sendBuffer = Encoding.UTF8.GetBytes($"Hello Arch Linux {i}");
                        int sendCnt = socket.Send(sendBuffer);
                    }

                    byte[] recvBuffer = new byte[BUFFERSIZE];
                    int recvCnt = socket.Receive(recvBuffer);

                    string recvStr = Encoding.UTF8.GetString(recvBuffer, 0, recvCnt);

                    System.Console.WriteLine($"서버가 준 데이터: {recvStr}");

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e);
                }

                Thread.Sleep(500);

            }
        }
    }
}
