using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory; //새션을 생성하는 매서드를 넘겨주는거야

        public void Init(EndPoint endPoint, Func<Session> sessionFactory, int register = 1, int backLog = 100)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(backLog);

            _sessionFactory = sessionFactory;

            for(int i = 0; i < register; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs(); //비동기 이벤트를 관리해주는 녀석
                args.Completed += OnAcceptCompleted;

                RegisterAccept(args); //한번 등록
            }
        }

        private void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);

            if (!pending)
            {
                //pending 은 대기가 걸렸는가. pending 이 false라는
                OnAcceptCompleted(null, args);
            }

        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                //에러 없이 처리된 경우
                Session session = _sessionFactory();
                session.Init(args.AcceptSocket);

                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAccept(args);
        }

    }
}
