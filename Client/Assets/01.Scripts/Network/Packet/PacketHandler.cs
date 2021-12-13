using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class PacketHandler
{
    public static void BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        BroadcastEnterGame broadEnter = packet as BroadcastEnterGame;
        //ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.Enter(broadEnter);
    }

    public static void BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        BroadcastLeaveGame broadLeave = packet as BroadcastLeaveGame;
        //ServerSession serverSession = session as ServerSession;
        PlayerManager.Instance.Leave(broadLeave);
    }

    public static void PlayerListHandler(PacketSession session, IPacket packet)
    {
        PlayerList pList = packet as PlayerList;
        //ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.Add(pList);
    }

    public static void BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        BroadcastMove broadMove = packet as BroadcastMove;
        //ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.Move(broadMove);
    }
}
