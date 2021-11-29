using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        PlayerInfoReq p = packet as PlayerInfoReq;
        System.Console.WriteLine($"PlayerInfoReq: {packet}");

        foreach(PlayerInfoReq.Skill s in p.skills)
        {
            System.Console.WriteLine($"skill {s.id} : LV.{s.level}, Dur:{s.duration}");
        }
    }
}