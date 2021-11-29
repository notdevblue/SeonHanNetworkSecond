#!/bin/bash

/home/han/Documents/SeonHanNetworkSecond/CSharpServer/ThreadServer/PacketGenerator/bin/Debug/netcoreapp6.0/PacketGenerator /home/han/Documents/SeonHanNetworkSecond/CSharpServer/ThreadServer/PacketGenerator/PDL.xml

sleep 0.5

cp GenPacks.cs /home/han/Documents/SeonHanNetworkSecond/CSharpServer/ThreadServer/DummyClient/Packet/GenPacks.cs
cp GenPacks.cs /home/han/Documents/SeonHanNetworkSecond/CSharpServer/ThreadServer/Server/Pakcet/GenPacks.cs

cp PacketHandler.cs /home/han/Documents/SeonHanNetworkSecond/CSharpServer/ThreadServer/DummyClient/Packet/PacketHandler.cs
cp PacketHandler.cs /home/han/Documents/SeonHanNetworkSecond/CSharpServer/ThreadServer/Server/Pakcet/PacketHandler.cs
