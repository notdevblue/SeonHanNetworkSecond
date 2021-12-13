using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    class RecvBuffer
    {
        //멤버변수는 언더바로 시작, Static변수는 대문자로 시작,
        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos; 

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }
        
        public int DataSize
        {
            get { return _writePos - _readPos; }
        }

        //승혁이 샌드위치
        public int FreeSize
        {
            get { return _buffer.Count - _writePos;  }
        }

        //버퍼에 기록된 데이터를 읽는 property
        public ArraySegment<byte> ReadSegment
        {
            get
            {
                return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize);
            }
        }
        //버퍼에 기록하기위해 공간을 요구하는 property
        public ArraySegment<byte> WriteSegment
        {
            get
            {
                return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize);
            }
        }
        //버퍼를 정리해주는 Cleaning method
        public void Clean()
        {
            int dataSize = DataSize;
            if(dataSize == 0)
            {
                _readPos = _writePos = 0;
            }
            else
            {
                //처음 위치로 readPos 에서부터 dataSize만큼 복사해서 가져가고, readPos, writePos 을 적절한 위치로
                //잡아줘야해
                //Array.Copy
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        //읽기 성공시의 method
        public bool OnRead(int numOfBytes)
        {
            if (numOfBytes > DataSize)
                return false;
            _readPos += numOfBytes;
            return true;
        }

        //쓰기 성공시의 method
        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;
            _writePos += numOfBytes;
            return true;
        }
    }
    
}
