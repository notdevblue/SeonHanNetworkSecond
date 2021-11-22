using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    class PacketProgram
    {
        static string genPackets;
        static Dictionary<string, string> typeDict = new Dictionary<string, string>();

        static PacketProgram()
        {
            typeDict.Add("bool", "ToBoolean");
            typeDict.Add("short", "ToInt16");
            typeDict.Add("ushort", "ToUInt16");
            typeDict.Add("int", "ToInt32");
            typeDict.Add("long", "ToInt64");
            typeDict.Add("float", "ToSingle");
            typeDict.Add("double", "ToDouble");
        }

        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            using(XmlReader reader = XmlReader.Create("PDL.xml", settings))
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
                File.WriteAllText("GenPacks.cs", genPackets);
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

            (string member, string read, string write) = ParseMember(r);
            genPackets += string.Format(PakcetFormat.packetFormat, packetName, member, read, write);
        }

        public static Tuple<string,string,string> ParseMember(XmlReader r)
        {
            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            // string packetName = r["name"];

            int depth = r.Depth + 1;

            while(r.Read()) // reans xml by line
            {
                if(r.Depth != depth)
                    break;
                string memberName = r["name"];

                if(string.IsNullOrEmpty(memberName))
                {
                    System.Console.WriteLine("Err : Member Without name");
                    return null;
                }

                if (string.IsNullOrEmpty(memberCode) == false)
                    memberCode += Environment.NewLine;
                if (string.IsNullOrEmpty(readCode) == false)
                    readCode += Environment.NewLine;
                if (string.IsNullOrEmpty(writeCode) == false)
                    writeCode += Environment.NewLine;

                string memberType = r.Name.ToLower();

                switch(memberType)
                {
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "double":
                    case "float":
                        memberCode += String.Format(PakcetFormat.memberFormat, memberType, memberName);
                        readCode   += String.Format(PakcetFormat.readFormat, memberName, typeDict[memberType], memberType);
                        writeCode  += String.Format(PakcetFormat.writeFormat, memberName, memberType);
                        break;

                    case "string":
                        memberCode += String.Format(PakcetFormat.memberFormat, memberType, memberName);
                        readCode   += String.Format(PakcetFormat.stringReadFormat, memberName);
                        writeCode  += String.Format(PakcetFormat.stringWriteFormat, memberName);
                        break;

                    case "list":
                        (string member, string read, string write) = ParseList(r);
                        memberCode += member;
                        readCode   += read;
                        writeCode  += write;
                        break;

                    default:
                        break;
                }
            }

            memberCode = memberCode.Replace(Environment.NewLine, Environment.NewLine + "\t");
            readCode   = memberCode.Replace(Environment.NewLine, Environment.NewLine + "\t\t");
            writeCode  = memberCode.Replace(Environment.NewLine, Environment.NewLine + "\t\t");

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static Tuple<string, string, string> ParseList(XmlReader r)
        {
            string listName = r["name"];

            if (string.IsNullOrEmpty(listName))
            {
                System.Console.WriteLine("Err: List without name");
                return null;
            }

            (string member, string read, string write) = ParseMember(r);

            string memberCode = string.Format(PakcetFormat.memberListFormat, FirstUpper(listName), FirstLower(listName), member, read, write);
            string readCode   = string.Format(PakcetFormat.listReadFormat, FirstUpper(listName), FirstLower(listName));
            string writeCode  = string.Format(PakcetFormat.listWriteFormat, FirstUpper(listName), FirstLower(listName));

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
