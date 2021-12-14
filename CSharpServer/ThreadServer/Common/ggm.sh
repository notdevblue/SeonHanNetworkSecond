#!/bin/bash

/home/han/Documents/SeonHanNetworkSecond/CSharpServer/ThreadServer/PacketGenerator/bin/Debug/netcoreapp6.0/PacketGenerator /home/han/Documents/SeonHanNetworkSecond/CSharpServer/ThreadServer/PacketGenerator/PDL.xml

sleep 0.2

cp GenPacks.cs /home/han/Documents/SeonHanNetworkSecond/CSharpServer/ThreadServer/DummyClient/Packet/GenPacks.cs
cp GenPacks.cs /home/han/Documents/SeonHanNetworkSecond/CSharpServer/ThreadServer/Server/Pakcet/GenPacks.cs

mv ServerPacketManager.cs /home/han/Documents/SeonHanNetworkSecond/CSharpServer/ThreadServer/DummyClient/Packet/ClientPacketHandler.cs
mv ClientPacketHandler.cs /home/han/Documents/SeonHanNetworkSecond/CSharpServer/ThreadServer/Server/Pakcet/ServerPacketHandler.cs
