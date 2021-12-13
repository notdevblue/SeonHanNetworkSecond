using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000; // 32비트 플래그
        const int WRITE_MASK = 0x7FFF0000;
        const int READ_MASK = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000; //5000번 대기해도 안열리면 잠시 쉬다 옴.

        //32비트로 나눠서 아래 16은 리드락이 사용하고
        // 위에 15는 라이트락이 사용 
        int _flag = EMPTY_FLAG;

        int _writeCount = 0;

        public void WriteLock()
        {
            // 라이트락이 이미 걸려있는가부터 확인해야해
            int lockThreadID = (_flag & WRITE_MASK) >> 16; //마스킹후 16비트 아래로 밀어
            if(lockThreadID == Thread.CurrentThread.ManagedThreadId)
            {
                //지금 락을 걸어둔 쓰레드가 다시 락을 요청한 경우 통과시켜줘야 한다.
                _writeCount++;
                return;
            }

            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;

            while(true)
            {
                for(int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    if(Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                }
                Thread.Yield();
            }
        }

        public void WriteUnLock()
        {
            _writeCount--;
            if(_writeCount == 0)
            {
                Interlocked.Exchange(ref _flag, EMPTY_FLAG); //원자성은 보장되어야 해
            }
        }

        public void ReadLock()
        {
            //라이트락 권한을 가지고 있는 녀석인데 리드락도 걸려고 함
            int lockThreadID = (_flag & WRITE_MASK) >> 16;
            if(Thread.CurrentThread.ManagedThreadId == lockThreadID)
            {
                Interlocked.Increment(ref _flag); //플래그를 하나 증가시키면 리드카운트가 증가
                return;
            }

            //만약 아무도 라이트락을 가지고 있지 않다면 ReadCount를 1올린다.
            while(true)
            {
                for(int i = 0; i < MAX_SPIN_COUNT; i++)
                {
                    int expected = (_flag & READ_MASK);
                    if(Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                    {
                        return;
                    }
                }
                Thread.Yield();
            }
        }

        public void ReadUnLock()
        {
            Interlocked.Decrement(ref _flag);
        }
    }
}
