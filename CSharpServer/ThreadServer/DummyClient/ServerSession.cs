using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DummyClient
{
    class PlayerInfoReq
    {
        public long playerId;
        public string name;
        // 구조체는 상속이 안돼. 이건 그냥 자료형이야
        // 클래스는 객체를 힙에다가 할당해 , struct는 stack할당하는데
        // 구조체의 필드 크기의 합이 16바이트를 넘기면 힙에 들어가
        public struct SkillInfo
        {
            public int id;
            public short level;
            public float duration;

            public void Write(ArraySegment<byte> segment, ref ushort count)
            {
                Array.Copy(BitConverter.GetBytes(this.id), 0, segment.Array, segment.Offset + count, sizeof(int));
                count += sizeof(int);
                Array.Copy(BitConverter.GetBytes(this.level), 0, segment.Array, segment.Offset + count, sizeof(short));
                count += sizeof(short);
                Array.Copy(BitConverter.GetBytes(this.duration), 0, segment.Array, segment.Offset + count, sizeof(float));
                count += sizeof(float);
            }

            public void Read(ArraySegment<byte> segment, ref ushort count)
            {
                this.id = BitConverter.ToInt32(segment.Array, segment.Offset + count);
                count += sizeof(int);
                this.level = BitConverter.ToInt16(segment.Array, segment.Offset + count);
                count += sizeof(short);
                this.duration = BitConverter.ToSingle(segment.Array, segment.Offset + count);
                count += sizeof(float);
            }
            // 단정도형, 배정도형
            // 0.25 => 2진수로 => 
        }

        public List<SkillInfo> skills = new List<SkillInfo>();

        public void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;
            //this.size = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort); //이거는 사이즈 기록을 더한거고
            count += sizeof(ushort); //이건 패킷아이디 부분을 건너뛴거고
            this.playerId = BitConverter.ToInt64(segment.Array, segment.Offset + count);
            count += sizeof(long);

            ushort nameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nameLen);
            count += nameLen;

            skills.Clear();
            //전체 리스트의 갯수를 사전에 가져온다.
            ushort skillLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
            count += sizeof(ushort);

            for(int i = 0; i < skillLen; i++)
            {
                SkillInfo skill = new SkillInfo();
                skill.Read(segment, ref count);
                skills.Add(skill);
            }
        }

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(1024);

            ushort count = 0;
            count += sizeof(ushort); //2바이트 띄고
            Array.Copy(BitConverter.GetBytes((ushort) PacketID.PlayerInfoReq), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(long));
            count += sizeof(long);

            ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name);
            Array.Copy(BitConverter.GetBytes(nameLen), 0, segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);
            byte[] nameByte = Encoding.Unicode.GetBytes(this.name);
            Array.Copy(nameByte, 0, segment.Array, segment.Offset + count, nameLen);
            count += nameLen;


            Array.Copy(BitConverter.GetBytes((ushort)this.skills.Count), 0, 
                            segment.Array, segment.Offset + count, sizeof(ushort));
            count += sizeof(ushort);

            foreach(SkillInfo skill in this.skills)
            {
                skill.Write(segment, ref count);
            }

            Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

            return SendBufferHelper.Close(count);
        }
    }

    //class PlayerInfoOK : Packet
    //{
    //    public int hp;
    //    public int attack;
    //}

    public enum PacketID
    {
        PlayerInfoReq = 1,
        //PlayerInfoOK = 2,
    }

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
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 10, level = 2, duration=3.4f});
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 11, level = 2, duration = 3.4f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 12, level = 3, duration = 5.4f });
            packet.skills.Add(new PlayerInfoReq.SkillInfo() { id = 13, level = 4, duration = 7.7f });

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
