using System;
using System.Net;
using System.Threading;
using ServerCore;

namespace Server
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

    public class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected From : {endPoint}");

            Packet k = new Packet() { size = 100, packetId = 10 };

            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            byte[] buffer1 = BitConverter.GetBytes(k.size);
            byte[] buffer2 = BitConverter.GetBytes(k.packetId);

            Array.Copy(buffer1, 0, segment.Array, segment.Offset, buffer1.Length);
            Array.Copy(buffer2, 0, segment.Array, segment.Offset + buffer2.Length, buffer2.Length);
            ArraySegment<byte> sendBuffer = SendBufferHelper.Close(buffer2.Length + buffer1.Length);


            // Send(sendBuffer);

            Thread.Sleep(5000);
            Disconnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected From : {endPoint}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);

            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);

            switch((PacketID) id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        long playerId = BitConverter.ToInt64(buffer.Array, buffer.Offset + count);
                        count += sizeof(long);

                        System.Console.WriteLine($"PlayerID : {playerId}");
                    }
                    break;

                case PacketID.PlayerInfoOK:
                    break;
            }


            System.Console.WriteLine($"RECV ID : {id}, Size : {size}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"전송된 바이트 수 : {numOfBytes }");
        }
    }
}