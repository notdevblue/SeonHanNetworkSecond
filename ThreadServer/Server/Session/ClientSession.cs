using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Server
{
    
    class ClientSession : PacketSession
    {
        public int sessionId { get; set; }
        public GameRoom Room { get; set; }
        
        public float posX { get; set; }
        public float posY { get; set; }
        public float posZ { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected From : {endPoint}");

            ServerProgram.Room.Push(() => ServerProgram.Room.Enter(this));
            //ServerProgram.Room.Enter(this); //룸이 하나밖에 없으니까 접속시에 바로 방으로
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            Console.WriteLine($"OnDisConnected From : {endPoint}");

            if(Room != null)
            {
                GameRoom room = Room;
                Room.Push(() => room.Leave(this));
                Room = null;
            }
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"전송된 바이트 수 : {numOfBytes }");
        }
    }
}
