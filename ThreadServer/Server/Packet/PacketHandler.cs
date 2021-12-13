using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        //여기는 뭘해주면 될까?
        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
    }

    public static void MoveHandler(PacketSession session, IPacket packet)
    {
        Move movePacket = packet as Move; //업캐스트, 다운캐스트

        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Move(clientSession, movePacket));
    }
}

