
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

public enum PacketID
{
    BroadcastEnterGame = 1, 
	LeaveGame = 2, 
	BroadcastLeaveGame = 3, 
	PlayerList = 4, 
	Move = 5, 
	BroadcastMove = 6, 
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


public class BroadcastEnterGame : IPacket
{
    public int playerId;
	public float posX;
	public float posY;
	public float posZ;
	public float r;
	public float g;
	public float b;
    
    public ushort Protocol { get { return (ushort)PacketID.BroadcastEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.r = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.g = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.b = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(1024);

        ushort count = 0;
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.r), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.g), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.b), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
        
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class LeaveGame : IPacket
{
    
    
    public ushort Protocol { get { return (ushort)PacketID.LeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(1024);

        ushort count = 0;
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        
        
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class BroadcastLeaveGame : IPacket
{
    public int playerId;
    
    public ushort Protocol { get { return (ushort)PacketID.BroadcastLeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(1024);

        ushort count = 0;
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
        
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class PlayerList : IPacket
{
    
	public struct Player
	{
	    public bool isSelf;
		public int playerId;
		public float posX;
		public float posY;
		public float posZ;
		public float r;
		public float g;
		public float b;
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.isSelf = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.r = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.g = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.b = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
	    }
	
	    public void Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        Array.Copy(BitConverter.GetBytes(this.isSelf), 0, segment.Array, segment.Offset + count, sizeof(bool));
			count += sizeof(bool);
			Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.r), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.g), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.b), 0, segment.Array, segment.Offset + count, sizeof(float));
			count += sizeof(float);
	    }    
	}
	
	public List<Player> players = new List<Player>();
	
    
    public ushort Protocol { get { return (ushort)PacketID.PlayerList; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        
		players.Clear();
		
		ushort playerLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		
		for (int i = 0; i < playerLen; i++)
		{
		    Player player = new Player();
		    player.Read(segment, ref count);
		    players.Add(player);
		}
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(1024);

        ushort count = 0;
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        
		Array.Copy(BitConverter.GetBytes((ushort)this.players.Count), 0,
		                segment.Array, segment.Offset + count, sizeof(ushort));
		count += sizeof(ushort);
		
		foreach (Player player in this.players)
		{
		    player.Write(segment, ref count);
		}
		
        
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class Move : IPacket
{
    public float posX;
	public float posY;
	public float posZ;
    
    public ushort Protocol { get { return (ushort)PacketID.Move; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(1024);

        ushort count = 0;
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
        
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

public class BroadcastMove : IPacket
{
    public int playerId;
	public float posX;
	public float posY;
	public float posZ;
    
    public ushort Protocol { get { return (ushort)PacketID.BroadcastMove; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(1024);

        ushort count = 0;
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
		count += sizeof(float);
        
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

