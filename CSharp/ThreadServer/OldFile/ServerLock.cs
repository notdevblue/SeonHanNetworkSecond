// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using System.Collections.Generic;

// namespace ServerLock
// {
//     class Lock
//     {
//         const int EMPTY_FLAG = 0x00000000;
//         const int WRITE_MASK = 0x7FFF0000; // F = 1111, 맨 앞 부호는 비트
//         const int READ_MASK = 0x0000FFFF;
//         const int MAX_SPIN_COUNT = 5000; // 5000번 대기해도 안 열리면 잠쉬 쉬다 옴

//         // 32비트로 나눠서 아레 16은 리드락, 위에 15는 라이트락

//         int _flag = EMPTY_FLAG;
//         int _writeCount = 0;
        
//         public void WriteLock()
//         {
//             // 라이트락이 이미 걸려있는가?
//             int lockThreadId = (_flag & WRITE_MASK) >> 16; // 마스킹 후 16비트 아레로 밈
//             if(lockThreadId == Thread.CurrentThread.ManagedThreadId)
//             {
//                 // 지금 락을 걸어둔 스레드가 다시 락을 요청한 경우 통과시켜줘야 함
//                 ++_writeCount;
//                 return;
//             }

//             int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK; // 이렇게 되기를 소망함

//             while(true)
//             {
//                 for (int i = 0; i < MAX_SPIN_COUNT; ++i)
//                 {
//                     if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
//                     // desired 로 _flag 를 바꾸려고 함, EMPTY_FLAG 일때, 바꿔지면 이전값을 리턴 => 안 바꿔지면 EMPTY_FLAG 이 아닌 다른 뭔가일것
//                     {
//                         _writeCount = 1;
//                         return;
//                     }
//                 }
//                 Thread.Yield();
//             }
//         }

//         public void WriteUnlock()
//         {
//             --_writeCount;
//             if(_writeCount == 0)
//             {
//                 Interlocked.Exchange(ref _flag, EMPTY_FLAG); // 원자성은 보장되어야 함
//             }
//         }

//         public void ReadLock()
//         {
//             // 라이트락 권한을 가지고 있는데 리드락도 걸려고 함
//             int lockThreadId = (_flag & WRITE_MASK) >> 16;
//             if (lockThreadId == Thread.CurrentThread.ManagedThreadId)
//             {
                
//                 Interlocked.Increment(ref _flag); // 플레그 하나 증가
//                 return;
//             }

//             // 만약 아무도 라이트락을 가지고 있지 않다면 ReadCount 를 1 올린다.
//             while(true)
//             {
//                 for (int i = 0; i < MAX_SPIN_COUNT; ++i)
//                 {
//                     int expected = (_flag & READ_MASK);
//                     if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
//                     {
//                         return;
//                     }
//                 }
//                 Thread.Yield();
//             }
//         }

//         public void ReadUnlock()
//         {
//             Interlocked.Decrement(ref _flag);
//         }
//     }

//     class Program
//     {

//         static volatile int count = 0;
//         static Lock _lock = new Lock(); // 우리가 만든 ReadWriteLock

//         static void Main2(string[] args)
//         {
//             Task t1 = new Task(() => {
//                 for (int i = 0; i < 100000; ++i)
//                 {
//                     _lock.WriteLock();
//                     ++count;
//                     _lock.WriteUnlock();
//                 }
//             });
            
//             Task t2 = new Task(() => {
//                 for (int i = 0; i < 100000; ++i)
//                 {
//                     _lock.WriteLock();
//                     --count;
//                     _lock.WriteUnlock();
//                 }
//             });

//             t1.Start();
//             t2.Start();

//             for (int i = 0; i < 100000; ++i)
//             {
//                 _lock.ReadLock();
//                 System.Console.WriteLine(count);
//                 _lock.ReadUnlock();
//             }

//                 Task.WaitAll(t1, t2);

//             System.Console.WriteLine(count);
//         }
//     }
// }