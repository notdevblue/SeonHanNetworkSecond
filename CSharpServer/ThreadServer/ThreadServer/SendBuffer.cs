using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class SendBuffer
    {
        byte[] _buffer;
        int _usedSize = 0;

        public int FreeSize
        {
            get
            {
                return _buffer.Length - _usedSize;
            }
        }

        public SendBuffer(int chuckSize)
        {
            _buffer = new byte[chuckSize];
        }

        public ArraySegment<byte> Open(int reserveSize) // ArraySegment only take ref not new
        {
            if(reserveSize > FreeSize) return new ArraySegment<byte>(_buffer, 0, 0);
            // array, offset, length
            return new ArraySegment<byte>(_buffer,_usedSize, reserveSize);
            // returns _buffer[_usedsize] ~ _buffer[_usedSize + reserveSize]
        }

        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;
            // shrinks array to only used size, erasing unused section

            return segment;
        }
    }

    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => null);

        public static int ChunkSize { get; set; } = 4096 * 100;
        
        public static ArraySegment<byte> Open(int reserveSize)
        {
            if(CurrentBuffer.Value == null) // ThreadLocal<SendBuffer>(() => ())
            {
                CurrentBuffer.Value = new SendBuffer(ChunkSize);
            }
            if(CurrentBuffer.Value.FreeSize < reserveSize) // when it's full, Helper makes new buffer
            {
                CurrentBuffer.Value = new SendBuffer(ChunkSize);
            }

            return CurrentBuffer.Value.Open(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }

    }
}