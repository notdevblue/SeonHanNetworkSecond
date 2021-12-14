
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
    #region Singleton
    static PacketManager _instance;
    public static PacketManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PacketManager();
            }
            return _instance;
        }
    }
    #endregion

    // 패킷 번호에 따라서 핸들러를 등록하는 Dictionary
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    // 받은 패킷을 리딩할 핸들러를 지정하는 Dictionary
    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();

    public void Register()
    {
        
_onRecv.Add((ushort)PacketID.PlayerInfoReq, MakePacket<PlayerInfoReq>);
_handler.Add((ushort)PacketID.PlayerInfoReq, PacketHandler.PlayerInfoReqHandler);


_onRecv.Add((ushort)PacketID.TestPack, MakePacket<TestPack>);
_handler.Add((ushort)PacketID.TestPack, PacketHandler.TestPackHandler);


    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);


        Action<PacketSession, ArraySegment<byte>> action = null;
        if (_onRecv.TryGetValue(id, out action))
        {
            action(session, buffer);
        }
    }

    private void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T t = new T();
        t.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(t.Protocol, out action))
        {
            action(session, t);
        }
    }

}
