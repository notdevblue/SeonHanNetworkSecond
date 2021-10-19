using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadServer
{
    class Program
    {


        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;

        static int number = 0;

        static void Thread_1()
        {
            // y = 1;
            // Thread.MemoryBarrier(); // 임계 영역 유발하는 공간을 막음
            // r1 = x;

            for (int i = 0; i < 10000; ++i)
            {
                // ++number;
                
                // System.Console.WriteLine(number);
                int next = Interlocked.Increment(ref number); // 값 반환함
                // System.Console.WriteLine(next); // 반드시 하나 증가된 값이 나옴


                // 인터락 연산은 아토믹 실행을 반드시 보장함
            }
        }

        static void Thread_2()
        {
            // x = 1;
            // Thread.MemoryBarrier(); // 임계 영역 유발하는 공간을 막음
            // r2 = y;

            for (int i = 0; i < 10000; ++i)
            {
                // --number;
                Interlocked.Decrement(ref number);
                // 인터락 연산은 아토믹 실행을 반드시 보장함
            }
        }


        static void Main(string[] args)
        {
            // int count = 0;

            // while (true)
            // {
            //     ++count;
                // x = y = r1 = r2 = 0;
                Task t1 = new Task(Thread_1);
                Task t2 = new Task(Thread_2);

                t1.Start();
                t2.Start();

                Task.WaitAll(t1, t2);

                // if(r1 == 0 && r2 == 0)
                // {
                //     break;
                // }
            // }

            System.Console.WriteLine($"Exit!! : {number}");
        }





        // #region 
        // // static void MainThread(object state) // 전방 스레드, 이거 끝나기 전까지는 프로그램이 안 끝남
        // // {
        // //     // System.Console.WriteLine(Thread.CurrentThread.Name);
        // //     // while(true)
        // //     //     Console.WriteLine("Hello Thread");
        // //     // Thread.Sleep(1000);

        // //     for (int i = 0; i < 5; ++i)
        // //     {
        // //         System.Console.WriteLine("Hello Thread");
        // //     }
        // // }

        // // static void Main(string[] args)
        // // {
        // //     // Thread t = new Thread(MainThread);
        // //     // t.IsBackground = true; // 백그라운드로 바꿈
        // //     // t.Name = "우앱이 기장놈";
        // //     // t.Start();
        // //     // System.Console.WriteLine("Hello World");
        // //     // // System.Console.WriteLine(Thread.CurrentThread.Name);
        // //     // t.Join();

        // //     ThreadPool.SetMinThreads(1, 1); // WorkerThread 수, CompletionPortThread 수
        // //     ThreadPool.SetMaxThreads(5, 5);
        // //     // for (int i = 0; i < 1000; ++i)
        // //     // {
        // //     //     // 부하가 큼
        // //     //     // Thread t = new Thread(MainThread);
        // //     //     // t.IsBackground = true;
        // //     //     // t.Start();

        // //     //     // 스레드풀 => 용역 업체 => 적재적소에 알맞는 자원들을 공급해준다.
        // //     //     ThreadPool.QueueUserWorkItem(MainThread);
        // //     //     // 스레드풀에서 꺼내오는 모든 스레드 = 백그라운드 스레드
        // //     // }

        // //     for (int i = 0; i < 5; ++i)
        // //     {
        // //         // ThreadPool.QueueUserWorkItem(obj => { while (true) { } });
        // //         Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning);
        // //         t.Start(); // Task 는 Start 해야 함


        // //         // Task = 스레드 풀에 있는 친구를 가져옴
        // //         // LongRunning 옵션을 주면 TaskThreadPool 을 별도로 만들어서 관리
        // //     }

        // //     ThreadPool.QueueUserWorkItem(MainThread);


        // //     while (true) { }

        // // }
        // #endregion

        // #region 
        // // static volatile bool _stop = false; // 이 변수는 최적화하지 않음
        // // static void ThreadMain()
        // // {
        // //     System.Console.WriteLine("Start Thread....");

        // //     // if(_stop == false)
        // //     // {
        // //     //     while(true)
        // //     //     {

        // //     //     }
        // //     // }
        // //     // 위와 아레는 이 함수만 보면 같음


        // //     while(_stop == false)
        // //     {
        // //         // 누군가 stop true 설정할때까지 계속
        // //     }


        // //     System.Console.WriteLine("Stop Thread....");
        // // }

        // // static void Main(string[] args)
        // // {
        // //     Task t = new Task(ThreadMain);
        // //     t.Start();

        // //     Thread.Sleep(1000);
        // //     _stop = true;
        // //     System.Console.WriteLine("Calling Stop!");
        // //     System.Console.WriteLine("Waiting for stop...");

        // //     t.Wait();

        // //     System.Console.WriteLine("Stop complete!");
        // // }
        // #endregion

        // #region

        // static void Main(string[] args)
        // {
        //     int[,] arr = new int[10000, 10000];
        //     // 

        //     {
        //         long start = DateTime.Now.Ticks;
        //         for (int i = 0; i < 10000; ++i)
        //         {
        //             for (int j = 0; j < 10000; ++j)
        //             {
        //                 arr[i, j] = 1;
        //                 // 접근이 빈번하게 발생하는 영역의 주변을 미리 캐싱함
        //             }
        //         }
        //         long end = DateTime.Now.Ticks;

        //         System.Console.WriteLine($"행우선 접근 걸린시간 {end - start}");
        //     }

        //     {
        //         long start = DateTime.Now.Ticks;
        //         for (int i = 0; i < 10000; ++i)
        //         {
        //             for (int j = 0; j < 10000; ++j)
        //             {
        //                 arr[j, i] = 1;
        //             }
        //         }
        //         long end = DateTime.Now.Ticks;

        //         System.Console.WriteLine($"열우선 접근 걸린시간 {end - start}");
        //     }
        // }

        //#endregion

        // Process
        // 독립된 메모리 보장
        // 프로세스 간 자원 공유는 OS 아니면 못함

        // 한 개의 프로세스 => 여러 개의 쓰레드
        // 서로 자원 공유 가능


    }
}
