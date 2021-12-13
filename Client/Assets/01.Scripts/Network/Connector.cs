using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Connector
    {
        Func<Session> _sessionFactory;

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
        {
            for(int i = 0; i < count; i++)
            {
                _sessionFactory = sessionFactory;

                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectedCompleted;
                args.RemoteEndPoint = endPoint;
                args.UserToken = socket;

                RegisterConnect(args);
            }
        }
        
        private void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = (Socket)args.UserToken;
            if (socket == null) return;

            bool pending = socket.ConnectAsync(args);
            if(pending == false)
            {
                OnConnectedCompleted(null, args);
            }
        }

        private void OnConnectedCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory();
                session.Init(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);

            }else
            {
                Console.WriteLine($"OnConnection Complete Failed : {args.SocketError}");
            }
        }
    }
}
