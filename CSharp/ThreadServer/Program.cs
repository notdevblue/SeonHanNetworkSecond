using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected From : {endPoint}");
            byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to GGM server");
            Send(sendBuffer);
            Thread.Sleep(1000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected From : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvString = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"받은 데이터 : {recvString}");
            return 0; //나중에 여기 고칠꺼야
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"전송된 바이트 수 : {numOfBytes }");
        }
    }

    class ServerProgram
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            //입장을 담당할 리스너를 만들자
            string host = Dns.GetHostName();
            Console.WriteLine(host);
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            Console.WriteLine(ipAddr);
            //엔드포인트는 최종적으로 ip 주소와 포트를 바인딩시켜서 만드는 도착점이다.
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 54000);

            _listener.Init(endPoint, () => { return new GameSession(); });
            while (true)
            {
                ;
            }
        }
    }
}
