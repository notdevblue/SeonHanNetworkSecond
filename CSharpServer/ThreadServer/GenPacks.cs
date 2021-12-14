
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

public enum PacketID
{
    PlayerInfoReq = 1, 
	TestPack = 2, 
	
}

public interface IPacket
{
	ushort Protocol { get; }
    void Read(ArraySegment<byte> segment);
    ArraySegment<byte> Write();
}




class PlayerInfoReq : IPacket
{
    public long playerId;
	public string name;
	
	public struct Skill
	{
	    public int id;
		public short level;
		public float duration;
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.id = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.level = BitConverter.ToInt16(segment.Array, segment.Offset + count);
			count += sizeof(short);
			this.duration = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
	    }
	
	    public void Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        Array.Copy(BitConverter.GetBytes(this.id), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.level), 0, segment.Array, segment.Offset + count, sizeof(short));
			count += sizeof(short);
			Array.Copy(BitConverter.GetBytes(this.duration), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
	    }    
	}
	
	public List<Skill> skills = new List<Skill>();
	
    
	public ushort Protocol { get { return (ushort)PacketID.PlayerInfoReq; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.playerId = BitConverter.ToInt64(segment.Array, segment.Offset + count);
		count += sizeof(long);
		ushort nameLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, nameLen);
		count += nameLen;
		
		skills.Clear();
		
		ushort skillLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		
		for (int i = 0; i < skillLen; i++)
		{
		    Skill skill = new Skill();
		    skill.Read(segment, ref count);
		    skills.Add(skill);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(1024);

        ushort count = 0;
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
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
		
		foreach (Skill skill in this.skills)
		{
		    skill.Write(segment, ref count);
		}
		
        
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

class TestPack : IPacket
{
    public int tt;
    
	public ushort Protocol { get { return (ushort)PacketID.TestPack; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.tt = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(1024);

        ushort count = 0;
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.tt), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

