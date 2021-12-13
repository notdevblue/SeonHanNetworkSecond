using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
    public class PacketFormat
    {
        //{0} 패킷 처리 등록부(Register)
        public static string managerFormat =
@"
using ServerCore;
using System;
using System.Collections.Generic;

public class PacketManager
{{
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance
    {{
        get
        {{
            return _instance;
        }}
    }}
    #endregion

    private PacketManager()
    {{
        Register();
    }}

    Dictionary<ushort, Action<PacketSession, IPacket>> _handler
        = new Dictionary<ushort, Action<PacketSession, IPacket>>();
    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _onRecv
        = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>,IPacket>>();

    public void Register()
    {{
        {0}
    }}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, 
            Action<PacketSession, IPacket> onRecvCallBack = null)
    {{
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (_onRecv.TryGetValue(id, out func))
        {{
            IPacket packet = func(session, buffer);

            if(onRecvCallBack != null){{ //유니티가 뭔가 준거
                onRecvCallBack(session, packet);
            }}else{{
                HandlePacket(session, packet);
            }}
        }}
    }}

    private T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer)
        where T : IPacket, new()
    {{
        T t = new T();
        t.Read(buffer);

        return t;
    }}

    public void HandlePacket(PacketSession session, IPacket t){{
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(t.Protocol, out action))
        {{
            action(session, t);
        }}
    }}
}}
";
        // {0} 패킷 이름
        public static string managerRegisterFormat =
@"
        _onRecv.Add((ushort)PacketID.{0}, MakePacket<{0}>);
        _handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);
";

        //{0} 패킷 이름
        //{1} 패킷 번호
        public static string packetEnumFormat = 
@"{0} = {1}, ";

        //{0} 패킷 enum 형태
        //{1} 패킷 클래스들 선언부
        public static string fileFormat =
@"
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

public enum PacketID
{{
    {0}
}}

public interface IPacket
{{
	ushort Protocol {{ get; }}
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}}

{1}
";
        //{0} 패킷의 이름
        //{1} 멤버 변수들
        //{2} 멤버 변수 Read 부분
        //{3} 멤버 변수 Write 부분
        public static string packetFormat =
@"
public class {0} : IPacket
{{
    {1}
    
    public ushort Protocol {{ get {{ return (ushort)PacketID.{0}; }} }}

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        {2}
    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(1024);

        ushort count = 0;
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.Protocol), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        {3}
        
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }}
}}
";
        //{0} 변수의 형식
        //{1} 변수의 이름
        public static string memberFormat =
@"public {0} {1};";

        //{0} 변수의 이름
        //{1} To 함수 타입
        //{2} 변수의 타입
        public static string readFormat =
@"this.{0} = BitConverter.{1}(segment.Array, segment.Offset + count);
count += sizeof({2});";

        //{0} 변수의 이름
        public static string stringReadFormat =
@"ushort {0}Len = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(segment.Array, segment.Offset + count, {0}Len);
count += {0}Len;";

        // {0} 변수의 이름
        // {1} 변수의 타입
        public static string writeFormat =
@"Array.Copy(BitConverter.GetBytes(this.{0}), 0, segment.Array, segment.Offset + count, sizeof({1}));
count += sizeof({1});";

        // {0} 변수의 이름
        public static string stringWriteFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetByteCount(this.{0});
Array.Copy(BitConverter.GetBytes({0}Len), 0, segment.Array, segment.Offset + count, sizeof(ushort));
count += sizeof(ushort);
byte[] {0}Byte = Encoding.Unicode.GetBytes(this.{0});
Array.Copy({0}Byte, 0, segment.Array, segment.Offset + count, {0}Len);
count += {0}Len;";

        //{0} 구조체 이름
        //{1} 변수 이름
        //{2} 구조체 멤버 선언
        //{3} 구조체 멤버 Read
        //{4} 구조체 멤버 Write
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
        //{0} 구조체 이름
        //{1} 리스트 변수 이름
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
}}";

        //{0} 구조체 이름
        //{1} 리스트 변수 이름
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
