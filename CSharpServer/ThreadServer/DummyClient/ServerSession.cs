using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DummyClient
{
    class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            //여기까지 코드가 도착하면 연결이 된거야
            Console.WriteLine($"Connected to {endPoint}");

            //데이터를 보내자
            PlayerInfoReq packet = new PlayerInfoReq() {
                playerId = 20, name = "GGM 경기게임마이스터고 선생님 농락하니? "
            };
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 10, level = 2, duration=3.4f});
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 11, level = 2, duration = 3.4f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 12, level = 3, duration = 5.4f });
            packet.skills.Add(new PlayerInfoReq.Skill() { id = 13, level = 4, duration = 7.7f });

            Send(packet.Write());
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected from : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

            Console.WriteLine($"RECV ID : {id}, Size : {size}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transfered Bytes : {numOfBytes}");
        }
    }
}
