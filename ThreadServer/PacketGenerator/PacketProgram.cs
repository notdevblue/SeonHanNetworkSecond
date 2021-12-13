using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    class PacketProgram
    {
        static string genPackets; //만들어진 패킷 코드 스트링을 저장하는 곳
        static Dictionary<string, string> typeDic = new Dictionary<string, string>();
        static int packetId = 0;
        static string packetEnums = "";

        //static string managerRegister = "";

        static string clientRegister = "";
        static string serverRegister = "";
        
        static PacketProgram()
        {
            typeDic.Add("bool", "ToBoolean");
            typeDic.Add("short", "ToInt16");
            typeDic.Add("ushort", "ToUInt16");
            typeDic.Add("int", "ToInt32");
            typeDic.Add("long", "ToInt64");
            typeDic.Add("float", "ToSingle");
            typeDic.Add("double", "ToDouble");
        }


        static void Main(string[] args)
        {
            string pdlPath = "PDL.xml";

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            if(args.Length >= 1)
            {
                pdlPath = args[0];
            }

            using(XmlReader reader = XmlReader.Create(pdlPath, settings))
            {
                reader.MoveToContent(); //헤더 건너뛰고 바로 컨텐츠까지 직진
                while(reader.Read())
                {
                    if(reader.Depth == 1 && reader.NodeType == XmlNodeType.Element)
                    {
                        ParsePacket(reader);
                    }
                    Console.WriteLine(reader.Name);
                }
                string fileText = string.Format(PacketFormat.fileFormat, packetEnums, genPackets);
                File.WriteAllText("GenPacks.cs", fileText);

                string clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
                File.WriteAllText("ClientPacketManager.cs", clientManagerText);

                string serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
                File.WriteAllText("ServerPacketManager.cs", serverManagerText);
                
            }
        }

        public static void ParsePacket(XmlReader r)
        {
            if (r.Name.ToLower() != "packet" ) return;

            string packetName = r["name"];
            if(string.IsNullOrEmpty(packetName) )
            {
                Console.WriteLine("Error packet without name!");
                return;
            }

            string usage = r["usage"];
            if(string.IsNullOrEmpty(usage))
            {
                Console.WriteLine("Error packet without usage!");
                return;
            }

            (string member, string read, string write) = ParseMember(r);
            genPackets += string.Format(PacketFormat.packetFormat, packetName, member, read, write);

            packetEnums += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetId) 
                            + Environment.NewLine + "\t";

            if(usage.Equals("client"))
            {
                serverRegister += string.Format(PacketFormat.managerRegisterFormat, packetName)
                            + Environment.NewLine;
            }else if(usage.Equals("server"))
            {
                clientRegister += string.Format(PacketFormat.managerRegisterFormat, packetName)
                            + Environment.NewLine;
            }
            else
            {
                Console.WriteLine($"Error : Packet usage is invalid!!");
                return;
            }
        }

        public static Tuple<string, string, string> ParseMember(XmlReader r)
        {
            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            //string packetName = r["name"];

            int depth = r.Depth + 1; // 파싱할 깊이

            while(r.Read())
            {
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                if(string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Error : Member without name");
                    return null;
                }

                if (string.IsNullOrEmpty(memberCode) == false)
                    memberCode += Environment.NewLine; //엔터 \n cr lf
                if (string.IsNullOrEmpty(readCode) == false)
                    readCode += Environment.NewLine; //엔터 \n cr lf
                if (string.IsNullOrEmpty(writeCode) == false)
                    writeCode += Environment.NewLine; //엔터 \n cr lf

                string memberType = r.Name.ToLower();

                switch (memberType)
                {
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "double":
                    case "float":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, 
                            memberName, typeDic[memberType], memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.stringReadFormat,memberName);
                        writeCode += string.Format(PacketFormat.stringWriteFormat, memberName);
                        break;
                    case "list":
                        (string member, string read, string write) = ParseList(r);
                        memberCode += member;
                        readCode += read;
                        writeCode += write;
                        break;
                    default:
                        break;
                }
            }

            memberCode = memberCode.Replace(Environment.NewLine, Environment.NewLine + "\t");
            readCode = readCode.Replace(Environment.NewLine, Environment.NewLine + "\t\t");
            writeCode = writeCode.Replace(Environment.NewLine, Environment.NewLine + "\t\t");

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static Tuple<string, string, string> ParseList(XmlReader r)
        {
            string listName = r["name"];

            if (string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("Error : List without name!");
                return null;
            }

            (string member, string read, string write) = ParseMember(r);

            string memberCode = string.Format(PacketFormat.memberListFormat, 
                                FirstUpper(listName), FirstLower(listName), member, read, write);
            string readCode = string.Format(PacketFormat.listReadFormat, 
                                FirstUpper(listName), FirstLower(listName));
            string writeCode = string.Format(PacketFormat.listWriteFormat,
                                FirstUpper(listName), FirstLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }
        
        public static string FirstUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        public static string FirstLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToLower() + input.Substring(1);
        }
    }
}
