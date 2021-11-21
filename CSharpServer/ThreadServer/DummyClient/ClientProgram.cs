using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DummyClient
{

    class ClientProgram
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 54000);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => new ServerSession());
            
            while(true)
            {
                try
                {
             
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            
        }
    }
}
