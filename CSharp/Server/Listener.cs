using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Listener
    {
        Socket _listensocket;
        Action<Socket> _onAcceptHandler;

        public void Init(EndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            _listensocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listensocket.Bind(endPoint);
            _listensocket.Listen(10);

            _onAcceptHandler = onAcceptHandler;

            // 비동기 이벤트를 관리해준다.
            // 작업에 대한 결과물을 기록.
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnAcceptCompleted;

            RegisterAccept(args); // 한번 등록
        }

        private void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null; // Accept 된 소켓이 저장됨

            bool pending = _listensocket.AcceptAsync(args); // args 에 기록해달라고 함


            if(!pending)
            {
                // pending = 대기가 걸렸는가
                // pending == false 라면 완성된 작업을

                OnAcceptCompleted(null, args);

            }

        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                // 에러 없이 처리된 경우
                _onAcceptHandler(args.AcceptSocket);
            }
            else // 에러
            {
                System.Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAccept(args);
        }

        // public Socket Accept()
        // {
        //     return _listensocket.Accept();
        // }
    }
}