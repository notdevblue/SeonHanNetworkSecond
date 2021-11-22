using System;
using System.Collections.Generic;
using System.Text;


namespace PacketGenerator
{
    public class PakcetFormat
    {
        //{0} = Packet's name
        //{1} = Member variable
        //{2} = Read member variable
        //{3} = Write member variable
        public static string packetFormat = 
@"
class {0} : Packet
{{
    {1}
    
    public {0}()
    {{
        this.packetId = (ushort)PacketID.{0};
    }}

    public override void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;
        this.size = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort); //이거는 사이즈 기록을 더한거고
        count += sizeof(ushort); //이건 패킷아이디 부분을 건너뛴거고
        
        {2}
    }}

    public override ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(1024);

        ushort count = 0;
        count += sizeof(ushort); //2바이트 띄고
        Array.Copy(BitConverter.GetBytes(this.packetId), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        {3}

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }}
}}
";

        //{0} Variable's type
        //{1} Variable's name
        public static string memberFormat = @"public {0} {1};";

        //{0} Variable's name
        //{1} To function type
        //{2} Variable's type
        public static string readFormat =
@"
this.{0} = BitConverter.{1}(segment.Array, segment.Offset + count);
count += sizeof({2});";
        public static string stringReadFormat =
@"ushort {0}Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, {0}Len);
count += {0}Len;";

        // {0} Variable's name
        // {1} Variable's type
        public static string writeFormat =
@"Array.Copy(BitConverter.GetBytes(this.{0}), 0, segment.Array, segment.Offset + count, sizeof({1}));
count += sizeof({1});";

        // {0} Variable's name
        public static string stringWriteFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetByteCount(this.{0});
Array.Copy(BitConverter.GetBytes({0}Len), 0, segment.Array, segment.Offset + count, sizeof(ushort));
count += sizeof(ushort);
byte[] {0}Byte = Encoding.Unicode.GetBytes(this.{0});
Array.Copy({0}Byte, 0, segment.Array, segment.Offset + count, {0}Len);
count += {0}Len;";


        //{0} Struct aame
        //{1} Variable name
        //{2} Struct member
        //{3} Struct member read
        //{4} Struct member write
        public static string memberListFormat =
@"
public struct {0}
{{
    {2}

    public void Read(ArraySegment<byte> segment, ref ushort count)
    {{
        {3}
    }}

    public void Write(ArraySegment<byte> segment, ref ushort count)
    {{
        {4}
    }}
}}

public List<{0}> {1}s = new List<{0}>();
";

        // {0} Struct name
        // {1} List Variable name
        public static string listReadFormat =
@"
{1}s.Clear();

ushort {1}Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
count += sizeof(ushort);

for (int i = 0; i < {1}Len; i++)
{{
    {0} {1} = new {0}();
    {1}.Read(segment, ref count);
    {1}s.Add({1});
}}
";
        //{0} Struct Name
        //{1} List Variable Name
        public static string listWriteFormat =
@"
Array.Copy(BitConverter.GetBytes((ushort)this.{1}s.Count), 0,
                segment.Array, segment.Offset + count, sizeof(ushort));
count += sizeof(ushort);

foreach ({0} {1} in this.{1}s)
{{
    {1}.Write(segment, ref count);
}}
";

    }
}