using System;
using System.Collections.Generic;
using System.Net;
using ServerCore;

namespace DummyClient
{
    public class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    public class PlayerInfoReq : Packet
    {
        public long plyaerId;
    }

    public class PlayerInfoOK : Packet
    {
        public int hp;
        public int attack;
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOK = 2
    }

    class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            //여기까지 코드가 도착하면 연결이 된거야
            Console.WriteLine($"Connected to {endPoint}");

            //데이터를 보내자
            PlayerInfoReq packet = new PlayerInfoReq() { size = 12, packetId = (ushort)PacketID.PlayerInfoReq, plyaerId = 20 };
            ArraySegment<byte> segment = SendBufferHelper.Open(1024);
            ushort count = 0; // records whole byte

            count += 2; // space for last size
        
            Array.Copy(BitConverter.GetBytes(packet.packetId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            Array.Copy(BitConverter.GetBytes(packet.plyaerId), 0, segment.Array, segment.Offset + count, sizeof(long));
            count += sizeof(long);

            Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));
            count += sizeof(ushort);

            ArraySegment<byte> sendBuffer = SendBufferHelper.Close(count);
            Send(sendBuffer);
            
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected from : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);

            System.Console.WriteLine($"RECV ID : {id}, Size : {size}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transfered Bytes : {numOfBytes}");
        }
    }
}