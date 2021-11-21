using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
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

            _listener.Init(endPoint, () => { return new ClientSession(); });
            while (true)
            {
                ;
            }
        }
    }
}
