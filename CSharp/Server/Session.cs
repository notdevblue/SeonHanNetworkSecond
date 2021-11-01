using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Session
    {
        const int DISCONNECTED = 1;
        const int CONNECTED = 0;

        Socket _socket;
        int _disconnected = CONNECTED; // 맴버변수

        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        bool _pending = false;
        object _lock = new object();

        public void Init(Socket socket)
        {
            _socket = socket;

            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += OnRecvCompleted;

            recvArgs.SetBuffer(new byte[1024], 0, 1024);
            
            // 연결에 사용자 정의 값 넣을 수 있음
            // recvArgs.UserToken = "HAN";

            RegisterRecv(recvArgs);

            _sendArgs.Completed += OnSendCompleted;
        }

        // 이부분은 나중에 구현합시다.
        public void Send(byte[] sendBuffer)
        {
            // _socket.Send(sendBuffer);
            // SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
            // sendArgs.Completed += OnSendCompleted;
            // sendArgs.SetBuffer(sendBuffer, 0, sendBuffer.Length);
            // RegisterSend(sendArgs);

            lock(_lock)
            {
                _sendQueue.Enqueue(sendBuffer);

                if(!_pending) // 아무도 안 보내고 있음
                {
                    RegisterSend();
                }
            }
        }

        public void Disconnect()
        {
            if(Interlocked.Exchange(ref _disconnected, DISCONNECTED) == DISCONNECTED)
            {
                return;
            }

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        #region 네트워크 통신부

        private void RegisterSend()
        {
            // bool pending = _socket.SendAsync(args);
            // if(!pending)
            // {
            //     OnSendCompleted(null, args);
            // }

            _pending = true; // 이미 밖에서 Lock 건 코드만 들어옴

            byte[] buffer = _sendQueue.Dequeue();
            _sendArgs.SetBuffer(buffer, 0, buffer.Length);

            bool pending = _socket.SendAsync(_sendArgs);

            if(!pending)
            {
                OnSendCompleted(null, _sendArgs);
            }

        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        if(_sendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                        else
                        {
                            _pending = false;
                        }
                    
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine($"OnSendCompleted Failed With\r\n > {e}");
                    }
                }
                else
                {
                    Disconnect(); // 실패하면 종료
                }
            }
        }

        private void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);

            if(!pending)
            {
                OnRecvCompleted(null, args);
            }
        }

        private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0)
            {
                try
                {
                    // buffer 위에서 설정함, BytesTransferred 하면 뒤에 쓰레기값 안 가져올 수 있음
                    string recvString = Encoding.UTF8.GetString(args.Buffer, 0, args.BytesTransferred);
                    System.Console.WriteLine(recvString);
                    RegisterRecv(args);
                }
                catch(Exception e)
                {
                    System.Console.WriteLine(e);
                }
            }
            else
            {
                // 소켓 종료 해야하는 경우
            }
        }
        #endregion // 네트워크 통신부


    }
}