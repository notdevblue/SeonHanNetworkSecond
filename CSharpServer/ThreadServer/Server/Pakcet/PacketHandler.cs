using ServerCore;
using Server;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{

    public static void ChatMSGHandler(PacketSession session, IPacket packet)
    {
        ChatMSG p = packet as ChatMSG;
        ClientSession clientSession = session as ClientSession;

        if(clientSession.Room == null)
        {
            return; // 방에 안 들어왔는데 메새지 보내면 잘못된검
        }

        clientSession.Room.BroadCast(clientSession, p.chat);
    }
}