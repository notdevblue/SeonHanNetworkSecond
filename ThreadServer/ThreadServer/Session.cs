using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;

        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;
            int packetCount = 0;
            while(true)
            {
                if(buffer.Count < HeaderSize)
                {
                    break;
                }
                //앞대가리에서 2바이트를 읽어
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if(buffer.Count < dataSize)
                {
                    break;
                }

                processLen += dataSize;
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                packetCount++;
                buffer = new ArraySegment<byte>(
                    buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }
            if(packetCount > 1)
            {
                Console.WriteLine($"패킷을 모아 받음 : {packetCount}");
            }
            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0; //멤버변수
        RecvBuffer _recvBuffer = new RecvBuffer(65535);

        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();

        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        //어떤 배열의 일부를 가져오는 형태야
        
        //bool _pending = false;

        object _lock = new object(); //락킹용 오브젝트 하나 만들어둘께

        public void Init(Socket socket)
        {
            _socket = socket;

            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += OnRecvCompleted;
            //recvArgs.SetBuffer(new byte[1024], 0, 1024);

            //recvArgs.UserToken = "GGM";
            RegisterRecv(recvArgs);

            _sendArgs.Completed += OnSendCompleted;
        }

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        public void Send(ArraySegment<byte> sendBuffer)
        {
            lock(_lock)
            {
                _sendQueue.Enqueue(sendBuffer);
                
                //현재 보내려고 대기중인 애가 없는거야.
                if(_pendingList.Count == 0)
                {
                    RegisterSend();
                }
            }
        }

        public void Send(List<ArraySegment<byte>> sendBufferList)
        {
            if (sendBufferList.Count == 0) return;

            lock (_lock)
            {
                sendBufferList.ForEach(x => _sendQueue.Enqueue(x));

                if (_pendingList.Count == 0)
                {
                    RegisterSend();
                }
            }
        }

        public void Disconnect()
        {
            if(Interlocked.Exchange(ref _disconnected, 1)  == 1)
            {
                return;
            }
            OnDisconnected(_socket.RemoteEndPoint);

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            Clear();
        }

        public void Clear()
        {
            lock(_lock)
            {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }

        #region 네트워크 통신부

        private void RegisterSend()
        {
            if (_disconnected == 1) return;
            
            while(_sendQueue.Count > 0)
            {
                ArraySegment<byte> buffer = _sendQueue.Dequeue();
                //_sendArgs.BufferList.Add(new ArraySegment<byte>(buffer, 0, buffer.Length));
                //이렇게 하면 안된다.
                _pendingList.Add(buffer);
            }
            _sendArgs.BufferList = _pendingList;

            try
            {
                
                bool pending = _socket.SendAsync(_sendArgs);
                if (!pending)
                {
                    OnSendCompleted(null, _sendArgs);
                }
            }catch (Exception e)
            {
                Console.WriteLine($"Register Send Failed {e}");
            }
            
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock(_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendingList.Clear();

                        OnSend(_sendArgs.BytesTransferred);

                        if (_sendQueue.Count > 0)
                            RegisterSend();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"On Send Completed Failed {e}");
                    }
                }
                else
                {
                    Console.WriteLine(args.SocketError);
                    Disconnect(); //실패하면 종료
                }
            }
            
        }

        private void RegisterRecv(SocketAsyncEventArgs args)
        {
            if (_disconnected == 1) return;

            _recvBuffer.Clean(); // 버퍼를 청소해주고
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            args.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(args);

                if (!pending)
                {
                    OnRecvCompleted(null, args);
                }
            } catch(Exception e)
            {
                Console.WriteLine($"Register Recv Failed {e}");
            }
            
        }

        private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0)
            {
                try
                {
                    if(_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }
                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if(processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    if(_recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterRecv(args);
                }catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                //소켓 종료를 해야할 경우임
            }
            
        }
        #endregion
    }
}
